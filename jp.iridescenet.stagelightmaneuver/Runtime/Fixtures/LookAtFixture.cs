using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    public class LookAtFixture:StageLightFixtureBase
    {
        
        [FormerlySerializedAs("panFixtureFixture")] [FormerlySerializedAs("fxPanFixture")] public LightPanFixture panFixture;
        [FormerlySerializedAs("tiltFixtureFixture")] [FormerlySerializedAs("fxTiltFixture")]
        public LightTiltFixture tiltFixture;
        public List<Transform> lookAtTransforms = new List<Transform>();
        public int lookAtTransformIndex = 0;
        public float lookAtWeight = 1f;
        public Vector3 resultAngle = Vector3.zero;
        
        public Vector3 panoffset = Vector3.zero;
        public Vector3 tiltOffset = Vector3.zero;


        public LookAtConstraint lookAtDummy;
        private Vector3 panVelocity = Vector3.zero;
        private Vector3 tiltVelocity = Vector3.zero;
        public float speed = 1f;
        private void Start()
        {
            Init();
        }
        
        public override void Init()
        {
            base.Init();
            PropertyTypes.Add(typeof(RotationProperty));
            InitLookAt();
        }


        public void InitLookAt()
        {
            if (lookAtDummy == null)
            {
                var go = new GameObject("LookAtDummy");
                go.transform.SetParent(transform);
                lookAtDummy = go.AddComponent<LookAtConstraint>();
                
                lookAtDummy.transform.localPosition = Vector3.zero;
                lookAtDummy.constraintActive = true;
                lookAtDummy.locked = true;
                lookAtDummy.weight = 1f;
            }
        }
        public override void EvaluateQue(float time)
        {
            
            resultAngle = Vector3.zero;
            lookAtTransformIndex = 0;
            speed = 0f;
            
            if(lookAtDummy == null)
                InitLookAt();
            
            while (stageLightDataQueue.Count > 0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var stageLightBaseProperties = queueData.TryGetActiveProperty<ClockProperty>() as ClockProperty;
                var lookAtProperty = queueData.TryGetActiveProperty<LookAtProperty>() as LookAtProperty;
                var stageLightOrderProperty = queueData.TryGetActiveProperty<StageLightOrderProperty>() as StageLightOrderProperty;
                var index = stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLight) :  parentStageLight.order;
                if (lookAtProperty == null || stageLightBaseProperties == null)
                    return;

                var normalizedTime = SlmUtility.GetNormalizedTime(time, queueData, typeof(LookAtProperty), index);

                lookAtTransformIndex = queueData.weight >= 0.5f ? lookAtProperty.lookAtIndex.value : lookAtTransformIndex;
                // calculate the angle between this transform and the target
                if (lookAtTransforms.Count > 0 && lookAtTransformIndex < lookAtTransforms.Count)
                {
                    var lookAtTransform = lookAtTransforms[lookAtTransformIndex];
                    lookAtDummy.constraintActive = true;
                    if (lookAtDummy.sourceCount == 0)
                    {
                        lookAtDummy.AddSource(new ConstraintSource { sourceTransform = lookAtTransform, weight = 1f });
                        // lookAtDummy.SetSource(0, new ConstraintSource {sourceTransform = lookAtTransform, weight = 1f});
                    }
                    else
                    {
                        lookAtDummy.SetSource(0, new ConstraintSource {sourceTransform = lookAtTransform, weight = 1f});
                    }
                    var targetDir = lookAtTransform.position - transform.position;
                    var forward = transform.forward;
                    var angle = Vector3.Angle(targetDir, forward);
                    var axis = Vector3.Cross(forward, targetDir);
                    var rotation = Quaternion.AngleAxis(angle, axis);
                    var rotationVector = rotation.eulerAngles; 
                    resultAngle += rotationVector * queueData.weight;
                    
                    speed += lookAtProperty.speed.value * queueData.weight;
                }
                   
                
                
            }
        }
        
        public override void UpdateFixture()
        {
            if(lookAtDummy == null)
                InitLookAt();

            var lookAtTransformLocalEulerAngles = lookAtDummy.transform.localEulerAngles;
            if (panFixture)
            {
                var panAngle =
                    panoffset + new Vector3(lookAtTransformLocalEulerAngles.x * panFixture.rotationVector.x,
                        lookAtTransformLocalEulerAngles.y * panFixture.rotationVector.y,
                        lookAtTransformLocalEulerAngles.z * panFixture.rotationVector.z);

                panFixture.rotateTransform.localEulerAngles = panAngle;
                // panFixture.rotateTransform.localEulerAngles += ( panAngle - panFixture.rotateTransform.localEulerAngles) * speed;
            }

            if (tiltFixture)
            {
                // Debug.Log(tiltFixture);
               var tiltAngle =
                    tiltOffset + new Vector3(lookAtTransformLocalEulerAngles.x * tiltFixture.rotationVector.x,
                        lookAtTransformLocalEulerAngles.y * tiltFixture.rotationVector.y,
                        lookAtTransformLocalEulerAngles.z * tiltFixture.rotationVector.z);
               
                
                tiltFixture.rotateTransform.localEulerAngles = tiltAngle;
                // tiltFixture.rotateTransform.localEulerAngles += (tiltAngle - tiltFixture.rotateTransform.localEulerAngles)  * speed;
                
            }
        }

        public void LateUpdate()
        {
            // throw new NotImplementedException();
        }
    }
}