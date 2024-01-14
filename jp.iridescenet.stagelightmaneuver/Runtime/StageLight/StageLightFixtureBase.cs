using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    public abstract class StageLightFixtureBase : MonoBehaviour
    {
        [FormerlySerializedAs("stageLights")] public List<StageLightFixture> stageLightFixtures;
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
    }
}