#if USE_VLB_ALTER
using System;
using UnityEngine;

using VLB;
using Random = UnityEngine.Random;

namespace StageLightManeuver
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class GoboChannel:StageLightChannelBase
    {
#region DoNotSaveToProfile-Configs
        [ChannelFieldBehavior(true, false)] public VolumetricLightBeam volumetricLightBeam;
        [ChannelFieldBehavior(true, false)] public MeshRenderer meshRenderer;
        [ChannelFieldBehavior(true, false)] public Transform goboTransform;
        [ChannelFieldBehavior(true, false)] public Texture2D goboTexture;
#endregion

#region Configs
        [ChannelFieldBehavior(true)] public string goboPropertyName = "_GoboTexture";
        [ChannelFieldBehavior(true)] public float speed = 0f;
        [ChannelFieldBehavior(true)] public Vector3 goboRotateVector = new Vector3(0, 0, 1);
        [ChannelFieldBehavior(true)] public Vector3 goboRotationOffset = new Vector3(0, 0, 0);
        [ChannelFieldBehavior(true)] public bool rotateStartOffsetRandom = false;
#endregion

#region params
        [ChannelFieldBehavior(false)] private MaterialPropertyBlock _materialPropertyBlock;
        [ChannelFieldBehavior(false)] private float timelineTime = 0f;
#endregion

        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }
        
        [ContextMenu("Init")]
        public override void Init()
        {
            goboTransform.localEulerAngles = goboRotateVector * (rotateStartOffsetRandom ? Random.Range(0f, 360f) : 0f);
            _materialPropertyBlock = new MaterialPropertyBlock();

            if (volumetricLightBeam)
            {
                volumetricLightBeam.RegisterOnBeamGeometryInitializedCallback(() =>
                {
                    var beamGeometry = volumetricLightBeam.transform.GetChild(0).GetComponent<MeshRenderer>(); 
                    meshRenderer = beamGeometry;
                    if(meshRenderer)meshRenderer.GetPropertyBlock(_materialPropertyBlock);
                });     
            }

        }

        public override void EvaluateQue(float time)
        {
            timelineTime = 0;
            goboTexture = null;
            speed = 0f;
            while (stageLightDataQueue.Count > 0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var stageLightBaseProperties = queueData.TryGetActiveProperty<ClockProperty>() as ClockProperty;
                var goboProperty = queueData.TryGetActiveProperty<GoboProperty>() as GoboProperty;
                var stageLightOrderProperty = queueData.TryGetActiveProperty<StageLightOrderProperty>() as StageLightOrderProperty;
                var index =stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLightFixture) :  parentStageLightFixture.order;
                if(goboProperty == null || stageLightBaseProperties == null)continue;

                var t = SlmUtility.GetNormalizedTime(time, queueData, typeof(GoboProperty), index);
               timelineTime += (time+SlmUtility.GetOffsetTime(time,queueData,typeof(GoboProperty),index)) * queueData.weight;
                if (queueData.weight > 0.5f)
                {
                    goboTexture = goboProperty.goboTexture.value;
                    goboPropertyName = goboProperty.goboPropertyName.value;
                }
                
                speed += goboProperty.goboRotationSpeed.value.Evaluate(t) * queueData.weight;
                
            }
            
            
        }
        

        public override void UpdateChannel()
        {
            
            if (goboTransform != null)
            {
                goboTransform.localEulerAngles = goboRotationOffset + (goboRotateVector*speed*timelineTime);
            }
            if (meshRenderer != null)
            {
                if (_materialPropertyBlock == null)
                {
                    Init();
                    if(_materialPropertyBlock == null) return;
                }
                
                if (goboTexture != null)
                {
                    _materialPropertyBlock.SetTexture(goboPropertyName,goboTexture);
                }
                else
                {
                    _materialPropertyBlock.SetTexture(goboPropertyName,Texture2D.whiteTexture);
                }
                meshRenderer.SetPropertyBlock(_materialPropertyBlock);
            }
        }
    }
}
#endif