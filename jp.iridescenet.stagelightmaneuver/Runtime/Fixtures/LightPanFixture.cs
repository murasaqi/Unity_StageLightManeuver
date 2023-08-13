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
    public class LightPanFixture: StageLightFixtureBase
    {
        // private LightTransformType _lightTransformType = LightTransformType.Pan;
        private float _angle;
        public Vector3 rotationVector = Vector3.up;
        public Transform rotateTransform;
        private Vector3 currentVelocity;
        public float smoothTime = 0.1f;
        private float maxSpeed = float.PositiveInfinity;
        private bool ignore = false;
        public bool useSmoothness = false;
        private float previousAngle = 0f;
        public float minAngleValue = -360;
        public float maxAngleValue = 360;
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
                var index = stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLight) :  parentStageLight.order;
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
                        }
                    }
                }
                else
                {
                    _angle += qPanProperty.rollTransform.value.Evaluate(normalizedTime) * weight;
                }
                
                smoothTime += qPanProperty.smoothTime.value * weight;
                if(weight > 0.5f) useSmoothness = qPanProperty.useSmoothness.value;
               
                
            }
            // if over limit angle, clamp it
            _angle = Mathf.Clamp(_angle, minAngleValue, maxAngleValue);
        }
        
        public override void UpdateFixture()
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