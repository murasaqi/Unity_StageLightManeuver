// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// namespace StageLightManeuver
// {
//     [Serializable]
//     
//     public class StaggerPrimitiveValue
//     {
//         public StaggerCalculationType staggerCalculationType = StaggerCalculationType.StaggerInOut;
//         static float animationDuration = 1f;
//         [Range(0,1)]public float delayRatio = 0.1f;
//         public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0,0,1,1);
//         public List<Vector2> staggerInfo = new List<Vector2>();
//         public List<Vector2> randomStaggerInfo = new List<Vector2>();
//         
//         
//         public StaggerPrimitiveValue()
//         {
//             staggerCalculationType = StaggerCalculationType.StaggerInOut;
//             animationDuration = 1f;
//             delayRatio = 0.1f;
//             animationCurve = AnimationCurve.EaseInOut(0,0,1,1);
//             staggerInfo = new List<Vector2>();
//             randomStaggerInfo = new List<Vector2>();
//         }
//         
//         public StaggerPrimitiveValue(StaggerPrimitiveValue staggerPrimitiveValue)
//         {
//             staggerCalculationType = staggerPrimitiveValue.staggerCalculationType;
//             delayRatio = staggerPrimitiveValue.delayRatio;
//             animationCurve = SlmUtility.CopyAnimationCurve(staggerPrimitiveValue.animationCurve);
//             staggerInfo = new List<Vector2>(staggerPrimitiveValue.staggerInfo);
//             randomStaggerInfo = new List<Vector2>(staggerPrimitiveValue.randomStaggerInfo);
//         }
//         
//         public void ResyncArraySize(int count)
//         {
//             var countDifference = count - staggerInfo.Count;
//             if (countDifference > 0)
//             {
//                 for (int i = 0; i < countDifference; i++)
//                 {
//                     staggerInfo.Add(new Vector2(0, 1));
//                 }
//             }
//             else if (countDifference < 0)
//             {
//                 for (int i = 0; i < -countDifference; i++)
//                 {
//                     staggerInfo.RemoveAt(staggerInfo.Count - 1);
//                 }
//             }
//         }
//         
//         public void CalculateStaggerTime()
//         {
//             if(staggerCalculationType == StaggerCalculationType.Manual) return;
//             if (delayRatio >= 1)
//             {
//                 delayRatio = 0.99f;
//             }
//             
//            
//             if( staggerCalculationType == StaggerCalculationType.StaggerIn)
//             {
//                 var delayStep = delayRatio / (staggerInfo.Count-1);    
//                 
//                 for (int i = 0; i < staggerInfo.Count; i++)
//                 {
//                     var delay = delayStep * i;
//                     staggerInfo[i] = new Vector2(delay, 1f);
//                 }
//             } else if( staggerCalculationType == StaggerCalculationType.StaggerOut)
//             {
//                 var delayStep = delayRatio / (staggerInfo.Count-1);
//                 for (int i = 0; i < staggerInfo.Count; i++)
//                 {
//                     var delay = animationDuration-delayStep * i;
//                     staggerInfo[i] = new Vector2(0f, delay);
//                 }
//             }else if (staggerCalculationType == StaggerCalculationType.StaggerInOut)
//             {
//                 var duration = animationDuration*(1-delayRatio);
//                 for (int i = 0; i < staggerInfo.Count; i++)
//                 {
//                     var delayStep = delayRatio / (staggerInfo.Count-1);
//                     var delay = (delayStep) * i;
//                     staggerInfo[i] = new Vector2(delay, delay+duration);
//                 }
//             }
//
//             
//         }
//         public void CalculateRandomStaggerTime()
//         {
//             if(staggerCalculationType != StaggerCalculationType.Random) return;
//             for (int i = 0; i < randomStaggerInfo.Count; i++)
//             {
//                 var duration = animationDuration * (1 - delayRatio);
//                 var totalDelay = animationDuration - duration;
//                 var delay = Random.Range(0f, totalDelay);
//                 randomStaggerInfo[i] = new Vector2(delay, delay+duration);
//             }
//         }
//         
//         public Vector2 GetStaggerStartEnd(int index)
//         {
//             if(staggerCalculationType == StaggerCalculationType.Random)
//             {
//                 return randomStaggerInfo[index];
//             }
//             return staggerInfo[index];
//         }
//         
//         public float Evaluate(float normalizedTime,int index)
//         {
//             var staggerStartEnd = GetStaggerStartEnd(index);
//             var progress = Mathf.InverseLerp(staggerStartEnd.x, staggerStartEnd.y, normalizedTime);
//             var resut = animationCurve.Evaluate(progress);
//             return resut;
//
//         }
//     }
// }