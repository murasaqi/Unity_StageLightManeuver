using System;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public enum LightTransformType
    {
        Pan,
        Tilt
    }
    
    public enum AnimationMode
    {
        Ease,
        AnimationCurve,
        Constant
    }
    
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class LightPanChannel: StageLightChannelBase
    {
#region params
        // [ChannelFieldBehavior(false)] private LightTransformType _lightTransformType = LightTransformType.Pan;
        [ChannelField(false)] private float _angle;
        [ChannelField(false)] public Vector3 rotationVector = Vector3.up;
        [ChannelField(false)] public Transform rotateTransform;
        [ChannelField(false)] private Vector3 currentVelocity;
        [ChannelField(false)] public float smoothTime = 0.1f;
        [ChannelField(false)] private float maxSpeed = float.PositiveInfinity;
        [ChannelField(false)] private bool ignore = false;
        [ChannelField(false)] public bool useSmoothness = false;
        [ChannelField(false)] private float previousAngle = 0f;
#endregion


#region Configs
        [ChannelField(true)] public float minAngleValue = -360;
        [ChannelField(true)] public float maxAngleValue = 360;
#endregion


#region DoNotSaveToProfile-Configs
#endregion


        void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }
        public override void Init()
        {
            PropertyTypes.Add(typeof(PanProperty));
            // rotationVector = _lightTransformType == LightTransformType.Pan ? Vector3.up : Vector3.left;
        }

        public override void EvaluateQue(float currentTime)
        {   
            base.EvaluateQue(currentTime);
            if(rotateTransform == null) return;
            smoothTime = 0f;
            _angle = 0f;
            while (stageLightDataQueue.Count>0)
            {
                var queueData = stageLightDataQueue.Dequeue();

                var qLightBaseProperty = queueData.TryGetActiveProperty<ClockProperty>() as ClockProperty;
                var qPanProperty = queueData.TryGetActiveProperty<PanProperty>() as PanProperty;
                var weight = queueData.weight;
                if (qPanProperty == null || qLightBaseProperty == null) continue;
                var stageLightOrderProperty = queueData.TryGetActiveProperty<StageLightOrderProperty>() as StageLightOrderProperty;
                var index = stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLightFixture) :  parentStageLightFixture.order;
                var normalizedTime = SlmUtility.GetNormalizedTime(currentTime,queueData,typeof(PanProperty),index);

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
                                _angle += positions[index].pan * weight;
                                break;
                            case ManualPanTiltMode.Add:
                                _angle += (positions[index].pan+qPanProperty.rollTransform.value.Evaluate(normalizedTime)) * weight;
                                break;
                            case ManualPanTiltMode.Multiply:
                                _angle += (positions[index].pan*qPanProperty.rollTransform.value.Evaluate(normalizedTime)) * weight;
                                break;
                        }
                    }
                }
                else
                {
                    _angle += qPanProperty.rollTransform.value.Evaluate(normalizedTime) * weight;
                }
                
                smoothTime += qPanProperty.smoothTime.value * weight;
                if(weight > 0.5f) 
                {
                    useSmoothness = qPanProperty.useSmoothness.value;
                }
                
                
            }
            // if over limit angle, clamp it
            _angle = Mathf.Clamp(_angle, minAngleValue, maxAngleValue);
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
    }
}