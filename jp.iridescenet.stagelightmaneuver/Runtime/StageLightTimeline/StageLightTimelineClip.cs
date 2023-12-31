using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEditor;

namespace StageLightManeuver
{
    
    [Serializable]
    public class StageLightTimelineClip : PlayableAsset, ITimelineClipAsset
    {
        
        [SerializeReference]public StageLightProfile referenceStageLightProfile;
        [HideInInspector] public StageLightTimelineBehaviour behaviour = new StageLightTimelineBehaviour();

        private StageLightQueueData _stageLightQueData = new StageLightQueueData();
        public StageLightQueueData StageLightQueueData
        {
            get
            {
                if (syncReferenceProfile && referenceStageLightProfile != null)
                {
                    _stageLightQueData.stageLightProperties = referenceStageLightProfile.stageLightProperties;
                    return _stageLightQueData;
                }
                else
                {
                    _stageLightQueData.stageLightProperties = behaviour.stageLightQueueData.stageLightProperties;
                    return _stageLightQueData;
                }
            }
        }
        public bool forceTimelineClipUpdate;
        public bool syncReferenceProfile = false;
        public bool syncClipName = false;
        public StageLightTimelineTrack track;
        public string exportPath = "";
        public StageLightTimelineMixerBehaviour mixer;
        
        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending; }
        }

        public string clipDisplayName;
        public bool stopEditorUiUpdate = false;

        public void OnEnable()
        {

        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {

            InitStageLightProfile();
            var playable = ScriptPlayable<StageLightTimelineBehaviour>.Create(graph, behaviour);
            behaviour = playable.GetBehaviour();
            var queData = StageLightQueueData;

            var playabledirector = owner.GetComponent<PlayableDirector>();

            if (queData.stageLightProperties.Count <=0)
            {
                AddAllProperty(playabledirector, queData);
            }

            if (syncReferenceProfile && referenceStageLightProfile != null)
            {
                InitSyncData();
            }
            
            return playable;
        }

        private void AddAllProperty(PlayableDirector playabledirector, StageLightQueueData queData)
        {
            
            if (StageLightQueueData.stageLightProperties.Find(x => x.GetType() == typeof(ClockProperty)) == null)
            {
                StageLightQueueData.stageLightProperties.Insert(0,new ClockProperty());    
            }

            if (StageLightQueueData.stageLightProperties.Find(x => x.GetType() == typeof(StageLightOrderProperty)) ==
                null)
            {
                StageLightQueueData.stageLightProperties.Insert(1,new StageLightOrderProperty());
            }
            
            var propertyTypes = new List<Type>();
            foreach (var tAssetOutput in playabledirector.playableAsset.outputs)
            {
                if(tAssetOutput.sourceObject == null) continue;
                if(tAssetOutput.sourceObject.GetType() == typeof(StageLightTimelineTrack))
                {
                    var track = tAssetOutput.sourceObject as TrackAsset;
                    foreach (var timelineClip in track.GetClips())
                    {
                        var stageLightTimelineClip = timelineClip.asset as StageLightTimelineClip;
                        if (stageLightTimelineClip == this)
                        {
                            var binding = playabledirector.GetGenericBinding(track);
                            if (binding != null)
                            {
                                var stageLightSupervisor = binding as StageLightSupervisor;
                                if (stageLightSupervisor != null)
                                {
                                    propertyTypes.AddRange(stageLightSupervisor.GetAllPropertyType());
                                }
                            }
                        }
                    }
                }
            }
            propertyTypes = propertyTypes.Distinct().ToList();

            foreach (var propertyType in propertyTypes)
            {
                // if not contain fixture type in queData, add it
                if (queData.stageLightProperties.Find( x => x.GetType() == propertyType) == null)
                {
                    var fixture = Activator.CreateInstance(propertyType) as SlmProperty;
                    queData.stageLightProperties.Add(fixture);
                }
            }
        }
        

        
        public void InitStageLightProfile()
        {
            if (StageLightQueueData == null)
            {
                behaviour.Init();
            }
        }

        public void OverwriteDiffProperty()
        {
            if (referenceStageLightProfile == null) return;
            var properties = behaviour.stageLightQueueData.stageLightProperties;
            var profileData = referenceStageLightProfile.stageLightProperties;
            // var diffData = new List<SlmProperty>();
            foreach (var stageLightProperty in profileData)
            {
                var find = properties.Find(x => x.GetType() == stageLightProperty.GetType());
                if (find != null)
                {
                    properties.Remove(find);
                    find.isEditable = false;
                    properties.Add(stageLightProperty);
                }
                properties.Add(stageLightProperty);

            }

        }

        [ContextMenu("Apply")]
        public void LoadProfile()
        {
            if (referenceStageLightProfile == null) return;


            var copyList = new List<SlmProperty>();
            foreach (var stageLightProperty in referenceStageLightProfile.stageLightProperties)
            {
                if(stageLightProperty == null) continue;
                var type = stageLightProperty.GetType();
                var copy = Activator.CreateInstance(type, BindingFlags.CreateInstance, null,
                        new object[] { stageLightProperty }, null)
                    as SlmProperty;
                copy.isEditable = true;
                copyList.Add(copy);
            }

            var timeProperty = copyList.Find(x => x.GetType() == typeof(ClockProperty));

            if (timeProperty == null)
            {
                copyList.Insert(0, new ClockProperty());
            }
            
            var orderProperty = copyList.Find(x => x.GetType() == typeof(StageLightOrderProperty));
            if(orderProperty == null)
            {
                copyList.Insert(1, new StageLightOrderProperty());
            }
            
            behaviour.stageLightQueueData.stageLightProperties = copyList;
            stopEditorUiUpdate = false;
        }
        
        public void SetProperties(List<SlmProperty> properties)
        {
            StageLightQueueData.stageLightProperties = properties;
            stopEditorUiUpdate = false;
        }

        public void SaveProfile()
        {
#if UNITY_EDITOR
            Undo.RegisterCompleteObjectUndo(referenceStageLightProfile, referenceStageLightProfile.name);
            var copyList = new List<SlmProperty>();
            foreach (var stageLightProperty in StageLightQueueData.stageLightProperties)
            {
                if(stageLightProperty ==null) continue;
                var type = stageLightProperty.GetType();
                var copy = Activator.CreateInstance(type, BindingFlags.CreateInstance, null,
                        new object[] { stageLightProperty }, null)
                    as SlmProperty;
                copy.isEditable = false;
                copyList.Add(copy);
            }

            referenceStageLightProfile.stageLightProperties.Clear();
            referenceStageLightProfile.stageLightProperties = copyList;
            referenceStageLightProfile.isUpdateGuiFlag = true;
            EditorUtility.SetDirty(referenceStageLightProfile);
            AssetDatabase.SaveAssets();
            
            track.ApplyProfileAllClip( referenceStageLightProfile);
#endif
        }

        public void InitSyncData()
        {
            
            if (referenceStageLightProfile != null)
            {

                var copyList = new List<SlmProperty>();
                foreach (var stageLightProperty in referenceStageLightProfile.stageLightProperties)
                {
                    if(stageLightProperty == null) continue;
                    var type = stageLightProperty.GetType();
                    var copy = Activator.CreateInstance(type, BindingFlags.CreateInstance, null,
                            new object[] { stageLightProperty }, null)
                        as SlmProperty;
                    copy.isEditable = true;
                    copyList.Add(copy);
                }

                StageLightQueueData.stageLightProperties = copyList;
            }
            
        }


    }
}
