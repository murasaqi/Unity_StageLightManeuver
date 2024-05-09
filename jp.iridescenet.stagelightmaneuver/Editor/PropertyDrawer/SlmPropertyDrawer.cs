using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    /// <summary>
    /// StageLightPropertyの基底Drawer
    /// </summary>
    [CustomPropertyDrawer(typeof(SlmProperty), true)]
    public class SlmPropertyDrawer : SlmTogglePropertyDrawer
    {
        public SlmPropertyDrawer() : base(true) { }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get SlmProperty from SerializedObject
            label.text = property.FindPropertyRelative("propertyName").stringValue;

            //  Draw header
            DrawHeader(position, property, label);
            if (property.isExpanded == false) return;

            var propertyOverride = property.FindPropertyRelative("propertyOverride").boolValue;
            EditorGUI.BeginDisabledGroup(propertyOverride == false);

            var slmProperty = GetValueFromCache(property) as SlmProperty;
            DrawToggleController(slmProperty);

            var fields = slmProperty.GetType().GetFields().ToList();
            var clockOverride = fields.Find(x => x.FieldType == typeof(SlmToggleValue<ClockOverride>));
            if (clockOverride != null)
            {
                fields.Remove(clockOverride);
                fields.Insert(0, clockOverride);
            }

            var useIndent = property.serializedObject.targetObject.GetType() != typeof(StageLightTimelineClip);
            if (useIndent) EditorGUI.indentLevel++;
            // EditorGUI.indentLevel++;
            foreach (var f in fields)
            {
                // Draw SlmToggleValue
                EditorGUI.BeginChangeCheck();
                try
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative(f.Name), true);
                }
                catch (NullReferenceException e)
                {
                    Debug.LogWarning(slmProperty.propertyName + "." + f.Name + " is null.\n" + e.Message);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            // EditorGUI.indentLevel--;
            if (useIndent) EditorGUI.indentLevel--;

            GUILayout.Space(SlmEditorStyleConst.SlmPropertyBottomMargin);
            EditorGUI.EndDisabledGroup();
        }

        protected void DrawHeader(Rect position, SerializedProperty property, GUIContent label, bool withToggle = true)
        {
            if (withToggle)
            {
                base.OnGUI(position, property, label);
            }
            else
            {
                base.DrawHeader(position, property, label);
            }
        }

        protected static void DrawToggleController(SlmProperty slmProperty)
        {
            GUILayout.Space(SlmEditorStyleConst.Spacing);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUIStyle style = new GUIStyle();
                style.normal.background = null;
                style.fixedWidth = 40;
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = Color.gray;
                // GUILayout.FlexibleSpace();
                if (GUILayout.Button("All", style))
                {
                    slmProperty.ToggleOverride(true);
                }

                GUILayout.Space(SlmEditorStyleConst.Spacing);
                if (GUILayout.Button("None", style))
                {
                    slmProperty.ToggleOverride(false);
                    slmProperty.propertyOverride = true;
                }
            }
            GUILayout.Space(SlmEditorStyleConst.Spacing);
        }
    }
}
