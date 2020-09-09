using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

public class SelectionClick : MonoBehaviour, IInputClickHandler {
    private AConfirmer confirmer;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (enabled)
        {
            confirmer.Confirm();
        }
    }

    // Use this for initialization
    void Start () {
        confirmer =  gameObject.GetComponent<AConfirmer>();
	}
	
	// Update is called once per frame
	void Update () {
		//function need for enabled checkbox to be present
	}
}
