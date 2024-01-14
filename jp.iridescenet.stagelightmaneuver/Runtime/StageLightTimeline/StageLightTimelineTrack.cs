using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using System.Reflection;
#endif
namespace StageLightManeuver
{

    [TrackColor(0.8239978f, 0.9150943f, 0.3338079f)]
    [TrackClipType(typeof(StageLightTimelineClip))]
    [TrackBindingType(typeof(StageLightUniverse))]
    public class StageLightTimelineTrack : TrackAsset
    {
        
        [FormerlySerializedAs("drawBeat")] [SerializeField] public bool drawCustomClip = true;
        
        [Header("Base Settings")] [SerializeField]
        public float bpm = 120;

        [SerializeField] public float bpmScale = 1;
        [SerializeField] public string exportPath = "Assets/";

        [Header("Clip UI Options", order = 0)] [SerializeField] [Range(0, 1f)]
        public float colorLineHeight = 0.1f;
        [SerializeField] public Color beatLineColor = new Color(0, 1, 0.7126422f, 0.2f);
        [SerializeField] public bool updateOnOutOfClip = false;
        public List<StageLightTimelineClip> stageLightTimelineClips = new List<StageLightTimelineClip>();
        public List<StageLightTimelineClip> selectedClips = new List<StageLightTimelineClip>();
         // public List<SlmProperty> slmProperties;

#if UNITY_EDITOR
        
        private SerializedObject serializedProfile;
#endif

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            #if UNITY_EDITOR
#endif
            var mixer = ScriptPlayable<StageLightTimelineMixerBehaviour>.Create(graph, inputCount);
            var stageLightTimelineMixer = mixer.GetBehaviour();
            stageLightTimelineClips.Clear();
            stageLightTimelineMixer.stageLightTimelineTrack = this;
            var timelineClips = GetClips().ToList();
            stageLightTimelineMixer.clips = timelineClips;
            var director = go.GetComponent<PlayableDirector>();
            foreach (var clip in timelineClips)
            {
                var stageLightTimelineClip = clip.asset as StageLightTimelineClip;
                stageLightTimelineClip.track = this;
                stageLightTimelineClip.mixer = stageLightTimelineMixer;
                stageLightTimelineClip.clipDisplayName = clip.displayName;
                stageLightTimelineClips.Add(stageLightTimelineClip);
            }

            return mixer;
        }

        public void SortProperty()
        {
            var clips = GetClips().ToList();
            foreach (var clip in clips)
            {
                var stageLightTimelineClip = clip.asset as StageLightTimelineClip;
                if(stageLightTimelineClip == null || stageLightTimelineClip.StageLightQueueData == null) continue;
                
                // Debug.Log(stageLightTimelineClip.StageLightQueueData.stageLightProperties.Count) as ClockProperty;
                var clockProperty = stageLightTimelineClip.StageLightQueueData.TryAddGetProperty(typeof(ClockProperty));
                Debug.Log(clockProperty);
                clockProperty.sortOrder = -999;
                stageLightTimelineClip.StageLightQueueData.TryAddGetProperty(typeof(StageLightOrderProperty)).sortOrder = -0;
                stageLightTimelineClip.StageLightQueueData.stageLightProperties.Sort((a, b) => a.propertyOrder.CompareTo(b.propertyOrder));
            }
        }

        public void OnEnable()
        {
#if UNITY_EDITOR
            Selection.selectionChanged -= OnSelection;
            Selection.selectionChanged += OnSelection;
#endif
        }

#if UNITY_EDITOR
        
        private void OnSelection()
        {
            
            var select = Selection.objects.ToList(); 
            selectedClips = new List<StageLightTimelineClip>();
            // selectedClips.Clear();
            foreach (var s in select)
            {
                if (s.GetType().ToString() == "UnityEditor.Timeline.EditorClip")
                {
                    var clip = s.GetType().GetField("m_Clip", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(s);
                    
                    var timelineClip = clip as TimelineClip;
                    if(timelineClip == null) continue;
                    if (timelineClip.asset.GetType() == typeof(StageLightTimelineClip))
                    {
                        var asset = timelineClip.asset as StageLightTimelineClip;
                        
                        selectedClips.Add(asset);

                    }
                }
                
            }
        }
        
        public void ApplyProfileAllClip(StageLightProfile stageLightProfile)
        {
            // var copy = SlmUtility.CopyProperties(stageLightProfile);
            foreach (var clip in stageLightTimelineClips)
            {
                if(clip.referenceStageLightProfile != stageLightProfile) continue;
                // if(clip.syncReferenceProfile) clip.SetProperties(copy);
                clip.InitSyncData();
            }
        }

        public void ApplyBPM()
        {
            var clips = GetClips().ToList();
            foreach (var clip in clips)
            {
                var stageLightTimelineClip = clip.asset as StageLightTimelineClip;
                var clockProperty = stageLightTimelineClip.behaviour.stageLightQueueData.TryGetActiveProperty<ClockProperty>();
                clockProperty.bpm.value = bpm;
                // Set dirty
                EditorUtility.SetDirty(stageLightTimelineClip);
                
            }
        }
        
#endif
    }
}
