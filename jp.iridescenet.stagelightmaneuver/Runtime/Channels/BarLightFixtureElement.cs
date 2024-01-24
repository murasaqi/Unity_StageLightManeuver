using UnityEngine;

#pragma warning disable 0108

namespace StageLightManeuver
{
    public class BarLightFixtureElement:LightFixtureBase
    {
        public Light light; //TODO CS0108: Component.light を隠してるので、newで明示するか変数名を変える
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