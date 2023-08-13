using System.Collections.Generic;
using System.Linq;

namespace StageLightManeuver
{
    public interface IStageLight
    {
        public List<StageLightFixtureBase> StageLightFixtures { get; set; }
        // public void Init(StageLight stageLight);

        public T TryGetFixture<T>() where T : StageLightFixtureBase
        {
            return StageLightFixtures.FirstOrDefault(x => x is T) as T;
        }
        
        public void Init()
        {
        }
        
        
        public void AddQue(SlmToggleValueBase slmToggleValueBase, float weight)
        {
        }
        
        public void EvaluateQue(float time);
    
    }
    
}