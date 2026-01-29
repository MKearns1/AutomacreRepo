using System;
using UnityEngine;
using UnityEngine.XR;

public class BotComponent_Basket : BotComponent
{
    public Transform Basket;
    public Transform PlacePoint;
    //public ProceduralWalker proceduralWalker;

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

    }

    public override void OnAttached()
    {
        Basket = transform.Find("Basket");

        if (GetComponentInParent<BotBodyBase>() != null)
        {
            Basket.rotation = GetComponentInParent<BotBodyBase>().transform.rotation;
        }
        if (GetComponentInParent<Bot_Workshop>() != null)
        {
            Basket.rotation = GetComponentInParent<Bot_Workshop>().transform.rotation;
        }

        /* proceduralWalker = GetComponentInChildren<ProceduralWalker>();
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
         DesignInfo = GetDesignInfo();*/

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
        BasketDesignInfo info = new BasketDesignInfo();

        return info;
    }

    public override void OnSelected()
    {
        //GameObject.Instantiate()
    }

    public override Vector3 GetSelectedArrowPos()
    {
        return transform.position;
       // return Vector3.Lerp(transform.position,Foot.position,0.5f) + Vector3.up;
    }

    public override void RemoveFromBot()
    {
        Destroy(gameObject);
    }
    public override void AssignVariablesStartup()
    {
        base.AssignVariablesStartup();
    }
}
[Serializable]

public class BasketDesignInfo : ComponentDesignInfo
{
    public Vector3 FootSize;

    public BasketDesignInfo()
    {

    }
}
