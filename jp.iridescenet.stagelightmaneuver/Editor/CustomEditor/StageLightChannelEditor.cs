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


            var fieldsInfo = serializedObject.targetObject.GetType().GetFields();
            foreach (var fieldInfo in fieldsInfo)
            {
                var serializedProperty = serializedObject.FindProperty(fieldInfo.Name);
                if (serializedProperty == null) continue;
                // if (fieldInfo.GetCustomAttributes(typeof(ChannelFieldAttribute), false).Length > 0)
                // {
                // EditorGUILayout.PropertyField(serializedProperty);
                // }
                var attribute = fieldInfo.GetCustomAttributes(typeof(ChannelFieldAttribute), false);
                var drawHr = fieldInfo.GetCustomAttribute(typeof(HrAttribute)) != null;
                if (attribute.Length > 0)
                {
                    var channelFieldBehaviorAttribute = (ChannelFieldAttribute)attribute[0];
                    if (channelFieldBehaviorAttribute.IsConfigField)
                    {
                        drawPropertyField(drawHr, serializedProperty);
                    }
                }
                else if (fieldInfo.IsNotSerialized == false)
                {
                    Debug.LogWarning(fieldInfo.Name + " is not have ChannelFieldAttribute\nin " + target.GetType().ToString());
                    drawPropertyField(drawHr, serializedProperty);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            void drawPropertyField(bool drawHr, SerializedProperty serializedProperty)
            {
                if (drawHr)
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                }

                EditorGUILayout.PropertyField(serializedProperty);
            }
        }
    }
}