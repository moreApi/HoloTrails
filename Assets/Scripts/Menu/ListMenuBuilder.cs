using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ListMenuBuilder : MonoBehaviour
{

    public GameObject ButtonPrefab;
    public float SpaceBetweenButtons = 1.2f;
    public float SpaceBetweenMenues = 2.2f;
    [TextArea(3, 10)]
    public string items = "Entry 1\n.Subentry1\n.Subentry2\nEntry2";
    public bool hasChanged = true;

}

#if UNITY_EDITOR
[CustomEditor(typeof(ListMenuBuilder))]
public class ListMenuBuilderInspector : Editor
{
    ListMenuBuilder lmb = null;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (lmb == null)
            lmb = (ListMenuBuilder)target;

        if (lmb.hasChanged)
        {
            if (lmb.ButtonPrefab == null)
                return;

            for (int i = lmb.transform.childCount-1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(lmb.transform.GetChild(i).gameObject);
            }

            MenuEntry menu = parseMenu(lmb.items);

            buildSubButton(menu, lmb.transform);

            foreach(Transform child in lmb.transform)
            {
                child.gameObject.SetActive(true);
            }
            
            

            lmb.hasChanged = false;
        }
    }

    private void buildSubButton(MenuEntry entry, Transform parent)
    {

        float yOffset = lmb.SpaceBetweenMenues;
        float xOffset = -lmb.SpaceBetweenButtons * ((entry.subMenu.Count / 2) - ((entry.subMenu.Count % 2 == 0) ? 0.5f : 0));

        foreach (MenuEntry subEntry in entry.subMenu)
        {
            GameObject subButton = Instantiate(lmb.ButtonPrefab, parent);
            subButton.transform.localPosition = new Vector3(xOffset, -yOffset, 0);

            xOffset += lmb.SpaceBetweenButtons;

            subButton.name = subEntry.Name;
            subButton.GetComponentInChildren<TextMesh>().text = subEntry.Name;
            
            buildSubButton(subEntry, subButton.transform);

            subButton.SetActive(false);
        }

    }

    public static MenuEntry parseMenu(string menuText)
    {

        string[] split = menuText.Split('\n');

        MenuEntry dummy = buildSubMenu("dummy", new List<string>(split));

        return dummy;
    }

    /** Takes list of sub entries */
    public static MenuEntry buildSubMenu(string name, List<string> subItems)
    {
        if (subItems.Count == 0)
        {
            return new MenuEntry(name);
        }

        MenuEntry root = new MenuEntry(name);

        List<string> subSubItems = null;
        string subItem = null;


        foreach (string item in subItems)
        {

            if (item.StartsWith("."))
            {
                string cleanedName = item.Substring(1); //remove leading dot
                subSubItems.Add(cleanedName);
            }
            else
            {
                if (subItem != null)
                {
                    MenuEntry tmpp = buildSubMenu(subItem, subSubItems);
                    root.subMenu.Add(tmpp);
                }

                subItem = item;
                subSubItems = new List<string>();
            }
        }

        MenuEntry tmp = buildSubMenu(subItem, subSubItems);
        root.subMenu.Add(tmp);

        return root;


    }
}

public class MenuEntry
{
    public string Name;
    public List<MenuEntry> subMenu;

    public MenuEntry(string Name)
    {
        this.Name = Name;
        subMenu = new List<MenuEntry>();
    }
}

#endif