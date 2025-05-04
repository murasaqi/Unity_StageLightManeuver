using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    public abstract class StageLightFixtureBase : MonoBehaviour
    {
        [FormerlySerializedAs("stageLights")] public List<StageLightFixture> stageLightFixtures = new List<StageLightFixture>();
        [SerializeField] private bool showGizmo = false;
        [SerializeField] private Color gizmoColor = Color.yellow;
        [SerializeField] private Vector3 gizmoSize = Vector3.one;

        public virtual void Init()
        {
        }
        public virtual void AddQue(StageLightQueueData stageLightQueData)
        {
        }

        public virtual void EvaluateQue(float time)
        {
        }

        public virtual void UpdateChannel()
        {
        }

        public virtual List<Type> GetAllPropertyType()
        {
            return new List<Type>();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showGizmo) return;
        
            Gizmos.color = gizmoColor;
            foreach (var fixture in stageLightFixtures)
            {
                if (fixture != null)
                {
                    DrawBoundCube(fixture.transform);
                    // Gizmos.DrawWireCube(fixture.transform.position, gizmoSize);
                }
            }
        }
        
        void DrawBoundCube(Transform transform)
        {
            var renderers = transform.GetComponentsInChildren<Renderer>();
            Vector3 min = Vector3.one*float.MaxValue, max = Vector3.one*float.MinValue;
            int count = 0;
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;
                var bounds = renderer.bounds;
                min = Vector3.Min(min, bounds.min);
                max = Vector3.Max(max, bounds.max);
                count++;
            }
        
            if(count > 0)
                Gizmos.DrawWireCube((min+max)/2, max-min);
            else
                Gizmos.DrawWireCube(transform.position, gizmoSize);
        }

        [ContextMenu("Toggle Gizmo")]
        public void ToggleGizmo()
        {
            showGizmo = !showGizmo;
        }

        public void DrawGizmo(bool show)
        {
            showGizmo = show;
        }
#endif
    }
}