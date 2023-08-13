using System;
using UnityEngine;

namespace StageLightManeuver
{



    [ExecuteAlways]
    [AddComponentMenu("")]
    public class XTransformFixture : StageLightFixtureBase
    {
        public Transform target;
        private float _positionX;
        public float offsetX = 0f;
        public float smoothTime = 0.1f;
        public bool useSmoothness = false;
        public float previousPositionX = 0f;
        public float currentPositionX = 0f;
      
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
                var index = stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLight) :  parentStageLight.order;
                var normalizedTime = SlmUtility.GetNormalizedTime(currentTime,queueData,typeof(XTransformProperty),index);
                
                _positionX += xTransformProperty.positionX.value.Evaluate(normalizedTime) * weight;
            }
            
            
        }

        public override void UpdateFixture()
        {
            base.UpdateFixture();
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