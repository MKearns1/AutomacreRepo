using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class AttatchPoint : MonoBehaviour
{
    public Transform AttachedComponent;
    public BotComponent botComponent;
    public string Name;
    public List<ComponentType> UnacceptedTypes = new List<ComponentType>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   /* public RaycastHit FootPlacementPosition()
    {
        int layernum = LayerMask.GetMask("Ground");
        int layermask = 1 << layernum;  

        Ray floorray = new Ray(transform.position+transform.forward, Vector3.down);
        RaycastHit hit;

        bool h = Physics.Raycast(floorray, out hit, 100, layernum);
        if (h)
        {
            Debug.Log(hit.collider.name);
        }

        return hit;
    }*/

    public void AttachNewComponent(BotComponent NewComp, BotController BC = null)
    {
        AttachedComponent = NewComp.transform;
        botComponent = NewComp;
        NewComp.transform.SetParent(transform, true);
        NewComp.OnAttached();

        //NewComp.DesignInfo = NewComp.GetDesignInfo();

        switch (NewComp.ComponentDefaultData.Type)
        {

            case ComponentType.None:

                break;

            case ComponentType.Walker:

                break;

            case ComponentType.Grabber:

                break;

            case ComponentType.Wheel:

                break;

        }
    }
}
