using UnityEngine;

namespace StageLightManeuver
{
    public class BarLightElement:StageLightBase
    {
        public Light light;
        public Transform pan;
        public Transform tilt;
        
        public override void Init()
        {
            base.Init();
            if (light == null)
            {
                light = GetComponentInChildren<Light>();
            }
        }

        public override void UpdateChannel()
        {
            
        }
        
        
    }
}