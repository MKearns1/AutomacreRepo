using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BotController_Procedural : BotController
{
    public BotBodyBase body;
    Dictionary<string, AttatchPoint> AttachmentPoints = new Dictionary<string, AttatchPoint>();
    public SupportManager supportManager;


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

        Debug.LogWarning("ahah"+ AssembleData.Components.Count);

        List<BotComponent_Walker> walkers = new();

        foreach (var key in Design_AttachPoints.Keys)
        {
            if (!AttachmentPoints.ContainsKey(key)) continue;

            Debug.Log(key);

            if(Design_AttachPoints[key].botComponent == null) continue;

            GameObject NewComponent = Instantiate(Design_AttachPoints[key].botComponent.ComponentDefaultData.DefaultPrefab, AttachmentPoints[key].transform);

            BotComponent NewCompScript = NewComponent.GetComponent<BotComponent>();

            AttachmentPoints[key].AttachNewComponent(NewCompScript);
            NewCompScript.Initialise(Design_AttachPoints[key].botComponent.GetDesignInfo(), this);

            Debug.LogWarning("this "+AttachmentPoints[key].botComponent);

            if(!(NewCompScript is BotComponent_Walker walker))continue;
           // if(walker.GetComponentInChildren<ProceduralWalker>() == null)return;
            walkers.Add(walker);

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

        body.DesiredOffsetFromGround = AssembleData.OffsetFromGround + Vector3.Distance(body.transform.position, body.LowestPoint.position);

        List<MovementCoordinator.MovementGroup> movementGroups = new List<MovementCoordinator.MovementGroup>();
        movementGroups = CreateMovementGroups(walkers);

        GetComponentInChildren<MovementCoordinator>().movementGroups = movementGroups;

        if (supportManager.Supported)
        {
            Ai.NavAgent.baseOffset = 2;
        }
        else
        {
            Ai.NavAgent.baseOffset = 1;
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

    public List<MovementCoordinator.MovementGroup> CreateMovementGroups(List<BotComponent_Walker> walkers)
    {
        Dictionary<int, MovementCoordinator.MovementGroup> groupMap = new();

        foreach (var walker in walkers)
        {
            if (!groupMap.ContainsKey(walker.MovementGroup))
            {
                groupMap[walker.MovementGroup] = new MovementCoordinator.MovementGroup
                {
                    Group = walker.MovementGroup,
                    Walkers = new List<ProceduralWalker>()
                };
            }

            var proceduralWalker = walker.GetComponentInChildren<ProceduralWalker>();
            if(proceduralWalker != null)
                groupMap[walker.MovementGroup].Walkers.Add(proceduralWalker);

        }

        return groupMap.Values.OrderBy(g => g.Group).ToList();
    }
}
