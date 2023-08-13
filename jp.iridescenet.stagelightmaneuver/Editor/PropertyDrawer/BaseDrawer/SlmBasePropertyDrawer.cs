using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    /// <summary>
    /// ヘッダー付きDrawer
    /// </summary>
    public class SlmBasePropertyDrawer : SlmBaseDrawer, IHeaderDrawer
    {
        private bool _canFoldout;

        /// <summary>
        /// 引数に<c>true</c>を指定すると折りたたみ可能になる
        /// </summary>
        /// <param name="canFoldout">折りたたみ可能か</param>
        public SlmBasePropertyDrawer(bool canFoldout = true) => this._canFoldout = canFoldout;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var expanded = DrawHeader(position, property, label, canFoldout: _canFoldout);
            if (expanded != property.isExpanded)
            {
                property.isExpanded = expanded;
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        public bool DrawHeader(Rect position, SerializedProperty property, GUIContent label, Color? backgroundColor = null, bool canFoldout = false)
        {
            EditorGUI.DrawRect(position, backgroundColor ?? new Color(0.3f, 0.3f, 0.3f));
            if (canFoldout)
            {
                var expanded = EditorGUI.Foldout(position, property.isExpanded, label, toggleOnLabelClick: false);
                return expanded;
            }
            else
            {
                EditorGUI.LabelField(position, label);
                return false;
            }
        }
    }
}
