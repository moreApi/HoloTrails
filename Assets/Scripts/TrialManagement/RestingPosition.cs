using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RestingPosition : MonoBehaviour, IFocusable
{
    private TrialConductor tc;

    private void Start()
    {
        tc = GameObject.Find("TrialManager").GetComponent<TrialConductor>();
    }

    public void OnFocusEnter()
    {
        if (tc != null)
            tc.cursorIsInRestingPos();
    }

    public void OnFocusExit()
    {
    }
}
