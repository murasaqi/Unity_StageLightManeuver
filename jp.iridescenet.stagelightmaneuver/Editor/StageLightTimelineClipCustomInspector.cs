using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace StageLightManeuver.StageLightTimeline.Editor
{
    public abstract class EditorGUIWidth : System.IDisposable
    {
        protected abstract void ApplyWidth(float width);
        public EditorGUIWidth(float width) { ApplyWidth(width); }
        public void Dispose() { ApplyWidth(0.0f); }
    }

    public class LabelWidth : EditorGUIWidth
    {
        public LabelWidth(float width) : base(width) { }
        protected override void ApplyWidth(float width) { EditorGUIUtility.labelWidth = width; }
    }


    [CustomEditor(typeof(StageLightTimelineClip))]
    [CanEditMultipleObjects]
    public class StageLightTimelineClipCustomInspector : UnityEditor.Editor
    {
        private List<StageLightProfile> allProfilesInProject = new List<StageLightProfile>();
        private List<string> profileNames = new List<string>();
        private int selectedProfileIndex = 0;
        private Dictionary<string, List<StageLightProfile>> folderNamesProfileDict = new Dictionary<string, List<StageLightProfile>>();
        private StageLightTimelineClip stageLightTimelineClip;
        public override void OnInspectorGUI()
        {
            stageLightTimelineClip = serializedObject.targetObject as StageLightTimelineClip;
            if(stageLightTimelineClip.stopEditorUiUpdate)  return;
            BeginInspector();
        }

        private void BeginInspector()
        {
            DrawProfileIO();

            // EditorGUILayout.Space(12);
            // GUI.backgroundColor = Color.red;
            // if (GUILayout.Button("Force Reflesh UI"))
            // {
            //     SlmBaseDrawer.ClearCache();
            // }
            // GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space(2);
            EditorGUI.BeginDisabledGroup( stageLightTimelineClip.syncReferenceProfile);
                // isMultiSelect = false;
                var stageLightProperties = new List<SlmProperty>();
                SerializedProperty serializedProperty;

                stageLightProperties = stageLightTimelineClip.StageLightQueueData.stageLightProperties;
                if(stageLightTimelineClip.StageLightQueueData == null) stageLightTimelineClip.behaviour.Init();
                var behaviourProperty = serializedObject.FindProperty("behaviour");
                var stageLightQueDataProperty = behaviourProperty.FindPropertyRelative("stageLightQueueData");
                serializedProperty =stageLightQueDataProperty.FindPropertyRelative("stageLightProperties");
                stageLightTimelineClip.behaviour.CheckRequiredProperties();
                // remove null property
                for (int i = 0; i < stageLightProperties.Count; i++)
                {
                    if (stageLightProperties[i] == null)
                    {
                        stageLightProperties.RemoveAt(i);
                        serializedProperty.DeleteArrayElementAtIndex(i);
                        i--;
                    }
                }

                EditorGUI.BeginChangeCheck();
                // EditorUtility.SetDirty(stageLightTimelineClip);
                var drawer = new StageLightPropertiesDrawer();
                drawer.OnGUI(EditorGUILayout.GetControlRect(), serializedProperty, GUIContent.none);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedProperty.serializedObject.ApplyModifiedProperties();
                }
            EditorGUI.EndDisabledGroup();
        }
        

        private void SetFilePath(StageLightTimelineClip stageLightTimelineClip)
        {
            var exportPath = stageLightTimelineClip.referenceStageLightProfile != null ? AssetDatabase.GetAssetPath(stageLightTimelineClip.referenceStageLightProfile) : "Asset";
            var exportName = stageLightTimelineClip.referenceStageLightProfile != null ? stageLightTimelineClip.referenceStageLightProfile.name+"(Clone)" : "new stageLightProfile";
            var path = EditorUtility.SaveFilePanel("Save StageLightProfile Asset", exportPath,exportName, "asset");
            string fileName = Path.GetFileName(path);
            if(path == "") return;
            path = path.Replace("\\", "/").Replace(Application.dataPath, "Assets");
            string dir = Path.GetDirectoryName(path);
            stageLightTimelineClip.exportPath = path;

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawProfileIO()
        {
             serializedObject.Update();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Profile", GUILayout.MaxWidth(60));
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("referenceStageLightProfile"),
                    new GUIContent(""));
                if (EditorGUI.EndChangeCheck())
                {
                    if (serializedObject.FindProperty("referenceStageLightProfile").objectReferenceValue == null)
                    {
                        stageLightTimelineClip.syncReferenceProfile = false;
                    }
                    serializedObject.ApplyModifiedProperties();
                }
            }

            if (stageLightTimelineClip.referenceStageLightProfile == null &&
                stageLightTimelineClip.syncReferenceProfile)
            {
                stageLightTimelineClip.syncReferenceProfile = false;
                serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUI.BeginDisabledGroup(stageLightTimelineClip.referenceStageLightProfile == null);


            if(stageLightTimelineClip == null)
                return;
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUI.backgroundColor= Color.green;
                GUI.contentColor = Color.white;
                if (GUILayout.Button("Load Profile",GUILayout.MaxWidth(100)))
                {
                    stageLightTimelineClip.stopEditorUiUpdate = true;
                    // set dirty
                    EditorUtility.SetDirty(stageLightTimelineClip);
                    stageLightTimelineClip.LoadProfile();
                    serializedObject.ApplyModifiedProperties();
                    stageLightTimelineClip.stopEditorUiUpdate = false;
                    // Repaint();
                }
                GUI.backgroundColor= new Color(0.9f,0.5f,1f);
                GUI.contentColor = Color.white;
                if (GUILayout.Button("Load Diff",GUILayout.MaxWidth(100)))
                {
                    stageLightTimelineClip.stopEditorUiUpdate = true;
                    // set dirty
                    EditorUtility.SetDirty(stageLightTimelineClip);
                    stageLightTimelineClip.OverwriteDiffProperty();
                    serializedObject.ApplyModifiedProperties();
                    stageLightTimelineClip.stopEditorUiUpdate = false;
                    // Repaint();
                }
                
                
                GUI.backgroundColor= Color.white;
                GUI.contentColor = Color.white;
                if (GUILayout.Button("Save Profile",GUILayout.MaxWidth(100)))
                {
                    stageLightTimelineClip.SaveProfile();
                    
                }
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("syncReferenceProfile"));
            if (EditorGUI.EndChangeCheck())
            {
                
                serializedObject.ApplyModifiedProperties();
                stageLightTimelineClip.InitSyncData();
                
            }
            
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("syncClipName"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
            
           


            using (new EditorGUILayout.HorizontalScope())
            {

                EditorGUI.BeginChangeCheck();
                var path = EditorGUILayout.PropertyField(serializedObject.FindProperty("exportPath"));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }
                GUI.backgroundColor = Color.white;
                if (GUILayout.Button("...",GUILayout.MaxWidth(30)))
                {
                    SetFilePath(stageLightTimelineClip);
                }

            }

            using (new EditorGUILayout.HorizontalScope())
            {

                GUILayout.FlexibleSpace();



                GUI.backgroundColor= Color.red;
                GUI.contentColor = Color.white;
                
                if (GUILayout.Button("Save as",GUILayout.MaxWidth(100)))
                {
                    ExportProfile(stageLightTimelineClip);
                }
                
                GUI.backgroundColor = Color.white;
            }
            
            
            
            EditorGUILayout.Space(1);

           
            if (GUILayout.Button("Select LightFixture",GUILayout.MaxWidth(120)))
            {
                if (stageLightTimelineClip.mixer != null && stageLightTimelineClip.mixer.trackBinding != null)
                {
                    var gameObjects = new List<GameObject>();
                    foreach (var stageLight in stageLightTimelineClip.mixer.trackBinding.stageLights)
                    {
                        gameObjects.Add(stageLight.gameObject);
                    }
                    Selection.objects = gameObjects.ToArray();
                }
                    
            }
        }

        private void ExportProfile(StageLightTimelineClip stageLightTimelineClip)
        {
           
            Undo.RegisterCompleteObjectUndo(stageLightTimelineClip, stageLightTimelineClip.name);
            EditorUtility.SetDirty(stageLightTimelineClip);
            var newProfile = CreateInstance<StageLightProfile>();
            newProfile.stageLightProperties = stageLightTimelineClip.StageLightQueueData.stageLightProperties;
            var exportPath = SlmUtility.GetExportPath(stageLightTimelineClip.exportPath,stageLightTimelineClip.clipDisplayName);

            var directory = Path.GetDirectoryName(exportPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            var fileName = Path.GetFileNameWithoutExtension(exportPath);
            var fileExtension = Path.GetExtension(exportPath);
            var filePath = Path.GetDirectoryName(exportPath);
            
            var files = Directory.GetFiles(filePath, "*" + fileExtension).ToList().Where( f => f.Contains(fileName)).ToList();
            var fileNames = files.Select(f => Path.GetFileNameWithoutExtension(f)).ToList();
            fileNames.Sort();
            
            var lastFileNumber = 0;
            var exportFileName = fileName;
            if (fileNames.Count > 0)
            {
                var lastFile = fileNames.Last();
                var match = Regex.Match(lastFile, @"\((\d+)\)$");
                if (match.Success)
                {
                    lastFileNumber = int.TryParse (match.Groups[1].Value, out lastFileNumber) ? lastFileNumber : 0;
                }

                fileName = fileName.Replace($"({lastFileNumber})", "");
                lastFileNumber++;
            }

            if (lastFileNumber == 0)
            {
                exportPath = filePath + "/" + fileName + fileExtension;
            }
            else
            {
                exportPath = filePath + "/" + fileName+ $"({lastFileNumber})" + fileExtension;
            }

            if (!exportPath.EndsWith(".asset"))
            {
                exportPath = (exportPath + ".asset");   
            }

            AssetDatabase.CreateAsset(newProfile, exportPath);
            AssetDatabase.Refresh();
            InitProfileList(stageLightTimelineClip);
            stageLightTimelineClip.referenceStageLightProfile = AssetDatabase.LoadAssetAtPath<StageLightProfile>(exportPath);
            AssetDatabase.SaveAssets();
        }


        private void OnDisable() { }
        private void OnDestroy() { }
        public void OnInspectorUpdate()
        {
            this.Repaint();
        }

        private void InitProfileList(StageLightTimelineClip stageLightTimelineClip)
        {
            allProfilesInProject = SlmUtility.GetProfileInProject();
            profileNames.Clear();

            // group by folder
            folderNamesProfileDict = new Dictionary<string, List<StageLightProfile>>();
            foreach (var profile in allProfilesInProject)
            {
                var path = AssetDatabase.GetAssetPath(profile);
                var parentDirectory = Path.GetDirectoryName(path).Replace("Assets/", "").Replace("Assets\\", "");
                parentDirectory = parentDirectory.Replace("\\", ">").Replace("/", ">");
                if (folderNamesProfileDict.ContainsKey(parentDirectory))
                {
                    folderNamesProfileDict[parentDirectory].Add(profile);
                }
                else
                {
                    folderNamesProfileDict.Add(parentDirectory, new List<StageLightProfile> {profile});
                }

            }

            foreach (var keyPair in folderNamesProfileDict)
            {
                foreach (var v in keyPair.Value)
                {
                    profileNames.Add($"{keyPair.Key}/{v.name}");
                }
            }
            
            selectedProfileIndex = allProfilesInProject.IndexOf(stageLightTimelineClip.referenceStageLightProfile);
        }

        private void DrawProfilesPopup(StageLightTimelineClip stageLightTimelineClip)
        {
            
            if(allProfilesInProject == null || allProfilesInProject.Count == 0)
                InitProfileList(stageLightTimelineClip);
            
            EditorGUI.BeginChangeCheck();
            selectedProfileIndex = EditorGUILayout.Popup("", selectedProfileIndex, profileNames.ToArray(), GUILayout.Width(120));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(stageLightTimelineClip, "Changed StageLightProfile");
                stageLightTimelineClip.referenceStageLightProfile = allProfilesInProject[selectedProfileIndex];
                serializedObject.ApplyModifiedProperties();
            }
        }



    }
}