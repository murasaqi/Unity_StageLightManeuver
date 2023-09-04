using System.Globalization;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    /// <summary>
    /// <see cref="StageLightManeuver"/>のUI用定数を保持する
    /// </summary>
    public struct SlmEditorStyleConst
    {
        public const float Spacing = 2f;

        // public static readonly float SingleLineHeight = EditorGUIUtility.singleLineHeight;
        // public static readonly float SpacingHeight = SingleLineHeight + Spacing;
        public const float NoSpacing = -4f;
        public const float NoMarginHeight = 0f;
        public const float SlmValuePropertyBottomMargin = 4f;
        public const float SlmPropertyBottomMargin = 4f;
        public const float AddPropertyButtonTopMargin = 4f;
    }

    /// <summary>
    /// プロパティーの名前とPositionのみを保持する規定Drawer
    /// </summary> 
    public class SlmBaseDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.LabelField(position, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected static Type GetPropertyDrawerTypeForType(Type valueType)
        {
            var scriptAttributeUtilityType = typeof(EditorGUI).Assembly.GetType("UnityEditor.ScriptAttributeUtility");

            var getDrawerTypeForTypeMethod = scriptAttributeUtilityType.GetMethod("GetDrawerTypeForType",
                BindingFlags.Static | BindingFlags.NonPublic);
            var drawerType = getDrawerTypeForTypeMethod.Invoke(null, new object[] { valueType }) as Type;
            return drawerType;
        }

        /// <summary>
        /// <see ref="SerializedProperty"/>からシリアライズ前の値を<see ref="CachedValue"/>から取得する
        /// </summary>
        protected static object GetValueFromCache(SerializedProperty property)
        {
            var hash = property.serializedObject.targetObject.GetHashCode();
            var key = property.propertyPath;
            object val = null;

            Dictionary<string, object> clipValue = null;
            if (CachedValue.TryGetValue(hash, out clipValue) == false)
            {
                clipValue = new Dictionary<string, object>();
                CachedValue.Add(hash, clipValue);

                val = property.GetValue<object>();
                clipValue.Add(key, val);
                return val;
            }

            if (clipValue.TryGetValue(key, out val) == false)
            {
                val = property.GetValue<object>();
                clipValue.Add(key, val);
            }

            return val;
        }

        /// <summary>
        /// <see cref="CachedValue"/> をクリア
        /// </summary>
        public static void ClearCache()
        {
            CachedValue.Clear();
        }

        /// <summary>
        /// <see cref="SerializedPropertyExtensions.GetValue"/> の結果をキャッシュする Dictionary
        /// </summary>
        private static readonly Dictionary<int, Dictionary<string, object>> CachedValue = new();
    }


    /// <summary>
    /// <see cref="SlmValueAttribute"/>が付与されたプロパティーの表示を変更
    /// </summary>
    [CustomPropertyDrawer(typeof(SlmValueAttribute), true)]
    public class SlmValueAttributeDrawer : SlmBaseDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = (SlmValueAttribute)this.attribute;
            if (attribute.isHidden) return;
            var displayName = attribute.name;
            if (displayName == null) displayName = label.text;
            var displayLabel = new GUIContent(displayName);


            var value = GetValueFromCache(property);
            var valueType = value.GetType();
            var drawerType = GetPropertyDrawerTypeForType(valueType);
            if (drawerType != null)
            {
                var drawer = Activator.CreateInstance(drawerType) as PropertyDrawer;
                drawer.OnGUI(position, property, displayLabel);
            }
            else
            {
                // Debug.Log($"PropertyDrawer not found for {valueType}");
                EditorGUILayout.PropertyField(property, displayLabel);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attribute = (SlmValueAttribute)this.attribute;
            if (attribute.isHidden)
            {
                return -2f;
            }
            else
            {
                var val = GetValueFromCache(property);
                var valueType = val.GetType();
                var drawerType = GetPropertyDrawerTypeForType(valueType);
                if (drawerType != null)
                {
                    var drawer = Activator.CreateInstance(drawerType) as PropertyDrawer;
                    return drawer.GetPropertyHeight(property, label);
                }
                else
                {
                    // return base.GetPropertyHeight(property, label);
                    return 0f;
                }
            }
        }
    }
}