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
        private List<string> channelList = new List<string>();
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            // return base.CreateInspectorGUI();
            
            _targetStageLightFixture = target as StageLightFixture;

             // ----- Fixture Profile -----
            root.Add(new PropertyField(serializedObject.FindProperty("lightFixtureProfile"), "Fixture Profile"));
            // lightFixtureProfile がセットされているか
            // bool isProfileSet = _targetStageLightFixture.lightFixtureProfile != null;

            var horizontal = new VisualElement();
            horizontal.style.flexDirection = FlexDirection.Row;
            // horizontal.style.paddingBottom = 4;
            horizontal.style.justifyContent = Justify.FlexEnd;

            horizontal.Add(new Button(() =>
            {
                foreach (var obj in targets)
                {
                    var stageLightFixture = obj as StageLightFixture;
                    if (stageLightFixture == null) continue;
                    SaveProfile(stageLightFixture);
                }
            })
            {
                text = "Save as"
            });
            horizontal.Add(new Button(() =>
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
                // clickable = isProfileSet
            });
            horizontal.Add(new Button(() =>
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
                // clickable = isProfileSet
            });
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
            channelList = new List<string>();
            channelList.Add("Add New Channel");
          
           

            Init();

            var center = new VisualElement();
            center.style.alignItems = Align.Center;
            var popupField = new PopupField<string>(channelList, 0);
            popupField.SetEnabled( channelList.Count > 1 );
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

            return root;
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
                                channelList.Add(t.Name);
                            }      
                        }
                    });
            }
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


        public void SaveProfile(StageLightFixture fixture, string? savePath=null)
        {
            var channels = fixture.StageLightChannels;
            var profile = fixture.lightFixtureProfile;
            if (profile == null) 
            {
                string path = savePath;
                if(path == "" || path == null) 
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