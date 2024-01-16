// using System.Collections.Generic;
// using UnityEngine;
// using Vector2 = System.Numerics.Vector2;
//
// namespace StageLightManeuver
// {
//     [AddComponentMenu("")]
//     public class ManualPanTiltArrayChannel : StageLightChannelBase
//     {
//         // public List<Vector2> panTiltPositions = new List<Vector2>();
//         [SerializeReference]public List<IStageLightChannel> stageLights = new List<IStageLightChannel>();
//
//
//         public override void Init()
//         {
//             base.Init();
//             updateOrder = 999;
//         }
//
//         public override void EvaluateQue(float time)
//         {
// //             for (int i = 0; i < panTiltPositions.Count; i++)
// //             {
// //                 if (i < stageLights.Count)
// //                 {
// //                     var iStageLightChannel = stageLights[i];
// //                     
// //                     panTiltPositions[i].X += panChannel.rotationSpeed;
// //                     
// //                     var tiltChannel = iStageLightChannel.TryGetChannel<LightTiltChannel>();
// //                     // rotationVector = til
// //                     tiltChannel.rotateTransform.transform.localEulerAngles = tiltChannel.rotationVector * panTiltPositions[i].Y;
// // Debug.Log($"{panTiltPositions[i].X},{panTiltPositions[i].Y},{panChannel.rotationVector},{tiltChannel.rotationVector}");
// //                 }
// //             }
//         }
//     }
// }