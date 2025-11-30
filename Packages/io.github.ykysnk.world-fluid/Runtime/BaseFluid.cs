using io.github.ykysnk.utils.Extensions;
using io.github.ykysnk.WorldBasic.Udon;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldFluid
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [PublicAPI]
    public abstract class BaseFluid : BasicUdonSharpBehaviour
    {
        [Header("Water Settings")] public float density = 1;
        public float drag = 1;
        public float angularDrag = 1f;

        private bool _oldIsPlayerHeadUnderWater;

        protected bool IsPlayerEntered { get; private set; }
        protected bool IsPlayerHeadUnderWater { get; private set; }

        public Collider Coll { get; private set; }

        protected virtual void Start()
        {
            Coll = GetComponent<Collider>();
            StartFreamRateLoop();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            var fluidInteractor = other.GetComponent<BaseFluidInteractor>();

            Log($"Object '{other.FullName()}' is enter fluid");

            if (Utilities.IsValid(fluidInteractor))
                fluidInteractor.EnterFluid(this, fluidInteractor.RandomKey);
            OnObjectEnterFluid(other);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            var fluidInteractor = other.GetComponent<BaseFluidInteractor>();

            Log($"Object '{other.FullName()}' is exit fluid");

            if (Utilities.IsValid(fluidInteractor))
                fluidInteractor.ExitFluid(this, fluidInteractor.RandomKey);
            OnObjectExitFluid(other);
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            var fluidInteractor = other.GetComponent<BaseFluidInteractor>();

            if (Utilities.IsValid(fluidInteractor))
                fluidInteractor.StayFluid(this, fluidInteractor.RandomKey);
            OnObjectStayFluid(other);
        }

        protected override void FreamRateLoop()
        {
            var player = Networking.LocalPlayer;
            _oldIsPlayerHeadUnderWater = IsPlayerHeadUnderWater;
            IsPlayerHeadUnderWater = IsPointUnderWater(player.GetBonePosition(HumanBodyBones.Head));

            if (IsPlayerHeadUnderWater == _oldIsPlayerHeadUnderWater) return;
            if (IsPlayerHeadUnderWater)
                OnPlayerHeadEnterFluid(player);
            else
                OnPlayerHeadExitFluid(player);
        }

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            IsPlayerEntered = true;
            Log("Local player is enter fluid");
            OnPlayerEnterFluid(player);
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            IsPlayerEntered = false;
            Log("Local player is exit fluid");
            OnPlayerExitFluid(player);
        }

        public override void OnPlayerTriggerStay(VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            OnPlayerStayFluid(player);
        }

        protected virtual void OnPlayerEnterFluid(VRCPlayerApi player)
        {
        }

        protected virtual void OnPlayerStayFluid(VRCPlayerApi player)
        {
        }

        protected virtual void OnPlayerExitFluid(VRCPlayerApi player)
        {
        }

        protected virtual void OnPlayerHeadEnterFluid(VRCPlayerApi player)
        {
        }

        protected virtual void OnPlayerHeadExitFluid(VRCPlayerApi player)
        {
        }

        protected virtual void OnObjectEnterFluid(Collider other)
        {
        }

        protected virtual void OnObjectStayFluid(Collider other)
        {
        }

        protected virtual void OnObjectExitFluid(Collider other)
        {
        }

        public bool IsPointUnderWater(Vector3 point) => Coll.bounds.Contains(point);
    }
}