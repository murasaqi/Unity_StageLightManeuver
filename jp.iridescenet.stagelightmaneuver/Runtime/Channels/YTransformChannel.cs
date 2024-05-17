using System;
using UnityEngine;

namespace StageLightManeuver
{



    [ExecuteAlways]
    [AddComponentMenu("")]
    public class YTransformChannel : StageLightChannelBase
    {
#region DoNotSaveToProfile-Configs
        [ChannelField(true, false)] public Transform target;
#endregion


#region Configs
        [ChannelField(true)] public float offsetY = 0f;
#endregion


#region params
        [ChannelField(false)] private float _positionY;
        [ChannelField(false)] public float smoothTime = 0.1f;
        [ChannelField(false)] public bool useSmoothness = false;
        [ChannelField(false)] public float previousPositionY = 0f;
        [ChannelField(false)] public float currentPositionY = 0f;
#endregion


        void Start()
        {
            Init();
        }

        public override void Init()
        {
            base.Init();
            PropertyTypes.Add(typeof(YTransformProperty));
        }

        public override void EvaluateQue(float currentTime)
        {
            base.EvaluateQue(currentTime);
            smoothTime = 0f;
            _positionY = 0f;

            while (stageLightDataQueue.Count >0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var yTransformProperty = queueData.TryGetActiveProperty<YTransformProperty>() as YTransformProperty;
                if (yTransformProperty == null) continue;
                var weight = queueData.weight;
                var stageLightOrderProperty = queueData.TryGetActiveProperty<StageLightOrderProperty>() as StageLightOrderProperty;
                var index = stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLightFixture) :  parentStageLightFixture.order;
                var normalizedTime = SlmUtility.GetNormalizedTime(currentTime,queueData,typeof(YTransformProperty),index);
                
                _positionY += yTransformProperty.positionY.value.Evaluate(normalizedTime) * weight;

                smoothTime += yTransformProperty.smoothTime.value * weight;
                if(weight > 0.5f)
                {
                    useSmoothness = yTransformProperty.useSmoothness.value;
                }
            }
            
            
        }

        public override void UpdateChannel()
        {
            base.UpdateChannel();
            if(useSmoothness) return;
            if(target == null) return;
            target.localPosition = new Vector3(target.localPosition.x, _positionY+offsetY, target.localPosition.z);
        }

        public void Update()
        {
            if(!useSmoothness) return;
            var smoothPositionY = Mathf.SmoothDamp(previousPositionY, _positionY, ref currentPositionY, smoothTime);
            if(target == null) return;
            target.localPosition = new Vector3(target.localPosition.x, smoothPositionY+offsetY, target.localPosition.z);
            previousPositionY = smoothPositionY;
        }
    }

}