using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class SelectionConfirmerSimple : MonoBehaviour, IFocusable, IInputClickHandler
{

    public Material confirmationMatrial;
    public float minDuration = 1;
    public bool dwell = false;

    private Material oldMaterial;
    private Renderer rend;

    private float confirmTime = 0;
    private float lingerTime = float.MaxValue;


    private void Start()
    {
        rend = gameObject.GetComponent<Renderer>();
        oldMaterial = rend.material;
    }

    void Update()
    {
        if ( dwell & Time.time - lingerTime > 0.5f)
        {
            ConfirmationAction();
        }
    }

    public  void ConfirmationAction()
    {
        rend.material = confirmationMatrial;
        confirmTime = Time.time;
    }

    public void ResetMaterial()
    {
        rend.material = oldMaterial;
    }

    public void OnFocusEnter()
    {
        lingerTime = Time.time;
    }

    public void OnFocusExit()
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
        lingerTime = float.MaxValue;
    }

    public void CancelMaterialChange()
    {
        CancelInvoke();
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        ConfirmationAction();
    }
}
