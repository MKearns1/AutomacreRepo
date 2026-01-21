using UnityEngine;

public class BotComponent_Grabber : BotComponent_LimbType
{
    public Transform Hand;
    public ProceduralGrabber proceduralGrabber;

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
        GrabberDesignInfo GrabberInfo = DesignInformation as GrabberDesignInfo;

        proceduralGrabber.EndPoint = Hand.transform;
        fabrik.TargetTransform = Hand.transform;
        Hand.localScale = GrabberInfo.HandSize;
        proceduralGrabber.enabled = true;


/*        GrabberDesignInfo WalkerConfiguration = DesignInformation as GrabberDesignInfo;

        body = BC.body;
        proceduralGrabber.EndPoint = Hand.transform;
        //GetComponentInChildren<ProceduralGrabber>().BotBody = body;
        GetComponentInChildren<FABRIK>().TargetTransform = Hand.transform;

        Debug.Log(WalkerConfiguration == null);
        GetComponentInChildren<LimbCreator>().NumberOfJoints = WalkerConfiguration.NumberofJoints;
      //  GetComponentInChildren<LimbCreator>().Length = WalkerConfiguration.LimbLength;
        Hand.localScale = WalkerConfiguration.HandSize;
        GetComponentInChildren<LimbCreator>().JointSize = WalkerConfiguration.JointSize;

        GetComponentInChildren<LimbCreator>().CreateJoints();
        GetComponentInChildren<LimbCreator>().CreateJoints();

        transform.GetComponentInChildren<ProceduralGrabber>().enabled = true;

        Debug.Log("Initialise Component : " + gameObject.name);
        //body.GetAllProceduralComponents();*/
    }

    public override void OnAttached()
    {
        base.OnAttached();
        proceduralGrabber = GetComponentInChildren<ProceduralGrabber>();

        Vector3 HandPos = proceduralGrabber.RestingPosition.position;

        GameObject newHand = Instantiate((ComponentDefaultData as GrabberDefinition).DefaultClawPrefab, HandPos, transform.rotation);
        Hand = newHand.transform;
        Transform botParent = WorkshopGeneral.GetTopParent(transform);
        if (botParent != transform)
        {
            Hand.SetParent(botParent, true);
        }

        fabrik.TargetTransform = newHand.transform;
        proceduralGrabber.enabled = false;
        LimbCreator.CreateJoints();
        LimbCreator.CreateJoints();
        DesignInfo = GetDesignInfo();
    }

    public override ComponentDesignInfo GetDesignInfo()
    {
        GrabberDesignInfo info = new GrabberDesignInfo();

        info.NumberofJoints = GetComponentInChildren<LimbCreator>().NumberOfJoints;
        info.LimbLength = GetComponentInChildren<LimbCreator>().Length;
        info.HandSize = Hand.localScale;
        info.JointSize = GetComponentInChildren<LimbCreator>().JointSize;

        return info;
    }

    public override Vector3 GetSelectedArrowPos()
    {
        return Vector3.Lerp(transform.position, Hand.position, 0.5f) + Vector3.up;
    }
    public override void RemoveFromBot()
    {
        Destroy(Hand.gameObject);
        Destroy(gameObject);
    }

    public override void AssignVariablesStartup()
    {
        base.AssignVariablesStartup();
        proceduralGrabber = GetComponentInChildren<ProceduralGrabber>();
    }
}
public class GrabberDesignInfo : LimbTypeDesignInfo
{
    public Vector3 HandSize;

    public GrabberDesignInfo()
    {

    }
}