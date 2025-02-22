using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    // [ExecuteAlways]
    [AddComponentMenu("")]
    public class MaterialTextureChannel:StageLightChannelBase
    {
#region DoNotSaveToProfile-Configs
        [ChannelField(true, false)] public List<MeshRenderer> meshRenderers;
        [ChannelField(true, false)] public int materialIndex;
        [ChannelField(true, false)] public Texture2D texture2D;
#endregion


#region Configs
        [ChannelField(true)] public string propertyName;
#endregion


#region params
        [ChannelField(false)] private Dictionary<MeshRenderer,MaterialPropertyBlock> _materialPropertyBlockDictionary = new Dictionary<MeshRenderer, MaterialPropertyBlock>();
#endregion

        private void Start()
        {
            Init();
        }

        protected void OnEnable()
        {
            Init();
        }

        public override void Init()
        {
            _materialPropertyBlockDictionary ??= new Dictionary<MeshRenderer, MaterialPropertyBlock>();
            _materialPropertyBlockDictionary.Clear();

            foreach (var meshRenderer in meshRenderers)
            {
                if(meshRenderer == null) continue;
                var materialPropertyBlock = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(materialPropertyBlock);

                _materialPropertyBlockDictionary.Add(meshRenderer, materialPropertyBlock);
            }

            PropertyTypes.Clear();
            PropertyTypes.Add(typeof(MaterialTextureProperty));
        }
        
        private void OnValidate()
        {
            Init();
        }
        
        public override void EvaluateQue(float currentTime)
        {
            texture2D = Texture2D.whiteTexture;

            while (stageLightDataQueue.Count>0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var timeProperty = queueData.TryGetActiveProperty<ClockProperty>();
                var materialTextureProperty = queueData.TryGetActiveProperty<MaterialTextureProperty>();
                var weight = queueData.weight;
                if (timeProperty == null || materialTextureProperty == null)
                {
                    return;
                };
                if (weight >= 0.5f)
                {
                    texture2D = materialTextureProperty.texture.value;
                    propertyName = materialTextureProperty.texturePropertyName.value;
                    if (materialIndex != materialTextureProperty.materialindex.value)
                    {
                        materialIndex = materialTextureProperty.materialindex.value;
                        Init();
                    }
                }
                
                
            }
        }

        public override void UpdateChannel()
        {
            
            if (_materialPropertyBlockDictionary == null )
            {
                Init();
            }

            foreach (var materialProperty in _materialPropertyBlockDictionary)
            {
                var meshRenderer = materialProperty.Key;
                var materialPropertyBlock = materialProperty.Value;
                
                if(meshRenderer ==null || materialPropertyBlock == null) continue;
                
                materialPropertyBlock.SetTexture(propertyName, texture2D);
                meshRenderer.SetPropertyBlock(materialPropertyBlock);

            }
            
            
        }

    }
    
    
}