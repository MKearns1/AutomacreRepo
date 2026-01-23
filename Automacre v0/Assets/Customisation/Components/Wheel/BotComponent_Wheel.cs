using System;
using UnityEngine;

public class BotComponent_Wheel : BotComponent
{
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

    }

    public override void OnAttached()
    {
        //Debug.LogWarning((ComponentDefaultData as WalkerDefinition).DefaultFootPrefab == null);

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
