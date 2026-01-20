using System;
using UnityEngine;
using UnityEngine.XR;

public class BotComponent_Walker : BotComponent_LimbType
{
    public Transform Foot;
    public ProceduralWalker proceduralWalker;

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

    }

    public override void Initialise(ComponentDesignInfo DesignInformation, BotController BC)
    {
        base.Initialise(DesignInformation, BC);
        WalkerDesignInfo GrabberInfo = DesignInformation as WalkerDesignInfo;

        proceduralWalker.EndPoint = Foot.transform;
        fabrik.TargetTransform = Foot.transform;
        Foot.localScale = GrabberInfo.FootSize;
        proceduralWalker.enabled = true;
        proceduralWalker.BotBody = body;
        body.GetAllProceduralComponents();


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
        Debug.LogWarning((ComponentDefaultData as WalkerDefinition).DefaultFootPrefab == null);

        Foot = newFoot.transform;
        Transform botParent = WorkshopGeneral.GetTopParent(transform);
        if (botParent != transform)
        {
            Foot.SetParent(botParent,true);
        }

        fabrik.TargetTransform = newFoot.transform;
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

        Ray floorray = new Ray(transform.position + transform.forward, Vector3.down);
        RaycastHit hit;

        bool h = Physics.Raycast(floorray, out hit, 100, layernum);
        if (h)
        {
            Debug.Log(hit.collider.name);
        }

        return hit;
    }


    public override ComponentDesignInfo GetDesignInfo()
    {
        WalkerDesignInfo info = new WalkerDesignInfo();

        info.NumberofJoints = GetComponentInChildren<LimbCreator>().NumberOfJoints;
        info.LimbLength = GetComponentInChildren<LimbCreator>().Length;
        info.FootSize = Foot.localScale;
        info.JointSize = GetComponentInChildren<LimbCreator>().JointSize;
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
}
[Serializable]

public class WalkerDesignInfo : LimbTypeDesignInfo
{
    public Vector3 FootSize;

    public WalkerDesignInfo()
    {

    }
}
