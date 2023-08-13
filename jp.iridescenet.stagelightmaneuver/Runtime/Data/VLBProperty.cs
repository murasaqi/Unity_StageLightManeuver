namespace StageLightManeuver
{
#if USE_VLB
    public class VLBProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<float> intensityMultiplier;
        public SlmToggleValue<float> lightRangeMultiplier;
        public SlmToggleValue<float> spotAngleMultiplier;
        
        public VLBProperty()
        {
            propertyName = "VLB Property";
            propertyOverride = true;
            clockOverride = new SlmToggleValue<ClockOverride>();
            intensityMultiplier = new SlmToggleValue<float>() { value = 1f };
            lightRangeMultiplier = new SlmToggleValue<float>() { value = 1f };
            spotAngleMultiplier = new SlmToggleValue<float>() { value = 1f };
        }
        
        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            clockOverride.propertyOverride = toggle;
            intensityMultiplier.propertyOverride = toggle;
            lightRangeMultiplier.propertyOverride = toggle;
            spotAngleMultiplier.propertyOverride = toggle;
        }
        
        
        public VLBProperty(VLBProperty other)
        {
            propertyName = other.propertyName;
            clockOverride = new SlmToggleValue<ClockOverride>(other.clockOverride);
            propertyOverride = other.propertyOverride;
            intensityMultiplier = new SlmToggleValue<float>(other.intensityMultiplier);
            lightRangeMultiplier = new SlmToggleValue<float>(other.lightRangeMultiplier);
            spotAngleMultiplier = new SlmToggleValue<float>(other.spotAngleMultiplier);
        }
        
        
        public override void OverwriteProperty(SlmProperty other)
        {
            VLBProperty vlbProperty = other as VLBProperty;
            if (vlbProperty == null) return;
            if(clockOverride.propertyOverride) clockOverride = new  SlmToggleValue<ClockOverride>(vlbProperty.clockOverride);
            if (vlbProperty.propertyOverride)
            {
                if(vlbProperty.intensityMultiplier.propertyOverride) intensityMultiplier.value = vlbProperty.intensityMultiplier.value;
                if(vlbProperty.lightRangeMultiplier.propertyOverride) lightRangeMultiplier.value = vlbProperty.lightRangeMultiplier.value;
                if(vlbProperty.spotAngleMultiplier.propertyOverride) spotAngleMultiplier.value = vlbProperty.spotAngleMultiplier.value;
            }
        }
    }
    
#endif
}