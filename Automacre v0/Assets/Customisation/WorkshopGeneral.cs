using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

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
    public GameObject TransformGizmoPrefab;
    public GameObject CurTransformGizmo;
    RectTransform cursor;

    Transform BodySpawnPos { get { return transform.GetChild(0); } }
    public GameObject CurBody;
    public List<GameObject> BodyTypes = new List<GameObject>();
    public bool Mirror;

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
        SetBotBody("Boxy");
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
            cursor.Find("CompIcon").GetComponent<Image>().enabled = false;
            cursor.Find("CompIcon").GetChild(0).GetComponent<Image>().enabled = false;
            cursor.Find("CompIcon").Find("CompImage").GetComponent<Image>().enabled = false;
            return;
        }

        CurrentSelectedComponentToPlace = GetComponentByName(name);
        cursor.Find("Text").GetComponent<TextMeshProUGUI>().text = name;
        cursor.Find("CompIcon").GetComponent<Image>().enabled = true;
        cursor.Find("CompIcon").GetChild(0).GetComponent<Image>().enabled = true;
        cursor.Find("CompIcon").Find("CompImage").GetComponent<Image>().enabled = true;
        cursor.Find("CompIcon").Find("CompImage").GetComponent<Image>().sprite = GameObject.FindFirstObjectByType<CanvasManager>().GetCompIcon(name);

    }

    public void SelectBotsComponent(BotComponent SelectedComponent)
    {
        ComponentOptionsPopUp OptionsCanvas =
            GameObject.FindFirstObjectByType<CanvasManager>().WorkshopUI.transform.Find("ComponentOptions").GetComponent<ComponentOptionsPopUp>();

        SelectedComponentOnBot = SelectedComponent;

        if (GameObject.FindGameObjectWithTag("SelectionArrow") != null) Destroy(GameObject.FindGameObjectWithTag("SelectionArrow"));
        GameObject SelectionArrow = GameObject.Instantiate(SelectionArrowPrefab, SelectedComponent.GetSelectedArrowPos(), SelectedComponent.transform.rotation);
        SelectedComponentOnBot.OnSelected();


        //HUD OPTIONS
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
        if (Mirror)
        {
            GetMirroredAttachPoint(RemovedComponent.GetComponentInParent<AttatchPoint>(),null, RemovedComponent.GetComponentInParent<Bot_Workshop>().DesignData.AttachPoints).botComponent.RemoveFromBot();
        }
        RemovedComponent.RemoveFromBot();
        ComponentOptionsPopUp OptionsCanvas = GameObject.FindFirstObjectByType<CanvasManager>().WorkshopUI.transform.Find("ComponentOptions").GetComponent<ComponentOptionsPopUp>();
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

    public void SetBotBody(string BodyTypeName)
    {
        Bot_Workshop BotBase = GameObject.FindFirstObjectByType<Bot_Workshop>();

        if (CurBody != null)
        {
            BotBase.DestroyAllComponents();
            Destroy(CurBody);
        }

        GameObject newBody = Instantiate(GetBodyTypeByName(BodyTypeName), BotBase.transform);
        newBody.transform.position = BodySpawnPos.position;
        CurBody = newBody;
        CurBody.name = "BotBody";
        BotBase.BodyType = BodyTypeName;
        BotBase.Invoke("GetAllAttachPoints", .1f);// Delay is needed to give time for the new body to be instantiated.
        //BotBase.GetAllAttachPoints();
    }
    GameObject GetBodyTypeByName(string name)
    {
        foreach (var body in BodyTypes)
        {
            if (body.GetComponent<WorkshopBot_Body>().BodyType == name) return body;
        }
        return null;
    }

    public AttatchPoint GetMirroredAttachPoint(AttatchPoint ap, List<AttatchPoint> BotAttachPoints = default, System.Collections.Generic.Dictionary<string, AttatchPoint> DataAP = default )
    {
        Vector3 PredictedMirrorPos = new Vector3(-ap.transform.localPosition.x,ap.transform.localPosition.y,ap.transform.localPosition.z);

        AttatchPoint closestPoint = null;
        float Closest = float.MaxValue;

        if (BotAttachPoints != null)
        {
            foreach (var attatchPoint in BotAttachPoints)
            {
                float curClosest = Vector3.Distance(PredictedMirrorPos, attatchPoint.transform.localPosition);
                if (curClosest < Closest)
                {
                    closestPoint = attatchPoint;
                    Closest = curClosest;
                }
            }
        }
        if (DataAP != null)
        {
            foreach (var attatchPoint in DataAP.Values)
            {
                float curClosest = Vector3.Distance(PredictedMirrorPos, attatchPoint.transform.localPosition);
                if (curClosest < Closest)
                {
                    closestPoint = attatchPoint;
                    Closest = curClosest;
                }
            }
        }
        return closestPoint;
    }

    public void SetMirrorMode(bool mirror)
    {
        Mirror = mirror;
    }
}
