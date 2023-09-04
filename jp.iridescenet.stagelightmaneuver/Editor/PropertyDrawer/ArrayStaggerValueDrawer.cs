using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StageLightManeuver.StageLightTimeline.Editor;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    [CustomPropertyDrawer(typeof(ArrayStaggerValue))]
    public class ArrayStaggerValueDrawer : SlmBaseDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var childDepth = property.depth + 1;
            var arrayStaggerValue = GetValueFromCache(property) as ArrayStaggerValue;
            while (property.NextVisible(true) && property.depth >= childDepth)
            {
                if (property.depth == childDepth)
                {
                    if (property.name == "lightStaggerInfo" || property.name == "randomStaggerInfo")
                    {
                        if (arrayStaggerValue.staggerCalculationType == StaggerCalculationType.Random &&
                            property.name == "randomStaggerInfo")
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                GUILayout.FlexibleSpace();
                                if (GUILayout.Button("Set Random", GUILayout.Width(100)))
                                {
                                    arrayStaggerValue.CalculateRandomStaggerTime();
                                }
                                GUILayout.FlexibleSpace();
                            }
                            DrawStaggerMinMaxSliders(arrayStaggerValue, property);
                            EditorGUILayout.EndFoldoutHeaderGroup();
                        }
                        if (arrayStaggerValue.staggerCalculationType != StaggerCalculationType.Random &&
                            property.name == "lightStaggerInfo")
                        {
                            DrawStaggerMinMaxSliders(arrayStaggerValue, property);
                        }
                    }
                    else if (property.name == "staggerCalculationType")
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(property);
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    else
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(property);
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    }

                }
                // GUILayout.Space(SlmDrawerConst.NoSpacing);
            }
        }

        private static void DrawStaggerMinMaxSliders(ArrayStaggerValue arrayStaggerValue, SerializedProperty serializedProperty)
        {
            var expand = EditorGUILayout.Foldout(serializedProperty.isExpanded, serializedProperty.displayName);
            if (expand != serializedProperty.isExpanded)
            {
                serializedProperty.isExpanded = expand;
                serializedProperty.serializedObject.ApplyModifiedProperties();
            }

            if (!expand)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            EditorGUI.indentLevel++;
            var arrayValue = GetValueFromCache(serializedProperty) as List<Vector2>;
            if (arrayValue == null) return;


            foreach (var value in arrayValue)
            {
                var min = value.x;
                var max = value.y;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.MinMaxSlider(ref min, ref max, 0, 1f);
                if (EditorGUI.EndChangeCheck())
                {
                    var index = arrayValue.IndexOf(value);
                    serializedProperty.GetArrayElementAtIndex(index).vector2Value = new Vector2(min, max);
                    serializedProperty.serializedObject.ApplyModifiedProperties();
                }

            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return SlmEditorStyleConst.NoMarginHeight;
        }
    }
}