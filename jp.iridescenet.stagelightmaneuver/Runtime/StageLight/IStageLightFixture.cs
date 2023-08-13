using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLightManeuver
{

    public interface IStageLightFixture
    {
        public List<StageLightBase> SyncStageLight { get; set; }
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
