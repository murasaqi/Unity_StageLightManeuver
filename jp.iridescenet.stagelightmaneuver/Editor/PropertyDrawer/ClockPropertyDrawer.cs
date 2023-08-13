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
            var clockProperty = property.GetValue<object>() as ClockProperty;
            if (clockProperty == null) return;
            label.text = clockProperty.propertyName;

            DrawHeader(position, property, label);
            if (property.isExpanded == false) return;
            DrawToggleController(clockProperty);

            var fields = clockProperty.GetType().GetFields().ToList();
            var loopType = clockProperty.loopType.value;

            var useIndent = property.serializedObject.targetObject.GetType() != typeof(StageLightTimelineClip);
            if (useIndent) EditorGUI.indentLevel++;
            fields.ForEach(f =>
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
            });
            if (useIndent) EditorGUI.indentLevel--;
        }

    }
}
