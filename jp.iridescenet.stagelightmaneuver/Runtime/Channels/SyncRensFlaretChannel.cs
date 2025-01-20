using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

#if USE_HDRP
using UnityEngine.Rendering.HighDefinition;
#elif USE_URP
using UnityEngine.Rendering.Universal;
#endif



namespace StageLightManeuver
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class SyncLensFlareChannel : StageLightChannelBase
    {
#region DoNotSaveToProfile-Configs
        [ChannelField(true, false)] public LensFlareComponentSRP lensFlareSRP;
        [ChannelField(true, false)] public LightChannel lightChannel;
#endregion


#region Configs
        [ChannelField(true)] public float maxIntensityLimit = 3;
        [ChannelField(true)] public bool brightnessDecreasesToBlack = true;
#endregion


#region params
        [ChannelField(false)] public float intensityMultiplier = 1f;
        [ChannelField(false)] private float scale = 0f;
#endregion

        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init(); 
            lightChannel = GetComponent<LightChannel>();
        }

        public override void Init()
        {
            
            PropertyTypes.Add(typeof(SyncLensFlareProperty));
        }
        public override void EvaluateQue(float currentTime)
        {
            if(lensFlareSRP == null) return;

            intensityMultiplier = 0f;
            scale = 0f;
            while (stageLightDataQueue.Count>0)
            {
                
                var data = stageLightDataQueue.Dequeue();
                // var t=GetNormalizedTime(currentTime,data,typeof(SyncLightMaterialProperty));

                var syncLightMaterialProperty = data.TryGetActiveProperty<SyncLensFlareProperty>();
                if(syncLightMaterialProperty != null)
                {
                    intensityMultiplier += syncLightMaterialProperty.intensitymultiplier.value * data.weight;
                    scale += syncLightMaterialProperty.scale.value * data.weight;
                }

            }
            
            base.EvaluateQue(currentTime);

        }

        public override void UpdateChannel()
        {
            if(lightChannel == null) return;
            
            var intensity = Mathf.Min(lightChannel.lightIntensity * intensityMultiplier,maxIntensityLimit);
            lensFlareSRP.intensity = intensity;
            lensFlareSRP.scale = scale;

            //? ライトの色を考慮
            //! レンズフレアの色変える方法わからんので、一旦単に明るさを変えるだけにする
            if (brightnessDecreasesToBlack)
            {
                float lightColorBrightness = lightChannel.lightColor.grayscale;
                lensFlareSRP.intensity *= lightColorBrightness;
            }
        }
    }

}
