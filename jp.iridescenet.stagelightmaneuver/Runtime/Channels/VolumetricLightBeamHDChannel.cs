#if USE_VLB

using System;
using System.Collections.Generic;
using UnityEngine;
using VLB;

namespace StageLightManeuver
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class VolumetricLightBeamHDChannel : StageLightChannelBase
    {
        public LightChannel lightChannel;
        internal VolumetricLightBeamHD volumetricLightBeamHd;
        public float intensityMultiplier;
        public float lightRangeMultiplier;
        public float spotAngleMultiplier;
        
        private float intensityMultiplierQue;
        private float lightRangeMultiplierQue;
        private float spotAngleMultiplierQue;

        public override void Init()
        {
            base.Init();
            volumetricLightBeamHd = lightChannel.volumetricLightBeamHd;
            if (volumetricLightBeamHd == null) return;
            intensityMultiplier = volumetricLightBeamHd.intensityMultiplier;
            lightRangeMultiplier = volumetricLightBeamHd.fallOffEndMultiplier;
            spotAngleMultiplier = volumetricLightBeamHd.spotAngleMultiplier;

            PropertyTypes = new List<Type>();
            PropertyTypes.Add(typeof(VLBProperty));
        }

        public override void EvaluateQue(float currentTime)
        {
            base.EvaluateQue(currentTime);
            if (volumetricLightBeamHd == null) return;
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
        
        public override void UpdateChannel()
        {
            base.UpdateChannel();
            if (volumetricLightBeamHd == null)
            {
                volumetricLightBeamHd = lightChannel.volumetricLightBeamHd;
                if(volumetricLightBeamHd == null) return;
            }
            // volumetricLightBeamHd.
            volumetricLightBeamHd.intensityMultiplier = intensityMultiplierQue;
            volumetricLightBeamHd.fallOffEndMultiplier = lightRangeMultiplierQue;
            volumetricLightBeamHd.spotAngleMultiplier = spotAngleMultiplierQue;
        }
    }
}

#endif