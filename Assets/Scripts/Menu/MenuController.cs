using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MenuController : AConfirmer
{

    private bool isToplevel;
    private TrialLogger logger;

    public enum State { active, selected, background, closed }

    public Material defaultMat;
    public Material selectionMat;
    public Material backgroundMat;

    public bool flowerMenu = false;

    public State state;


    public override void ConfirmationAction()
    {
        setMenuState(State.selected);
        logger.logMenu(gameObject.name);
    }



    // Use this for initialization
    new void Start()
    {
        base.Start();
        isToplevel = name.Contains("Anchor");
        logger = GameObject.Find("TrialManager").GetComponent<TrialLogger>();
    }

    /** false recursevly, true only one level*/
    public void setMenuState(State state)
    {
        Material newMat = defaultMat;

        State newState = state;

        switch (state)
        {
            case State.selected:
                if (flowerMenu & this.state == State.selected)
                {
                    //go one level back / close sub menu
                    setMenuState(State.closed);
                    setMenuState(State.active);
                    newState = State.active; //state from earlier line gets overwritten beause of nesting
                }
                else
                {
                    if (!isToplevel)
                        transform.parent.GetComponent<MenuController>().setChildenMenuState(State.background);
                    setChildenMenuState(State.active);
                    newMat = selectionMat;
                }
                break;

            case State.active:

                gameObject.SetActive(true);
                newMat = defaultMat;
                
                break;

            case State.background:
                setChildenMenuState(State.closed);

                if (flowerMenu)
                    newMat = backgroundMat;
                else
                    newMat = defaultMat;
                break;

            case State.closed:
                setChildenMenuState(State.closed);
                gameObject.SetActive(false);
                break;
        }

        if (!isToplevel)
        {
            Renderer rend = GetComponent<Renderer>();
            rend.material = newMat;
        }

        this.state = newState;

    }

    public void setChildenMenuState(MenuController.State state)
    {
        foreach (Transform trans in transform)
        {
            if (trans.name != "Text")
            {
                trans.GetComponent<MenuController>().setMenuState(state);
            }
        }
    }

}
