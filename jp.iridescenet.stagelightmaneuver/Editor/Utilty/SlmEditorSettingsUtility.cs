﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Disable warning CS8632
// CS8632 : '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
// ファイル全体を Nullable コンテキストにしたくないので一旦警告無視
#pragma warning disable 8632

namespace StageLightManeuver
{
    public static class SlmEditorSettingsUtility
    {
        private static string? stageLightManeuverSettingsPath = _defaultStageLightManeuverSettingsPath; //TODO CS8632: ここでNull許容型を使う必要があるか確認
        private const string _defaultStageLightManeuverSettingsPath = "Assets/StageLightManeuver/StageLightManeuverSettings.asset";

        /// <summary>
        /// <see cref="StageLightManeuverSettings"/>のアセットを返す。無ければ作成する。
        /// </summary>
        public static StageLightManeuverSettings GetStageLightManeuverSettingsAsset()
        {
            var stageLightManeuverSettingsAsset =
                AssetDatabase.LoadAssetAtPath<StageLightManeuverSettings>(stageLightManeuverSettingsPath);
            if (stageLightManeuverSettingsAsset == null)
            {
                var guids = AssetDatabase.FindAssets("t:StageLightManeuverSettings");
                // Debug.Log([StageLightManeuverSettings] Search Settings asset");
                if (guids.Length <= 0)
                {
                    var slmSettings = StageLightManeuverSettings.CreateInstance<StageLightManeuverSettings>();
                    stageLightManeuverSettingsPath = _defaultStageLightManeuverSettingsPath;
                    // Create Directory if not exist
                    var directoryPath = System.IO.Path.GetDirectoryName(stageLightManeuverSettingsPath);
                    if (!AssetDatabase.IsValidFolder(directoryPath))
                    {
                        AssetDatabase.CreateFolder("Assets", "StageLightManeuver");
                    }
                    AssetDatabase.CreateAsset(slmSettings, _defaultStageLightManeuverSettingsPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    stageLightManeuverSettingsAsset =
                        AssetDatabase.LoadAssetAtPath<StageLightManeuverSettings>(stageLightManeuverSettingsPath);
                    return stageLightManeuverSettingsAsset;
                }

                stageLightManeuverSettingsPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                stageLightManeuverSettingsAsset =
                    AssetDatabase.LoadAssetAtPath<StageLightManeuverSettings>(stageLightManeuverSettingsPath);
            }

            return stageLightManeuverSettingsAsset;
        }


        /// <summary>
        /// <paramref name="stageLightProperties"/>の順番を<paramref name="slmPropertyOrder"/>に従って登録する
        /// </summary>
        /// <returns></returns>
        private static List<SlmProperty> SetPropertyOrder(List<SlmProperty> stageLightProperties,
            Dictionary<string, int> slmPropertyOrder)
        {
            // stageLightProperties.RemoveAll(x => x == null);
            foreach (var slmProperty in stageLightProperties)
            {
                if (slmProperty == null) continue;
                if (slmPropertyOrder.ContainsKey(slmProperty.GetType().Name))
                {
                    slmProperty.propertyOrder = slmPropertyOrder[slmProperty.GetType().Name];
                }
                else
                {
                    slmProperty.propertyOrder = 9999;
                }
                
            }

            return stageLightProperties;
        }

        private static Dictionary<string, int> GetPropertyOrders()
        {
            Dictionary<string, int> slmPropertyOrder = null;
            var stageLightManeuverSettingsAsset = GetStageLightManeuverSettingsAsset();
            slmPropertyOrder = stageLightManeuverSettingsAsset.SlmPropertyOrder;
            return slmPropertyOrder;
        }


        /// <summary>
        /// <paramref name="stageLightProperties"/>に<see cref="StageLightManeuverSettings"/>の設定を適用して並び変えたリストを返す
        /// </summary>
        public static List<SlmProperty> SortByPropertyOrder(List<SlmProperty> stageLightProperties,
            in Dictionary<string, int> slmPropertyOrder)
        {
            stageLightProperties = SetPropertyOrder(stageLightProperties, slmPropertyOrder);
            stageLightProperties.Sort((x, y) => x.propertyOrder.CompareTo(y.propertyOrder));
            return stageLightProperties;
        }

        /// <summary>
        /// <paramref name="stageLightProperties"/>に<see cref="StageLightManeuverSettings"/>の設定を適用して並び変えたリストを返す
        /// </summary>
        public static List<SlmProperty> SortByPropertyOrder(List<SlmProperty> stageLightProperties)
        {
            var slmPropertyOrder = GetPropertyOrders();
            stageLightProperties = SetPropertyOrder(stageLightProperties, slmPropertyOrder);
            stageLightProperties.Sort((x, y) => x.propertyOrder.CompareTo(y.propertyOrder));
            return stageLightProperties;
        }
    }
}