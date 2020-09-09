using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

//change visibility levels in Cursor.cs accordingly 
public class GravityCursor : ObjectCursor{

    private GravityGazeManager ggm;


    new protected void Start()
    {
        ggm = GameObject.Find("InputManager").GetComponent<GravityGazeManager>();
        base.Start();

    }

    /// <summary>
    /// Update the cursor's transform
    /// </summary>
    protected override void UpdateCursorTransform()
    {
        // Get the necessary info from the gaze source
        RaycastHit hitResult = ggm.HitInfo;
        GameObject newTargetedObject = ggm.HitObject;

        // Get the forward vector looking back at camera
        Vector3 lookForward = -ggm.GazeNormal;

        // Normalize scale on before update
        targetScale = Vector3.one;

        // If no game object is hit, put the cursor at the default distance
        if (TargetedObject == null)
        {
            this.TargetedObject = null;
            targetPosition = ggm.HitPosition + (lookForward * SurfaceCursorDistance); // ggm.GazeOrigin + ggm.GazeNormal * DefaultCursorDistance;
            targetRotation = lookForward.magnitude > 0 ? Quaternion.LookRotation(lookForward, Vector3.up) : transform.rotation;
        }
        else
        {
            // Update currently targeted object
            this.TargetedObject = newTargetedObject;

            if (TargetedCursorModifier != null)
            {
                TargetedCursorModifier.GetModifiedTransform(this, out targetPosition, out targetRotation, out targetScale);
            }
            else
            {
                // If no modifier is on the target, just use the hit result to set cursor position
                targetPosition = hitResult.point + (lookForward * SurfaceCursorDistance);
                targetRotation = Quaternion.LookRotation(Vector3.Lerp(hitResult.normal, lookForward, LookRotationBlend), Vector3.up);
            }
        }

        float deltaTime = UseUnscaledTime
            ? Time.unscaledDeltaTime
            : Time.deltaTime;

        // Use the lerp times to blend the position to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, deltaTime / PositionLerpTime);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, deltaTime / ScaleLerpTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, deltaTime / RotationLerpTime);
    }
}


