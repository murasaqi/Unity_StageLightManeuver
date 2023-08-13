using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    
  
    [Serializable]
    [AddComponentMenu("")]
    public abstract class StageLightFixtureBase: MonoBehaviour,IStageLightFixture
    {
        public List<Type> PropertyTypes = new List<Type>();
        public Queue<StageLightQueueData> stageLightDataQueue = new Queue<StageLightQueueData>();
        public int updateOrder = 0;
        public List<StageLightBase> SyncStageLight { get; set; }
        public StageLight ParentStageLight { get; set; }
        public float offsetDuration = 0f;
        [FormerlySerializedAs("parentStageLightFixture")] [FormerlySerializedAs("parentStageLightFx")]
        public StageLight parentStageLight;
        // public int Index { get; set; }
        public virtual void EvaluateQue(float currentTime)
        {

        }

        public virtual void UpdateFixture()
        {
            
        }

        public virtual void Init()
        {
            PropertyTypes.Clear();
            SyncStageLight = new List<StageLightBase>();
            foreach (var stageLightFixtureBase in GetComponentsInChildren<StageLightBase>())
            {
                SyncStageLight.Add(stageLightFixtureBase);
            }
        }
        
    }
}
