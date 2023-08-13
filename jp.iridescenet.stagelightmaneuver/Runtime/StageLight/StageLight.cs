using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public class StageLight: MonoBehaviour,IStageLight
    {
        
        [SerializeReference] private List<StageLightFixtureBase> stageLightFixtures = new List<StageLightFixtureBase>();
        public List<StageLightFixtureBase> StageLightFixtures { get => stageLightFixtures; set => stageLightFixtures = value; }
 
        public int order = 0;
        [ContextMenu("Init")]
        public void Init()
        {
            FindFixtures();
            stageLightFixtures.Sort( (a,b) => a.updateOrder.CompareTo(b.updateOrder));
            foreach (var stageLightFixture in StageLightFixtures)
            {
                stageLightFixture.Init();
                stageLightFixture.parentStageLight = this;
            }
        }


        private void Start()
        {
            Init();
        }

        public void AddQue(StageLightQueueData stageLightQueData)
        {
            // base.AddQue(stageLightQueData);
            foreach (var stageLightFixture in StageLightFixtures)
            {
                if(stageLightFixture != null)stageLightFixture.stageLightDataQueue.Enqueue(stageLightQueData);
            }
        }

        public void EvaluateQue(float time)
        {
            // base.EvaluateQue(time);
            foreach (var stageLightFixture in StageLightFixtures)
            {
                if (stageLightFixture != null)
                {
                    stageLightFixture.EvaluateQue(time);
                    // stageLightFixture.Index = Index;
                }
            }
        }

        public void UpdateFixture()
        {
            if(stageLightFixtures == null) stageLightFixtures = new List<StageLightFixtureBase>();
            foreach (var stageLightFixture in stageLightFixtures)
            {
                if(stageLightFixture)stageLightFixture.UpdateFixture();
            }
        }


        private void Update()
        {
            // UpdateFixture();
        }

        private void OnDestroy()
        {
            // Debug.Log("On Destroy");
            // for (int i = stageLightFixtures.Count-1; i >=0; i--)
            // {
            //     try
            //     {
            //         if(stageLightFixtures[i]!= null)DestroyImmediate(stageLightFixtures[i]);
            //     }
            //     catch (Exception e)
            //     {
            //         Console.WriteLine(e);
            //         throw;
            //     }
            // }
        }


        [ContextMenu("Find Fixtures")]
        public void FindFixtures()
        {
            if (stageLightFixtures != null)
            {
                StageLightFixtures.Clear();
            }
            else
            {
                stageLightFixtures = new List<StageLightFixtureBase>();
            }
            StageLightFixtures = GetComponents<StageLightFixtureBase>().ToList();
        }
    }
}