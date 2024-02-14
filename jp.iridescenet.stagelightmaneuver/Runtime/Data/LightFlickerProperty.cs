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
        
        [SlmValue("GameTIme")] public SlmToggleValue<bool> gameTime;
        [SlmValue("Flicker")] public SlmToggleValue<FastNoise.NoiseType> noiseType;
        private FastNoise fastNoise = new FastNoise();
        [SlmValue("Noise Amplitude")] public SlmToggleValue<float> noiseAmplitude;
        // [SlmValue("Noise Curve")] public SlmToggleValue<AnimationCurve> noiseCurve;
        [SlmValue("Time Scale")] public SlmToggleValue<float> timeScale;
        [SlmValue("Randomize")] public SlmToggleValue<float> randomize;
    [SlmValue("Noise Seed")] public SlmToggleValue<Vector2> noiseSeed;
        public LightFlickerProperty()
        {
            propertyName = "Flicker";
            propertyOverride = true;
            gameTime = new SlmToggleValue<bool>(){value = false};
            noiseType = new SlmToggleValue<FastNoise.NoiseType>(){value = FastNoise.NoiseType.Simplex};
            noiseAmplitude = new SlmToggleValue<float>(){value = 1f};
            noiseSeed = new SlmToggleValue<Vector2>(){value = new Vector2(0,0)};
            timeScale = new SlmToggleValue<float>(){value = 1f};
            randomize = new SlmToggleValue<float>(){value = 0f};
        }
        
        public override void ToggleOverride(bool toggle)
        {
            propertyOverride = toggle;
            gameTime.propertyOverride = toggle;
            // clockOverride.propertyOverride = toggle;
            noiseType.propertyOverride = toggle;
            noiseAmplitude.propertyOverride = toggle;
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
            
            return noiseAmplitude.value* fastNoise.GetNoise(time * timeScale.value, childIndex * randomize.value);
                
        }
        
        public LightFlickerProperty(LightFlickerProperty other)
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            gameTime = new SlmToggleValue<bool>(other.gameTime);
            noiseType = new SlmToggleValue<FastNoise.NoiseType>()
            {
                propertyOverride = other.noiseType.propertyOverride,
                value = other.noiseType.value
            };
            noiseAmplitude = new SlmToggleValue<float>(other.noiseAmplitude);
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
                    if(otherProperty.gameTime.propertyOverride) gameTime = new SlmToggleValue<bool>(otherProperty.gameTime);
                    if(otherProperty.noiseType.propertyOverride) noiseType.value = otherProperty.noiseType.value;
                    // if(otherProperty.clockOverride.propertyOverride) clockOverride = new SlmToggleValue<ClockOverride>(otherProperty.clockOverride);
                    if(otherProperty.noiseAmplitude.propertyOverride) 
                    {
                        noiseAmplitude = new SlmToggleValue<float>()
                        {
                            value = otherProperty.noiseAmplitude.value
                        };
                            
                    }
                    if(otherProperty.noiseSeed.propertyOverride) 
                    {
                        noiseSeed = new SlmToggleValue<Vector2>()
                        {
                            value = new Vector2(otherProperty.noiseSeed.value.x, otherProperty.noiseSeed.value.y)
                        };
                    }
                    if(otherProperty.timeScale.propertyOverride) {
                        timeScale = new SlmToggleValue<float>()
                        {
                            value = otherProperty.timeScale.value
                        };
                    }
                    randomize = new SlmToggleValue<float>()
                    {
                        value = otherProperty.randomize.value
                    };
                }
                    
            }
        }
        
        
    }
}