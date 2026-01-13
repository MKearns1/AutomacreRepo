using System.Collections.Generic;
using UnityEngine;

public class Bot_Workshop : MonoBehaviour
{
    //public Dictionary<string, AttatchPoint> AttatchPoints = new Dictionary<string, AttatchPoint>();
    public BotRuntimeData DesignData = new BotRuntimeData();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //AttatchPoints.Clear();
        DesignData.AttachPoints.Clear();

        foreach (Transform child in transform.Find("BotBase").transform)
        {
            if (!child.gameObject.activeInHierarchy) continue;

            if (child.GetComponent<AttatchPoint>() == null) continue;

            AttatchPoint childAP = child.GetComponent<AttatchPoint>();

            DesignData.AttachPoints.Add(childAP.Name, childAP);
        }
        Debug.Log(gameObject.name + " Attached " + DesignData.AttachPoints.Count);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
