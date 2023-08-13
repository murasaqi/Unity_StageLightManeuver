// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace StageLightManeuver
// {
//
//     [Serializable]
//     [AddComponentMenu("")]
//   
//     public class ManualLightArrayFixture:StageLightFixtureBase
//     {
//         public List<LightPrimitiveValue> lightPrimitiveValues = new List<LightPrimitiveValue>();
//         [SerializeReference]public List<IStageLightFixture> stageLights = new List<IStageLightFixture>();
//
//         public override void Init()
//         {
//             base.Init();
//             updateOrder = 999;
//         }
//
//         public override void EvaluateQue(float currentTime)
//         {
//             for (int i = 0; i < lightPrimitiveValues.Count; i++)
//             {
//                 if (i < stageLights.Count)
//                 {
//                     var iStageLightFixture = stageLights[i];
//                     var lightFixture = iStageLightFixture.TryGetFixture<LightFixture>();
//                     lightFixture.lightIntensity = lightPrimitiveValues[i].intensity;
//                     lightFixture.spotAngle = lightPrimitiveValues[i].angle;
//                     lightFixture.innerSpotAngle = lightPrimitiveValues[i].innerAngle;
//                     lightFixture.spotRange = lightPrimitiveValues[i].range;
//                     var tiltFixture = iStageLightFixture.TryGetFixture<LightTiltFixture>();
//                 }
//             }
//         }
//     }
// }