using System.Collections.Generic;
using System.Linq;

namespace StageLightManeuver
{
    public interface IStageLight
    {
        public List<StageLightChannelBase> StageLightChannels { get; set; }
        // public void Init(StageLight stageLight);

        public T TryGetChannel<T>() where T : StageLightChannelBase
        {
            return StageLightChannels.FirstOrDefault(x => x is T) as T;
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