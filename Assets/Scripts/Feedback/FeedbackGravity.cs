using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

public class FeedbackGravity : MonoBehaviour
{

    private GravityGazeManager ggm;
    private Camera view;
    private Transform cursor;

    public float range = 0.1f;
    public float force = 2;
    public bool flipHorizontal = false;
    public bool active = true;

    // Use this for initialization
    void Start()
    {
        ggm = GameObject.Find("InputManager").GetComponent<GravityGazeManager>();
        //cursor = GameObject.Find("TrialManager").GetComponent<FeedbackConditionManager>().cursorObj.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (active)
        {
            Vector3 self2D = new Vector3(transform.position.x, transform.position.y);
            Vector3 cur2D = new Vector3(ggm.HitPositionSanGravity.x, ggm.HitPositionSanGravity.y);

            float dist = Vector3.Distance(self2D, cur2D);

            if (dist < range)
            {
                //print("dist:" + dist);

                //calc diff projected onto unit sphere
                Vector3 targetFromView = transform.position - ggm.GazeOrigin;
                Vector3 targetFromViewNormal = targetFromView.normalized;
                Vector3 normDiff = targetFromViewNormal - ggm.GazeNormal;

                //calc force factor
                Vector3 diff = self2D - cur2D;
                //print("2: " + transform.position + " : " + cursor.transform.position + " : " + diff);
                if (flipHorizontal)
                {
                    diff = new Vector3(-diff.x, diff.y, diff.z);
                }
                float relativeDistance = dist / range;
                float factor = (-Mathf.Pow(relativeDistance - 0.5f, 2) + 0.25f) * force;


                //print("3: " + relativeDistance + " : " + factor + " : " + normDiff);
                ggm.aggregatedGravity += factor * normDiff;
            }
        }
    }
}
