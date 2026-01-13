using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BotController : MonoBehaviour
{
    public BotBodyBase body;
    public List<Transform> components = new List<Transform>();
    Dictionary<string, AttatchPoint> AttachmentPoints = new Dictionary<string, AttatchPoint>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(AttachmentPoints.Count == 0) InitialiseAttachPoints();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AssembleBot(BotRuntimeData AssembleData)
    {
        InitialiseAttachPoints();

        Dictionary<string, AttatchPoint> Design_AttachPoints = AssembleData.AttachPoints;

        Debug.Log(AttachmentPoints.Count);

        foreach (var key in Design_AttachPoints.Keys)
        {
            if (!AttachmentPoints.ContainsKey(key)) continue;

            Debug.Log(key);

            if(Design_AttachPoints[key].botComponent == null) continue;

            GameObject NewComponent = Instantiate(Design_AttachPoints[key].botComponent.ComponentDefaultData.DefaultPrefab, AttachmentPoints[key].transform);

            BotComponent NewCompScript = NewComponent.GetComponent<BotComponent>();

            AttachmentPoints[key].AttachNewComponent(NewCompScript);
           // NewCompScript.Initialise(Design_AttachPoints[key].botComponent.DesignInfo, this);
            NewCompScript.Initialise(Design_AttachPoints[key].botComponent.GetDesignInfo(), this);


            /*switch (Design_AttachPoints[key].botComponent.Type)
            {
                case ComponentType.None:

                    break;

                case ComponentType.Walker:
                    GameObject walker = Instantiate(WorkshopGeneral.instance.LegPrefab, AttachmentPoints[key].transform);
                    AttachmentPoints[key].AttachNewComponent(walker.GetComponent<BotComponent>());
                    walker.GetComponent<BotComponent>().Initialise(this);
                    Debug.Log("Added Walker Component");
                    break;

                case ComponentType.Grabber:

                    break;

                case ComponentType.Wheel:

                    break;
            }*/

            // GameObject ComponentType = DesingAPs[key];
            // GameObject newComponent = Instantiate();



        }
    }

    public void InitialiseAttachPoints()
    {
        AttachmentPoints.Clear();

        foreach (Transform child in transform.Find("Base").transform)
        {
            if (!child.gameObject.activeInHierarchy) continue;

            if (child.GetComponent<AttatchPoint>() == null) continue;

            AttatchPoint childAP = child.GetComponent<AttatchPoint>();

            AttachmentPoints.Add(childAP.Name, childAP);
        }
        Debug.Log(gameObject.name + " Attached " + AttachmentPoints.Count);
    }
}
