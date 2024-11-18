using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace StageLightManeuver
{
    [CustomEditor(typeof(StageLightFixtureProfile), true)]
    public class StageLightFixtureProfileEditor : Editor
    {
        public StageLightFixtureProfile fixtureProfile;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            fixtureProfile = target as StageLightFixtureProfile;

            EditorGUI.BeginChangeCheck();
            EditorUtility.SetDirty(fixtureProfile);

            var channelData = fixtureProfile.LoadChannelData();
            EditorGUILayout.LabelField(channelData.Count + " channel settings saved in this profile.", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            foreach (var channel in channelData)
            {
                EditorGUILayout.LabelField("・ " + channel.channelType.Replace("StageLightManeuver.", ""));
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Repaint();
                ReloadPreviewInstances();
            }
        }
        void OnInspectorUpdate()
        {
            Repaint();
        }

    }

}