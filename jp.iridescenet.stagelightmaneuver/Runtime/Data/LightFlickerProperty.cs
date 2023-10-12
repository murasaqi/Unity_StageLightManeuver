using System;
using UnityEngine;

namespace StageLightManeuver
{
    // [Serializable]
    // public enum NoiseType
    // {
    //     Value2DNoise,
    //     PerlinDNoise,
    //     Simplex2DNoise,
    //     CubicNoise,
    //     Gradient2DNoise,
    //     FractalNoise,
    //     CellularNoise2D,
    //     WhiteNoise2D,
    // }
    [Serializable]
    public class LightFlickerProperty : SlmProperty
    {
        [SlmValue("Flicker")] public SlmToggleValue<FastNoise.NoiseType> noiseType;
        private FastNoise fastNoise = new FastNoise();
        [SlmValue("Noise Curve")] public SlmToggleValue<AnimationCurve> noiseCurve;
        [SlmValue("Time Scale")] public SlmToggleValue<float> timeScale;
        [SlmValue("Randomize")] public SlmToggleValue<float> randomize;
    [SlmValue("Noise Seed")] public SlmToggleValue<Vector2> noiseSeed;
        public LightFlickerProperty()
        {
            propertyName = "Flicker";
            propertyOverride = true;
            noiseType = new SlmToggleValue<FastNoise.NoiseType>(){value = FastNoise.NoiseType.Simplex};
            noiseCurve  = new SlmToggleValue<AnimationCurve>(){value = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0,0),
                new Keyframe(1,1)
            })};
            noiseSeed = new SlmToggleValue<Vector2>(){value = new Vector2(0,0)};
            timeScale = new SlmToggleValue<float>(){value = 1f};
            randomize = new SlmToggleValue<float>(){value = 0f};
        }
        
        public override void ToggleOverride(bool toggle)
        {
            propertyOverride = toggle;
            // clockOverride.propertyOverride = toggle;
            noiseType.propertyOverride = toggle;
            noiseCurve.propertyOverride = toggle;
            noiseSeed.propertyOverride = toggle;
            timeScale.propertyOverride = toggle;
            randomize.propertyOverride = toggle;
        }

        public float GetNoiseValue(float time, int childIndex)
        {
            fastNoise.SetNoiseType(noiseType.value);
            fastNoise.SetFrequency(1f);
            fastNoise.SetSeed(0);
            fastNoise.SetFractalOctaves(1);
            fastNoise.SetFractalLacunarity(2f);
            fastNoise.SetFractalGain(0.5f);
            fastNoise.SetFractalType(FastNoise.FractalType.FBM);
            fastNoise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Euclidean);
            fastNoise.SetCellularReturnType(FastNoise.CellularReturnType.Distance2Add); 
            
            return noiseCurve.value.Evaluate(fastNoise.GetNoise( time*timeScale.value + randomize.value*childIndex , noiseSeed.value.x, noiseSeed.value.y));
                
        }
        
        public LightFlickerProperty(LightFlickerProperty other)
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            // clockOverride = new SlmToggleValue<ClockOverride>(other.clockOverride);
            noiseType = new SlmToggleValue<FastNoise.NoiseType>()
            {
                propertyOverride = other.noiseType.propertyOverride,
                value = other.noiseType.value
            };
            noiseCurve = new SlmToggleValue<AnimationCurve>()
            {
                propertyOverride = other.noiseCurve.propertyOverride,
                value = SlmUtility.CopyAnimationCurve(other.noiseCurve.value)
            };
            noiseSeed = new SlmToggleValue<Vector2>()
            {
                propertyOverride = other.noiseSeed.propertyOverride,
                value = new Vector2(other.noiseSeed.value.x, other.noiseSeed.value.y)
            };
            timeScale = new SlmToggleValue<float>()
            {
                propertyOverride = other.timeScale.propertyOverride,
                value = other.timeScale.value
            };
            randomize   = new SlmToggleValue<float>()
            {
                propertyOverride = other.randomize.propertyOverride,
                value = other.randomize.value
            };
        }
        
        
        public override void OverwriteProperty(SlmProperty other)
        {
            if (other is LightFlickerProperty)
            {
                var otherProperty = other as LightFlickerProperty;
                if (other.propertyOverride)
                {
                    if(otherProperty.noiseType.propertyOverride) noiseType.value = otherProperty.noiseType.value;
                    // if(otherProperty.clockOverride.propertyOverride) clockOverride = new SlmToggleValue<ClockOverride>(otherProperty.clockOverride);
                    if(otherProperty.noiseCurve.propertyOverride) noiseCurve = new SlmToggleValue<AnimationCurve>()
                    {
                        value = SlmUtility.CopyAnimationCurve(otherProperty.noiseCurve.value)
                    };
                    if(otherProperty.noiseSeed.propertyOverride) noiseSeed = new SlmToggleValue<Vector2>()
                    {
                        value = new Vector2(otherProperty.noiseSeed.value.x, otherProperty.noiseSeed.value.y)
                    };
                    if(otherProperty.timeScale.propertyOverride) timeScale = new SlmToggleValue<float>()
                    {
                        value = otherProperty.timeScale.value
                    };
                    randomize = new SlmToggleValue<float>()
                    {
                        value = otherProperty.randomize.value
                    };
                }
                    
            }
        }
        
        
    }
}