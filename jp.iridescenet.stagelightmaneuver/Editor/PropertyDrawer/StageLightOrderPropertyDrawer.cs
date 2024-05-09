using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    [CustomPropertyDrawer(typeof(StageLightOrderProperty))]
    public class StageLightOrderPropertyDrawer : SlmPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = property.FindPropertyRelative("propertyName").stringValue;

            DrawHeader(position, property, label, withToggle: false);
            if (property.isExpanded == false) return;
            var stageLightOrderProperty = GetValueFromCache(property) as StageLightOrderProperty;
            // DrawToggleController(stageLightOrderProperty);

            var fields = typeof(StageLightOrderProperty).GetFields().ToList();

            var useIndent = property.serializedObject.targetObject.GetType() != typeof(StageLightTimelineClip);
            if (useIndent) EditorGUI.indentLevel++;
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
                    Debug.LogWarning(stageLightOrderProperty.propertyName + "." + f.Name + " is null.\n" + e.Message);
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

    }
}
