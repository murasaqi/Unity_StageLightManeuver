#if USE_VLB

using System;
using System.Collections.Generic;
using UnityEngine;
using VLB;

namespace StageLightManeuver
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class VolumetricLightBeamSDFixture : StageLightFixtureBase
    {
        public LightFixture lightFixture;
        internal VolumetricLightBeamSD volumetricLightBeamSd;
        public float intensityMultiplier;
        public float lightRangeMultiplier;
        public float spotAngleMultiplier;
        
        private float intensityMultiplierQue;
        private float lightRangeMultiplierQue;
        private float spotAngleMultiplierQue;

        public override void Init()
        {
            base.Init();
            volumetricLightBeamSd = lightFixture.volumetricLightBeamSd;
            if (volumetricLightBeamSd == null) return;
            intensityMultiplier = volumetricLightBeamSd.intensityMultiplier;
            lightRangeMultiplier = volumetricLightBeamSd.fallOffEndMultiplier;
            spotAngleMultiplier = volumetricLightBeamSd.spotAngleMultiplier;

            PropertyTypes = new List<Type>();
            PropertyTypes.Add(typeof(VLBProperty));
        }

        public override void EvaluateQue(float currentTime)
        {
            base.EvaluateQue(currentTime);
            if (volumetricLightBeamSd == null) return;
            intensityMultiplierQue = 0f;
            lightRangeMultiplierQue = 0f;
            spotAngleMultiplierQue = 0f;
            hasQue = false;
            while (stageLightDataQueue.Count > 0)
            {
                var data = stageLightDataQueue.Dequeue();
                var vlbProperty = data.TryGetActiveProperty<VLBProperty>();
                if (vlbProperty == null) continue;
                var weight = data.weight;

                intensityMultiplierQue += vlbProperty.intensityMultiplier.value * weight;
                lightRangeMultiplierQue += vlbProperty.lightRangeMultiplier.value * weight;
                spotAngleMultiplierQue += vlbProperty.spotAngleMultiplier.value * weight;
                hasQue = true;
            }
        }
        
        public override void UpdateFixture()
        {
            base.UpdateFixture();
            if (volumetricLightBeamSd == null)
            {
                volumetricLightBeamSd = lightFixture.volumetricLightBeamSd;
                if(volumetricLightBeamSd == null) return;
            }
            // volumetricLightBeamHd.
            volumetricLightBeamSd.intensityMultiplier = intensityMultiplierQue;
            volumetricLightBeamSd.fallOffEndMultiplier = lightRangeMultiplierQue;
            volumetricLightBeamSd.spotAngleMultiplier = spotAngleMultiplierQue;
        }
    }
}

#endif