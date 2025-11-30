using io.github.ykysnk.WorldBasic.Udon;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldFluid
{
    [RequireComponent(typeof(Rigidbody))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [PublicAPI]
    public abstract class BaseFluidInteractor : BasicUdonSharpBehaviour
    {
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public Collider coll;

        [HideInInspector] public float volume;

        public float customVolume;
        public float dampeningFactor = .1f;

        [HideInInspector] public int inFluidCount;
        [HideInInspector] public BaseFluid fluid;

        public bool simulateWaterTurbulence;
        [Range(0, 5)] public float turbulenceStrength = 1;

        [HideInInspector] public float[] rndTimeOffset = new float[6];

        public float floatStrength = 2;

        [SerializeField] [HideInInspector] private float airDrag;
        [SerializeField] [HideInInspector] private float airAngularDrag;

        private float _time;
        private float _waterAngularDrag = 1f;
        private float _waterDrag = 3f;

        protected abstract void FluidUpdate();

        #region Functions

        protected float CalculateVolume()
        {
            var scale = transform.localScale;
            return coll.bounds.size.x / scale.x * (coll.bounds.size.y / scale.y) * (coll.bounds.size.z / scale.z);
        }

        #endregion

        #region UnityFunctions

        public virtual void Start()
        {
            volume = customVolume != 0 ? customVolume : CalculateVolume();
            StartFreamRateLoop();
        }

        protected override void OnChange()
        {
            coll = GetComponent<Collider>();
            rb = GetComponent<Rigidbody>();
            airDrag = rb.drag;
            airAngularDrag = rb.angularDrag;
        }

        public void Awake()
        {
            rndTimeOffset = new float[6];

            for (var i = 0; i < 6; i++)
                rndTimeOffset[i] = Random.Range(0f, 6f);
        }

        #endregion

        #region Functions for FluidUpdate

        protected override void FreamRateLoop()
        {
            if (customVolume != 0)
                volume = customVolume;

            _time += Time.fixedDeltaTime / 4;

            if (inFluidCount > 0)
                FluidUpdate();
        }

        protected Vector3 GenerateTurbulence()
        {
            var turbulence = new Vector3(
                Mathf.PerlinNoise(_time + rndTimeOffset[0], _time + rndTimeOffset[1]) * 2 - 1,
                0,
                Mathf.PerlinNoise(_time + rndTimeOffset[4], _time + rndTimeOffset[5]) * 2 - 1);

            Debug.DrawRay(transform.position, turbulence);

            return turbulence * turbulenceStrength;
        }

        public void EnterFluid(BaseFluid enteredFluid, int key)
        {
            if (!IsKeyCorrect(key)) return;
            fluid = enteredFluid;
            inFluidCount++;

            _waterDrag = fluid.drag;
            _waterAngularDrag = fluid.angularDrag;

            rb.drag = _waterDrag;
            rb.angularDrag = _waterAngularDrag;
        }

        public void StayFluid(BaseFluid stayFluid, int key)
        {
            if (!IsKeyCorrect(key)) return;
            if (Utilities.IsValid(fluid)) return;

            EnterFluid(stayFluid, RandomKey);
        }

        public void ExitFluid(BaseFluid fluidToExit, int key)
        {
            if (!IsKeyCorrect(key)) return;
            if (fluid == fluidToExit)
                fluid = null;

            inFluidCount--;

            if (inFluidCount != 0) return;

            rb.drag = airDrag;
            rb.angularDrag = airAngularDrag;
        }

        #endregion
    }
}