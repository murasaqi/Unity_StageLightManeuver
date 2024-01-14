using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public abstract class LightFixtureBase: MonoBehaviour
    {
        
        // [SerializeField]private int index = 0;
        // public int Index { get => index; set => index = value; }
        [SerializeField]private List<LightFixtureBase> syncStageLight = new List<LightFixtureBase>();

        public List<LightFixtureBase> SyncStageLight
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
                // Debug.Log(stageLightFixture.name);
                // stageLightFixture.Index = i;
                stageLight.EvaluateQue(time);
                i++;
            }
           
        }
        
        
        [ContextMenu("Get StageLights in Children")]
        public void AddStageLightInChild()
        {
            syncStageLight.Clear();
            syncStageLight = GetComponentsInChildren<LightFixtureBase>().ToList();

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
