using System;
using UnityEngine;

namespace StageLightManeuver
{



    [ExecuteAlways]
    [AddComponentMenu("")]
    public class YTransformChannel : StageLightChannelBase
    {
        public Transform target;
        private float _positionY;
        public float offsetY = 0f;
        public float smoothTime = 0.1f;
        public bool useSmoothness = false;
        public float previousPositionY = 0f;
        public float currentPositionY = 0f;
      
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
            _positionY = 0f;

            while (stageLightDataQueue.Count >0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var xTransformProperty = queueData.TryGetActiveProperty<XTransformProperty>() as XTransformProperty;
                if (xTransformProperty == null) continue;
                var weight = queueData.weight;
                var stageLightOrderProperty = queueData.TryGetActiveProperty<StageLightOrderProperty>() as StageLightOrderProperty;
                var index = stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLight) :  parentStageLight.order;
                var normalizedTime = SlmUtility.GetNormalizedTime(currentTime,queueData,typeof(YTransformProperty),index);
                
                _positionY += xTransformProperty.positionX.value.Evaluate(normalizedTime) * weight;
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
            var smoothPositionX = Mathf.SmoothDamp(previousPositionY, _positionY, ref currentPositionY, smoothTime);
            if(target == null) return;
            target.localPosition = new Vector3(target.localPosition.x, smoothPositionX+offsetY, target.localPosition.z);
            previousPositionY = smoothPositionX;

        }
    }

}