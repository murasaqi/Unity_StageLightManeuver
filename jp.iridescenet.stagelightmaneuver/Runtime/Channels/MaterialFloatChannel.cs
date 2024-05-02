using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{ 
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class MaterialFloatChannel:StageLightChannelBase
    {
        public Renderer meshRenderer;
        public List<Renderer> syncMeshRenderers = new List<Renderer>();
        public int materialIndex;
        private MaterialPropertyBlock _materialPropertyBlock;
        [SerializeField] private float floatValue = 1;
        // [SerializeField] private Color color = Color.white;
        [SerializeField] private string colorPropertyName = "_materialPropertyName";
        private Dictionary<Renderer,MaterialPropertyBlock> _materialPropertyBlocks = null;
        private int propertyId;
        
        void Start()
        {
            Init();
        }
        
        private void OnEnable()
        {
            Init();
        }
        
        [ContextMenu("Get Mesh Renderer")]
        public void GetMeshRenderer()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        
        public override void Init()
        {
            
            _materialPropertyBlock = new MaterialPropertyBlock();
            if(meshRenderer)meshRenderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlocks = new Dictionary<Renderer, MaterialPropertyBlock>();
            foreach (var meshRenderer in syncMeshRenderers)
            {
                if(meshRenderer == null) continue;
                var materialPropertyBlock = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(materialPropertyBlock);
                _materialPropertyBlocks.Add(meshRenderer,materialPropertyBlock);
            }
            
            PropertyTypes.Add(typeof(MaterialFloatProperty));
        }
        
        private void OnValidate()
        {
            Init();
        }
        
        
        public override void EvaluateQue(float currentTime)
        {
            floatValue = 0;

            while(stageLightDataQueue.Count > 0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var clockProperty = queueData.TryGetActiveProperty<ClockProperty>();
                if(clockProperty == null) continue;
                var materialFloatProperty = queueData.TryGetActiveProperty<MaterialFloatProperty>();
                if(materialFloatProperty == null) continue;
                var weight = queueData.weight;
                var stageLightOrderProperty = queueData.TryGetActiveProperty<StageLightOrderProperty>();
                var index = stageLightOrderProperty!= null ? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLightFixture) : parentStageLightFixture.order;

                var t = SlmUtility.GetNormalizedTime(currentTime, queueData, typeof(MaterialFloatProperty), index);
                floatValue += materialFloatProperty.floatValue.value.Evaluate(t) * weight;
            }
        }

        public override void UpdateChannel()
        {
            if (_materialPropertyBlock == null || _materialPropertyBlocks == null) return;
            {
                Init();
            }
            if(_materialPropertyBlock ==null) return;

            if (meshRenderer)
            {
                if(meshRenderer.sharedMaterials.Length <= materialIndex) return;
                meshRenderer.GetPropertyBlock(_materialPropertyBlock,materialIndex);
                _materialPropertyBlock.SetFloat(colorPropertyName,floatValue);
                meshRenderer.SetPropertyBlock(_materialPropertyBlock,materialIndex);
            }
            
            foreach (var materialPropertyBlock in _materialPropertyBlocks)
            {
                if(materialPropertyBlock.Key == null) continue;
                if(materialPropertyBlock.Key.sharedMaterials.Length <= materialIndex) continue;
                materialPropertyBlock.Key.GetPropertyBlock(materialPropertyBlock.Value,materialIndex);
                materialPropertyBlock.Value.SetFloat(colorPropertyName,floatValue);
                materialPropertyBlock.Key.SetPropertyBlock(materialPropertyBlock.Value,materialIndex);
            }
        }
    }
}