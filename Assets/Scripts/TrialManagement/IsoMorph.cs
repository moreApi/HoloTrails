using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoMorph : MonoBehaviour {

    private IsoAnalyser analyzer;

    public GameObject[] targets;
    public GameObject center;

    public float _distance = 0.2f;
    public float distance
    {
        get { return _distance; }
        set { _distance = value;  morphed = false; }
    }
    public float _size = 0.5f;
    public float size
    {
        get { return _size; }
        set { _size = value; morphed = false; }
    }


    public bool morphed = false;

    private float oldDistance = 0.2f;

    void Start()
    {
        analyzer = GetComponent<IsoAnalyser>();
    }

	// Update is called once per frame
	void Update () {
		if (!morphed)
        {
            morph();
            morphed = true;
        }
	}

    private void morph()
    {
        Vector3 newScale = new Vector3(size, 0.001f, size);

        center.transform.localScale = newScale * 1.5f;
        Light halo = center.GetComponent<Light>();
        halo.range = size * 2 * 1.5f;

        foreach (GameObject obj in targets)
        {
            Transform trans = obj.transform;

            trans.localScale = newScale;
            trans.localPosition *= distance / oldDistance;

            halo = obj.GetComponent<Light>();
            halo.range = size * 2;

        }
        oldDistance = distance;

        analyzer.Analyze();
    }
}
