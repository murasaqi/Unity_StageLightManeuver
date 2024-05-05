using System;
using UnityEngine;

namespace StageLightManeuver
{



    [ExecuteAlways]
    [AddComponentMenu("")]
    public class XTransformChannel : StageLightChannelBase
    {
#region DoNotSaveToProfile-Configs
        [ChannelField(true, false)] public Transform target;
#endregion


#region Configs
        [ChannelField(true)] public float offsetX = 0f;
#endregion


#region params
        [ChannelField(false)] private float _positionX;
        [ChannelField(false)] public float smoothTime = 0.1f;
        [ChannelField(false)] public bool useSmoothness = false;
        [ChannelField(false)] public float previousPositionX = 0f;
        [ChannelField(false)] public float currentPositionX = 0f;
#endregion

        void Start()
        {
            Init();
        }

        public override void Init()
        {
            base.Init();
            PropertyTypes.Add(typeof(XTransformProperty));
        }

        public override void EvaluateQue(float currentTime)
        {
            base.EvaluateQue(currentTime);
            smoothTime = 0f;
            _positionX = 0f;

            while (stageLightDataQueue.Count >0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var xTransformProperty = queueData.TryGetActiveProperty<XTransformProperty>() as XTransformProperty;
                if (xTransformProperty == null) continue;
                var weight = queueData.weight;
                var stageLightOrderProperty = queueData.TryGetActiveProperty<StageLightOrderProperty>() as StageLightOrderProperty;
                var index = stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLightFixture) :  parentStageLightFixture.order;
                var normalizedTime = SlmUtility.GetNormalizedTime(currentTime,queueData,typeof(XTransformProperty),index);
                
                _positionX += xTransformProperty.positionX.value.Evaluate(normalizedTime) * weight;

                smoothTime += xTransformProperty.smoothTime.value * weight;
                if(weight > 0.5f)
                {
                    useSmoothness = xTransformProperty.useSmoothness.value;
                }
            }
            
            
        }

        public override void UpdateChannel()
        {
            base.UpdateChannel();
            if(useSmoothness) return;
            if(target == null) return;
            target.localPosition = new Vector3(_positionX+offsetX, target.localPosition.y, target.localPosition.z);
        }

        public void Update()
        {
            if(!useSmoothness) return;
            var smoothPositionX = Mathf.SmoothDamp(previousPositionX, _positionX+offsetX, ref currentPositionX, smoothTime);
            if(target == null) return;
            target.localPosition = new Vector3(smoothPositionX, target.localPosition.y, target.localPosition.z);
            previousPositionX = smoothPositionX;
            
        }
    }

}