using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{

    [Serializable]
    public class MinMaxEasingValue
    {
        [SlmValue("Mode")] public AnimationMode mode = AnimationMode.Ease;
        [SlmValue("Inverse")] public bool inverse = false;
        [FormerlySerializedAs("valueRange")] [SlmValue("Range")]public Vector2 minMaxLimit = new Vector2(-180, 180);
        [FormerlySerializedAs("valueMinMax")] public Vector2 minMaxValue = new Vector2(-180, 180);
        [SlmValue("Easing")]public EaseType easeType = EaseType.Linear;
        [SlmValue("Constant")]public float constant = 0;
        [SlmValue("Curve")]public AnimationCurve animationCurve = new AnimationCurve(new Keyframe[]
        {
            new Keyframe(0,0),
            new Keyframe(1,1)
        });


        public MinMaxEasingValue()
        {
            mode = AnimationMode.Ease;
            minMaxLimit = new Vector2(-180, 180);
            minMaxValue = new Vector2(-90, 90);
            easeType = EaseType.Linear;
            constant = 0;
            animationCurve = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0,0),
                new Keyframe(1,1)
            });
        }
        
        public MinMaxEasingValue(MinMaxEasingValue other)
        {
            mode = other.mode;
            inverse = other.inverse;
            minMaxLimit = new Vector2( other.minMaxLimit.x, other.minMaxLimit.y);
            minMaxValue = new Vector2( other.minMaxValue.x, other.minMaxValue.y);
            easeType = other.easeType;
            constant = other.constant;
            animationCurve = SlmUtility.CopyAnimationCurve(other.animationCurve);
        }
        
        public MinMaxEasingValue(AnimationMode mode, Vector2 minMaxLimit, Vector2 minMaxValue, EaseType easeType, float constant, AnimationCurve animationCurve)
        {
            this.mode = mode;
            this.minMaxLimit = new Vector2( minMaxLimit.x, minMaxLimit.y);
            this.minMaxValue = new Vector2( minMaxValue.x, minMaxValue.y);
            this.easeType = easeType;
            this.constant = constant;
            var keys = new List<Keyframe>();
            
            foreach (var keyframe in animationCurve.keys)
            {
                keys.Add(new Keyframe(keyframe.time, keyframe.value));
            }
            this.animationCurve = new AnimationCurve(keys.ToArray());
        }

        public float Evaluate(float t)
        {
            var time = inverse ? 1f - t : t;
            var value = 0f;
            if (mode == AnimationMode.AnimationCurve)
            {
                value = animationCurve.Evaluate(time);
            }
            else if (mode == AnimationMode.Ease)
            {
                value = EaseUtil.GetEaseValue(easeType, time, 1f,minMaxValue.x,
                    minMaxValue.y);
            }
            else if (mode == AnimationMode.Constant)
            {
                value = constant;
            }

            return value;
        }
    }
}