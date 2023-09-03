using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    /// <summary>
    /// <see cref="SlmProperty"/>の配列を描画するプロパティドロワー
    /// </summary>
    // [CustomPropertyDrawer(typeof(List<SlmProperty>))]
    public class StageLightPropertiesDrawer : SlmBaseDrawer
    {
        bool isInitialized = false;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var stageLightProperties = GetValueFromCache(property) as List<SlmProperty>;
            stageLightProperties.RemoveAll(x => x == null);
            if (isInitialized == false)
            {
                stageLightProperties = SlmEditorSettingsUtility.SortByPropertyOrder(stageLightProperties);
                isInitialized = true;
            }
            for (int i = 0; i < stageLightProperties.Count; i++)
            {
                var slmProperty = stageLightProperties[i];
                if (slmProperty == null) continue;

                // EditorGUI.BeginDisabledGroup(slmProperty.isEditable == false);
                var serializedSlmProperty = property.GetArrayElementAtIndex(i);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedSlmProperty, true);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedSlmProperty.serializedObject.ApplyModifiedProperties();
                    // serializedSlmProperty.serializedObject.Update();
                }

                if (serializedSlmProperty.isExpanded)
                {
                    var propertyAttribute = slmProperty.GetType().GetCustomAttribute<SlmPropertyAttribute>();
                    if (propertyAttribute is { isRemovable: false }) continue;
                    var action = new Action(() =>
                    {
                        stageLightProperties.Remove(slmProperty);
                        return;
                    });
                    DrawRemoveButton(property.serializedObject, stageLightProperties, action);
                    GUILayout.Space(SlmEditorStyleConst.Spacing);
                }
                
                // EditorGUI.EndDisabledGroup();
            }

            GUILayout.Space(SlmEditorStyleConst.AddPropertyButtonTopMargin / 2);
            // EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), Color.gray);
            GUILayout.Space(SlmEditorStyleConst.AddPropertyButtonTopMargin / 2);
            DrawAddPropertyButton(property.serializedObject, stageLightProperties);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0f;
            var stageLightProperties = GetValueFromCache(property) as List<SlmProperty>;
            stageLightProperties.RemoveAll(x => x == null);

            for (int i = 0; i < stageLightProperties.Count; i++)
            {
                var slmProperty = stageLightProperties[i];
                if (slmProperty == null) continue;
                height += EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i));
                height += EditorGUIUtility.singleLineHeight;
            }
            height += EditorGUIUtility.singleLineHeight;
            return height;
        }

        private static void DrawRemoveButton(SerializedObject serializedObject, List<SlmProperty> properties, Action onRemove)
        {
            GUILayout.Space(SlmEditorStyleConst.Spacing);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Remove", GUILayout.Width(120)))
                {
                    onRemove?.Invoke();
                    serializedObject.Update();
                    serializedObject.ApplyModifiedProperties();

                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.Space(SlmEditorStyleConst.Spacing);
        }

        private void DrawAddPropertyButton(SerializedObject serializedObject, List<SlmProperty> stageLightProperties)
        {
            EditorGUI.BeginChangeCheck();
            var selectList = new List<string>();

            SlmEditorUtility.SlmPropertyTypes.ForEach(t =>
            {
                if (t != typeof(RollProperty)) selectList.Add(t.Name);
            });

            selectList.Insert(0, "Add Property");
            foreach (var property in stageLightProperties)
            {
                if (property == null) continue;
                if (selectList.Find(x => x == property.GetType().Name) != null)
                {
                    selectList.Remove(property.GetType().Name);
                }
            }
            EditorGUI.BeginDisabledGroup(selectList.Count <= 1);
            var select = EditorGUILayout.Popup(0, selectList.ToArray(), GUILayout.MinWidth(200));
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                var type = SlmEditorUtility.GetTypeByClassName(selectList[select]);
                var property = Activator.CreateInstance(type) as SlmProperty;

                if (property.GetType() == typeof(ManualLightArrayProperty))
                {
                    var manualLightArrayProperty = property as ManualLightArrayProperty;
                    var lightProperty = stageLightProperties.Find(x => x.GetType() == typeof(LightProperty)) as LightProperty;
                    var lightIntensityProperty = stageLightProperties.Find(x => x.GetType() == typeof(LightIntensityProperty)) as LightIntensityProperty;
                    if (lightProperty != null)
                    {
                        manualLightArrayProperty.initialValue.angle = lightProperty.spotAngle.value.constant;
                        manualLightArrayProperty.initialValue.innerAngle = lightProperty.innerSpotAngle.value.constant;
                        manualLightArrayProperty.initialValue.range = lightProperty.range.value.constant;
                    }
                    if (lightIntensityProperty != null)
                    {
                        manualLightArrayProperty.initialValue.intensity = lightIntensityProperty.lightToggleIntensity.value.constant;
                    }
                }
                stageLightProperties.Add(property);

                serializedObject.Update();
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}