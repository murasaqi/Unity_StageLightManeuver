using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StageLightManeuver;
using StageLightManeuver.StageLightTimeline.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace StageLightManeuver
{
    [CustomEditor(typeof(StageLightManeuverSettings), true)]
    public class StageLightManeuverSettingsEditor : Editor
    {
        public StageLightManeuverSettings stageLightManeuverSettings;
        private ReorderableList _reorderableSlmProperties;
        private bool _isExpandedReorderableProperties = false;
        private List<string> _slmPropertyTypes;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            stageLightManeuverSettings = target as StageLightManeuverSettings;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Runtime settings", EditorStyles.boldLabel);

            // Draw line
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Editor settings", EditorStyles.boldLabel);
            

            var exportProfilePathProp = serializedObject.FindProperty("exportProfilePath");
            EditorGUILayout.PropertyField(exportProfilePathProp);

            // Property order Setting
            EditorGUILayout.Space(SlmEditorStyleConst.Spacing);
            if (_reorderableSlmProperties == null)
            {
                var slmProperties = stageLightManeuverSettings.SlmPropertyOrder
                                        .OrderBy(x => x.Value)
                                        .ToDictionary(x => x.Key, x => x.Value).Keys
                                        .ToList();
                _reorderableSlmProperties = new ReorderableList(slmProperties, typeof(string), true, true, false, false);
                _reorderableSlmProperties.drawHeaderCallback = (rect) =>
                {
                    rect.x += 10;
                    _isExpandedReorderableProperties = EditorGUI.Foldout(rect, _isExpandedReorderableProperties, "SlmProperty Order");
                };
                _reorderableSlmProperties.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = _reorderableSlmProperties.list[index];
                    // 型名を表示
                    EditorGUI.LabelField(rect, element.ToString().Split('.').Last());
                };
            }

            if (_isExpandedReorderableProperties)
            {
                _reorderableSlmProperties.DoLayoutList();
            }
            else
            {
                // exec private method DoListHeader() from ReorderableList by reflection
                var rect = EditorGUILayout.GetControlRect();
                var method = _reorderableSlmProperties.GetType().GetMethod("DoListHeader", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(_reorderableSlmProperties, new object[] { rect });
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            }


            if (EditorGUI.EndChangeCheck())
            {
                UpdatePropertyOrder();
                stageLightManeuverSettings.SaveSlmPropertyOrder();
                serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(stageLightManeuverSettings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Repaint();
                ReloadPreviewInstances();
            }
        }

        private void UpdatePropertyOrder()
        {
            var slmPropertyOrder = stageLightManeuverSettings.SlmPropertyOrder;
            for (int i = 0; i < _reorderableSlmProperties.list.Count; i++)
            {
                var typeName = _reorderableSlmProperties.list[i] as string;
                slmPropertyOrder[typeName] = i;
            }

            stageLightManeuverSettings.SlmPropertyOrder = slmPropertyOrder;

            EditorUtility.SetDirty(stageLightManeuverSettings);
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }

}
