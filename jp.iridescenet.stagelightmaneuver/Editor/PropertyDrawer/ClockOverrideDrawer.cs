using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StageLightManeuver.StageLightTimeline.Editor;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    [CustomPropertyDrawer(typeof(ClockOverride))]
    public class ClockOverrideDrawer : SlmBaseDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var loopType = property.FindPropertyRelative("loopType");
            var childDepth = property.depth + 1;
            while (property.NextVisible(true) && property.depth >= childDepth)
            {
                if (property.depth == childDepth)
                {
                    if (loopType.propertyType == SerializedPropertyType.Enum && loopType.enumValueIndex == 3)
                    {
                        if (property.name == "arrayStaggerValue")
                        {
                            EditorGUILayout.PropertyField(property, GUIContent.none);
                        }

                        if (property.name == "loopType")
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(property);
                            if (EditorGUI.EndChangeCheck())
                            {
                                property.serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                    else
                    {
                        if (property.name == "arrayStaggerValue") continue;
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(property);
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.serializedObject.ApplyModifiedProperties();
                            // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                        }
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return SlmEditorStyleConst.NoMarginHeight;
        }
    }
}