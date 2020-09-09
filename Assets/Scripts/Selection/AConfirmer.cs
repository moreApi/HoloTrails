using HoloToolkit.Unity.InputModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public abstract class AConfirmer : MonoBehaviour, IFocusable
{
    NetworkSelectionController nmc;

    protected void Start()
    {
        nmc = GetComponentInParent<NetworkSelectionController>();
    }

    abstract public void ConfirmationAction();

    public void Confirm()
    {
        nmc.ConfirmGazedAt();
    }



    public void OnFocusEnter()
    {
        nmc.currentGazedAt = this;
    }

    public void OnFocusExit()
    {
        if (nmc.currentGazedAt == this)
        {
            nmc.currentGazedAt = null;
        }
    }
}

