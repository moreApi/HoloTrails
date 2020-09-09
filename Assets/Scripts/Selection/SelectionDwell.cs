using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class SelectionDwell : MonoBehaviour, IFocusable
{

    public float dwellTime = 0.500f;

    private AConfirmer confirmer;
    private float enterTime = 0;

    public void OnFocusEnter()
    {
        enterTime = Time.time;
    }

    public void OnFocusExit()
    {
        enterTime = 0;
    }

    // Use this for initialization
    void Start()
    {
        confirmer = gameObject.GetComponent<AConfirmer>();
    }

    // Update is called once per frame
    void Update () {
		if (enterTime != 0)
        {
            if (enterTime + dwellTime < Time.time)
            {
                confirmer.Confirm();
                enterTime = 0;
            }
        }
	}
}
