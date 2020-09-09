using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FlowerMenuBuilder : MonoBehaviour
{

    public GameObject ButtonPrefab;
    public float SpaceBetweenButtons = 1f;
    public float SpaceBetweenMenues = 2.2f;
    [TextArea(3, 10)]
    public string items = "Entry 1\n.Subentry1\n.Subentry2\nEntry2";
    public bool hasChanged = true;

}

#if UNITY_EDITOR
[CustomEditor(typeof(FlowerMenuBuilder))]
public class FlowerMenuBuilderInspector : Editor
{
    FlowerMenuBuilder fmb = null;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (fmb == null)
            fmb = (FlowerMenuBuilder)target;

        if (fmb.hasChanged)
        {
            if (fmb.ButtonPrefab == null)
                return;

            for (int i = fmb.transform.childCount - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(fmb.transform.GetChild(i).gameObject);
            }

            MenuEntry menu = ListMenuBuilderInspector.parseMenu(fmb.items);

            buildSubButton(menu, fmb.transform);
            
            foreach (Transform child in fmb.transform)
            {
                child.gameObject.SetActive(true);
            }
            



            fmb.hasChanged = false;
        }
    }

    private void buildSubButton(MenuEntry entry, Transform parent)
    {

        float zOffset = -fmb.SpaceBetweenMenues;
        float xOffset = fmb.SpaceBetweenButtons;
        

        for (int i = 0; i < entry.subMenu.Count; i++)
        {
            MenuEntry subEntry = entry.subMenu[i];

            GameObject subButton = Instantiate(fmb.ButtonPrefab, parent);
            Vector3 newTrans = Quaternion.Euler(0, i * 360 / entry.subMenu.Count,0) * new Vector3(xOffset, zOffset,0);

            subButton.transform.localPosition = newTrans;

            subButton.name = subEntry.Name;
            subButton.GetComponentInChildren<TextMesh>().text = subEntry.Name;


            buildSubButton(subEntry, subButton.transform);

            subButton.SetActive(false); 
        }

    }
}
#endif
