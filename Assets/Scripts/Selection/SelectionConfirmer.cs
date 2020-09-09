using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class SelectionConfirmer : AConfirmer, IFocusable
{

    public Material confirmationMatrial;
    public float minDuration = 1;

    public bool targetIsActive = false;

    private Material oldMaterial;
    private Renderer rend;
    private TrialConductor trialConductor;
    private TrialLogger logger;

    private float confirmTime = 0;
    private float lingerTime = 0;


    private new void Start()
    {
        base.Start();
        rend = gameObject.GetComponent<Renderer>();
        trialConductor = GameObject.Find("TrialManager").GetComponent<TrialConductor>();
        logger = GameObject.Find("TrialManager").GetComponent<TrialLogger>();
        oldMaterial = rend.material;
    }

    public override void ConfirmationAction()
    {
        if (targetIsActive)
        {
            rend.material = confirmationMatrial;
            confirmTime = Time.time;
            trialConductor.receiveConfirmation();
            targetIsActive = false;
            logger.serverLog(TrialEventType.Hit,""+(Time.time-lingerTime));
        }
        else
        {
            logger.log(TrialEventType.Miss);
        }
    }

    public void ResetMaterial()
    {
        rend.material = oldMaterial;
    }

    public new void OnFocusEnter()
    {
        base.OnFocusEnter();
        lingerTime = Time.time;
    }

    public new void OnFocusExit()
    {
        base.OnFocusExit();

        if (!targetIsActive)
        {
            float diff = Time.time - confirmTime;
            if (diff < minDuration)
            {
                Invoke("ResetMaterial", Mathf.Clamp(minDuration - diff, 0, float.MaxValue));
            }
            else
            {
                ResetMaterial();
            }
        }
    }

    public void CancelMaterialChange()
    {
        CancelInvoke();
    }
}
