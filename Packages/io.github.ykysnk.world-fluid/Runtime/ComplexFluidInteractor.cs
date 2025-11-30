using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldFluid
{
    [AddComponentMenu("yky/World Fluid/Complex Fluid Interactor")]
    public class ComplexFluidInteractor : BaseFluidInteractor
    {
        [SerializeField] [Min(0.001f)] private float drawRadius = .1f;

        public Transform[] floaters =
        {
        };

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        private void OnDrawGizmos()
        {
            foreach (var floater in floaters)
            {
                if (!Utilities.IsValid(floater)) continue;
                float difference = 0;

                if (inFluidCount > 0 && Utilities.IsValid(fluid))
                    difference = floater.position.y - fluid.transform.position.y;

                Gizmos.color = difference < 0 ? Color.green : Color.red;
                Gizmos.DrawSphere(floater.position, drawRadius);
            }
        }
#endif

        protected override void FluidUpdate()
        {
            foreach (var floater in floaters)
            {
                if (!Utilities.IsValid(floater) || !Utilities.IsValid(fluid)) continue;
                var difference = floater.position.y - fluid.transform.position.y;

                if (!(difference < 0)) continue;
                var buoyancy = Vector3.up * (floatStrength * Mathf.Abs(difference) * Physics.gravity.magnitude *
                                             volume * fluid.density);

                if (simulateWaterTurbulence)
                    buoyancy += GenerateTurbulence();

                rb.AddForceAtPosition(buoyancy, floater.position, ForceMode.Force);
                rb.AddForceAtPosition(rb.velocity * (dampeningFactor / floaters.Length * volume), floater.position,
                    ForceMode.Force);
            }
        }

        #region Functions

        public Vector3[] DefineCorners()
        {
            var extents = coll.bounds.size / 2;

            var corners = new[]
            {
                new Vector3(extents[0], extents[1], extents[2]), new Vector3(extents[0], extents[1], extents[2]) * -1,
                new Vector3(-extents[0], extents[1], extents[2]), new Vector3(-extents[0], extents[1], extents[2]) * -1,
                new Vector3(extents[0], -extents[1], extents[2]), new Vector3(extents[0], -extents[1], extents[2]) * -1,
                new Vector3(extents[0], extents[1], -extents[2]), new Vector3(extents[0], extents[1], -extents[2]) * -1
            };

            return corners;
        }

        #endregion
    }
}