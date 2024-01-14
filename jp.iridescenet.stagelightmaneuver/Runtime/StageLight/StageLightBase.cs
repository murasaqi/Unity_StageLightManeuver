using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public abstract class StageLightBase: MonoBehaviour,IStageLightChannel
    {
        
        // [SerializeField]private int index = 0;
        // public int Index { get => index; set => index = value; }
        [SerializeField]private List<StageLightBase> syncStageLight = new List<StageLightBase>();

        public List<StageLightBase> SyncStageLight
        {
            get => syncStageLight;
            set=> syncStageLight = value;
        }

        public virtual void Init()
        {
            
        }

        public virtual void UpdateChannel()
        {
            
        }

        public virtual void AddQue(StageLightQueueData stageLightQueData)
        {
            
            foreach (var stageLight in SyncStageLight)
            {
                if(stageLight!=null)stageLight.AddQue(stageLightQueData);
            }
        }

        public virtual void EvaluateQue(float time)
        {
            var i = 0;
            foreach (var stageLight in SyncStageLight)
            {
                // Debug.Log(stageLight.name);
                // stageLight.Index = i;
                stageLight.EvaluateQue(time);
                i++;
            }
           
        }
        
        
        [ContextMenu("Get StageLights in Children")]
        public void AddStageLightInChild()
        {
            syncStageLight.Clear();
            syncStageLight = GetComponentsInChildren<StageLightBase>().ToList();

            if (syncStageLight == null || syncStageLight.Count == 0)
                return;
            for (int i = syncStageLight.Count ; i > 0; i--)
            {
                if (syncStageLight[i-1] == this)
                {
                    syncStageLight.RemoveAt(i-1);
                }
            }
            
        }
        
       

    }
}
