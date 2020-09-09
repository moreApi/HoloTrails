using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectionClickMiss : MonoBehaviour, IInputClickHandler
{
    private TrialLogger logger;

    void Start()
    {
        logger = GameObject.Find("TrialManager").GetComponent<TrialLogger>();
    }


    public void OnInputClicked(InputEventData eventData)
    {
        logger.serverLog(TrialEventType.Miss);
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        logger.serverLog(TrialEventType.Miss);
    }
}
