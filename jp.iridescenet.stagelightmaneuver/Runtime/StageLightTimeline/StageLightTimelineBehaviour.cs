using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    [Serializable]
    public class StageLightTimelineBehaviour : PlayableBehaviour
    {
        [FormerlySerializedAs("stageLightQueData")] [SerializeField] public StageLightQueueData stageLightQueueData = new StageLightQueueData();
        public StageLightQueueData syncStageLightQueData = new StageLightQueueData();
        public override void OnPlayableCreate(Playable playable)
        {
            // stageLightProfile = ScriptableObject.CreateInstance<StageLightProfile>();
            // stageLightProfile.name = "StageLightProfile";
            // stageLightProfile.stageLightProperties = new List<SlmProperty>();
        }
        
        
        public void Init()
        {
            stageLightQueueData = new StageLightQueueData();
            syncStageLightQueData = new StageLightQueueData();
        }
        
        
        
        
        public void CheckRequiredProperties()
        {
            if(stageLightQueueData == null || stageLightQueueData.stageLightProperties == null)
            {
                Init();
            }
            if(stageLightQueueData.stageLightProperties.Find(x => x?.GetType() == typeof(ClockProperty)) ==
               null)
            {
                stageLightQueueData.stageLightProperties.Insert(0,new ClockProperty());
            }
            
            if (stageLightQueueData.stageLightProperties.Find(x => x?.GetType() == typeof(StageLightOrderProperty)) ==
                null)
            {
                stageLightQueueData.stageLightProperties.Insert(1,new StageLightOrderProperty());
            }
            
            
        }
        public void RemoveNullProperties()
        {
            stageLightQueueData.stageLightProperties.RemoveAll(item => item == null);
        }
    }

}