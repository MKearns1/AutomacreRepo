using System;
using UnityEngine;

public class BotComponent_Wheel : BotComponent
{
    public Transform Wheel;

    public override void AssignVariablesStartup()
    {
        base.AssignVariablesStartup();
    }

    public override void Awake()
    {
        base.Awake();
    }

    public override ComponentDesignInfo GetDesignInfo()
    {
        WheelDesignInfo info = new WheelDesignInfo();

        info.WheelSize = Vector3.one;
        return info;
    }

    public override Vector3 GetSelectedArrowPos()
    {
        return base.GetSelectedArrowPos();
    }

    public override void Initialise(ComponentDesignInfo DesignInformation, BotController BC)
    {
        //base.Initialise(DesignInformation, BC);

        GetComponentInChildren<ProceduralWheel>().enabled = true;
        GetComponentInChildren<ProceduralWheel>().BotBody = body;
        GetComponentInChildren<ProceduralWheel>().WheelPart = Wheel;
        GetComponentInChildren<FABRIK>().TargetTransform = Wheel.GetChild(0).transform;

    }

    public override void OnAttached()
    {
        //Debug.LogWarning((ComponentDefaultData as WalkerDefinition).DefaultFootPrefab == null);

        Vector3 wheelPos = WheelPlacementPos().point;
        GameObject newWheel = Instantiate((ComponentDefaultData as WheelDefinition).DefaultWheelPrefab, wheelPos, transform.rotation);

        Wheel = newWheel.transform;
        Transform botParent = WorkshopGeneral.GetTopParent(transform);
        if (botParent != transform)
        {
            Wheel.SetParent(botParent, true);
        }

        GetComponentInChildren<FABRIK>().TargetTransform = newWheel.transform.GetChild(0).transform;
        GetComponentInChildren<LimbCreator>().CreateJoints();
        GetComponentInChildren<LimbCreator>().CreateJoints();
        GetComponentInChildren<ProceduralWheel>().enabled = false;
        DesignInfo = GetDesignInfo();

        Debug.LogWarning("Wheel attached");
    }

    public override void OnSelected()
    {
        base.OnSelected();
    }

    public override void RemoveFromBot()
    {
        Destroy(this.gameObject);
    }

    public RaycastHit WheelPlacementPos()
    {
        int layernum = LayerMask.GetMask("Ground");
        int layermask = 1 << layernum;

        Ray floorray = new Ray(transform.position + transform.forward*.2f, Vector3.down);
        RaycastHit hit;

        bool h = Physics.Raycast(floorray, out hit, 100, layernum);
        if (h)
        {
            Debug.Log(hit.collider.name);
        }

        return hit;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GetComponentInParent<BotBodyBase>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
[Serializable]
public class WheelDesignInfo : ComponentDesignInfo
{
    public Vector3 WheelSize;

    public WheelDesignInfo()
    {

    }
}
