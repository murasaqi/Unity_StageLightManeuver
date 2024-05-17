using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace StageLightManeuver
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class EnvironmentChannel:StageLightChannelBase
    {
        [ChannelField(false)] public AmbientMode initialAmbientMode = AmbientMode.Flat;

#region Configs
        [ChannelField(true)] public float initialIntensity = 1;
        [ChannelField(true)] public Color initialAmbientColor = new Color(0.212f, 0.227f, 0.259f);
#endregion

#region params
        [ChannelField(false)] private float _intensity = 0;
        [ChannelField(false)] private Color _ambientColor;
        [ChannelField(false)] private AmbientMode _ambientMode;
#endregion

        public override void Init()
        {
            base.Init();
            PropertyTypes.Clear();
            PropertyTypes.Add(typeof(EnvironmentProperty));
        }
        
        private void OnValidate()
        {
            Init();
        }
        void Start()
        {
            Init();
        }
        private void OnEnable()
        {
            Init();
        }


        public override void EvaluateQue(float currentTime)
        {
            _intensity = 0;
            _ambientColor = new Color(0, 0, 0, 0);
            while (stageLightDataQueue.Count >0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var environmentProperty = queueData.TryGetActiveProperty<EnvironmentProperty>() as EnvironmentProperty;
                if (environmentProperty == null) continue;
                var index = queueData.TryGetActiveProperty<StageLightOrderProperty>()?.stageLightOrderQueue.GetStageLightIndex(parentStageLightFixture) ?? parentStageLightFixture.order;
                var weight = queueData.weight;
                var t = SlmUtility.GetNormalizedTime(currentTime,queueData,typeof(EnvironmentProperty),index);
                _intensity += environmentProperty.ambientintensity.value.Evaluate(t) * weight;
                _ambientColor += environmentProperty.ambientColor.value.Evaluate(t) *weight;
                if(weight >= 0.5f) _ambientMode = environmentProperty.ambientMode.value;
            }
        }
        
        public override void UpdateChannel()
        {
            base.UpdateChannel();
            RenderSettings.ambientMode = _ambientMode;
            RenderSettings.ambientSkyColor = SlmUtility.GetHDRColor(_ambientColor, _intensity);
        }
        
        private void SetInitialValue()
        {
            RenderSettings.ambientMode = initialAmbientMode;
            // RenderSettings.ambientIntensity = initialIntensity;
            // RenderSettings.ambientSkyColor = SlmUtility.GetHDRColor(initialAmbientColor,initialIntensity);
        }

        private void OnDisable()
        {
            SetInitialValue();
        }
        
        private void OnDestroy()
        {
            SetInitialValue();
        }
    }
}