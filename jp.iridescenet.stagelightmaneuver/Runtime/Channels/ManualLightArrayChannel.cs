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
//     public class ManualLightArrayChannel:StageLightChannelBase
//     {
//         public List<LightPrimitiveValue> lightPrimitiveValues = new List<LightPrimitiveValue>();
//         [SerializeReference]public List<IStageLightChannel> stageLightFixtures = new List<IStageLightChannel>();
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
//                 if (i < stageLightFixtures.Count)
//                 {
//                     var iStageLightChannel = stageLightFixtures[i];
//                     var lightChannel = iStageLightChannel.TryGetChannel<LightChannel>();
//                     lightChannel.lightIntensity = lightPrimitiveValues[i].intensity;
//                     lightChannel.spotAngle = lightPrimitiveValues[i].angle;
//                     lightChannel.innerSpotAngle = lightPrimitiveValues[i].innerAngle;
//                     lightChannel.spotRange = lightPrimitiveValues[i].range;
//                     var tiltChannel = iStageLightChannel.TryGetChannel<LightTiltChannel>();
//                 }
//             }
//         }
//     }
// }