using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{ 
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class MaterialFloatChannel:StageLightChannelBase
    {
#region Configs
#endregion

#region DoNotSaveToProfile-Configs
        [ChannelField(true, false)] public Renderer meshRenderer;
        [ChannelField(true, false)] public List<Renderer> syncMeshRenderers = new List<Renderer>();
        [ChannelField(true, false)] public int materialIndex;
#endregion

#region params
        [ChannelField(false)] private MaterialPropertyBlock _materialPropertyBlock;
        [ChannelField(false)] [SerializeField] private float floatValue = 1;
        // [ChannelFieldBehavior(false)] [SerializeField] private Color color = Color.white;
        [ChannelField(false)] [SerializeField] private string colorPropertyName = "_materialPropertyName";
        [ChannelField(false)] private Dictionary<Renderer,MaterialPropertyBlock> _materialPropertyBlocks = null;
        [ChannelField(false)] private int propertyId;
#endregion

        private List<Material> sharedMaterials = new();

        
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
            _materialPropertyBlock ??= new MaterialPropertyBlock();
            if(meshRenderer)meshRenderer.GetPropertyBlock(_materialPropertyBlock);

            _materialPropertyBlocks ??= new Dictionary<Renderer, MaterialPropertyBlock>();
            _materialPropertyBlocks.Clear();

            foreach (var meshRenderer in syncMeshRenderers)
            {
                if(meshRenderer == null || (_materialPropertyBlocks.TryGetValue(meshRenderer, out var prop) && prop != null)) continue;
                var materialPropertyBlock = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(materialPropertyBlock);

                _materialPropertyBlocks.Add(meshRenderer, materialPropertyBlock);
            }
            
            PropertyTypes.Clear();
            PropertyTypes.Add(typeof(MaterialColorProperty));
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
            if (_materialPropertyBlock == null || _materialPropertyBlocks == null)
            {
                Init();
            }

            if(_materialPropertyBlock ==null) return;

            if (meshRenderer)
            {
                meshRenderer.GetSharedMaterials(sharedMaterials);
                if(sharedMaterials.Count <= materialIndex) return;
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