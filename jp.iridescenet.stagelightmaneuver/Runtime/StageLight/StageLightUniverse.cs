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
        [FormerlySerializedAs("stageLight")] [FormerlySerializedAs("stageLightFx")] public StageLightFixture stageLightFixture;
    }
    [Serializable]
    public class StageLightFixtureOrderSetting
    {
        public string name = "New Stage Light Fixture Order";
        [FormerlySerializedAs("stageLightOrder")] public List<StageLightIndex> stageLightFixtureOrder = new List<StageLightIndex>();
    }
    
    [ExecuteAlways]
    public class StageLightUniverse: StageLightFixtureBase
    {
        [HideInInspector] public float weight = 0;
        // public List<StageLightFixture> AllStageLights => stageLightFixtures;
        
        [FormerlySerializedAs("stageLightOrderSettings")] 
        public List<StageLightFixtureOrderSetting> stageLightFixtureOrderSettings = new List<StageLightFixtureOrderSetting>();

        [ContextMenu("Initialize")]
        public override void Init()
        {
            stageLightFixtures ??= new List<StageLightFixture>();
            var index = 0;
            foreach (var stageLight in stageLightFixtures)
            {
                if (stageLight)
                {
                    stageLight.order = index;
                    stageLight.Init();
                    index++;
                }
                
            }
        }

        
        [ContextMenu("Find Stage Light Fixtures in Children")]
        public void FindStageLightsInChildren()
        {
            stageLightFixtures.Clear();
            stageLightFixtures.AddRange(GetComponentsInChildren<StageLightFixture>());
            Init();
        }
        
        private void OnValidate()
        {
            Init();
        }

        public override void AddQue(StageLightQueueData stageLightQueData)
        {
            weight = stageLightQueData.weight;
            foreach (var stageLight in stageLightFixtures)
            {
                if(stageLight != null)stageLight.AddQue(stageLightQueData);
            }
        }

        public override void EvaluateQue(float time)
        {
            foreach (var stageLight in stageLightFixtures)
            {
                 if(stageLight != null)stageLight.EvaluateQue(time);
            }
        }
        
        
        
        public override List<Type> GetAllPropertyType()
        {
            var types = new List<Type>();
            foreach (var stageLight in stageLightFixtures)
            {
                types.AddRange(stageLight.GetAllPropertyType());
            }

            // remove same type from list
            types = types.Distinct().ToList();
            return types;
        }

        public override void UpdateChannel()
        {
            foreach (var stageLightBase in stageLightFixtures)
            {
                if(stageLightBase != null)stageLightBase.UpdateChannel();
            }
        }
        void Update()
        {
            // if (a < stageLightSettings.Count)
            // {
            //     var stageLightSetting = stageLightSettings[a];
            //     foreach (var stageLightFixture in stageLightFixtures)
            //     {
            //         stageLightFixture.AddQue(stageLightSetting,fader);
            //     }
            // }
            //
            // if (b < stageLightSettings.Count)
            // {
            //     var stageLightSetting =stageLightSettings[b];
            //     foreach (var stageLightFixture in stageLightFixtures)
            //     {
            //         stageLightFixture.AddQue(stageLightSetting, 1f-fader);
            //     }
            // }
        }
    }
    
    
    
}