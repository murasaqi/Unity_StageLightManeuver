namespace StageLightManeuver
{
    public class SyncLightMaterialProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<float> intensitymultiplier;
        public SlmToggleValue<bool> brightnessDecreasesToBlack;
        public SlmToggleValue<float> maxIntensityLimit;
        public SyncLightMaterialProperty()
        {
            propertyName = "Sync Light Material";
            propertyOverride = false;
            clockOverride = new SlmToggleValue<ClockOverride>();
            intensitymultiplier = new SlmToggleValue<float>() { value = 1f };
            brightnessDecreasesToBlack = new SlmToggleValue<bool>() { value = false };
            maxIntensityLimit = new SlmToggleValue<float>() { value = 3f };
            
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            clockOverride.propertyOverride = toggle;
            intensitymultiplier.propertyOverride = toggle;
            brightnessDecreasesToBlack.propertyOverride = toggle;
            maxIntensityLimit.propertyOverride = toggle;
        }

        public SyncLightMaterialProperty(SyncLightMaterialProperty other)
        {
            propertyName = other.propertyName;
            clockOverride = new SlmToggleValue<ClockOverride>(other.clockOverride);
            propertyOverride = other.propertyOverride;
            intensitymultiplier = new SlmToggleValue<float>(){value = other.intensitymultiplier.value};
            brightnessDecreasesToBlack = new SlmToggleValue<bool>() { value = other.brightnessDecreasesToBlack.value };
            maxIntensityLimit = new SlmToggleValue<float>() { value = other.maxIntensityLimit.value };
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            SyncLightMaterialProperty syncLightMaterialProperty = other as SyncLightMaterialProperty;
            if (syncLightMaterialProperty == null) return;
            if(clockOverride.propertyOverride) clockOverride = new  SlmToggleValue<ClockOverride>(syncLightMaterialProperty.clockOverride);
            if (syncLightMaterialProperty.propertyOverride)
            {
                if(syncLightMaterialProperty.intensitymultiplier.propertyOverride) intensitymultiplier.value = syncLightMaterialProperty.intensitymultiplier.value;
                if(syncLightMaterialProperty.brightnessDecreasesToBlack.propertyOverride) brightnessDecreasesToBlack.value = syncLightMaterialProperty.brightnessDecreasesToBlack.value;
                if(syncLightMaterialProperty.maxIntensityLimit.propertyOverride) maxIntensityLimit.value = syncLightMaterialProperty.maxIntensityLimit.value;
                
            }
        }
    }
}