using System;
using UnityEngine;

public class ProceduralWalker : MonoBehaviour
{
   // [Serializable]
    public ProceduralSettings Settings;
    public BotBodyBase BotBody;

    Transform StartPoint;
    public Transform EndPoint;
    Vector3 GroundHitLocation;
    public Vector3 NextStepPos;
    Vector3 NextStepPos2;

    Ray FloorRay;

    public LayerMask GroundLayer;

    public bool moving;
    public bool MovementAllowed;
    float moveProgress;
    float StepSpeed;
    public float StepSpeedMultiplier = 2;
    Vector3 PrevPos;
    Vector3 MoveToPos;
    Vector3 DefaultFootPlacementOffset;
    public bool ShowDebug;
    public AnimationCurve MovementCurve;
    Vector3 SurfaceNormal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       //StartPoint = transform.Find("GameObject").transform.Find("ConnectionPoint").transform;
       //StartPoint = transform.Find("ConnectionPoint").transform;
       StartPoint = transform;
       //EndPoint = transform.Find("GameObject").transform.Find("EndPoint").transform;
       //EndPoint = GameObject.Find("EndPoint").transform ;
      // DefaultFootPlacementOffset = EndPoint.position - StartPoint.position;
       DefaultFootPlacementOffset = BotBody.transform.InverseTransformPoint( EndPoint.position);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 DefaultFootPos = BotBody.transform.TransformPoint(DefaultFootPlacementOffset);// StartPoint.position + DefaultFootPlacementOffset;
        Vector3 DefaultFootPosDirection = DefaultFootPos - StartPoint.position;

       // FloorRay = new Ray(StartPoint.position, Vector3.down);
        FloorRay = new Ray(StartPoint.position, DefaultFootPosDirection);
        Debug.DrawRay(FloorRay.origin, FloorRay.direction);
        RaycastHit hit;

        if(Physics.Raycast(FloorRay, out hit, DefaultFootPosDirection.magnitude, GroundLayer))
        {
            GroundHitLocation = hit.point;
            SurfaceNormal = hit.normal;
        }
        else
        {
            FloorRay = new Ray(DefaultFootPos, Vector3.down);
            Debug.DrawRay(FloorRay.origin, FloorRay.direction);
            if (Physics.Raycast(FloorRay, out hit, 1000, GroundLayer))
            {
                GroundHitLocation = hit.point;
                SurfaceNormal = hit.normal;
            }
        }

            NextStepPos = GroundHitLocation;
        NextStepPos2 = NextStepPos + BotBody.transform.forward * Settings.MaxStrideLength * 1.1f;
        RaycastHit nextstepHit;

        Ray NextStepRay = new Ray(StartPoint.position, NextStepPos2 - StartPoint.position);

        if (Physics.Raycast(NextStepRay, out nextstepHit, 2, GroundLayer))
        {
            NextStepPos2 = nextstepHit.point;
        }

        StepSpeed = BotBody.moveSpeed * StepSpeedMultiplier;

        Vector3 a = EndPoint.position;
        Vector3 b = NextStepPos2;

        a.y = 0;
        b.y = 0;

        if (!moving && MovementAllowed && Vector3.Distance(a, b) > Settings.MaxStrideLength)
        {
            moving = true;
           // EndPoint.transform.position = NextStepPos;
           //NextStepPos2 = NextStepPos + BotBody.transform.forward * BotBody.moveSpeed / 1;
           PrevPos = EndPoint.position;
            MoveToPos = NextStepPos2;
        }

        if (moving)
        {
            MoveTransition(PrevPos, MoveToPos, moveProgress);
           // Debug.LogWarning("FR" + gameObject.name);

            float Dist = Vector3.Distance(EndPoint.position, MoveToPos);
            if (Dist < .1f)
            {
               // Debug.LogWarning("POOOOOOOOOOOOOOOOOOOP" + gameObject.name);
                EndPoint.position = MoveToPos;
                moveProgress = 0;
                MoveFinished();
            }
        }

    }

    public void MoveTransition(Vector3 startPos, Vector3 EndPos, float amount)
    {
        moveProgress += Time.deltaTime * StepSpeed;
        float t = Mathf.Clamp01(moveProgress);
        t= Mathf.SmoothStep(0,1,t);
        if(MovementCurve.length > 0)
        t = MovementCurve.Evaluate(t);

        Vector3 flatPos = Vector3.Lerp(startPos, EndPos, t);

        float Fulldist = Vector3.Distance(PrevPos, EndPos);
        float curdist = Vector3.Distance(EndPoint.position, EndPos);

        float maxJumpHeight = 1;
        float height = 4 * maxJumpHeight * t * (1 - t);
        flatPos.y += height;
        EndPoint.position = flatPos;

        Vector3 Direction = new Vector3(MoveToPos.x, 0, MoveToPos.z) - new Vector3(PrevPos.x, 0, PrevPos.z);
        Direction = Vector3.Normalize(Direction);

        var rotation = Quaternion.LookRotation(Direction);
        EndPoint.rotation = Quaternion.Lerp(EndPoint.rotation, rotation, moveProgress);
        EndPoint.rotation = NormalRotation(t);
    }

    Quaternion NormalRotation(float t)
    {
        Vector3 forward = MoveToPos - PrevPos;
        forward.y = 0f;

        if (forward.sqrMagnitude > 0.0001f)
        {
            forward = Vector3.ProjectOnPlane(forward, SurfaceNormal).normalized;

            Quaternion targetRot =
                Quaternion.LookRotation(forward, SurfaceNormal);

            return 
                Quaternion.Slerp(EndPoint.rotation, targetRot, t);
        }
        return EndPoint.rotation;
    }
    void MoveFinished()
    {
        moving = false;

    }

    public bool OnGround()
    {
        return Physics.Raycast(EndPoint.position, Vector3.down, .2f, GroundLayer);
    }

    private void OnDrawGizmos()
    {
        if(!Application.isPlaying)return;
        if(!ShowDebug)return;
        Gizmos.color = new Color(1,1,1,.25f);
        Gizmos.DrawWireSphere(StartPoint.position,Settings.MaxReachLength);
        //Gizmos.DrawWireSphere(GroundHitLocation, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(NextStepPos, 0.1f);
        Gizmos.DrawWireSphere(NextStepPos, Settings.MaxStrideLength);

        Vector3 DefaultFootPos = BotBody.transform.TransformPoint(DefaultFootPlacementOffset);// StartPoint.position + DefaultFootPlacementOffset;


        Gizmos.DrawWireCube(DefaultFootPos, Vector3.one*.2f);


        RaycastHit nextstepHit;

        Ray NextStepRay = new Ray(StartPoint.position, NextStepPos2 - StartPoint.position);

        if (Physics.Raycast(NextStepRay, out nextstepHit, 2, GroundLayer))
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawWireSphere(nextstepHit.point, .1f);
        }

        Gizmos.DrawRay(NextStepRay);

        //if (FloorRay)
        {

        }
    }
}


[Serializable]
public struct ProceduralSettings
{
    public float MaxStrideLength;
    public float MaxReachLength;
}
