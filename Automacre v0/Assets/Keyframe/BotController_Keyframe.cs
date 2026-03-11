using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BotController_Keyframe : BotController
{
    //public BotBodyBase body;
    public bool Prebuilt;
    Dictionary<string, AttatchPoint> AttachmentPoints = new Dictionary<string, AttatchPoint>();
    public SupportManager supportManager;

    Transform Body;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(AttachmentPoints.Count == 0) InitialiseAttachPoints();
        Body = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = Ai.transform.forward;

        Vector3 desiredDir =
            Ai.NavAgent.desiredVelocity.normalized;

        float turnAmount =
            Vector3.SignedAngle(
                forward,
                desiredDir,
                Vector3.up
            );


        Body.transform.position = Ai.transform.position;
        Body.transform.rotation = Ai.transform.rotation;

        Body.GetChild(0).GetComponent<Animator>().SetFloat("Velocity", Ai.NavAgent.velocity.magnitude);
        Body.GetChild(0).GetComponent<Animator>().SetFloat("Angle", turnAmount);


        if (Ai.NavAgent.velocity.magnitude > 0)
        {
            Body.GetChild(0).GetComponent<Animator>().speed = Ai.NavAgent.velocity.magnitude/4;
        }
        else
        {
            Body.GetChild(0).GetComponent<Animator>().speed = 1;
        }

       


    }

    public void AssembleBot(BotRuntimeData AssembleData)
    {
        InitialiseAttachPoints();

        Dictionary<string, AttatchPoint> Design_AttachPoints = AssembleData.AttachPoints;

        Debug.Log(AttachmentPoints.Count);

        foreach (var key in Design_AttachPoints.Keys)
        {
            if (!AttachmentPoints.ContainsKey(key)) continue;

            Debug.Log(key);

            if(Design_AttachPoints[key].botComponent == null) continue;

            GameObject NewComponent = Instantiate(Design_AttachPoints[key].botComponent.ComponentDefaultData.DefaultPrefab, AttachmentPoints[key].transform);

            BotComponent NewCompScript = NewComponent.GetComponent<BotComponent>();

            AttachmentPoints[key].AttachNewComponent(NewCompScript);
            //NewCompScript.Initialise(Design_AttachPoints[key].botComponent.GetDesignInfo(), this);

        }

        if (supportManager.Supported)
        {
            Ai.NavAgent.baseOffset = 2;
        }
        else
        {
            Ai.NavAgent.baseOffset = 1;
        }
    }

    public void InitialiseAttachPoints()
    {
        AttachmentPoints.Clear();

        foreach (Transform child in transform.Find("Base").transform)
        {
            if (!child.gameObject.activeInHierarchy) continue;

            if (child.GetComponent<AttatchPoint>() == null) continue;

            AttatchPoint childAP = child.GetComponent<AttatchPoint>();

            AttachmentPoints.Add(childAP.Name, childAP);
        }
        Debug.Log(gameObject.name + " Attached " + AttachmentPoints.Count);
    }
}
