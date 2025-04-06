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
namespace StageLightManeuver
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class LightChannel : StageLightChannelBase
    {
        public enum FallOffMode
        {
            Default = 0,
        }

#region DoNotSaveToProfile-Configs
        [ChannelField(true, false)] public List<Light> lights = new List<Light>();
#if USE_HDRP
        [ChannelField(true, false)] public Dictionary<Light,HDAdditionalLightData> lightData = new Dictionary<Light, HDAdditionalLightData>();
#endif
#endregion


#region params
        [ChannelField(false)] public Color lightColor;
        [ChannelField(false)] public float lightIntensity;
        [ChannelField(false)] public float innerSpotAngle;
        [ChannelField(false)] public float spotAngle;
        [ChannelField(false)] public float spotRange;
        [ChannelField(false)] public bool ignoreLightCookie = false;
        [ChannelField(false)] public Texture lightCookie;
#endregion


#region Configs
        [ChannelField(true)] public float intensityMultiplier = 1f;
        [ChannelField(true)] public float limitIntensityMin = 0f;
        [ChannelField(true)] public float limitIntensityMax = 10000f;
        [ChannelField(true)] public float limitInnerSpotAngleMin = 0f;
        [ChannelField(true)] public float limitInnerSpotAngleMax = 100f;
        [ChannelField(true)] public float limitSpotAngleMin = 0f;
        [ChannelField(true)] public float limitSpotAngleMax = 100f;
        [ChannelField(true)] public float limitSpotRangeMin = 0f;
        [ChannelField(true)] public float limitSpotRangeMax = 100f;
        [ChannelField(true)] public bool syncColorToIntensity = false;
        [ChannelField(true)] public bool autoSpotRangeZeroOnIntensityZero = true;
        [ChannelField(true)] public float syncIntensityRangeMin = 0f;
        [ChannelField(true)] public float syncIntensityRangeMax = 20f;
        [ChannelField(true)] public FallOffMode fallOffMode = FallOffMode.Default;
        [ChannelField(true)] public Gradient fallOffGradient;
#endregion


#region DoNotSaveToProfile-Configs
#if USE_VLB
        [ChannelField(true, false)] public VolumetricLightBeamHD volumetricLightBeamHd;
        [ChannelField(true, false)] public VolumetricLightBeamSD volumetricLightBeamSd;
        [ChannelField(true, false)] public VolumetricCookieHD volumetricCookieHd;
#endif
        // public UniversalAdditionalLightData universalAdditionalLightData;
#endregion


        public void GetLightInChildrenAndFetchData()
        {
            lights = GetComponentsInChildren<Light>().ToList();
            
            if(lights.Count <= 0) return;
            lightColor = lights[0].color;
            lightIntensity = lights[0].intensity;
            spotAngle = lights[0].spotAngle;
            innerSpotAngle = lights[0].innerSpotAngle;
            spotRange = lights[0].range;
            lightCookie = lights[0].cookie;
            
        }
        public override void Init()
        {
            base.Init();
#if USE_HDRP
            lightData.Clear();
#endif
            GetLightInChildrenAndFetchData();
            foreach (var light in lights)
            {
                if(light == null) continue;
                lightColor = light.color;
                lightIntensity = light.intensity;
                spotAngle = light.spotAngle;
                innerSpotAngle = light.innerSpotAngle;
                spotRange = light.range;
                lightCookie = light.cookie;
#if USE_HDRP
                lightData.Add(light, light.GetComponent<HDAdditionalLightData>());
#endif

#if USE_VLB
                volumetricLightBeamHd = light.GetComponent<VolumetricLightBeamHD>();
                volumetricLightBeamSd = light.GetComponent<VolumetricLightBeamSD>();
                volumetricCookieHd = light.GetComponent<VolumetricCookieHD>();
#endif
                // PropertyType.Add(typeof(property));
            }
            PropertyTypes = new List<Type>();
            PropertyTypes.Add(typeof(LightProperty));
            PropertyTypes.Add(typeof(LightColorProperty));
            PropertyTypes.Add(typeof(LightIntensityProperty));

        }

        public override void EvaluateQue(float currentTime)
        {
            if(lights == null) return;
            base.EvaluateQue(currentTime);

            lightColor = new Color(0,0,0,1);
            lightIntensity = 0f;
            spotAngle = 0f;
            innerSpotAngle = 0f;
            spotRange = 0f;
            lightCookie = null;
            while (stageLightDataQueue.Count>0)
            {
                var data = stageLightDataQueue.Dequeue();
                var clockProperty= data.TryGetActiveProperty<ClockProperty>() as ClockProperty;
                var lightProperty = data.TryGetActiveProperty<LightProperty>() as LightProperty;
                var lightColorProperty = data.TryGetActiveProperty<LightColorProperty>() as LightColorProperty;
                var lightDimmerProperty = data.TryGetActiveProperty<LightIntensityProperty>() as LightIntensityProperty;
                var lightFlickerProperty = data.TryGetActiveProperty<LightFlickerProperty>() as LightFlickerProperty;
                var weight = data.weight;
                var stageLightOrderProperty = data.TryGetActiveProperty<StageLightOrderProperty>() as StageLightOrderProperty;
                var index =stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLightFixture) :  parentStageLightFixture.order;
                if(clockProperty == null) continue;
                
                // Debug.Log($"{lightProperty.clockOverride.value.childStagger}, {lightProperty.clockOverride.value.propertyOverride}");
                var manualLightArrayProperty = data.TryGetActiveProperty<ManualLightArrayProperty>();
                var manualColorArrayProperty = data.TryGetActiveProperty<ManualColorArrayProperty>();
                
                if (manualLightArrayProperty != null)
                {
                    var values = manualLightArrayProperty.lightValues.value;
                    if (index < values.Count)
                    {
                        var lightValue = values[index];
                        lightIntensity += lightValue.intensity * weight;
                        spotAngle += lightValue.angle * weight;
                        innerSpotAngle += lightValue.innerAngle * weight;
                        spotRange += lightValue.range * weight;
                    }
                }
                else
                {
                    if (lightDimmerProperty != null)
                    {
                        var t = SlmUtility.GetNormalizedTime(currentTime, data, typeof(LightIntensityProperty),index);
                        lightIntensity += lightDimmerProperty.lightToggleDimmer.value.Evaluate(t) * weight;
                    }
                    if(lightFlickerProperty != null)
                    {
                        var staggerValue = clockProperty.staggerDelay.value * (index + 1);
                        var clipDuration = clockProperty.clipProperty.clipEndTime - clockProperty.clipProperty.clipStartTime;
                        var offset = clipDuration * staggerValue;
                        lightIntensity *= lightFlickerProperty.GetNoiseValue(currentTime +offset, index) * weight;
                    }

                    if (lightProperty != null)
                    {
                        var t = SlmUtility.GetNormalizedTime(currentTime, data, typeof(LightProperty),index);
                        spotAngle += lightProperty.spotAngle.value.Evaluate(t) * weight;
                        innerSpotAngle += lightProperty.innerSpotAngle.value.Evaluate(t) * weight;
                        spotRange += lightProperty.range.value.Evaluate(t) * weight;
                    }
                }

                if (manualColorArrayProperty != null)
                {
                    var values = manualColorArrayProperty.colorValues.value;
                    if (index < values.Count)
                    {
                        var colorValue = values[index];
                        lightColor += colorValue.color * weight;
                    }
                    
                }else if (lightColorProperty != null)
                {
                    var t = SlmUtility.GetNormalizedTime(currentTime, data, typeof(LightColorProperty),index);
                    lightColor += lightColorProperty.lightToggleColor.value.Evaluate(t) * weight;
                }

                if (weight >= 0.5f)
                {
                    if (lightProperty != null)
                    {
                        lightCookie = lightProperty.cookie.value;
                    }
                }
            }
            
            lightIntensity = Mathf.Clamp(lightIntensity, limitIntensityMin, limitIntensityMax) * intensityMultiplier;
            spotAngle = Mathf.Clamp(spotAngle, limitSpotAngleMin, limitSpotAngleMax);
            innerSpotAngle = Mathf.Clamp(innerSpotAngle, limitInnerSpotAngleMin, limitInnerSpotAngleMax);
            spotRange = Mathf.Clamp(spotRange, limitSpotRangeMin, limitSpotRangeMax);
            
            if (syncColorToIntensity && syncIntensityRangeMin < syncIntensityRangeMax)
            {
                // ライト輝度に合わせて、カラーの輝度を調整
                float h, s, v;
                // Color.RGBToHSV(lightColor, out h, out s, out v);
                v = lightColor.grayscale;
                if (v <= syncIntensityRangeMax) 
                {
                    var ratio = Mathf.InverseLerp(syncIntensityRangeMin, syncIntensityRangeMax, lightIntensity);
                    var fallOffColor = fallOffGradient.Evaluate(ratio);
                    // float newV = Mathf.Lerp(0.0f, v, ratio);
                    // lightColor = Color.HSVToRGB(h, s, newV);
                    float brightness = v;

                    // lightColor に fallOffColor を ratio の割合で混ぜる
                    // カラー合成にしたいので、輝度は変えない lightColor の値を維持
                    lightColor = Color.Lerp(fallOffColor, lightColor, ratio);
                    Color.RGBToHSV(lightColor, out h, out s, out v);
                    // lightColor = Color.HSVToRGB(h, s, newV);
                    lightColor = Color.HSVToRGB(h, s, brightness);
                }
            }
        }

        public override void UpdateChannel()
        {
            if (lights==null) return;
            foreach (var light in lights)
            {
#if USE_HDRP
                
                //? Unityエディタのバージョンによっては以下のコードが必要かも(UIが更新されない)
                // light.color = lightColor;
                // light.intensity = lightIntensity;
                // light.spotAngle = spotAngle;
                // light.innerSpotAngle = innerSpotAngle;
                // light.range = spotRange;
                // if(!ignoreLightCookie)light.cookie = lightCookie;
                if (lightData.ContainsKey(light))
                {
                    var hdAdditionalLightData = lightData[light];
                    // Debug.Log(hdAdditionalLightData.intensity);
                    // hdAdditionalLightData.SetIntensity(lightIntensity);
                    // hdAdditionalLightData.SetLightDimmer(lightIntensity);
                    hdAdditionalLightData.intensity = lightIntensity;
                    hdAdditionalLightData.SetColor( lightColor);
                    hdAdditionalLightData.SetSpotAngle(spotAngle);
                    hdAdditionalLightData.innerSpotPercent = innerSpotAngle;
                    hdAdditionalLightData.range = spotRange;
                    if (lightCookie)
                    {
                        hdAdditionalLightData.SetCookie(lightCookie);
                    }
                    else
                    {
                        if (light.type == LightType.Point)
                        {
                            // SetCookieの処理がNULLを想定してないが初期値はNULLの為、直接設定
                            // ※hdAdditionalLightData.SetCookieも、セットしているだけ（HDRP 14.0.8、17.0.8で確認）
                            light.cookie = null;
                        }
                        else
                        {
                            hdAdditionalLightData.SetCookie( Texture2D.whiteTexture);
                        }
                    }
                    // hdAdditionalLightData.UpdateAllLightValues();
                    // hdAdditionalLightData.setli
                    // lightData[light].intensity=lightIntensity;
                }
#else
                light.color = lightColor;
                light.intensity = lightIntensity;
                light.spotAngle = spotAngle;
                light.innerSpotAngle = innerSpotAngle;
                light.range = autoSpotRangeZeroOnIntensityZero && light.intensity <= 0f ? 0 : spotRange;
                if(!ignoreLightCookie)light.cookie = lightCookie;
#endif

#if USE_VLB
                // VLB のプロパティをライトに合わせて更新
                //! BeamSD 動作未チェック 
                //TODO: 動作確認
                if (volumetricLightBeamHd )
                {
                    // VLB.VolumetricLightBeamHD.AssignPropertiesFromAttachedSpotLight の実装を参照
                    // volumetricLightBeamHd.colorMode = ColorMode.Flat;
                    // volumetricLightBeamHd.colorFlat = lightColor;

                    // volumetricLightBeamHd.AssignPropertiesFromAttachedSpotLight();
                    
                    // UpdateAfterManualPropertyChange 内で AssignPropertiesFromAttachedSpotLight が呼ばれる
                    volumetricLightBeamHd.UpdateAfterManualPropertyChange();
                } 
                else if (volumetricLightBeamSd && volumetricLightBeamSd.trackChangesDuringPlaytime)
                {
                    // VLB.VolumetricLightBeamSD.AssignPropertiesFromAttachedSpotLight の実装を参照
                    // volumetricLightBeamSd.colorMode = ColorMode.Flat;
                    // volumetricLightBeamSd.color = lightColor; 

                    // UpdateAfterManualPropertyChange 内で AssignPropertiesFromAttachedSpotLight が呼ばれる
                    volumetricLightBeamSd.UpdateAfterManualPropertyChange();
                }
                
                if (volumetricCookieHd && !ignoreLightCookie)
                {
                    volumetricCookieHd.cookieTexture = lightCookie;
                }
#endif
            }
            
            
          
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
