using UnityEngine;

public class BotComponent_LimbType : BotComponent
{
    //public Transform Hand;
    public LimbCreator LimbCreator;
    public FABRIK fabrik;

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

    public override void Initialise(ComponentDesignInfo DesignInformation, BotController_Procedural BC)
    {
        LimbTypeDesignInfo LimbDesigninfo = DesignInformation as LimbTypeDesignInfo;

        body = BC.body;
        LimbCreator.NumberOfJoints = LimbDesigninfo.NumberofJoints;
        LimbCreator.JointSize = LimbDesigninfo.JointSize;
        LimbCreator.Length = LimbDesigninfo.LimbLength;
        LimbCreator.CreateJoints();
        LimbCreator.CreateJoints();
        //LimbCreator.PoleOffset = LimbDesigninfo.PoleOffset;
        LimbCreator.CreatePole(transform.TransformPoint(LimbDesigninfo.PoleOffset));Debug.Log(LimbDesigninfo.PoleOffset);
        //LimbCreator.Pole.position = LimbCreator.transform.position + LimbCreator.PoleOffset;
/*


        body = BC.body;
        //  GameObject Foot = Instantiate(WorkshopGeneral.instance.FootPrefab, transform.position + Vector3.down + transform.forward, Quaternion.identity);
        //GetComponentInChildren<ProceduralGrabber>().EndPoint = Hand.transform;
        //GetComponentInChildren<ProceduralGrabber>().BotBody = body;
        //GetComponentInChildren<FABRIK>().TargetTransform = Hand.transform;

        Debug.Log(WalkerConfiguration == null);
        GetComponentInChildren<LimbCreator>().NumberOfJoints = WalkerConfiguration.NumberofJoints;
      //  GetComponentInChildren<LimbCreator>().Length = WalkerConfiguration.LimbLength;
        //Hand.localScale = WalkerConfiguration.HandSize;
        GetComponentInChildren<LimbCreator>().JointSize = WalkerConfiguration.JointSize;

        GetComponentInChildren<LimbCreator>().CreateJoints();
        GetComponentInChildren<LimbCreator>().CreateJoints();

        //transform.GetComponentInChildren<ProceduralGrabber>().enabled = true;

        Debug.Log("Initialise Component : " + gameObject.name);
        //body.GetAllProceduralComponents();*/
    }


    public override void OnAttached()
    {
        LimbCreator = GetComponentInChildren<LimbCreator>();
        fabrik = GetComponentInChildren<FABRIK>();

        LimbCreator.CreatePole(transform.TransformPoint((ComponentDefaultData as LimbTypeDefinition).DefaultPoleOffset));

        /*Vector3 HandPos = GetComponentInChildren<ProceduralGrabber>().RestingPosition.position;

        GameObject newHand = Instantiate((ComponentDefaultData as GrabberDefinition).DefaultClawPrefab, HandPos, transform.rotation);
        Hand = newHand.transform;
        Transform botParent = WorkshopGeneral.GetTopParent(transform);
        if (botParent != transform)
        {
            Hand.SetParent(botParent, true);
        }

        transform.GetComponentInChildren<FABRIK>().TargetTransform = newHand.transform;
        transform.GetComponentInChildren<ProceduralGrabber>().enabled = false;
        transform.GetComponentInChildren<LimbCreator>().CreateJoints();
        transform.GetComponentInChildren<LimbCreator>().CreateJoints();*/

    }

    public override ComponentDesignInfo GetDesignInfo()
    {
        LimbTypeDesignInfo info = new LimbTypeDesignInfo();

        info.NumberofJoints = GetComponentInChildren<LimbCreator>().NumberOfJoints;
        info.LimbLength = GetComponentInChildren<LimbCreator>().Length;
        info.JointSize = GetComponentInChildren<LimbCreator>().JointSize;
        info.PoleOffset = GetComponentInChildren<LimbCreator>().PoleOffset;

        return info;
    }

    public override void AssignVariablesStartup()
    {
        base.AssignVariablesStartup();
        LimbCreator = GetComponentInChildren<LimbCreator>();
        fabrik = GetComponentInChildren<FABRIK>();
    }
}
public class LimbTypeDesignInfo : ComponentDesignInfo
{
    public int NumberofJoints;
    public float LimbLength;
    public float JointSize;
    public Vector3 PoleOffset;

    public LimbTypeDesignInfo()
    {

    }
}