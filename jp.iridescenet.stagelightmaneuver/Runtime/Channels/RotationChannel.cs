using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class RotationChannel:StageLightChannelBase
    {
        public Transform target;
        public Vector3 rotationAxis = new Vector3(0,0,1);
        public Vector3 offsetRotation = new Vector3(0,0,0);
        [FormerlySerializedAs("rotationScalar")] public float rotationSpeed = 0f;
        private Vector3 rotation = Vector3.zero;


        private void Start()
        {
            Init();
        }
        
        public override void Init()
        {
            base.Init();
            PropertyTypes.Add(typeof(RotationProperty));
            rotation = Vector3.zero;
        }
        

        public override void EvaluateQue(float time)
        {

            rotationSpeed = 0f;
            // var offsetTime = 0f;
            while (stageLightDataQueue.Count > 0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var stageLightBaseProperties = queueData.TryGetActiveProperty<ClockProperty>() as ClockProperty;
                var rotationProperty = queueData.TryGetActiveProperty<RotationProperty>() as RotationProperty;
                var stageLightOrderProperty = queueData.TryGetActiveProperty<StageLightOrderProperty>() as StageLightOrderProperty;
                var index = stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLight) :  parentStageLight.order;
                if (rotationProperty == null || stageLightBaseProperties == null)
                    return;

                var normalizedTime = SlmUtility.GetNormalizedTime(time, queueData, typeof(RotationProperty),index);
                // offsetTime += SlmUtility.GetOffsetTime(time, queueData, typeof(RotationProperty),index) * queueData.weight;
                rotationSpeed += rotationProperty.rotationSpeed.value.Evaluate(normalizedTime) * queueData.weight;
              
            }

            // rotation = rotationSpeed * time;
        }

        public override void UpdateChannel()
        {
            rotation += rotationAxis * rotationSpeed * Time.deltaTime;
            if(target) target.localEulerAngles = offsetRotation+rotation;
        }
    }
    
    
}