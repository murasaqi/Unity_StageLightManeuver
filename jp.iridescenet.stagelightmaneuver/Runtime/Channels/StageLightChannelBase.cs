using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    
  
    [Serializable]
    [AddComponentMenu("")]
    public abstract class StageLightChannelBase: MonoBehaviour,IStageLightChannel
    {
        [HideInInspector] public List<Type> PropertyTypes = new List<Type>();
        public Queue<StageLightQueueData> stageLightDataQueue = new Queue<StageLightQueueData>();
        [HideInInspector]public int updateOrder = 0;
        public List<LightFixtureBase> SyncStageLight { get; set; }
        // [HideInInspector]public LightFixture ParentStageLight { get; set; }
        [HideInInspector]public float offsetDuration = 0f;
        [FormerlySerializedAs("parentStageLight")] [HideInInspector]public LightFixture parentLightFixture;
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
