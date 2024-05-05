using System;
using UnityEngine;

namespace StageLightManeuver
{



    [ExecuteAlways]
    [AddComponentMenu("")]
    public class ZTransformChannel : StageLightChannelBase
    {
#region DoNotSaveToProfile-Configs
        [ChannelField(true, false)] public Transform target;
#endregion


#region Configs
        [ChannelField(true)]public float offsetZ = 0f;
#endregion


#region params
        [ChannelField(false)] private float _positionZ;
        [ChannelField(false)] public float smoothTime = 0.1f;
        [ChannelField(false)] public bool useSmoothness = false;
        [ChannelField(false)] public float previousPositionZ = 0f;
        [ChannelField(false)] public float currentPositionZ = 0f;
#endregion


        void Start()
        {
            Init();
        }

        public override void Init()
        {
            base.Init();
            PropertyTypes.Add(typeof(ZTransformProperty));
        }

        public override void EvaluateQue(float currentTime)
        {
            base.EvaluateQue(currentTime);
            smoothTime = 0f;
            _positionZ = 0f;

            while (stageLightDataQueue.Count >0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var zTransformProperty = queueData.TryGetActiveProperty<ZTransformProperty>() as ZTransformProperty;
                if (zTransformProperty == null) continue;
                var weight = queueData.weight;
                var stageLightOrderProperty = queueData.TryGetActiveProperty<StageLightOrderProperty>() as StageLightOrderProperty;
                var index = stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLightFixture) :  parentStageLightFixture.order;
                var normalizedTime = SlmUtility.GetNormalizedTime(currentTime,queueData,typeof(ZTransformProperty),index);
                
                _positionZ += zTransformProperty.positionZ.value.Evaluate(normalizedTime) * weight;

                smoothTime += zTransformProperty.smoothTime.value * weight;
                if(weight > 0.5f)
                {
                    useSmoothness = zTransformProperty.useSmoothness.value;
                }
            }
            
            
        }

        public override void UpdateChannel()
        {
            base.UpdateChannel();
            if(useSmoothness) return;
            if(target == null) return;
            target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, _positionZ+offsetZ);
        }

        public void Update()
        {
            if(!useSmoothness) return;
            var smoothPositionZ = Mathf.SmoothDamp(previousPositionZ, _positionZ, ref currentPositionZ, smoothTime);
            if(target == null) return;
            target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, smoothPositionZ+offsetZ);
            previousPositionZ = smoothPositionZ;
            

        }
    }

}