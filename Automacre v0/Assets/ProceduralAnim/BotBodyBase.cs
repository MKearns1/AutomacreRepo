using UnityEngine;
using System.Collections.Generic;

public class BotBodyBase : MonoBehaviour
{
    public float moveSpeed;

    Ray FloorRay;

    public LayerMask GroundLayer;
    Vector3 GroundHitLocation;
    public float DesiredOffsetFromGround = 1;
    public List<ProceduralWalker> ProceduralComponents;
    float heightVelocity = 0;
    public float DistanceFromGround;
    public Vector3 PredictedBodyPos;
    public float PredictionLookAheadTime = .5f;
    //public List<AttatchPoint> AttatchPoints = new List<AttatchPoint>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetAllProceduralComponents();
       // ProceduralComponents[Random.Range(0,ProceduralComponents.Count)].MovementAllowed = true;
        
    }

    // Update is called once per frame
    void Update()
    {

        /*        FloorRay = new Ray(transform.position, Vector3.down);
                Debug.DrawRay(FloorRay.origin, FloorRay.direction);
                RaycastHit hit;

                if (Physics.Raycast(FloorRay, out hit, 2, GroundLayer))
                {
                    GroundHitLocation = hit.point;
                }

                Vector3 DesiredPos = GroundHitLocation + Vector3.up * DesiredOffsetFromGround;*/


        Vector3 AvgHeight = Vector3.zero;
        float CompAvgHeight = 0;

        //if(ProceduralComponents.Count == 0) return;
        foreach (var comp in ProceduralComponents)
        {
            if(comp.moving)continue;
            float y = comp.EndPoint.position.y;
            CompAvgHeight += y;

        }

/*        foreach (var comp in ProceduralComponents)
        {
            if (comp == GetMostBehindComponent() && !comp.moving)
            {
                comp.MovementAllowed = true;
            }
            else { comp.MovementAllowed = false;} 
        }*/

            AvgHeight /= ProceduralComponents.Count;
        CompAvgHeight /= ProceduralComponents.Count;
        CompAvgHeight += DesiredOffsetFromGround;

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


        float smoothedHeight;
        smoothedHeight = transform.position.y;

        smoothedHeight = Mathf.SmoothDamp(smoothedHeight, CompAvgHeight, ref heightVelocity, .25f);

        //transform.position = new Vector3(transform.position.x, smoothedHeight, transform.position.z);

        PredictedBodyPos = transform.position + transform.parent.GetComponentInChildren<BotAI>().NavAgent.velocity * PredictionLookAheadTime;

        RaycastHit PredictedGroundHit;
        if (Physics.Raycast(GetComponentInParent<BotController>().Ai.transform.position, Vector3.down, out PredictedGroundHit, 50, LayerMask.GetMask("Ground")))
        {
            PredictedBodyPos.y = PredictedGroundHit.point.y + DesiredOffsetFromGround;
        }


        Vector3 BotPosition = GetComponentInParent<BotController>().Ai.transform.position;

        RaycastHit groundHit;
        if (Physics.Raycast(GetComponentInParent<BotController>().Ai.transform.position, Vector3.down, out groundHit, 5, LayerMask.GetMask("Ground")))
        {
            BotPosition.y = groundHit.point.y + DesiredOffsetFromGround;
            DistanceFromGround = groundHit.distance;
            // Debug.Log("KOKOKOKO");
        }


        Vector3 avgNormal = Vector3.zero;
        int normalCount = 0;

        foreach (var comp in ProceduralComponents)
        {
           // if (!comp.OnGround()) continue;

            avgNormal += comp.SurfaceNormal;
            normalCount++;
        }

        if (normalCount > 0)
            avgNormal.Normalize();
        else
            avgNormal = Vector3.up;

        Vector3 forward = Vector3.ProjectOnPlane(
    GetComponentInParent<BotController>().Ai.transform.forward,
    avgNormal
).normalized;

        Quaternion targetRotation =
            Quaternion.LookRotation(forward, avgNormal);

        //BotPosition.y = CompAvgHeight;
        transform.position = Vector3.Lerp(transform.position, BotPosition, Time.deltaTime * 5f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        //transform.rotation = targetRotation;
        //transform.rotation = GetComponentInParent<BotController>().Ai.NavAgent.transform.rotation;
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
       // ProceduralComponents[Random.Range(0, ProceduralComponents.Count)].MovementAllowed = true;

    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        //if (!ShowDebug) return;
        if (!this.enabled) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(PredictedBodyPos, Vector3.one * .6f);
    }
}
