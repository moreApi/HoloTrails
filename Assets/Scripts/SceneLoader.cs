using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour , IInputClickHandler{

    public string Scene;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        SceneManager.LoadScene(Scene);
    }
    
}
