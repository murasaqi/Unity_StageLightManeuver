using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    [CustomEditor(typeof(StageLightChannelBase), true)]
    [CanEditMultipleObjects]
    public class StageLightChannelEditor: Editor
    {
        public StageLightChannelBase stageLightChannel;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck(); 
            EditorUtility.SetDirty(target);
            
            EditorGUILayout.LabelField(target.GetType().ToString(), EditorStyles.boldLabel);
            // 水平線
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            

            var fieldsInfo = serializedObject.targetObject.GetType().GetFields();
            foreach (var fieldInfo in fieldsInfo)
            {
                var serializedProperty = serializedObject.FindProperty(fieldInfo.Name);
                if (serializedProperty == null) continue;
                // if (fieldInfo.GetCustomAttributes(typeof(ChannelFieldBehaviorAttribute), false).Length > 0)
                // {
                    // EditorGUILayout.PropertyField(serializedProperty);
                // }
                var attribute = fieldInfo.GetCustomAttributes(typeof(ChannelFieldBehaviorAttribute), false);
                if (attribute.Length > 0)
                {
                    var channelFieldBehaviorAttribute = (ChannelFieldBehaviorAttribute) attribute[0];
                    if (channelFieldBehaviorAttribute.IsConfigField)
                    {
                        EditorGUILayout.PropertyField(serializedProperty);
                    }
                }
                else
                {
                    Debug.LogWarning(fieldInfo.Name + " is not have ChannelFieldBehaviorAttribute\nin " + target.GetType().ToString());
                    EditorGUILayout.PropertyField(serializedProperty);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}