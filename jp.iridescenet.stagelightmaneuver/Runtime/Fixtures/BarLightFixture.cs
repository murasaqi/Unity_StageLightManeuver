using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


#if USE_HDRP

using UnityEngine.Rendering.HighDefinition;
#elif USE_URP

using UnityEngine.Rendering.Universal;

#endif


#if USE_VLB
using VLB;
#endif

#if USE_VLB_ALTER
using VLB;
#endif




namespace StageLightManeuver
{

    public struct BarLightPrimitiveValue
    {
        
    }

    public class BarLightFixture : StageLightFixtureBase
    {
        private float lightIndex = 0;
        public List<Light> lights = new List<Light>();
#if USE_HDRP
        public Dictionary<Light,HDAdditionalLightData> lightData = new Dictionary<Light, HDAdditionalLightData>();
#endif

#if USE_VLB
        public Dictionary<Light,VolumetricLightBeamHD> volumetricLightBeamHd;
        public Dictionary<Light,VolumetricCookieHD> volumetricCookieHd;
#endif
        
#if USE_VLB_ALTER
        public Dictionary<Light,VolumetricLightBeam> volumetricLightBeamAlters = new Dictionary<Light, VolumetricLightBeam>();
#endif
        
        [ContextMenu("GetLightInChildren")]
        public void GetLightInChildren()
        {
            var lightList = GetComponentsInChildren<Light>().ToList();

        }

        public override void Init()
        {
            base.Init();
            
            foreach (var light in lights)
            {
                if(light == null) continue;
#if USE_HDRP
                lightData.Add(light, light.GetComponent<HDAdditionalLightData>());
#endif

#if USE_VLB
                // volumetricLightBeamHd = light.GetComponent<VolumetricLightBeamHD>();
                // volumetricCookieHd = light.GetComponent<VolumetricCookieHD>();
#endif
                
#if USE_VLB_ALTER
                var vlb = light.GetComponent<VolumetricLightBeam>();
                if(vlb != null) volumetricLightBeamAlters.Add(light, vlb);
#endif
            }

            PropertyTypes = new List<Type>();
            PropertyTypes.Add(typeof(BarLightIntensityProperty));

        }

        public override void EvaluateQue(float currentTime)
        {
            if(lights.Count <= 0) return;
            base.EvaluateQue(currentTime);

            while (stageLightDataQueue.Count>0)
            {
                var data = stageLightDataQueue.Dequeue();
                var stageLightBaseProperty= data.TryGetActiveProperty<ClockProperty>() as ClockProperty;
                var stageLightOrderProperty = data.TryGetActiveProperty<StageLightOrderProperty>() as StageLightOrderProperty;
                var index = stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLight) :  parentStageLight.order;
                var barLightProperty = data.TryGetActiveProperty<BarLightIntensityProperty>() as BarLightIntensityProperty;
                var weight = data.weight;
                if(barLightProperty == null || stageLightBaseProperty == null) continue;
                var normalizedTime = SlmUtility.GetNormalizedTime(currentTime, data, typeof(BarLightIntensityProperty),index);
                
                if(barLightProperty != null)
                {
                    lightIndex = 0;
                    foreach (var light in lights)
                    {
                        var t =barLightProperty.clockOverride.propertyOverride ? SlmUtility.GetNormalizedTime(currentTime, data, typeof(BarLightIntensityProperty),index) : normalizedTime;
                        light.intensity += barLightProperty.intensity.value.Evaluate(lightIndex/(lights.Count-1)) * weight;
                        lightIndex++;
                    }
                }
                
                if (weight > 0.5f)
                {
#if USE_VLB
                    // if(volumetricCookieHd != null)volumetricCookieHd. = lightProperty.cookie.value;
#endif
                }
            }
            
        }

        public override void UpdateFixture()
        {
//             if (lights==null) return;
//             foreach (var light in lights)
//             {
// #if USE_HDRP
//                 if (lightData.ContainsKey(light))
//                 {
//                     var hdAdditionalLightData = lightData[light];
//                     // Debug.Log(hdAdditionalLightData.intensity);
//                     // hdAdditionalLightData.SetIntensity(lightIntensity);
//                     // hdAdditionalLightData.SetLightDimmer(lightIntensity);
//                     hdAdditionalLightData.intensity = lightIntensity;
//                     hdAdditionalLightData.color = lightColor;
//                     hdAdditionalLightData.SetSpotAngle(spotAngle);
//                     hdAdditionalLightData.innerSpotPercent = innerSpotAngle;
//                     hdAdditionalLightData.range = spotRange;
//                     if(lightCookie)hdAdditionalLightData.SetCookie(lightCookie);
//                     // hdAdditionalLightData.UpdateAllLightValues();
//                     // hdAdditionalLightData.setli
//                     // lightData[light].intensity=lightIntensity;
//                 }
// #else
//                 light.color = lightColor;
//                  light.intensity = lightIntensity;
//                 light.spotAngle = spotAngle;
//                 light.innerSpotAngle = innerSpotAngle;
//                 light.range = spotRange;
//                 light.cookie = lightCookie;
// #endif
//
// #if USE_VLB
//                 if(volumetricCookieHd) volumetricCookieHd.cookieTexture = lightCookie;
// #endif
//             }
            
            
          
        }
        void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }
    }
}