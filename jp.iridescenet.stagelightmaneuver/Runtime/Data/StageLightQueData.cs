// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace StageLightManeuver
// {
//     [Serializable]
//     public class StageLightQueData
//     {
//
//         [SerializeReference]public List<SlmProperty> stageLightProperties;
//         public float weight = 1;
//         
//         
//         public StageLightQueData(StageLightQueData stageLightQueData)
//         {
//             this.stageLightProperties = stageLightQueData.stageLightProperties;
//             this.weight = stageLightQueData.weight;
//         }
//         
//         public StageLightQueData()
//         {
//             stageLightProperties = new List<SlmProperty>();
//             // stageLightProperties.Add(new TimeProperty());
//             weight = 1f;
//         }
//         public T TryGet<T>() where T : SlmProperty
//         {
//             foreach (var property in stageLightProperties)
//             {
//                 if (property.GetType() == typeof(T))
//                 {
//                     return property as T;
//                 }
//             }
//             return null;
//         }
//         
//     }
// }