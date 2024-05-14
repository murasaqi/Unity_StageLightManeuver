using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    [CustomPropertyDrawer(typeof(ClockProperty))]
    public class ClockPropertyDrawer : SlmPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = property.FindPropertyRelative("propertyName").stringValue;

            DrawHeader(position, property, label, withToggle: false);
            if (property.isExpanded == false) return;
            var clockProperty = GetValueFromCache(property) as ClockProperty;
            DrawToggleController(clockProperty);

            var fields = typeof(ClockProperty).GetFields().ToList();
            var loopType = (LoopType)property.FindPropertyRelative("loopType").FindPropertyRelative("value").enumValueIndex;

            var useIndent = property.serializedObject.targetObject.GetType() != typeof(StageLightTimelineClip);
            if (useIndent) EditorGUI.indentLevel++;
            foreach (var f in fields)
            {
                if (loopType == LoopType.FixedStagger)
                {
                    if (f.Name == "arrayStaggerValue" || f.Name == "loopType")
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(property.FindPropertyRelative(f.Name));
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    }
                }
                else
                {
                    if (f.Name != "arrayStaggerValue")
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(property.FindPropertyRelative(f.Name));
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    }
                }
            }
            if (useIndent) EditorGUI.indentLevel--;
        }

    }
}
