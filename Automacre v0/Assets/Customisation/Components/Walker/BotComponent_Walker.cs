using System;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class BotComponent_Walker : BotComponent_LimbType
{
    public Transform Foot;
    public ProceduralWalker proceduralWalker;
    public int MovementGroup;

    public override void Awake()
    {
        base.Awake();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = FootPlacementPosition().point;
    }

    public override void Initialise(ComponentDesignInfo DesignInformation, BotController_Procedural BC)
    {
        base.Initialise(DesignInformation, BC);
        WalkerDesignInfo WalkerInfo = DesignInformation as WalkerDesignInfo;
        DesignInfo = WalkerInfo;
        proceduralWalker.EndPoint = Foot.transform;
        fabrik.TargetTransform = Foot.transform;
        Foot.localScale = WalkerInfo.FootSize;
        proceduralWalker.enabled = true;
        proceduralWalker.BotBody = body;
        proceduralWalker.DefaultFootPlacementOffset = WalkerInfo.FootOffset;
        Foot.transform.position = FootPlacementPosition().point;
        MovementGroup = WalkerInfo.MovementGroup;
        //GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = Foot.transform.position;
        //body.GetAllProceduralComponents();


        /*        WalkerDesignInfo WalkerConfiguration = DesignInformation as WalkerDesignInfo;

                body = BC.body;
              //  GameObject Foot = Instantiate(WorkshopGeneral.instance.FootPrefab, transform.position + Vector3.down + transform.forward, Quaternion.identity);
                GetComponentInChildren<ProceduralWalker>().EndPoint = Foot.transform;
                GetComponentInChildren<ProceduralWalker>().BotBody = body;
                GetComponentInChildren<FABRIK>().TargetTransform = Foot.transform;

                Debug.Log(WalkerConfiguration == null);
                GetComponentInChildren<LimbCreator>().NumberOfJoints = WalkerConfiguration.NumberofJoints;
                GetComponentInChildren<LimbCreator>().Length = WalkerConfiguration.LimbLength;
                Foot.localScale = WalkerConfiguration.FootSize;
                GetComponentInChildren<LimbCreator>().JointSize = WalkerConfiguration.JointSize;

                GetComponentInChildren<LimbCreator>().CreateJoints();
                GetComponentInChildren<LimbCreator>().CreateJoints();

                transform.GetComponentInChildren<ProceduralWalker>().enabled = true;

                Debug.Log("Initialise Component : " + gameObject.name);
                body.GetAllProceduralComponents();*/
    }

    public override void OnAttached()
    {
        proceduralWalker = GetComponentInChildren<ProceduralWalker>();
        LimbCreator = GetComponentInChildren<LimbCreator>();
        fabrik = GetComponentInChildren<FABRIK>();
        proceduralWalker.enabled = false;
        base.OnAttached();

        Vector3 footpos = FootPlacementPosition().point;
        GameObject newFoot = Instantiate((ComponentDefaultData as WalkerDefinition).DefaultFootPrefab, footpos, transform.rotation);

        AreaConstraint c = newFoot.transform.AddComponent<AreaConstraint>();
        //c.Origin = transform;
        //c.MaxDistance = LimbCreator.Length;
        newFoot.transform.GetComponent<Transformable>().Constraint = c;

        Debug.LogWarning((ComponentDefaultData as WalkerDefinition).DefaultFootPrefab == null);

        Foot = newFoot.transform;
        Foot.transform.rotation = Quaternion.FromToRotation(Foot.transform.up, FootPlacementPosition().normal) * Foot.transform.rotation;         ;
        Transform botParent = WorkshopGeneral.GetTopParent(transform);
        if (botParent != transform)
        {
            Foot.SetParent(botParent,true);
        }

        LimbCreator.Length = Mathf.Clamp(Mathf.Round(Vector3.Distance(transform.position, footpos) * 1.5f),1,8);
        
        fabrik.TargetTransform = newFoot.transform.Find("Joint");
        proceduralWalker.enabled = false;
        LimbCreator.CreateJoints();
        LimbCreator.CreateJoints();
        DesignInfo = GetDesignInfo();

        Debug.LogWarning("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
    }

    public RaycastHit FootPlacementPosition()
    {
        int layernum = LayerMask.GetMask("Ground");
        int layermask = 1 << layernum;

        Vector3 RaystartPos = transform.TransformPoint((ComponentDefaultData as WalkerDefinition).DefaultFootOffset);
        Debug.LogWarning("ray " + RaystartPos);

        if (DesignInfo is WalkerDesignInfo info)
        {
            Transform pp;
            if (body != null)
            {
                pp = body.transform;

            }
            else
            {
                pp = transform.parent.parent.transform;
                
            }
                Debug.LogWarning("POOOOOOOOOOOOOOOOOP33333333");
            //RaystartPos = transform.parent.parent.transform.TransformPoint(info.FootOffset);
            RaystartPos = pp.TransformPoint(info.FootOffset);
            Debug.LogWarning("rayinfo " + RaystartPos);
        }

        Ray floorray = new Ray(RaystartPos, Vector3.down);
        //floorray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        bool h = Physics.Raycast(floorray, out hit, 100, layernum);
        if (h)
        {
            Debug.Log(hit.collider.name);
        }
        //GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = hit.point;
        return hit;
    }


    public override ComponentDesignInfo GetDesignInfo()
    {
        WalkerDesignInfo info = new WalkerDesignInfo();

        info.NumberofJoints = GetComponentInChildren<LimbCreator>().NumberOfJoints;
        info.LimbLength = GetComponentInChildren<LimbCreator>().Length;
        info.FootSize = Foot.localScale;
        info.JointSize = GetComponentInChildren<LimbCreator>().JointSize;
        info.PoleOffset = GetComponentInChildren<LimbCreator>().PoleOffset;

        // Gets Offset from base
        info.FootOffset = transform.parent.parent.transform.InverseTransformPoint(Foot.position); info.FootOffset.y = 0;
        info.MovementGroup = MovementGroup;
        return info;


        info.NumberofJoints = LimbCreator.NumberOfJoints;
        info.LimbLength = LimbCreator.Length;
        info.FootSize = Foot.localScale;
        info.JointSize = LimbCreator.JointSize;

        return info;
    }

    public override void OnSelected()
    {
        //GameObject.Instantiate()

        GetComponent<SelectionHighlight>()?.SetHighlight(true);
    }

    public override Vector3 GetSelectedArrowPos()
    {
        return Vector3.Lerp(transform.position,Foot.position,0.5f) + Vector3.up;
    }

    public override void RemoveFromBot()
    {
        Destroy(Foot.gameObject);
        Destroy(gameObject);
    }
    public override void AssignVariablesStartup()
    {
        base.AssignVariablesStartup();
        proceduralWalker = GetComponentInChildren<ProceduralWalker>();
    }

    public override void Highlight()
    {

    }

}
[Serializable]

public class WalkerDesignInfo : LimbTypeDesignInfo
{
    public Vector3 FootSize;
    public Vector3 FootOffset;
    public int MovementGroup;

    public WalkerDesignInfo()
    {

    }
}
