using UnityEngine;

public class BotComponent_Grabber : BotComponent
{
    public Transform Hand;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

/*    public override void Initialise(ComponentDesignInfo DesignInformation, BotController BC)
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
        GetComponentInChildren<LimbCreator>().JointSize = WalkerConfiguration.JointSize;

        GetComponentInChildren<LimbCreator>().CreateJoints();
        GetComponentInChildren<LimbCreator>().CreateJoints();

        transform.GetComponentInChildren<ProceduralWalker>().enabled = true;

        Debug.Log("Initialise Component : " + gameObject.name);
        body.GetAllProceduralComponents();
    }*/


    public override void OnAttached()
    {
        Vector3 HandPos = GetComponentInChildren<ProceduralGrabber>().RestingPosition.position;

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
        transform.GetComponentInChildren<LimbCreator>().CreateJoints();

    }
}
