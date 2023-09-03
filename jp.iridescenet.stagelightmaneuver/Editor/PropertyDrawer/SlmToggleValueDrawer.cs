using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    [CustomPropertyDrawer(typeof(SlmToggleValue<>), true)]
    public class SlmToggleValueDrawer : SlmBaseDrawer
    {
        protected static bool IsVerticalLayoutField(object value)
        {
            var hasVerticalLayoutType = (value.GetType() == typeof(MinMaxEasingValue) ||
                    value.GetType() == typeof(ClockOverride) ||
                    value.GetType().IsArray || value.GetType().IsGenericType);
            return hasVerticalLayoutType;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawSlmToggleValue(property);
        }

        /// <summary>
        /// <see ref="SlmToggleValue"/> の共通描画処理
        /// </summary>
        /// <param name="property"></param>
        protected static void DrawSlmToggleValue(SerializedProperty property)
        {
            if (property == null) return;

            var propertyOverride = property.FindPropertyRelative("propertyOverride");
            SerializedProperty value = property.FindPropertyRelative("value");
            if (value == null) return;
            // Debug.Log(value.type);
            var valueObject = GetValueFromCache(value);
            if (valueObject == null) return;

            GUILayout.Space(SlmEditorStyleConst.NoSpacing);
            if (propertyOverride != null)
            {
                if (valueObject.GetType() == typeof(SlmToggleValue<ClockProperty>))
                {
                    var slmToggleValue = valueObject as SlmToggleValue<ClockProperty>;
                    slmToggleValue.sortOrder = -999;
                    property.serializedObject.ApplyModifiedProperties();
                }

                var hasMultiLineObject = IsVerticalLayoutField(valueObject);
                if (!hasMultiLineObject) GUILayout.Space(SlmEditorStyleConst.NoSpacing);
                if (!hasMultiLineObject) EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();
                var isOverride = EditorGUILayout.ToggleLeft(property.displayName, propertyOverride.boolValue, GUILayout.Width(160));
                GUILayout.Space(SlmEditorStyleConst.NoSpacing);
                if (EditorGUI.EndChangeCheck())
                {
                    propertyOverride.boolValue = isOverride;
                    property.serializedObject.ApplyModifiedProperties();
                    // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                }

                if (hasMultiLineObject) EditorGUI.indentLevel++;

                EditorGUI.BeginDisabledGroup(!isOverride);

                if (valueObject.GetType().BaseType == typeof(SlmProperty))
                {
                    foreach (SerializedProperty childProperty in value)
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(childProperty);
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.serializedObject.ApplyModifiedProperties();
                            // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                        }
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(value, GUIContent.none);
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
                EditorGUI.EndDisabledGroup();
                if (!hasMultiLineObject) EditorGUILayout.EndHorizontal();
                // EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, 1));

                if (hasMultiLineObject) EditorGUI.indentLevel--;
                GUILayout.Space(SlmEditorStyleConst.SlmValuePropertyBottomMargin);
            }
            // else
            // {
            //     if (valueObject.GetType() == typeof(ArrayStaggerValue) ||
            //         valueObject.GetType() == typeof(StageLightOrderQueue))
            //     {
            //         EditorGUI.BeginChangeCheck();
            //         EditorGUILayout.PropertyField(value, GUIContent.none);
            //         if (EditorGUI.EndChangeCheck())
            //         {
            //             property.serializedObject.ApplyModifiedProperties();
            //         }
            //     }
            // }
        }
        
        public static void DrawOneLineSlmToggleValue(SerializedProperty serializedProperty,int marginBottom = 0)
        {
           
            var propertyOverride = serializedProperty.FindPropertyRelative("propertyOverride");
            if(propertyOverride == null) return;
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            var isOverride = EditorGUILayout.ToggleLeft(serializedProperty.displayName, propertyOverride.boolValue, GUILayout.Width(120));
            if (EditorGUI.EndChangeCheck())
            {
                propertyOverride.boolValue = isOverride;
                serializedProperty.serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("value"), GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                serializedProperty.serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var value = property.FindPropertyRelative("value");
            if (value == null) return SlmEditorStyleConst.NoMarginHeight;
            var valueObject = GetValueFromCache(value);
            if (valueObject == null) return SlmEditorStyleConst.NoMarginHeight;
            if (IsVerticalLayoutField(valueObject)) return SlmEditorStyleConst.NoMarginHeight;

            return SlmEditorStyleConst.Spacing;
        }
    }
}