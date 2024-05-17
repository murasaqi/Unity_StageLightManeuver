using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;


namespace StageLightManeuver
{
    /// <summary>
    /// <see cref="StageLightChannelBase"/> のシリアライズ、デシリアライズ用の構造体
    /// </summary>
    [Serializable]
    public struct ChannelData
    {
        /// <summary>
        /// Channel Type
        /// </summary>
        public string channelType;

        /// <summary>
        /// フィールドの名前をキーとし、値をstring型(json)で保存する
        /// チャンネルのフィールドの情報を保存する
        /// </summary>
        [SerializeField]
        [HideInInspector]
        public SerializedDictionary<string, string> fieldInfo;
    }

    // 参照を保持するための構造体
    // 参照はヒエラルキーの相対的位置と名前を保持
    // [Serializable]
    // public struct ReferenceData
    // {
    //     public string path;
    //     public string name;
    //     public string type;
    //     public string jsonInfo;
    // }


    // 参照以外の情報をシリアライズ、デシリアライズするためのクラス
    // StageLightChannel は MonoBehaviour を継承しているため
    // リフレクションを使って フィールドの型と名前、値を取得してDictionaryに保存
    public static class SlmChannelSerialization
    {
        /// <summary>
        /// チャンネルの情報をシリアライズする
        /// </summary>
        /// <param name="channel">シリアライズするチャンネル</param>
        /// <returns>シリアライズされたチャンネル</returns>
        public static ChannelData SerializeChannel(StageLightChannelBase channel)
        {
            var channelData = new ChannelData
            {
                channelType = channel.GetType().ToString(),
                fieldInfo = new SerializedDictionary<string, string>()
            };

            FieldInfo[] fields = channel.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                // フィールド内が参照型の場合はシリアライズしない
                if (field.FieldType.IsClass && field.FieldType != typeof(string))
                {
                    continue;
                }

                // ChannelFieldAttribute を取得
                var attribute = field.GetCustomAttributes(typeof(ChannelFieldAttribute), false) as ChannelFieldAttribute[];

                // ChannelFieldAttribute が付与されていない場合はスキップ
                if (attribute.Length == 0)
                {
                    continue;
                }

                // フィールドが設定フィールドとしてマークされていて、
                // かつプロファイル保存フラグが立てられているときのみ保存する
                if (attribute[0].IsConfigField == false || attribute[0].SaveToProfile == false)
                {
                    continue;
                }

                var value = field.GetValue(channel);

                if (field.FieldType.IsPrimitive)
                {
                    channelData.fieldInfo[field.Name] = value.ToString();
                }
                else if (field.FieldType == typeof(string))
                {
                    channelData.fieldInfo[field.Name] = value as string;
                }
                else
                {
                    channelData.fieldInfo[field.Name] = JsonUtility.ToJson(value);
                }
            }

            return channelData;
        }

        /// <summary>
        /// チャンネルの情報をデシリアライズする
        /// </summary>
        /// <param name="channelData">デシリアライズするチャンネル</param>
        /// <returns>デシリアライズされたチャンネル</returns>
        // public static StageLightChannelBase DeserializeChannel(ChannelData channelData)
        // {
        // }

        /// <summary>
        /// チャンネルの情報を復元する
        /// </summary>
        /// <param name="channel">復元するチャンネル</param>
        /// <param name="channelData">復元元の<see cref="ChannelData"/></param>

        public static void RestoreChannelData(StageLightChannelBase channel, ChannelData channelData)
        {
            if (channel.GetType().ToString() != channelData.channelType)
            {
                Debug.LogError("Channel Type is not matched:\n" +
                               $"Expected: {channel.GetType().ToString()}\n" +
                               $"Actual: {channelData.channelType}\n");
                return;
            }

            FieldInfo[] fields = channel.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var type = field.FieldType;
                if (channelData.fieldInfo.ContainsKey(field.Name))
                {
                    if (type.IsPrimitive)
                    {
                        var strValue = channelData.fieldInfo[field.Name];
                        var value = Convert.ChangeType(strValue, type);
                        field.SetValue(channel, value);
                    }
                    else if (type == typeof(string))
                    {
                        var value = channelData.fieldInfo[field.Name];
                        field.SetValue(channel, value);
                    }
                    else
                    {
                        var json = channelData.fieldInfo[field.Name];
                        var value = JsonUtility.FromJson(json, type);
                        field.SetValue(channel, value);
                    }
                }
            }
        }
    }
}