using UnityEngine;
using System.Collections.Generic;

public class SupportManager : MonoBehaviour
{
    public List<BotComponent> SupportComponents;
    public bool Supported;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindAllSupportComponents();

        if(SupportComponents.Count == 1)
        {
            BotComponent_Walker walker = (SupportComponents[0] as BotComponent_Walker);
            walker.proceduralWalker.DefaultFootPlacementOffset = walker.transform.forward*.5f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FindAllSupportComponents()
    {
        SupportComponents.Clear();
        foreach (Transform Attachpoint in transform)
        {
            if (Attachpoint.gameObject.GetComponent<AttatchPoint>() == null) continue;

            if (Attachpoint.gameObject.GetComponent<AttatchPoint>().botComponent == null) continue;

            if (!Attachpoint.gameObject.GetComponent<AttatchPoint>().botComponent.ComponentDefaultData.ProvidesSupport) continue;

            SupportComponents.Add(Attachpoint.gameObject.GetComponent<AttatchPoint>().botComponent);
        }

        if(SupportComponents.Count > 0)
        {
            Supported = true;
        }
        else
        {
            Supported = false;
        }
    }

    public Vector3 GetAverageSupportPoint()
    {
        Vector3 Avg = Vector3.zero;

        foreach (var Comp in SupportComponents)
        {
            Avg += Comp.transform.localPosition;
        }

        return Avg/SupportComponents.Count;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position+GetAverageSupportPoint(), .1f);


        for (int i = 1; i < SupportComponents.Count; i++) 
        {
            Gizmos.DrawLine(SupportComponents[i - 1].transform.position, SupportComponents[i].transform.position);

        }
    }
}
