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
        public List<StageLightBase> SyncStageLight { get; set; }
        // [HideInInspector]public StageLight ParentStageLight { get; set; }
        [HideInInspector]public float offsetDuration = 0f;
        [HideInInspector]public StageLight parentStageLight;
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
            SyncStageLight = new List<StageLightBase>();
            foreach (var stageLightChannelBase in GetComponentsInChildren<StageLightBase>())
            {
                SyncStageLight.Add(stageLightChannelBase);
            }
        }
        
    }
}
