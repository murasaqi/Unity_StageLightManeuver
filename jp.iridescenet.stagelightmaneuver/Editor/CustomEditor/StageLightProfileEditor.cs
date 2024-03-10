using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StageLightManeuver;
using StageLightManeuver.StageLightTimeline.Editor;
using UnityEditor;
using UnityEngine;


namespace StageLightManeuver
{
    [CustomEditor(typeof(StageLightProfile), true)]
    // Custom Editor stage light profile
    public class StageLightProfileEditor : Editor
    {
        public StageLightProfile stageLightProfile;
        public override void OnInspectorGUI()
        {

            serializedObject.Update();
            stageLightProfile = target as StageLightProfile;
            var stageLightProperties = stageLightProfile.stageLightProperties;
            var serializedProperty = serializedObject.FindProperty("stageLightProperties");

            EditorGUI.BeginChangeCheck();
            EditorUtility.SetDirty(stageLightProfile);
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