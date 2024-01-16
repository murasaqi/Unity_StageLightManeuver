using System;
using UnityEngine;

namespace StageLightManeuver
{



    [ExecuteAlways]
    [AddComponentMenu("")]
    public class ZTransformChannel : StageLightChannelBase
    {
        public Transform target;
        private float _positionZ;
        public float offsetZ = 0f;
        public float smoothTime = 0.1f;
        public bool useSmoothness = false;
        public float previousPositionZ = 0f;
        public float currentPositionZ = 0f;
      
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
            _positionZ = 0f;

            while (stageLightDataQueue.Count >0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var xTransformProperty = queueData.TryGetActiveProperty<XTransformProperty>() as XTransformProperty;
                if (xTransformProperty == null) continue;
                var weight = queueData.weight;
                var stageLightOrderProperty = queueData.TryGetActiveProperty<StageLightOrderProperty>() as StageLightOrderProperty;
                var index = stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLightFixture) :  parentStageLightFixture.order;
                var normalizedTime = SlmUtility.GetNormalizedTime(currentTime,queueData,typeof(ZTransformProperty),index);
                
                _positionZ += xTransformProperty.positionX.value.Evaluate(normalizedTime) * weight;
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
            var targetPosition = new Vector3(target.localPosition.x, target.localPosition.y, _positionZ+offsetZ);
            if(target == null) return;
            target.localPosition = Vector3.Lerp(target.localPosition, targetPosition, smoothTime);
            previousPositionZ = currentPositionZ;
            

        }
    }

}