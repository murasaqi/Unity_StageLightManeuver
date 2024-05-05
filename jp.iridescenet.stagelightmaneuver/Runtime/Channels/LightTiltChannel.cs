using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class LightTiltChannel: StageLightChannelBase
    {
#region prams
        // [ChannelFieldBehavior(false)] private LightTransformType _lightTransformType = LightTransformType.Tilt;
        [ChannelField(false)] private float _angle =0f;
        [ChannelField(false)] public Vector3 rotationVector = Vector3.left;
        [ChannelField(false)] public Transform rotateTransform;
        [ChannelField(false)] public bool ignore = false;
        [ChannelField(false)] private Vector3 currentVelocity;
        [ChannelField(false)] public float smoothTime = 0.05f;
        [ChannelField(false)] private float maxSpeed = float.PositiveInfinity;
        [ChannelField(false)] [FormerlySerializedAs("smoothness")] public bool useSmoothness = false;
        [ChannelField(false)] private float previousAngle = 0f;
#endregion


#region Config
        [ChannelField(true)] public float minAngleValue = -360;
        [ChannelField(true)] public float maxAngleValue = 360;
#endregion

#region DoNotSaveToProfile-Configs
#endregion

        public override void EvaluateQue(float currentTime)
        {
            base.EvaluateQue(currentTime);
            if(rotateTransform == null) return;
            _angle = 0f;
            smoothTime = 0f;
            while (stageLightDataQueue.Count>0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var qTiltProperty = queueData.TryGetActiveProperty<TiltProperty>() as TiltProperty;
                var timeProperty = queueData.TryGetActiveProperty<ClockProperty>() as ClockProperty;
                var weight = queueData.weight;
                var stageLightOrderProperty = queueData.TryGetActiveProperty<StageLightOrderProperty>() as StageLightOrderProperty;
                var index = stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLightFixture) :  parentStageLightFixture.order;

                if (qTiltProperty == null || timeProperty == null) continue;
                var normalizedTime = SlmUtility.GetNormalizedTime(currentTime, queueData, typeof(TiltProperty), index);
                var manualPanTiltProperty = queueData.TryGetActiveProperty<ManualPanTiltProperty>();
                
                var lookAtProperty = queueData.TryGetActiveProperty<LookAtProperty>();
                ignore = lookAtProperty != null;
                
                if(manualPanTiltProperty != null)
                {
                    var positions = manualPanTiltProperty.positions.value;
                    var mode = manualPanTiltProperty.mode.value;
                    if (index < positions.Count)
                    {
                        switch (mode)
                        {
                            case ManualPanTiltMode.Overwrite:
                                _angle += positions[index].tilt * weight;
                                break;
                            case ManualPanTiltMode.Add:
                                _angle += (positions[index].tilt+qTiltProperty.rollTransform.value.Evaluate(normalizedTime)) * weight;
                                break;
                            case ManualPanTiltMode.Multiply:
                                _angle += (positions[index].tilt*qTiltProperty.rollTransform.value.Evaluate(normalizedTime)) * weight;
                                break;
                        }
                        // Debug.Log($"tilt({Index}): {positions[Index].tilt}, weight: {weight}");
                    }
                }
                else
                {
                    _angle += qTiltProperty.rollTransform.value.Evaluate(normalizedTime) * weight;
                }
                
                smoothTime += qTiltProperty.smoothTime.value * weight;
                if(weight > 0.5f)
                {
                    useSmoothness = qTiltProperty.useSmoothness.value;
                }

            }
            
            _angle = Mathf.Clamp(_angle, minAngleValue, maxAngleValue);
        }
        
        void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }
        
        public override void UpdateChannel()
        {
            if(ignore) return;
            
            if(useSmoothness) return;
            rotateTransform.localEulerAngles = rotationVector * _angle;
            
        }

        public void Update()
        {
            if (useSmoothness)
            {
                var smoothAngle = Mathf.SmoothDampAngle(previousAngle, _angle, ref currentVelocity.x, smoothTime, maxSpeed);
                rotateTransform.localEulerAngles = rotationVector * smoothAngle;
                previousAngle = smoothAngle;
            }
            
            
        }

        public override void Init()
        {
            PropertyTypes.Add(typeof(TiltProperty));
        }
    }
}