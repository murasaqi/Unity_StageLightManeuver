using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLightManeuver
{

    public interface IStageLightChannel
    {
        public List<LightFixtureBase> SyncStageLight { get; set; }
        public void EvaluateQue(float time);

        public void AddStageLightInChild()
        {
        }

        public void AddQue(SlmToggleValueBase slmToggleValueBase, float weight)
        {
        }

        public void Init()
        {
        }

    }
    
 
    
    
    
}
