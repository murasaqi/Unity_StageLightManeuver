using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public class StageLightFixture: StageLightFixtureBase,IStageLightFixture
    {
        [SerializeReference] private List<StageLightChannelBase> stageLightChannels = new List<StageLightChannelBase>();
        public List<StageLightChannelBase> StageLightChannels { get => stageLightChannels; set => stageLightChannels = value; }
 
        public int order = 0;
        [ContextMenu("Init")]
        public override void Init()
        {
            FindChannels();
            stageLightChannels.Sort( (a,b) => a.updateOrder.CompareTo(b.updateOrder));
            foreach (var stageLightChannel in StageLightChannels)
            {
                stageLightChannel.Init();
                stageLightChannel.parentStageLightFixture = this;
            }

            stageLightFixtures = new List<StageLightFixture>() { this };
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
                if(stageLightChannel != null)stageLightChannel.stageLightDataQueue.Enqueue(stageLightQueData);
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
            if(stageLightChannels == null) stageLightChannels = new List<StageLightChannelBase>();
            foreach (var stageLightChannel in stageLightChannels)
            {
                if(stageLightChannel)stageLightChannel.UpdateChannel();
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
            // for (int i = stageLightChannels.Count-1; i >=0; i--)
            // {
            //     try
            //     {
            //         if(stageLightChannels[i]!= null)DestroyImmediate(stageLightChannels[i]);
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
                StageLightChannels.Clear();
            }
            else
            {
                stageLightChannels = new List<StageLightChannelBase>();
            }
            StageLightChannels = GetComponents<StageLightChannelBase>().ToList();
        }

#if UNITY_EDITOR
        [ContextMenu("Save Profile (Testing)")]
        public void SaveProfile()
        {
            var channels = StageLightChannels;

            var profile = ScriptableObject.CreateInstance<LightFixtureProfile>();
            profile.Init(channels);

            var lightName = gameObject.name;
            var path = EditorUtility.SaveFilePanel("Save LightFixtureProfile Asset", "Asset", lightName, "asset");
            string fileName = Path.GetFileName(path);
            if(path == "") return;
            path = path.Replace("\\", "/").Replace(Application.dataPath, "Assets");
            // string dir = Path.GetDirectoryName(path);

            AssetDatabase.CreateAsset(profile, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }

        [ContextMenu("Load Profile (Testing)")]
        public void LoadProfile()
        {
            var path = EditorUtility.OpenFilePanel("Load LightFixtureProfile Asset", "Asset", "asset");
            if(path == "") return;
            path = path.Replace("\\", "/").Replace(Application.dataPath, "Assets");
            var profile = AssetDatabase.LoadAssetAtPath<LightFixtureProfile>(path);
            if(profile == null || profile.GetType() != typeof(LightFixtureProfile)) return;

            var channels = StageLightChannels;

            // gameObject に channels 内の各チャンネルをコンポーネントとして追加する
            // 同じチャンネルが存在する場合はフィールドをコピー

            // var listChannelData = profile.LoadChannelData();
            
            profile.RestoreChannelData(channels);
            Init(); 
        }
#endif
    }
}