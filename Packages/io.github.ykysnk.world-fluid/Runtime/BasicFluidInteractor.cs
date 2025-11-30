using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldFluid
{
    [AddComponentMenu("yky/World Fluid/Basic Fluid Interactor")]
    public class BasicFluidInteractor : BaseFluidInteractor
    {
        protected override void FluidUpdate()
        {
            if (!Utilities.IsValid(fluid)) return;
            var fluidSurface = fluid.coll ? fluid.coll.bounds.max.y : fluid.transform.position.y;
            var difference = transform.position.y - fluidSurface;

            if (!(difference < 0)) return;
            var buoyancy = Vector3.up * (floatStrength * Mathf.Abs(difference) * Physics.gravity.magnitude * volume *
                                         fluid.density);

            if (simulateWaterTurbulence)
            {
                buoyancy += GenerateTurbulence();
                rb.AddTorque(GenerateTurbulence() * 0.5f);
            }

            rb.AddForceAtPosition(buoyancy, transform.position, ForceMode.Force);
            rb.AddForceAtPosition(-rb.velocity * (dampeningFactor * volume), transform.position, ForceMode.Force);
        }
    }
}