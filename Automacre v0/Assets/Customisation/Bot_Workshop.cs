using System.Collections.Generic;
using UnityEngine;

public class Bot_Workshop : MonoBehaviour
{
    //public Dictionary<string, AttatchPoint> AttatchPoints = new Dictionary<string, AttatchPoint>();
    public BotRuntimeData DesignData = new BotRuntimeData();
    public string BodyType;
   // public Transform LowestPoint  { get {return transform.Find("BotBody"). } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //AttatchPoints.Clear();
        GetAllAttachPoints();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        //Physics.Raycast(LowestPoint.transform.position, Vector3.down, out hit, 100, LayerMask.GetMask("Ground"));
        Physics.Raycast(transform.Find("BotBody").GetComponent<WorkshopBot_Body>().LowestPoint.transform.position, Vector3.down, out hit, 100, LayerMask.GetMask("Ground"));
        float Offset = Vector3.Distance(hit.point, transform.Find("BotBody").GetComponent<WorkshopBot_Body>().LowestPoint.transform.position);

        DesignData.OffsetFromGround = Offset;
        //Debug.Log(Offset);
    }

    [ContextMenu("GetAllAttachPoints")]
    public void GetAllAttachPoints()
    {
        DesignData.AttachPoints.Clear();

        foreach (Transform child in transform.Find("BotBody").transform)
        {
            if (!child.gameObject.activeInHierarchy) continue;

            if (child.GetComponent<AttatchPoint>() == null) continue;

            AttatchPoint childAP = child.GetComponent<AttatchPoint>();

            DesignData.AttachPoints.Add(childAP.Name, childAP);
        }
        Debug.Log(gameObject.name + " Attached " + DesignData.AttachPoints.Count);
    }

    [ContextMenu("DestroyComponents")]
    public void DestroyAllComponents()
    {
        DesignData.AttachPoints.Clear();

        foreach (Transform child in transform.Find("BotBody").transform)
        {
            if (!child.gameObject.activeInHierarchy) continue;

            if (child.GetComponent<AttatchPoint>() == null) continue;

            AttatchPoint childAP = child.GetComponent<AttatchPoint>();

            if (childAP.botComponent == null) continue;
            childAP.botComponent.RemoveFromBot();
        }
    }
}
