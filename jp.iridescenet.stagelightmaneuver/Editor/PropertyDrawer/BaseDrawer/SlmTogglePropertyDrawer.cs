using System.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    /// <summary>
    /// Toggle付きPropertyDrawer
    /// </summary>
    public class SlmTogglePropertyDrawer : SlmBasePropertyDrawer, IToggleDrawer
    {
        /// <summary>
        /// 引数に<c>true</c>を指定すると折りたたみ可能になる
        /// </summary>
        /// <param name="canFoldout">折りたたみ可能か</param>
        public SlmTogglePropertyDrawer(bool canFoldout = true) : base(canFoldout) { }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawToggleHeader(position, property, label);
        }

        private void DrawToggleHeader(Rect position, SerializedProperty property, GUIContent label)
        {
            // Draw header
            base.OnGUI(position, property, GUIContent.none);

            position.x += 5;
            DrawToggle(position, property, label);
        }

        protected virtual void DrawHeader(Rect position, SerializedProperty property, GUIContent label)
        {
            // Draw header
            base.OnGUI(position, property, label);
        }

        public bool DrawToggle(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyOverride = property.FindPropertyRelative("propertyOverride");
            var isOverride = EditorGUI.ToggleLeft(position, label, propertyOverride.boolValue);

            if (propertyOverride.boolValue != isOverride)
            {
                propertyOverride.boolValue = isOverride;
                property.serializedObject.ApplyModifiedProperties();
            }

            return isOverride;
        }
    }
}
