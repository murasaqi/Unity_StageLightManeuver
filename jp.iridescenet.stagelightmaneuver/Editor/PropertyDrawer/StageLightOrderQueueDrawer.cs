using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    [CustomPropertyDrawer(typeof(StageLightOrderQueue))]
    public class StageLightOrderQueueDrawer : SlmBaseDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var stageLightOrderQueue = property.GetValue<object>() as StageLightOrderQueue;
            var settingListName = new List<string>();
            if (stageLightOrderQueue == null) return;
            settingListName.Add("(0) None");
            var stageLightOrderSettingList = stageLightOrderQueue.stageLightOrderSettingList;
            foreach (var stageLightOrderSetting in stageLightOrderSettingList)
            {
                var dropDownIndex = stageLightOrderSettingList.IndexOf(stageLightOrderSetting) + 1;
                settingListName.Add($"({dropDownIndex}) {stageLightOrderSetting.name}");
            }
            EditorGUI.BeginChangeCheck();
            var index = EditorGUILayout.Popup("Settings", stageLightOrderQueue.index + 1, settingListName.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                stageLightOrderQueue.index = index - 1;
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return SlmEditorStyleConst.NoMarginHeight;
        }

    }
}