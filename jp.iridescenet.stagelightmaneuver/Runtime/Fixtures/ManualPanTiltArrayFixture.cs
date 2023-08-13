// using System.Collections.Generic;
// using UnityEngine;
// using Vector2 = System.Numerics.Vector2;
//
// namespace StageLightManeuver
// {
//     [AddComponentMenu("")]
//     public class ManualPanTiltArrayFixture : StageLightFixtureBase
//     {
//         // public List<Vector2> panTiltPositions = new List<Vector2>();
//         [SerializeReference]public List<IStageLightFixture> stageLights = new List<IStageLightFixture>();
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
// //                     var iStageLightFixture = stageLights[i];
// //                     
// //                     panTiltPositions[i].X += panFixture.rotationSpeed;
// //                     
// //                     var tiltFixture = iStageLightFixture.TryGetFixture<LightTiltFixture>();
// //                     // rotationVector = til
// //                     tiltFixture.rotateTransform.transform.localEulerAngles = tiltFixture.rotationVector * panTiltPositions[i].Y;
// // Debug.Log($"{panTiltPositions[i].X},{panTiltPositions[i].Y},{panFixture.rotationVector},{tiltFixture.rotationVector}");
// //                 }
// //             }
//         }
//     }
// }