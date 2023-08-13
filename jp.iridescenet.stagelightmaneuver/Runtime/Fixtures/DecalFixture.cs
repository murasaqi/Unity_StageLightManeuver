using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if USE_HDRP
using UnityEngine.Rendering.HighDefinition;
#elif USE_URP
using UnityEngine.Rendering.Universal;
#endif
namespace StageLightManeuver
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class DecalFixture: StageLightFixtureBase
    {
        [FormerlySerializedAs("lightFixtureFixture")] [FormerlySerializedAs("lightFxFixture")]
        public LightFixture lightFixture;
        public Texture2D decalTexture;
        public Color decalColor = Color.white;
        public float decalSizeScaler = 1f;
        public float floorHeight = 0f;
        public float decalDepthScaler = 1f;
        public float fadeFactor = 1f;
        public float opacity = 1f;
        public float radius = 1f;
        public DecalProjector decalProjector;
        public Material decalMaterial;
        public bool autoDisableDecal = true;
        public float autoDisableDecalTime = 1f;
        private float _autoDisableDecalTime = 0f;
        private Material _instancedDecalMaterial = null;
        private float _radius = 1f;
        private float _depth = 1f;
        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }
        
        public override void EvaluateQue(float time)
        {
            if(decalMaterial == null || decalProjector == null || decalProjector.material == null) return;
            
            opacity = 0f;
            fadeFactor = 0f;
            decalSizeScaler = 0f;
            decalDepthScaler = 0f;
            floorHeight = 0f;
            radius = 0f;
            
            while (stageLightDataQueue.Count >0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var timeProperty = queueData.TryGetActiveProperty<ClockProperty>() as ClockProperty;
                var qDecalProperty = queueData.TryGetActiveProperty<DecalProperty>() as DecalProperty;
                var stageLightOrderProperty = queueData.TryGetActiveProperty<StageLightOrderProperty>() as StageLightOrderProperty;
                var index = stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLight) :  parentStageLight.order;
                if (qDecalProperty == null || timeProperty == null) continue;
                var weight = queueData.weight;
                
                var t = SlmUtility.GetNormalizedTime(time, queueData,typeof(DecalProperty), index);

                opacity += qDecalProperty.opacity.value * weight;
                fadeFactor += qDecalProperty.fadeFactor.value * weight;
                decalSizeScaler += qDecalProperty.decalSizeScaler.value * weight;
                decalDepthScaler += qDecalProperty.decalDepthScaler.value * weight;
                floorHeight += qDecalProperty.floorHeight.value * weight;
                radius += qDecalProperty.radius.value * weight;
                if(weight > 0.5f)decalTexture = qDecalProperty.decalTexture.value;
                _autoDisableDecalTime = autoDisableDecalTime;
            }

            if(lightFixture)decalColor = lightFixture.lightColor;

        }

        public override void UpdateFixture()
        {
            if(decalProjector ==null) return;
            
            var floor = new Vector3(0,floorHeight,0);
            var distance = Vector3.Distance(transform.position,floor);
            if (lightFixture != null)
            {
                var angle = lightFixture.spotAngle;
                _radius = Mathf.Tan(angle * Mathf.Deg2Rad) * distance * decalSizeScaler;     
            }else
            {
                _radius = radius;
            }

            _depth = distance * decalDepthScaler;
            decalProjector.size = new Vector3(_radius,_radius, _depth);
            decalProjector.fadeFactor = fadeFactor;
            if (lightFixture != null) decalProjector.fadeFactor *= lightFixture.lightIntensity; 
            decalProjector.pivot = new Vector3(0, 0, _depth / 2f);
            
            decalProjector.material.SetFloat("_Alpha",opacity*
                                                      Vector3.Distance(Vector3.zero, new Vector3(
                                                          Mathf.Clamp(decalColor.r,0,1f), 
                                                          Mathf.Clamp(decalColor.g,0,1), 
                                                          Mathf.Clamp(decalColor.b,0,1))));
            decalProjector.material.SetColor("_Color",decalColor);
            decalProjector.material.SetTexture("_MainTex",decalTexture);
            
            decalProjector.gameObject.SetActive(decalTexture != null);
        }
        
        private float opacityVelocity;
        private void LateUpdate()
        {
            
            if(!autoDisableDecal || decalProjector == null)  return;
            if (stageLightDataQueue.Count == 0)
            {
                if(_autoDisableDecalTime > 0f)_autoDisableDecalTime -= Time.deltaTime;
            }
            else
            {
                _autoDisableDecalTime = autoDisableDecalTime;
            }
            
            if (_autoDisableDecalTime <= 0f)
            {
                opacity = Mathf.SmoothDampAngle(opacity, 0f, ref opacityVelocity, 0.1f);

                decalProjector.material.SetFloat("_Alpha",opacity*
                                                          Vector3.Distance(Vector3.zero, new Vector3(
                                                              Mathf.Clamp(decalColor.r,0,1f), 
                                                              Mathf.Clamp(decalColor.g,0,1), 
                                                              Mathf.Clamp(decalColor.b,0,1))));
            }

        }

        public override void Init()
        {
            if(_instancedDecalMaterial != null) DestroyImmediate(_instancedDecalMaterial);
            if (decalProjector != null)
            {
                _instancedDecalMaterial = Material.Instantiate(decalMaterial);
                decalProjector.material = _instancedDecalMaterial;     
            }
            
            lightFixture = GetComponent<LightFixture>();
            PropertyTypes.Add( typeof(DecalProperty));
        }
        
        
    }
}
