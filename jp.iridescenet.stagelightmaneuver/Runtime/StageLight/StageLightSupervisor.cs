using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;


namespace StageLightManeuver
{

    [Serializable]
    public class StageLightIndex
    {
        public int index = 0;
        [FormerlySerializedAs("stageLightChannel")] [FormerlySerializedAs("stageLightFx")] public StageLight stageLight;
    }
    [Serializable]
    public class StageLightOrderSetting
    {
        public string name = "New Stage Light Order";
        public List<StageLightIndex> stageLightOrder = new List<StageLightIndex>();
    }
    
    [ExecuteAlways]
    public class StageLightSupervisor: MonoBehaviour
    {
        public List<StageLight> stageLights = new List<StageLight>();

        [HideInInspector] public float weight = 0;
        // public List<StageLight> AllStageLights => stageLights;
        
        public List<StageLightOrderSetting> stageLightOrderSettings = new List<StageLightOrderSetting>();

        [ContextMenu("Initialize")]
        public void Init()
        {
            
            
            
            var index = 0;
            foreach (var stageLight in stageLights)
            {
                if (stageLight)
                {
                    stageLight.order = index;
                    stageLight.Init();
                    index++;
                }
                
            }
        }

        
        [ContextMenu("Find Stage Lights in Children")]
        public void FindStageLightsInChildren()
        {
            stageLights.Clear();
            stageLights.AddRange(GetComponentsInChildren<StageLight>());
            Init();
        }
        
        private void OnValidate()
        {
            Init();
        }

        public void AddQue(StageLightQueueData stageLightQueData)
        {
            weight = stageLightQueData.weight;
            foreach (var stageLight in stageLights)
            {
                if(stageLight != null)stageLight.AddQue(stageLightQueData);
            }
        }

        public void EvaluateQue(float time)
        {
            foreach (var stageLight in stageLights)
            {
                 if(stageLight != null)stageLight.EvaluateQue(time);
            }
        }
        
        
        
        public List<Type> GetAllPropertyType()
        {
            var types = new List<Type>();
            foreach (var stageLight in stageLights)
            {

                if (stageLight.GetType() == typeof(StageLight))
                {
                    StageLight sl = (StageLight) stageLight;
                    types.AddRange(sl.StageLightChannels.SelectMany(channel => channel.PropertyTypes));
                }
            }

            // remove same type from list
            types = types.Distinct().ToList();
            return types;
        }

        public void UpdateChannel()
        {
            foreach (var stageLightBase in stageLights)
            {
                if(stageLightBase != null)stageLightBase.UpdateChannel();
            }
        }
        void Update()
        {
            // if (a < stageLightSettings.Count)
            // {
            //     var stageLightSetting = stageLightSettings[a];
            //     foreach (var stageLight in stageLights)
            //     {
            //         stageLight.AddQue(stageLightSetting,fader);
            //     }
            // }
            //
            // if (b < stageLightSettings.Count)
            // {
            //     var stageLightSetting =stageLightSettings[b];
            //     foreach (var stageLight in stageLights)
            //     {
            //         stageLight.AddQue(stageLightSetting, 1f-fader);
            //     }
            // }
        }
    }
    
    
    
}