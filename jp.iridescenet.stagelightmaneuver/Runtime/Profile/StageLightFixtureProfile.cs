using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using UnityEngine;

// #if USE_HDRP
// using UnityEngine.Rendering.HighDefinition;
// #elif USE_HDRP
// using UnityEngine.Rendering.Universal;
// #endif


namespace StageLightManeuver
{
    // [Serializable]
    // public struct SerializedPropertyData
    // {
    //     public string type;
    //     public string jsonInfo;
    // }
    // [CreateAssetMenu(fileName = "New LightFixtureProfile", menuName = "StageLightManeuver/LightFixtureProfile")]
    public class StageLightFixtureProfile: ScriptableObject
    {
        // public string name;
        // [TextArea(3, 10)]
        // public string description;
       
        // public bool canUpdateGuiFlag = false;

        [SerializeField, HideInInspector]
        private List<string> serializedChannels = new List<string>();

        // [ContextMenu("Init")]
        public void Init()
        {
        }

        public void Init(List<StageLightChannelBase> channels)
        {
            SaveChannelData(channels);
            Init();
        }

        public StageLightFixtureProfile()
        {
            Init();
        }

        public StageLightFixtureProfile(List<StageLightChannelBase> channels)
        {
            Init(channels);
        }

        public void SaveChannelData(List<StageLightChannelBase> channels)
        {
            foreach(var channel in channels)
            {
                ChannelData channelData = SlmChannelSerialization.SerializeChannel(channel);
                var json = JsonUtility.ToJson(channelData);
                serializedChannels.Add(json);
            }
        }

        public List<ChannelData> LoadChannelData()
        {
            var channels = new List<ChannelData>();
            foreach (var json in serializedChannels)
            {
                ChannelData channelData = JsonUtility.FromJson<ChannelData>(json);
                channels.Add(channelData);
            }
            return channels;
        }

        public void RestoreChannelData(List<StageLightChannelBase> channels)
        {
            var channelDataCollection = LoadChannelData();
            foreach (var channelData in channelDataCollection)
            {
                var channel = channels.Find(x => x.GetType().ToString() == channelData.channelType);
                if (channel != null)
                {
                    SlmChannelSerialization.RestoreChannelData(channel, channelData);
                }
                else
                {
                    Debug.LogWarning("Channel Type is not found: " + channelData.channelType);
                } 
            }
        }

        // public LightFixtureProfile Clone()
        // {
        // }

        // public T TryGetChannel<T>() where T : StageLightChannelBase
        // {
        // }

        // public List<StageLightChannelBase> GetChannels()
        // {
        // }
    }
}
