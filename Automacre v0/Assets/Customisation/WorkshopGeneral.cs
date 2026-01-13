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

    public BotComponent CurrentSelectedComponentToPlace;
    public BotComponent SelectedComponentOnBot;
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
        //Cursor.visible = false;
        cursor = GameObject.Find("Cursor").GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.Find("Canvas").transform as RectTransform, Input.mousePosition, null, out pos);

        cursor.anchoredPosition = pos;
    }

    public void SelectMenuComponent(string name)
    {
        BotComponent comp = GetComponentByName(name);

        if(comp == CurrentSelectedComponentToPlace)
        {
            CurrentSelectedComponentToPlace = null;
            cursor.Find("Text").GetComponent<TextMeshProUGUI>().text = "";
            return;
        }

        CurrentSelectedComponentToPlace = GetComponentByName(name);
        cursor.Find("Text").GetComponent<TextMeshProUGUI>().text = name;

    }

    public void SelectBotsComponent(BotComponent SelectedComponent)
    {
        ComponentOptionsPopUp OptionsCanvas = GameObject.Find("Canvas").transform.Find("ComponentOptions").GetComponent<ComponentOptionsPopUp>();

        switch (SelectedComponent.ComponentDefaultData.Type)
        {
            case ComponentType.Walker:

                GameObject NewOptionsPopup = Instantiate(OptionsCanvas.Options_Walker, OptionsCanvas.GetComponent<RectTransform>());
                OptionsCanvas.CurrentOption = NewOptionsPopup;
                OptionsCanvas.SetOptionDetails(SelectedComponent);

                break;
        }

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
