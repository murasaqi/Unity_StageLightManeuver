using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    [CustomPropertyDrawer(typeof(ClockProperty))]
    public class StageLightMasterClockPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            // ヘッダーを描画
            var headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded, label, true);
            
            // 展開されている場合のみ内容を表示
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                
                // 必要な情報のみを表示
                var bpmProp = property.FindPropertyRelative("bpm");
                var bpmScaleProp = property.FindPropertyRelative("bpmScale");
                var offsetTimeProp = property.FindPropertyRelative("offsetTime");
                
                if (bpmProp != null)
                {
                    EditorGUILayout.PropertyField(bpmProp);
                }
                
                if (bpmScaleProp != null)
                {
                    EditorGUILayout.PropertyField(bpmScaleProp);
                }
                
                if (offsetTimeProp != null)
                {
                    EditorGUILayout.PropertyField(offsetTimeProp);
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            
            if (property.isExpanded)
            {
                // 展開時の高さを計算（ヘッダー + 3つのプロパティ）
                height += EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 3;
            }
            
            return height;
        }
    }
}