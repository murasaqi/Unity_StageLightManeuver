using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class ReflectionProbeChannel:StageLightChannelBase
    {
        public List<ReflectionProbe> reflectionProbes = new List<ReflectionProbe>();
        public List<float> intensityBias = new List<float>();
        private float intensity = 0;

        public override void Init()
        {
            base.Init();
            PropertyTypes.Clear();
            PropertyTypes.Add(typeof(ReflectionProbeProperty));
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
            intensity = 0;
            while (stageLightDataQueue.Count >0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var reflectionProbeProperty = queueData.TryGetActiveProperty<ReflectionProbeProperty>() as ReflectionProbeProperty;
                if (reflectionProbeProperty == null) continue;
                var index = queueData.TryGetActiveProperty<StageLightOrderProperty>()?.stageLightOrderQueue.GetStageLightIndex(parentStageLightFixture) ?? parentStageLightFixture.order;
                var weight = queueData.weight;
                var t = SlmUtility.GetNormalizedTime(currentTime,queueData,typeof(ReflectionProbeProperty),index);
                intensity += reflectionProbeProperty.intensity.value.Evaluate(t) * weight;
            }
        }

        public override void UpdateChannel()
        {
            base.UpdateChannel();
            foreach (var reflectionProbe in reflectionProbes)
            {
                var i = reflectionProbes.IndexOf(reflectionProbe);
                var bias = intensityBias.Count > i ? intensityBias[i] : 1f;
                if(reflectionProbe == null) continue;
                reflectionProbe.intensity = intensity * bias;
            }
        }
    }
}