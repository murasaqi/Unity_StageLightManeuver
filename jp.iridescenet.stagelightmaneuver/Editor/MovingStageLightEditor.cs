using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace StageLightManeuver
{
    [CustomEditor(typeof(StageLight))]
    [CanEditMultipleObjects]
    public class MovingStageLightEditor:Editor
    {
        private StageLight targetStageLight;
        private List<string> fixtureList = new List<string>();
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            // return base.CreateInspectorGUI();
            
            targetStageLight = target as StageLight;
            var indexField = new PropertyField(serializedObject.FindProperty("index"));
            indexField.SetEnabled(false); 
            root.Add(indexField);
            root.Add(new PropertyField(serializedObject.FindProperty("stageLightFixtures")));
            fixtureList = new List<string>();
            fixtureList.Add("Add New Fixture");
          
           

            Init();

            var center = new VisualElement();
            center.style.alignItems = Align.Center;
            var popupField = new PopupField<string>(fixtureList, 0);
            popupField.SetEnabled( fixtureList.Count > 1 );
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
                    var fixture =bound.Invoke(targetStageLight.gameObject, null) as StageLightFixtureBase;
                    if(fixture)fixture.Init();
                    targetStageLight.FindFixtures();
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
            if(targetStageLight == null) return;
            foreach ( var assemblyName in referencedAssemblies )
            {
                var assembly = Assembly.Load( assemblyName );

                if ( assembly == null )
                {
                    continue;
                }
                var types = assembly.GetTypes();
                types.Where(t => t.IsSubclassOf(typeof(StageLightFixtureBase)))
                    .ToList()
                    .ForEach(t =>
                    {
                        if (targetStageLight.StageLightFixtures != null && targetStageLight.StageLightFixtures.Count>=0)
                        {
                            if (targetStageLight.StageLightFixtures.Find(x =>x!= null && x.GetType().Name == t.Name) == null)
                            {
                                // Debug.Log(t.Name);
                                fixtureList.Add(t.Name);
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
    }
}