using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionConditionManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public void setDwellSelection(float seconds)
    {
        //assuming only one Isowheel is active
        GameObject currentIsoWheel = GameObject.Find("IsoWheel");
        GameObject[] targets = currentIsoWheel.GetComponent<IsoMorph>().targets;

        foreach (GameObject obj in targets)
        {
            disableAllSelection(obj);
            SelectionDwell tmp = obj.GetComponent<SelectionDwell>();
            tmp.enabled = true;
            tmp.dwellTime = seconds;
        }
    }

    public void setClickerSelection()
    {
        //assuming only one Isowheel is active
        GameObject currentIsoWheel = GameObject.Find("IsoWheel");
        GameObject[] targets = currentIsoWheel.GetComponent<IsoMorph>().targets;

        foreach (GameObject obj in targets)
        {
            disableAllSelection(obj);
            obj.GetComponent<SelectionClick>().enabled = true;
        }
    }

    public void setAirTapSelection()
    {
        setClickerSelection();
    }

    private void disableAllSelection(GameObject target)
    {
        target.GetComponent<SelectionClick>().enabled = false;
        target.GetComponent<SelectionDwell>().enabled = false;
    }
}
