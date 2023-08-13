using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
 
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class BarLightPanFixture: StageLightFixtureBase
    {
        // private LightTransformType _lightTransformType = LightTransformType.Pan;
        public List<LightPanFixture> lightPanFixtures = new List<LightPanFixture>();
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
            // base.EvaluateQue(currentTime);
          
            // while (stageLightDataQueue.Count>0)
            // {
            //     var queueData = stageLightDataQueue.Dequeue();
            //
            //     var qLightBaseProperty = queueData.TryGetActiveProperty<ClockProperty>() as ClockProperty;
            //     var qPanProperty = queueData.TryGetActiveProperty<PanProperty>() as PanProperty;
            //     var weight = queueData.weight;
            //     if (qPanProperty == null || qLightBaseProperty == null) continue;
            //   
            //     var normalizedTime = SlmUtility.GetNormalizedTime(currentTime,queueData,typeof(PanProperty),Index);
            //
            //     var manualPanTiltProperty = queueData.TryGetActiveProperty<ManualPanTiltProperty>();
            //    var lookAtProperty = queueData.TryGetActiveProperty<LookAtProperty>();
            //    ignore = lookAtProperty != null;
            //        if(manualPanTiltProperty != null)
            //     {
            //         var positions = manualPanTiltProperty.positions.value;
            //         var mode = manualPanTiltProperty.mode.value;
            //         if (Index < positions.Count)
            //         {
            //            switch (mode)
            //             {
            //                 case ManualPanTiltMode.Overwrite:
            //                     _angle += positions[Index].pan * weight;
            //                     break;
            //                 case ManualPanTiltMode.Add:
            //                     _angle += (positions[Index].pan+qPanProperty.rollTransform.value.Evaluate(normalizedTime)) * weight;
            //                     break;
            //             }
            //         }
            //     }
            //     else
            //     {
            //         _angle += qPanProperty.rollTransform.value.Evaluate(normalizedTime) * weight;
            //     }
            //     
            //     smoothTime += qPanProperty.smoothTime.value * weight;
            //     if(weight > 0.5f) useSmoothness = qPanProperty.useSmoothness.value;
            //    
                
        }
            
            // // if over limit angle, clamp it
            // _angle = Mathf.Clamp(_angle, minAngleValue, maxAngleValue);
        
        
        public override void UpdateFixture()
        {
            // if(ignore) return;
            //
            // if(useSmoothness) return;
            // rotateTransform.localEulerAngles = rotationVector * _angle;
            
        }

        public void Update()
        {
            // if (useSmoothness)
            // {
            //     var smoothAngle = Mathf.SmoothDampAngle(previousAngle, _angle, ref currentVelocity.x, smoothTime, maxSpeed);
            //     rotateTransform.localEulerAngles = rotationVector * smoothAngle;
            //     previousAngle = smoothAngle;
            // }
        }
    }
}