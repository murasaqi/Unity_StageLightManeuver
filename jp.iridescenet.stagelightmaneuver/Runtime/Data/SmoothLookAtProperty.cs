namespace StageLightManeuver
{
    public class SmoothLookAtProperty:SlmProperty
    {

        public SlmToggleValue<int> targetIndex;
        public SmoothLookAtProperty()
        {
            propertyName = "Smooth LookAt";
            propertyOverride = true;
            targetIndex = new SlmToggleValue<int>();

        }

        public SmoothLookAtProperty(SmoothLookAtProperty other)
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            targetIndex = new SlmToggleValue<int>(other.targetIndex);
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            targetIndex.propertyOverride = toggle;
            
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            base.OverwriteProperty(other);
            SmoothLookAtProperty smoothLookAtProperty = other as SmoothLookAtProperty;
            if (smoothLookAtProperty == null) return;   
            targetIndex = new SlmToggleValue<int>(smoothLookAtProperty.targetIndex);
            
            
            
        }
        
        
    }
}