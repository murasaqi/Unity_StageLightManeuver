using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace StageLightManeuver
{
    [CustomEditor(typeof(StageLightFixture))]
    [CanEditMultipleObjects]
    public class MovingStageLightEditor:Editor
    {
        private StageLightFixture _targetStageLightFixture;
        private List<string> _channelList = new List<string>();
        private bool IsMultipleSelectionMode => targets.Length > 1;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            // return base.CreateInspectorGUI();
            
            _targetStageLightFixture = target as StageLightFixture;

            // ----- Fixture Profile -----
            var profileField = new PropertyField(serializedObject.FindProperty("lightFixtureProfile"),  "Fixture Profile");
            var pathField = new TextField("Export Path")
            {
                value = _targetStageLightFixture.profileExportPath,
                isDelayed = true,
            };
            
            pathField.RegisterValueChangedCallback(evt =>
            {
                _targetStageLightFixture.profileExportPath = evt.newValue;
                
                if (string.IsNullOrEmpty(_targetStageLightFixture.profileExportPath))
                {
                    var settings = SlmEditorSettingsUtility.GetStageLightManeuverSettingsAsset();
                    _targetStageLightFixture.profileExportPath = settings.lightFixtureProfileExportPath;
                }
                serializedObject.ApplyModifiedProperties();
            });

            var horizontal = new VisualElement();
            horizontal.style.flexDirection = FlexDirection.Row;
            // horizontal.style.paddingBottom = 4;
            horizontal.style.justifyContent = Justify.FlexEnd;

            var buttonSaveAs = new Button(() =>
            {
                foreach (var obj in targets)
                {
                    var stageLightFixture = obj as StageLightFixture;
                    if (stageLightFixture == null) continue;
                    SaveProfile(stageLightFixture, stageLightFixture.profileExportPath);
                }
            })
            {
                text = "Save as"
            };

            var buttonSave = new Button(() =>
            {
                foreach (var obj in targets)
                {
                    var stageLightFixture = obj as StageLightFixture;
                    if (stageLightFixture == null) continue;
                    SaveProfile(stageLightFixture);
                }
            })
            {
                text = "Save",
            };
            
            var buttonLoad = new Button(() =>
            {
                foreach (var obj in targets)
                {
                    var stageLightFixture = obj as StageLightFixture;
                    if (stageLightFixture == null) continue;
                    LoadProfile(stageLightFixture);
                }
            })
            {
                text = "Load",
            };

            UpdateButtonState();

            profileField.RegisterValueChangeCallback(evt => UpdateButtonState());  
            
            horizontal.Add(buttonSaveAs);
            horizontal.Add(buttonSave);
            horizontal.Add(buttonLoad);


            root.Add(profileField);
            pathField.SetEnabled(false);
            // pathField.SetEnabled(!IsMultipleSelectionMode);
            root.Add(pathField);
            root.Add(horizontal);

            // -----
            // 水平線を追加
            root.Add(new IMGUIContainer(() =>
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                // EditorGUILayout.LabelField("Fixture Profile", EditorStyles.boldLabel);

            }));

            var indexField = new PropertyField(serializedObject.FindProperty("index"));
            indexField.SetEnabled(false); 
            root.Add(indexField);
            root.Add(new PropertyField(serializedObject.FindProperty("stageLightChannels")));
            _channelList = new List<string>();
            _channelList.Add("Add New Channel");

            Init();

            var center = new VisualElement();
            center.style.alignItems = Align.Center;
            var popupField = new PopupField<string>(_channelList, 0);
            popupField.SetEnabled( _channelList.Count > 1 );
            popupField.RegisterValueChangedCallback((evt =>
            {
                if (popupField.index != 0)
                {
                    var type = GetTypeByClassName(popupField.value);
                    MethodInfo mi = typeof(GameObject).GetMethod(
                        "AddComponent",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                        null,
                        new Type[0],
                        null
                    );
                    MethodInfo bound = mi.MakeGenericMethod(type);
                    
                    foreach (var obj in targets) //! Add Channel to all Fixtures when multiple selections are made
                    {
                        var stageLightFixture = obj as StageLightFixture;
                        if (stageLightFixture == null) continue;
                        var channel =bound.Invoke(stageLightFixture.gameObject, null) as StageLightChannelBase;
                        if(channel)channel.Init();
                        stageLightFixture.FindChannels();
                    }
                }
            }));
            center.Add(popupField);
            root.Add(center);
            root.Add(new PropertyField(serializedObject.FindProperty("syncStageLight")));
            
            // pathField.RegisterValueChangeCallback(evt =>
            // {
            //     var defaultPath = SlmEditorSettingsUtility.GetStageLightManeuverSettingsAsset().lightFixtureProfileExportPath;
            //     var path = _targetStageLightFixture.profileExportPath;
            //     if (root.focusController.focusedElement != pathField && path == "")
            //     {
            //         _targetStageLightFixture.profileExportPath = defaultPath;
            //         serializedObject.ApplyModifiedProperties();
            //     }
            // });

            return root;

            void UpdateButtonState()
            {
                var isProfileSet = _targetStageLightFixture.lightFixtureProfile != null;
                var isSingleSelection = IsMultipleSelectionMode == false;
                var canSave = isProfileSet 
                              && string.IsNullOrEmpty(_targetStageLightFixture.profileExportPath) == false 
                              && isSingleSelection;
                
                buttonSaveAs.SetEnabled(isSingleSelection);
                buttonSave.SetEnabled(canSave);
                buttonLoad.SetEnabled(isProfileSet);
            }
        }

        private void Init()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var referencedAssemblies = executingAssembly.GetReferencedAssemblies();
            if(_targetStageLightFixture == null) return;
            foreach ( var assemblyName in referencedAssemblies )
            {
                var assembly = Assembly.Load( assemblyName );

                if ( assembly == null )
                {
                    continue;
                }
                var types = assembly.GetTypes();
                types.Where(t => t.IsSubclassOf(typeof(StageLightChannelBase)))
                    .ToList()
                    .ForEach(t =>
                    {
                        if (_targetStageLightFixture.StageLightChannels != null && _targetStageLightFixture.StageLightChannels.Count>=0)
                        {
                            if (_targetStageLightFixture.StageLightChannels.Find(x =>x!= null && x.GetType().Name == t.Name) == null)
                            {
                                // Debug.Log(t.Name);
                                _channelList.Add(t.Name);
                            }      
                        }
                    });
            }
        }

        private static Type GetTypeByClassName( string className )
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


        public void SaveProfile(StageLightFixture fixture, string? savePath=null)
        {
            var channels = fixture.StageLightChannels;
            var profile = fixture.lightFixtureProfile;
            if (profile == null) 
            {
                string path = savePath;
                if(string.IsNullOrEmpty(path)) 
                {
                    var settings = SlmEditorSettingsUtility.GetStageLightManeuverSettingsAsset();
                    path = settings.lightFixtureProfileExportPath;
                }
                profile = ScriptableObject.CreateInstance<LightFixtureProfile>();

                var lightName = fixture.gameObject.name;
                path = path.Replace("<LightName>", lightName);
                if (!path.EndsWith(".asset"))
                {
                    path = (path + ".asset");
                }
                string fileName = Path.GetFileName(path);
                path = path.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                AssetDatabase.CreateAsset(profile, path);
                fixture.lightFixtureProfile = profile;
            }
            profile.Init(channels);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void LoadProfile(StageLightFixture fixture)
        {
            var profile = fixture.lightFixtureProfile;
            if (profile == null) return;

            var channels = fixture.StageLightChannels;
            
            profile.RestoreChannelData(channels);
            Init(); 
        }
    }
}