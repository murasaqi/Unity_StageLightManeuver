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
                    Gizmos.DrawWireCube(fixture.transform.position, gizmoSize);
                }
            }
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