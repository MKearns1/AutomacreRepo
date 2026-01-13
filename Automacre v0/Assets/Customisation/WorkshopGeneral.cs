using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class WorkshopGeneral : MonoBehaviour
{
    public static WorkshopGeneral instance;

    public GameObject LegPrefab;
    public GameObject FootPrefab;
    public List<BotComponent> ComponentsList = new List<BotComponent>();
    //public List<GameObject> ComponentsList = new List<GameObject>();

    public BotComponent CurrentSelectedComponent;
    RectTransform cursor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        Cursor.visible = false;
        cursor = GameObject.Find("Cursor").GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.Find("Canvas").transform as RectTransform, Input.mousePosition, null, out pos);

        cursor.anchoredPosition = pos;
    }

    public void SelectComponent(string name)
    {
        BotComponent comp = GetComponentByName(name);

        if(comp == CurrentSelectedComponent)
        {
            CurrentSelectedComponent = null;
            cursor.Find("Text").GetComponent<TextMeshProUGUI>().text = "";
            return;
        }

        CurrentSelectedComponent = GetComponentByName(name);
        cursor.Find("Text").GetComponent<TextMeshProUGUI>().text = name;

    }

    public BotComponent GetComponentByName(string name)
    {
        BotComponent component1 = null;

        foreach (BotComponent component in ComponentsList)
        {
            Debug.Log(component.CompName);
            if (component.CompName != name) continue;

            component1 = component;
        }
        if (component1 == null) Debug.LogWarning("Component Name couldnt be found");
        return component1;
    }
}
