using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class RotationChannel:StageLightChannelBase
    {
#region DoNotSaveToProfile-Configs
        [ChannelField(true, false)] public Transform target;
#endregion


#region Configs
        [ChannelField(true)] public Vector3 rotationAxis = new Vector3(0,0,1);
        [ChannelField(true)] public Vector3 offsetRotation = new Vector3(0,0,0);
#endregion


#region params
        [ChannelField(false)] [FormerlySerializedAs("rotationScalar")] public float rotationSpeed = 0f;
        [ChannelField(false)] private Vector3 rotation = Vector3.zero;
#endregion


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
                var index = stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLightFixture) :  parentStageLightFixture.order;
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