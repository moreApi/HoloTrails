// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;

/// <summary>
/// The gaze manager manages everything related to a gaze ray that can interact with other objects.
/// </summary>
public class GravityGazeManager : GazeManager
{
    public Vector3 aggregatedGravity = Vector3.zero;

    public Vector3 HitPositionSanGravity { get; private set; }

    //change modifiers in "GazeManger" from private to protected (virtual)!!
    protected override GameObject RaycastPhysics()
    {
        GameObject previousFocusObject = HitObject;

        //rotate gaze acording to gravity
        Vector3 gravitatedGazeNormal = GazeNormal + aggregatedGravity;
        gravitatedGazeNormal.Normalize();
        aggregatedGravity = Vector3.zero;


        // If there is only one priority, don't prioritize
        if (RaycastLayerMasks.Length == 1)
        {
            IsGazingAtObject = Physics.Raycast(GazeOrigin, gravitatedGazeNormal, out hitInfo, MaxGazeCollisionDistance, RaycastLayerMasks[0]);
        }
        else
        {
            // Raycast across all layers and prioritize
            RaycastHit? hit = PrioritizeHits(Physics.RaycastAll(new Ray(GazeOrigin, gravitatedGazeNormal), MaxGazeCollisionDistance, -1));

            IsGazingAtObject = hit.HasValue;
            if (IsGazingAtObject)
            {
                hitInfo = hit.Value;
            }
        }

        if (IsGazingAtObject)
        {
            HitObject = HitInfo.collider.gameObject;
            HitPosition = HitInfo.point;
            lastHitDistance = HitInfo.distance;
            HitPositionSanGravity = GazeOrigin + (GazeNormal * lastHitDistance);
        }
        else
        {
            HitObject = null;
            HitPosition = GazeOrigin + (gravitatedGazeNormal * lastHitDistance);
            HitPositionSanGravity = GazeOrigin + (GazeNormal * lastHitDistance);
        }
        return previousFocusObject;
    }
}
