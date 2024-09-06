using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StageLightManeuver;
using StageLightManeuver.StageLightTimeline.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;


namespace StageLightManeuver
{
    [CustomEditor(typeof(StageLightClipProfile), true)]
    // Custom Editor stage light profile
    public class StageLightClipProfileEditor : Editor
    {
        [FormerlySerializedAs("stageLightProfile")] public StageLightClipProfile stageLightClipProfile;
        public override void OnInspectorGUI()
        {

            serializedObject.Update();
            stageLightClipProfile = target as StageLightClipProfile;
            var stageLightProperties = stageLightClipProfile.stageLightProperties;
            var serializedProperty = serializedObject.FindProperty("stageLightProperties");

            EditorGUI.BeginChangeCheck();
            EditorUtility.SetDirty(stageLightClipProfile);
            var drawer = new StageLightPropertiesDrawer();
            drawer.OnGUI(EditorGUILayout.GetControlRect(), serializedProperty, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                serializedProperty.serializedObject.ApplyModifiedProperties();

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