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
    public class SyncLensFlareFixture : StageLightFixtureBase
    {
       

        public LensFlareComponentSRP lensFlareSRP;
        
        public float intensityMultiplier = 1f;
        public float maxIntensityLimit = 3;
        public LightFixture lightFixture;
        private float scale = 0f;
        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init(); 
            lightFixture = GetComponent<LightFixture>();
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

        public override void UpdateFixture()
        {
            if(lightFixture == null) return;
           
            var intensity = Mathf.Min(lightFixture.lightIntensity * intensityMultiplier,maxIntensityLimit);
            lensFlareSRP.intensity = intensity;
            lensFlareSRP.scale = scale;
        }
    }

}
