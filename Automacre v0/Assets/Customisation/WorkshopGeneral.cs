using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class WorkshopGeneral : MonoBehaviour
{
    public static WorkshopGeneral instance;

    public bool useKeyframeAnim;
    public GameObject LegPrefab;
    public GameObject FootPrefab;
    public List<BotComponent> ComponentsList = new List<BotComponent>();
    public List<BotComponent> ComponentsList_Keyframed = new List<BotComponent>();

    //public List<GameObject> ComponentsList = new List<GameObject>();

    public BotComponent CurrentSelectedComponentToPlace;
    public BotComponent SelectedComponentOnBot;
    public ComponentOptionDetails CurrentCopiedOptions;
    public GameObject SelectionArrowPrefab;
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

        

        /* if (SelectedComponentOnBot == null) { GameObject.FindFirstObjectByType<ComponentOptionsPopUp>(FindObjectsInactive.Include).gameObject.SetActive(false); }
         else { GameObject.FindFirstObjectByType<ComponentOptionsPopUp>(FindObjectsInactive.Include).gameObject.SetActive(true); }*/
        if(GameObject.FindFirstObjectByType<ComponentOptionsPopUp>(FindObjectsInactive.Include) != null)
        GameObject.FindFirstObjectByType<ComponentOptionsPopUp>(FindObjectsInactive.Include).gameObject.SetActive(SelectedComponentOnBot != null);
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

        SelectedComponentOnBot = SelectedComponent;

        if (GameObject.FindGameObjectWithTag("SelectionArrow") != null) Destroy(GameObject.FindGameObjectWithTag("SelectionArrow"));
        GameObject SelectionArrow = GameObject.Instantiate(SelectionArrowPrefab, SelectedComponent.GetSelectedArrowPos(), SelectedComponent.transform.rotation);

        if (OptionsCanvas.CurrentOption != null) Destroy(OptionsCanvas.CurrentOption);

        GameObject NewOptionsPopup;

        switch (SelectedComponent.ComponentDefaultData.Type)
        {
            case ComponentType.Walker:

                NewOptionsPopup = Instantiate(OptionsCanvas.Options_Walker, OptionsCanvas.GetComponent<RectTransform>());
                OptionsCanvas.CurrentOption = NewOptionsPopup;
                OptionsCanvas.SetOptionDetails(SelectedComponent);

                break;
            case ComponentType.Grabber:

                NewOptionsPopup = Instantiate(OptionsCanvas.Options_Grabber, OptionsCanvas.GetComponent<RectTransform>());
                OptionsCanvas.CurrentOption = NewOptionsPopup;
                OptionsCanvas.SetOptionDetails(SelectedComponent);

                break;
        }

    }

    public void RemoveComponentFromBot(BotComponent RemovedComponent)
    {
        RemovedComponent.RemoveFromBot();
        ComponentOptionsPopUp OptionsCanvas = GameObject.Find("Canvas").transform.Find("ComponentOptions").GetComponent<ComponentOptionsPopUp>();
        if (GameObject.FindGameObjectWithTag("SelectionArrow") != null) Destroy(GameObject.FindGameObjectWithTag("SelectionArrow"));
        OptionsCanvas.LeaveOptions();
    }

    public BotComponent GetComponentByName(string name)
    {
        BotComponent component1 = null;
        List<BotComponent> list = ComponentsList;

        if (useKeyframeAnim)
        {
            list = ComponentsList_Keyframed;
        }

        foreach (BotComponent component in list)
        {
            Debug.Log(component.CompName);
            if (component.CompName != name) continue;

            component1 = component;
        }
        if (component1 == null) Debug.LogWarning("Component Name couldnt be found");
        return component1;
    }

    public static Transform GetTopParent(Transform t)
    {
        Transform curParent = t;

        while (curParent.parent != null)
        {
            curParent = curParent.parent;

        }

        return curParent;
    }

    public void SetAnimType(bool keyframed)
    {
        useKeyframeAnim = keyframed;
    }
}
