using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    [BaseTypeRequired(typeof(StageLightChannelBase))]
    public class ChannelFieldBehaviorAttribute : PropertyAttribute
    {
        public bool IsConfigField;
        private bool _saveToProfile;
        public bool SaveToProfile
        {
            get => _saveToProfile && IsConfigField;
            set => _saveToProfile = value;
        }

        public ChannelFieldBehaviorAttribute(bool isConfigField, bool saveToProfile=true)
        {
            IsConfigField = isConfigField;
            SaveToProfile = saveToProfile;
        }
    }
  
    [Serializable]
    [AddComponentMenu("")]
    public abstract class StageLightChannelBase: MonoBehaviour,IStageLightChannel
    {
        [HideInInspector] public List<Type> PropertyTypes = new List<Type>();
        public Queue<StageLightQueueData> stageLightDataQueue = new Queue<StageLightQueueData>();
        [HideInInspector]public int updateOrder = 0;
        public List<LightFixtureBase> SyncStageLight { get; set; }
        // [HideInInspector]public StageLightFixture ParentStageLight { get; set; }
        [HideInInspector]public float offsetDuration = 0f;
        [FormerlySerializedAs("parentStageLight")] [HideInInspector]public StageLightFixture parentStageLightFixture;
        // public int Index { get; set; }
        internal bool hasQue = false;
        public virtual void EvaluateQue(float currentTime)
        {

        }

        public virtual void UpdateChannel()
        {
            
        }

        public virtual void Init()
        {
            PropertyTypes.Clear();
            SyncStageLight = new List<LightFixtureBase>();
            foreach (var stageLightChannelBase in GetComponentsInChildren<LightFixtureBase>())
            {
                SyncStageLight.Add(stageLightChannelBase);
            }
        }
        
    }
}
