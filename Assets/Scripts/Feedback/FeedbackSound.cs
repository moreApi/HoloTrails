using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FeedbackSound : MonoBehaviour, IFocusable
{
    public bool active = true;
    public void OnFocusEnter()
    {
        if (active)
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.Play();
        }

    }

    public void OnFocusExit()
    {


    }
}
    