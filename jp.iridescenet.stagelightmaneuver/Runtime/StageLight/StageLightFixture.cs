using System.ComponentModel;
using System.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// #if UNITY_EDITOR
// using UnityEditor;
// #endif
using UnityEngine;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public class StageLightFixture : StageLightFixtureBase, IStageLightFixture
    {
        [SerializeReference] private List<StageLightChannelBase> stageLightChannels = new List<StageLightChannelBase>();
        // private List<StageLightChannelBase> channelsBuffer = new List<StageLightChannelBase>();
        public bool isSync { get => syncReferenceProfile && lightFixtureProfile != null; }
        public List<StageLightChannelBase> StageLightChannels 
        {
            get
            {
                return stageLightChannels;
                // if (isSync)
                // {
                //     return channelsBuffer;
                // }
                // else
                // {
                //     return stageLightChannels;
                // }
            }
            set
            {
                stageLightChannels = value;

                // if (isSync)
                // {
                //     channelsBuffer = value;
                // }
                // else
                // {
                //     stageLightChannels = value;
                // }
            }
        }

        public LightFixtureProfile lightFixtureProfile;
        [SerializeField] private bool syncReferenceProfile = false;

#if UNITY_EDITOR
        [Delayed] public string profileExportPath = null;
#endif

        public int order = 0;

        [ContextMenu("Init")]
        public override void Init()
        {
            FindChannels();
            StageLightChannels.Sort((a, b) => a.updateOrder.CompareTo(b.updateOrder));
            foreach (var stageLightChannel in StageLightChannels)
            {
                stageLightChannel.Init();
                stageLightChannel.parentStageLightFixture = this;
            }

            stageLightFixtures = new List<StageLightFixture>() { this };

            // if (isSync)
            // {
            //     InitSyncChannel();
            // }
            // else
            // {
            //     channelsBuffer.Clear();
            // }
        }


        private void Start()
        {
            Init();
        }

        public override void AddQue(StageLightQueueData stageLightQueData)
        {
            // base.AddQue(stageLightQueData);
            foreach (var stageLightChannel in StageLightChannels)
            {
                if (stageLightChannel != null) stageLightChannel.stageLightDataQueue.Enqueue(stageLightQueData);
            }
        }

        public override void EvaluateQue(float time)
        {
            // base.EvaluateQue(time);
            foreach (var stageLightChannel in StageLightChannels)
            {
                if (stageLightChannel != null)
                {
                    stageLightChannel.EvaluateQue(time);
                    // stageLightChannel.Index = Index;
                }
            }
        }

        public override void UpdateChannel()
        {
            if (StageLightChannels == null) StageLightChannels = new List<StageLightChannelBase>();
            foreach (var stageLightChannel in StageLightChannels)
            {
                if (stageLightChannel) stageLightChannel.UpdateChannel();
            }
        }
        
        public override List<Type> GetAllPropertyType()
        {
            var types = new List<Type>();
            types.AddRange(StageLightChannels.SelectMany(channel => channel.PropertyTypes));
            return types;
        }


        private void Update()
        {
            // UpdateChannel();
        }

        private void OnDestroy()
        {
            // Debug.Log("On Destroy");
            // for (int i = StageLightChannels.Count-1; i >=0; i--)
            // {
            //     try
            //     {
            //         if(StageLightChannels[i]!= null)DestroyImmediate(StageLightChannels[i]);
            //     }
            //     catch (Exception e)
            //     {
            //         Console.WriteLine(e);
            //         throw;
            //     }
            // }
        }


        [ContextMenu("Find Channels")]
        public void FindChannels()
        {
            if (stageLightChannels != null)
            {
                stageLightChannels.Clear();
            }
            else
            {
                stageLightChannels = new List<StageLightChannelBase>();
            }
            stageLightChannels = GetComponents<StageLightChannelBase>().ToList();
        }

        // private void InitSyncChannel()
        // {
        //     if(lightFixtureProfile == null) return;

        //     channelsBuffer.Clear();
        //     channelsBuffer = new List<StageLightChannelBase>(stageLightChannels);
        //     foreach (var channel in channelsBuffer)
        //     {
        //         channel.parentStageLightFixture = this;
        //         channel.Init();
        //     }
        // }
    }
}