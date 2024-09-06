using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    public static class SlmEditorUtility
    {
        
        public static List<Type> SlmPropertyTypes = GetTypes(typeof(SlmProperty));
        public static List<Type> GetTypes(Type T)
        {
            var assemblyList = AppDomain.CurrentDomain.GetAssemblies();

            var typeList = new List<Type>();
            foreach ( var assembly in assemblyList )
            {
                
                //
                if ( assembly == null )
                {
                    continue;
                }
                

                var types = assembly.GetTypes();
                typeList.AddRange(types.Where(t => t.IsSubclassOf(T))
                    .ToList());
              
            }

            return typeList;
        }
        public static void DrawDefaultGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property = property.serializedObject.FindProperty(property.propertyPath);
            var fieldRect = position;
            fieldRect.height = EditorGUIUtility.singleLineHeight;

            using (new EditorGUI.PropertyScope(fieldRect, label, property))
            {
                if (property.hasChildren)
                {
                    // 子要素があれば折り畳み表示
                    property.isExpanded = EditorGUI.Foldout(fieldRect, property.isExpanded, label);
                }
                else
                {
                    // 子要素が無ければラベルだけ表示
                    EditorGUI.LabelField(fieldRect, label);
                    return;
                }

                fieldRect.y += EditorGUIUtility.singleLineHeight;
                fieldRect.y += EditorGUIUtility.standardVerticalSpacing;

                if (property.isExpanded)
                {

                    using (new EditorGUI.IndentLevelScope())
                    {
                        // 最初の要素を描画
                        property.NextVisible(true);
                        var depth = property.depth;
                        EditorGUI.PropertyField(fieldRect, property, true);
                        fieldRect.y += EditorGUI.GetPropertyHeight(property, true);
                        fieldRect.y += EditorGUIUtility.standardVerticalSpacing;

                        // それ以降の要素を描画
                        while (property.NextVisible(false))
                        {

                            // depthが最初の要素と同じもののみ処理
                            if (property.depth != depth)
                            {
                                break;
                            }

                            EditorGUI.PropertyField(fieldRect, property, true);
                            fieldRect.y += EditorGUI.GetPropertyHeight(property, true);
                            fieldRect.y += EditorGUIUtility.standardVerticalSpacing;
                        }
                    }
                }
            }
        }

        public static StageLightTimelineClip currentEditingClip;

        public static void OverwriteProperties(StageLightClipProfile stageLightClipProfileCopy,
            List<StageLightTimelineClip> selectedClips)
        {
            var properties = stageLightClipProfileCopy.stageLightProperties.FindAll(x => x.propertyOverride == true);

            foreach (var p in properties)
            {
                if (p.propertyOverride == false) continue;
                foreach (var selectedClip in selectedClips)
                {
                    if (selectedClip.StageLightQueueData == null) continue;

                    foreach (var property in selectedClip.StageLightQueueData.stageLightProperties)
                    {
                        if (property == null) continue;
                        if (property.GetType() == p.GetType())
                        {
                            property.OverwriteProperty(p);
                            selectedClip.forceTimelineClipUpdate = true;
                            break;
                        }
                    }

                }
            }
        }

        public static void DrawList(SerializedProperty self)
        {
            if (!self.isArray || self.propertyType == SerializedPropertyType.String)
            {
                EditorGUILayout.PropertyField(self, new GUIContent(self.displayName), true);
                return;
            }

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(self,
                    new GUIContent(string.Format("{0} [{1}]", self.displayName, self.arraySize)), false);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(EditorGUIUtility.TrIconContent("d_winbtn_graph_max_h"), "RL FooterButton",
                        GUILayout.Width(16)))
                {
                    self.isExpanded = true;
                    for (var i = 0; i < self.arraySize; i++)
                        self.GetArrayElementAtIndex(i).isExpanded = true;
                    return;
                }

                if (GUILayout.Button(EditorGUIUtility.TrIconContent("d_winbtn_graph_min_h"), "RL FooterButton",
                        GUILayout.Width(16)))
                {
                    self.isExpanded = false;
                    for (var i = 0; i < self.arraySize; i++)
                        self.GetArrayElementAtIndex(i).isExpanded = false;
                    return;
                }

                if (GUILayout.Button(EditorGUIUtility.TrIconContent("Toolbar Plus"), "RL FooterButton",
                        GUILayout.Width(16)))
                    self.InsertArrayElementAtIndex(self.arraySize);
            }

            if (!self.isExpanded)
                return;

            using (new EditorGUI.IndentLevelScope(1))
            {
                if (self.arraySize <= 0)
                    EditorGUILayout.LabelField("Array is Empty");

                for (var i = 0; i < self.arraySize; i++)
                {
                    var prop = self.GetArrayElementAtIndex(i);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(prop, new GUIContent(i.ToString()),
                            prop.propertyType != SerializedPropertyType.Generic);
                        if (GUILayout.Button(EditorGUIUtility.TrIconContent("Toolbar Minus"), "RL FooterButton",
                                GUILayout.Width(16)))
                        {
                            self.DeleteArrayElementAtIndex(i);
                            return;
                        }
                    }

                    if (prop.propertyType != SerializedPropertyType.Generic || !prop.isExpanded)
                        continue;
                    using (new EditorGUI.IndentLevelScope(1))
                    {
                        using (new GUILayout.VerticalScope("box"))
                        {
                            var skipCount = 0;
                            while (prop.NextVisible(true))
                            {
                                if (skipCount > 0)
                                {
                                    skipCount--;
                                    continue;
                                }

                                if (prop.depth != self.depth + 2)
                                    break;
                                if (prop.isArray && prop.propertyType != SerializedPropertyType.String)
                                {
                                    DrawList(prop);
                                    skipCount = prop.arraySize + 1;
                                    continue;
                                }

                                EditorGUILayout.PropertyField(prop, false);
                            }
                        }
                    }
                }
            }
        }

        public static void InitAndProperties(StageLightClipProfile stageLightClipProfileCopy,
            List<StageLightTimelineClip> selectedClips)
        {
            stageLightClipProfileCopy.stageLightProperties.Clear();
            var propertyTypes = new List<System.Type>();

            foreach (var selectedClip in selectedClips)
            {
                foreach (var property in selectedClip.StageLightQueueData.stageLightProperties)
                {
                    if (property == null) continue;
                    if (propertyTypes.Contains(property.GetType())) continue;
                    propertyTypes.Add(property.GetType());
                }
            }

            foreach (var propertyType in propertyTypes)
            {

                // var type = stageLightProperty.GetType();
                var slm = (Activator.CreateInstance(propertyType, BindingFlags.CreateInstance, null,
                        new object[] { }, null)
                    as SlmProperty);
                // var property = System.Activator.CreateInstance(propertyType) as SlmProperty;
                stageLightClipProfileCopy.TryAdd(slm);
            }

            // Repaint();
        }

        public static List<StageLightTimelineClip> SelectClips()
        {
            var select = Selection.objects.ToList();
            var selectedClips = new List<StageLightTimelineClip>();
            // selectedClips.Clear();
            foreach (var s in select)
            {
                if (s.GetType().ToString() == "UnityEditor.Timeline.EditorClip")
                {
                    var clip = s.GetType().GetField("m_Clip", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(s);

                    var timelineClip = clip as TimelineClip;
                    if (timelineClip == null) continue;
                    if (timelineClip.asset.GetType() == typeof(StageLightTimelineClip))
                    {
                        // stringBuilder.AppendLine(timelineClip.displayName);
                        var asset = timelineClip.asset as StageLightTimelineClip;

                        selectedClips.Add(asset);

                    }
                }

            }

            return selectedClips;
            // selectedClipsField.value = stringBuilder.ToString();

        }

        public static float GetDefaultPropertyHeight(SerializedProperty property, GUIContent label)
        {
            property = property.serializedObject.FindProperty(property.propertyPath);
            var height = 0.0f;

            // プロパティ名
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.standardVerticalSpacing;

            if (!property.hasChildren)
            {
                // 子要素が無ければラベルだけ表示
                return height;
            }

            if (property.isExpanded)
            {

                // 最初の要素
                property.NextVisible(true);
                var depth = property.depth;
                height += EditorGUI.GetPropertyHeight(property, true);
                height += EditorGUIUtility.standardVerticalSpacing;

                // それ以降の要素
                while (property.NextVisible(false))
                {
                    // depthが最初の要素と同じもののみ処理
                    if (property.depth != depth)
                    {
                        break;
                    }

                    Debug.Log(property.name);
                    height += EditorGUI.GetPropertyHeight(property, true);
                    height += EditorGUIUtility.standardVerticalSpacing;
                }

                // 最後はスペース不要なので削除
                height -= EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }

        public static bool DrawAddPropertyField(List<StageLightTimelineClip> stageLightTimelineClips, string pullDownName = "Add Property")
        {
            EditorGUI.BeginChangeCheck();
            var selectList = new List<string>();

            SlmPropertyTypes.ForEach(t => { selectList.Add(t.Name); });

            selectList.Insert(0, pullDownName);
            var isChanged = false;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(selectList.Count <= 1);
            var select = EditorGUILayout.Popup(0, selectList.ToArray(), GUILayout.Width(150));
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var stageLightTimelineClip in stageLightTimelineClips)
                {
                   var result =AddPropertyInClip(stageLightTimelineClip, selectList[select], false);

                   if (isChanged == false && result) isChanged = true;
                }
            }
            EditorGUILayout.Space();
            EditorGUI.EndChangeCheck();
            EditorGUILayout.EndHorizontal();
            return isChanged;
            // AssetDatabase.SaveAssets();
        }
        public static Type GetTypeByClassName( string className )
        {
            foreach( Assembly assembly in AppDomain.CurrentDomain.GetAssemblies() ) {
                foreach( Type type in assembly.GetTypes() ) {
                    if( type.Name == className ) {
                        return type;
                    }
                }
            }
            return null;
        }
        public static bool AddPropertyInClip(StageLightTimelineClip stageLightTimelineClip, string propertyTypeName, bool saveAssets = true)
        {
            Undo.RecordObject(stageLightTimelineClip, "Add Property");
            EditorUtility.SetDirty(stageLightTimelineClip);
            var type = GetTypeByClassName(propertyTypeName);
           

            var result =stageLightTimelineClip.StageLightQueueData.TryAddProperty(type);
            if (result && saveAssets)
            {
                EditorUtility.SetDirty(stageLightTimelineClip);

               AssetDatabase.SaveAssets();
                   
            }
            
            return result;
        }
    }
}