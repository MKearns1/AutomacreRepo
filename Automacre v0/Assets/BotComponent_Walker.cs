using System;
using UnityEngine;

public class BotComponent_Walker : BotComponent
{
    public Transform Foot;

    public override void Awake()
    {
        //base.Awake();
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
        WalkerDesignInfo WalkerConfiguration = DesignInformation as WalkerDesignInfo;

        body = BC.body;
      //  GameObject Foot = Instantiate(WorkshopGeneral.instance.FootPrefab, transform.position + Vector3.down + transform.forward, Quaternion.identity);
        GetComponentInChildren<ProceduralWalker>().EndPoint = Foot.transform;
        GetComponentInChildren<ProceduralWalker>().BotBody = body;
        GetComponentInChildren<FABRIK>().TargetTransform = Foot.transform;

        Debug.Log(WalkerConfiguration == null);
        GetComponentInChildren<LimbCreator>().NumberOfJoints = WalkerConfiguration.NumberofJoints;
        GetComponentInChildren<LimbCreator>().Length = WalkerConfiguration.LimbLength;
        Foot.localScale = WalkerConfiguration.FootSize;
        GetComponentInChildren<LimbCreator>().CreateJoints();
        GetComponentInChildren<LimbCreator>().CreateJoints();

        transform.GetComponentInChildren<ProceduralWalker>().enabled = true;

        Debug.Log("Initialise Component : " + gameObject.name);
        body.GetAllProceduralComponents();
    }

    public override void OnAttached()
    {
        Vector3 footpos = FootPlacementPosition().point;

        GameObject newFoot = Instantiate((ComponentDefaultData as WalkerDefinition).DefaultFootPrefab, footpos, transform.rotation);
        Foot = newFoot.transform;
        Transform botParent = WorkshopGeneral.GetTopParent(transform);
        if (botParent != transform)
        {
            Foot.SetParent(botParent,true);
        }

        transform.GetComponentInChildren<FABRIK>().TargetTransform = newFoot.transform;
        transform.GetComponentInChildren<ProceduralWalker>().enabled = false;
        transform.GetComponentInChildren<LimbCreator>().CreateJoints();
        transform.GetComponentInChildren<LimbCreator>().CreateJoints();

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
}
[Serializable]

public class WalkerDesignInfo : ComponentDesignInfo
{
    public int NumberofJoints;
    public float LimbLength;
    public Vector3 FootSize;
    public float JointSize;

    public WalkerDesignInfo()
    {

    }
}
