using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    [CustomEditor(typeof(StageLightChannelBase), true)]
    [CanEditMultipleObjects]
    public class StageLightChannelEditor : Editor
    {
        public StageLightChannelBase stageLightChannel;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorUtility.SetDirty(target);

            // EditorGUILayout.LabelField(target.GetType().ToString(), EditorStyles.boldLabel);
            // // 水平線
            // EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            var channel = target as StageLightChannelBase;
            var fixture = channel.GetComponent("StageLightFixture") as StageLightFixture;
            var isSync = fixture.isSync;

            var fieldsInfo = serializedObject.targetObject.GetType().GetFields();
            foreach (var fieldInfo in fieldsInfo)
            {
                var serializedProperty = serializedObject.FindProperty(fieldInfo.Name);
                if (serializedProperty == null) continue;

                var attributes = fieldInfo.GetCustomAttributes(typeof(ChannelFieldAttribute), false);
                var drawHr = fieldInfo.GetCustomAttribute(typeof(HrAttribute)) != null;

                if (attributes.Length > 0)
                {
                    var channelFieldAttribute = (ChannelFieldAttribute)attributes[0];
                    drawPropertyField(drawHr, fieldInfo, channelFieldAttribute);
                }
                else if (fieldInfo.IsNotSerialized == false)
                {
                    drawPropertyField(drawHr, fieldInfo);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            void drawPropertyField(bool drawHr, FieldInfo fieldInfo, ChannelFieldAttribute channelFieldAttribute = null)
            {
                var serializedProperty = serializedObject.FindProperty(fieldInfo.Name);
                if (channelFieldAttribute == null)
                {
                    Debug.LogWarning(serializedProperty.displayName + " is not have ChannelFieldAttribute\nin " + target.GetType().ToString());
                    drawField(fieldInfo);
                    return;
                }

                if (drawHr)
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                }

                if (channelFieldAttribute.IsConfigField && channelFieldAttribute.SaveToProfile && isSync)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    drawField(fieldInfo);
                    EditorGUI.EndDisabledGroup();
                }
                else if (channelFieldAttribute.IsConfigField)
                {
                    drawField(fieldInfo);
                }

                void drawField(FieldInfo fi)
                {
                    var value = fi.GetValue(target);
                    var valueType = value?.GetType();
                    var drawerType = SlmEditorUtility.GetPropertyDrawerTypeForType(valueType);

                    if (drawerType != null)
                    {
                        var drawer = Activator.CreateInstance(drawerType) as PropertyDrawer;
                        drawer.OnGUI(GUILayoutUtility.GetRect(0, 0), serializedProperty, new GUIContent(serializedProperty.displayName));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(serializedProperty);
                    }
                }
            }
        }
    }
}