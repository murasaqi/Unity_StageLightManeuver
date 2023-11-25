namespace StageLightManeuver
{
    public class SyncLensFlareProperty:SlmAdditionalProperty
    {
        
        public SlmToggleValue<float> intensitymultiplier;
        public SlmToggleValue<float> maxIntensityLimit;
        public SlmToggleValue<float> scale;
        public SyncLensFlareProperty()
        {
            propertyName = "Sync Lens Flare";
            propertyOverride = true;
            clockOverride = new SlmToggleValue<ClockOverride>();
            intensitymultiplier = new SlmToggleValue<float>() { value = 1f };
            maxIntensityLimit = new SlmToggleValue<float>() { value = 100f };
            scale = new SlmToggleValue<float>() { value = 1f };
            
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            clockOverride.propertyOverride = toggle;
            intensitymultiplier.propertyOverride = toggle;
            maxIntensityLimit.propertyOverride = toggle;
            scale.propertyOverride = toggle;
        }

        public SyncLensFlareProperty(SyncLensFlareProperty other)
        {
            propertyName = other.propertyName;
            clockOverride = new SlmToggleValue<ClockOverride>(other.clockOverride);
            propertyOverride = other.propertyOverride;
            intensitymultiplier = new SlmToggleValue<float>(){value = other.intensitymultiplier.value};
            maxIntensityLimit = new SlmToggleValue<float>() { value = other.maxIntensityLimit.value };
            scale = new SlmToggleValue<float>() { value = other.scale.value };
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            SyncLensFlareProperty syncLensFlareProperty = other as SyncLensFlareProperty;
            if (syncLensFlareProperty == null) return;
            if(clockOverride.propertyOverride) clockOverride = new  SlmToggleValue<ClockOverride>(syncLensFlareProperty.clockOverride);
            if (syncLensFlareProperty.propertyOverride)
            {
                if(syncLensFlareProperty.intensitymultiplier.propertyOverride) intensitymultiplier.value = syncLensFlareProperty.intensitymultiplier.value;
                if(syncLensFlareProperty.maxIntensityLimit.propertyOverride) maxIntensityLimit.value = syncLensFlareProperty.maxIntensityLimit.value;
                if(syncLensFlareProperty.scale.propertyOverride) scale.value = syncLensFlareProperty.scale.value;
            }
        }
    }
}