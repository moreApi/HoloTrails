using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

public class FeedbackGlow : MonoBehaviour, IFocusable {

    float enterTime = 0;

    public bool active = true;

    Light halo;

    void Start()
    {
        halo = gameObject.GetComponent<Light>();
    }
    void Update()
    {
        if (active && halo.enabled & enterTime + 20 < Time.time)
        {
            halo.enabled = false;
        }
    }

    public void OnFocusEnter()
    {

        if (active)
        {
            enterTime = Time.time;
            halo.enabled = true;
        }
    }

    public void OnFocusExit()
    {
        if (active)
        {

            halo.enabled = false;
        }
    }    
}
