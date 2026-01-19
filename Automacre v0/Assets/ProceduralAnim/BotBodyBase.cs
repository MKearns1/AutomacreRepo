using UnityEngine;
using System.Collections.Generic;

public class BotBodyBase : MonoBehaviour
{
    public float moveSpeed;

    Ray FloorRay;

    public LayerMask GroundLayer;
    Vector3 GroundHitLocation;
    public float DesiredHeight = 1;
    public List<ProceduralWalker> ProceduralComponents;
    float heightVelocity = 0;
    //public List<AttatchPoint> AttatchPoints = new List<AttatchPoint>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GetAllProceduralComponents();
       // ProceduralComponents[Random.Range(0,ProceduralComponents.Count)].MovementAllowed = true;
        
    }

    // Update is called once per frame
    void Update()
    {


        FloorRay = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(FloorRay.origin, FloorRay.direction);
        RaycastHit hit;

        if (Physics.Raycast(FloorRay, out hit, 2, GroundLayer))
        {
            GroundHitLocation = hit.point;
        }

        Vector3 DesiredPos = GroundHitLocation + Vector3.up * DesiredHeight;

        //transform.position = Vector3.Lerp(transform.position, DesiredPos, Time.deltaTime * 5);

        Vector3 AvgHeight = Vector3.zero;
        float CompAvgHeight = 0;
        bool stepping = false;

        foreach (var comp in ProceduralComponents)
        {
            float y = comp.EndPoint.position.y;
            CompAvgHeight += y;
            if (comp.moving)
            {
                stepping = true;

                /*foreach (var comp2 in ProceduralComponents)
                {
                    if(comp2 != comp)
                    {
                        comp.MovementAllowed = false;
                    }
                    else { comp.MovementAllowed = true; }
                }*/
                
            }

        }

        foreach (var comp in ProceduralComponents)
        {
            if (comp == GetMostBehindComponent() && !comp.moving)
            {
                comp.MovementAllowed = true;
            }
            else { comp.MovementAllowed = false;} 
        }

            AvgHeight /= ProceduralComponents.Count;
        CompAvgHeight /= ProceduralComponents.Count;
        CompAvgHeight += DesiredHeight;

/*
        Vector3 avgNormal = Vector3.zero;
        Vector3 center = Vector3.zero;

        foreach (var comp in ProceduralComponents)
        {
            center += comp.EndPoint.position;
        }
        center /= ProceduralComponents.Count;

        for (int i = 0; i < ProceduralComponents.Count; i++)
        {
            Vector3 a = ProceduralComponents[i].EndPoint.position - center;
            Vector3 b = ProceduralComponents[(i + 1) % ProceduralComponents.Count].EndPoint.position - center;

            avgNormal += Vector3.Cross(a, b);
        }

        avgNormal.Normalize();

        Vector3 desiredUp = Vector3.Slerp(
    transform.up,
    avgNormal,
    0.02f   // tilt strength
);
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, desiredUp).normalized;

        Quaternion desiredRotation = Quaternion.LookRotation(forward, desiredUp);
        transform.rotation = Quaternion.Slerp(
    transform.rotation,
    desiredRotation,
    Time.deltaTime * 5f
);
*/

        /*        //float maxAllowedHeight = Mathf.Infinity;

                //foreach (var comp in ProceduralComponents)
                //{
                //    maxAllowedHeight = Mathf.Min(
                //        maxAllowedHeight,
                //        comp.Settings.MaxReachLength
                //    );
                //}

                //float clampedHeight =
                //    Mathf.Min(DesiredHeight, maxAllowedHeight * 0.9f);

                //CompAvgHeight += clampedHeight;*/


        Ray FaceRay = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(FaceRay, out hit, 3, GroundLayer))
        {
            //transform.Rotate(Vector3.up*Time.deltaTime * 4000);
            if(Vector3.Angle(hit.normal, Vector3.up) > 80)
            transform.rotation = Quaternion.LookRotation(Vector3.Reflect(transform.forward, hit.normal), Vector3.up);
        }
        Debug.DrawRay(FaceRay.origin, FaceRay.direction);

        // if(!stepping)

       // transform.position = transform.position + transform.forward * Time.deltaTime * moveSpeed;

        float smoothedHeight;
        smoothedHeight = transform.position.y;

        smoothedHeight = Mathf.SmoothDamp(smoothedHeight, CompAvgHeight, ref heightVelocity, .25f);

        transform.position = new Vector3(transform.position.x, smoothedHeight, transform.position.z);
    }




    public ProceduralWalker GetMostBehindComponent()
    {
        ProceduralWalker furthestComponent = null;
        float furthestdist = -Mathf.Infinity;

        foreach (var comp in ProceduralComponents)
        {
            float dist = Vector3.Distance(comp.EndPoint.position, comp.NextStepPos);

            if(dist > furthestdist)
            {
                furthestdist = dist;
                furthestComponent = comp;
            }

        }

        return furthestComponent;
    }

    public void GetAllProceduralComponents()
    {
        ProceduralComponents.Clear();
        foreach (Transform Attachpoint in transform)
        {
            if(Attachpoint.gameObject.GetComponent<AttatchPoint>() == null)continue;

            if(Attachpoint.gameObject.GetComponent<AttatchPoint>().botComponent == null) continue;

            if(Attachpoint.gameObject.GetComponent<AttatchPoint>().botComponent.GetComponentInChildren<ProceduralWalker>(false) == null)continue;

            ProceduralComponents.Add(Attachpoint.gameObject.GetComponent<AttatchPoint>().botComponent.GetComponentInChildren<ProceduralWalker>(false));
        }
        ProceduralComponents[Random.Range(0, ProceduralComponents.Count)].MovementAllowed = true;

    }
}
