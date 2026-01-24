using System;
using UnityEngine;

public class ProceduralWalker : MonoBehaviour
{
   // [Serializable]
    public ProceduralSettings Settings;
    public BotBodyBase BotBody;

    Vector3 ComponentPlacementOffset;

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
    public Vector3 DefaultFootPlacementOffset;
    Vector3 DefaultFootPos;
    public bool ShowDebug;
    public AnimationCurve MovementCurve;
    Vector3 SurfaceNormal;
    Vector3 PredictedBodyPos;
    Vector3 PredictedFootPos;
    Ray PredictedFootPosToFloor;
    float DistBetweenStartAndEnd;
    float maxLimbLength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       //StartPoint = transform.Find("GameObject").transform.Find("ConnectionPoint").transform;
       //StartPoint = transform.Find("ConnectionPoint").transform;
       StartPoint = transform;
        //EndPoint = transform.Find("GameObject").transform.Find("EndPoint").transform;
        //EndPoint = GameObject.Find("EndPoint").transform ;
        // DefaultFootPlacementOffset = EndPoint.position - StartPoint.position;
        DefaultFootPlacementOffset = (transform.forward).normalized*2f;
        DefaultFootPlacementOffset.y = 0;
        ComponentPlacementOffset = BotBody.transform.position - transform.position;
        maxLimbLength = GetComponent<LimbCreator>().Length;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(BotBody.GetComponent<BotAI>().NavAgent.velocity);
        DefaultFootPos = BotBody.transform.TransformPoint(DefaultFootPlacementOffset);// StartPoint.position + DefaultFootPlacementOffset;
        DefaultFootPos.y = transform.position.y;
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

         //   NextStepPos = GroundHitLocation;
        NextStepPos2 = NextStepPos + BotBody.transform.forward * Settings.MaxStrideLength * 1.1f;
        RaycastHit nextstepHit;

        Ray NextStepRay = new Ray(StartPoint.position, NextStepPos2 - StartPoint.position);

        if (Physics.Raycast(NextStepRay, out nextstepHit, 4, GroundLayer))
        {
            NextStepPos2 = nextstepHit.point;
        }

        StepSpeed = 1 * StepSpeedMultiplier;


        NextStepPos2 = DefaultFootPos;
       
        PredictedBodyPos = BotBody.transform.position + BotBody.GetComponent<BotAI>().NavAgent.velocity / 2;
        PredictedFootPos = PredictedBodyPos + BotBody.transform.TransformVector(DefaultFootPlacementOffset);
        PredictedFootPos.y = transform.position.y;

        RaycastHit DefaultPosBlockedHit;
        Vector3 DefaultBlockDir = DefaultFootPos - StartPoint.position;
        Ray CandidateBlockedRay = new Ray(StartPoint.position, DefaultBlockDir.normalized);
        bool DefaultPosBlocked = Physics.Raycast(CandidateBlockedRay, out DefaultPosBlockedHit, DefaultBlockDir.magnitude, GroundLayer);

        RaycastHit PredictedFootRaycastFloor;
        PredictedFootPosToFloor = new(PredictedFootPos, Vector3.down);
        bool PredictedHasGround = Physics.Raycast(PredictedFootPosToFloor, out PredictedFootRaycastFloor, 2, GroundLayer);

        RaycastHit PredictedPosBlockedHit;
        Vector3 PredictedPosBlockDir = PredictedFootRaycastFloor.point - StartPoint.position;
        Ray PredictedPosBlockRay = new Ray(StartPoint.position, PredictedPosBlockDir.normalized);
        bool PredictedPosBlocked = Physics.Raycast(PredictedPosBlockRay, out PredictedPosBlockedHit, PredictedPosBlockDir.magnitude, GroundLayer);

        DistBetweenStartAndEnd = Vector3.Distance(StartPoint.position, EndPoint.position);
        bool FootToofar = DistBetweenStartAndEnd > GetComponent<LimbCreator>().Length;

        Vector3 chosenPos = GetFootPlacementPos();

        if (DefaultPosBlocked)
        {
            Debug.Log("DefaultBlocked");
            chosenPos = DefaultPosBlockedHit.point;
        }
        else if (PredictedHasGround == false) // IF the predicted spot didnt hit anything AND the default pos was NOT blocked
        {

        }
        else if (PredictedPosBlocked) // IF the default pos was NOT blocked AND the predicted did hit the ground
        {
            Debug.Log("PredictedBlocked");
            chosenPos = PredictedPosBlockedHit.point;
        }
        else // IF the default pos was NOT blocked AND the predicted did have a ground AND between the Predicted was NOT blocked
        {
            float DistBetweenStartAndCandidate = Vector3.Distance(StartPoint.position, PredictedFootRaycastFloor.point);
            bool CandidateToofar = DistBetweenStartAndCandidate > GetComponent<LimbCreator>().Length;

            if (!CandidateToofar)
            {
                chosenPos = PredictedFootRaycastFloor.point;
            }
        }

        NextStepPos = chosenPos;

        /*
        else
        {
            if (PredictedHasGround)
            {
                if (PredictedPosBlocked)
                {
                    Debug.Log("PredictedBlocked");
                    NextStepPos = PredictedPosBlockedHit.point;
                }
                else
                {
                    Debug.Log("aaaaaa");

                    float DistBetweenStartAndCandidate = Vector3.Distance(StartPoint.position, PredictedFootRaycastFloor.point);
                    bool CandidateToofar = DistBetweenStartAndCandidate > GetComponent<LimbCreator>().Length;

                    if (CandidateToofar || FootToofar)
                    {
                        NextStepPos = GetFootPlacementPos();
                    }
                    else
                    {
                        NextStepPos = PredictedFootRaycastFloor.point;
                    }
                }
            }
            else
            {
                Debug.Log("sssss");

                NextStepPos = GetFootPlacementPos();
            }

        }*/

        if (!moving && MovementAllowed && ShouldStep())
        {
            moving = true;
            // EndPoint.transform.position = NextStepPos;
            //NextStepPos2 = NextStepPos + BotBody.transform.forward * BotBody.moveSpeed / 1;
            PrevPos = EndPoint.position;
            MoveToPos = NextStepPos;
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

    public bool ShouldStep()
    {
        Vector3 a = EndPoint.position;
        Vector3 b = DefaultFootPos;
        a.y = 0;
        b.y = 0;

        float FootDistance = Vector3.Distance(a, b);
        bool FootFarAway = FootDistance > Settings.MaxStrideLength;

        Vector3 c = EndPoint.position;
        c.y = StartPoint.position.y;

        float FootAngleError = Vector3.Angle(StartPoint.forward, (c - StartPoint.position).normalized);
        //Debug.Log(FootAngleError);
        bool BodyRotatedTooMuch = FootAngleError > 30;

       // return  BodyRotatedTooMuch;
        return FootFarAway || BodyRotatedTooMuch;
    }

    public Vector3 GetFootPlacementPos()
    {
        RaycastHit PredictedFootRaycastFloor;
        bool HasGround = Physics.Raycast(PredictedFootPosToFloor, out PredictedFootRaycastFloor, 2, GroundLayer);

        if (HasGround)
        {
            return PredictedFootRaycastFloor.point;
        }

        float Iterations = 10;
        for (int i = (int)Iterations; i > 0; i--)
        {
            Debug.Log(i);
            float lerpAmount = i / Iterations;
            Vector3 RayOrigin = Vector3.Lerp(BotBody.transform.position, PredictedFootPos, lerpAmount);
            Ray newRay = new(RayOrigin, Vector3.down);
            HasGround = Physics.Raycast(newRay, out PredictedFootRaycastFloor, maxLimbLength, GroundLayer);

            if(!HasGround)continue;

            return PredictedFootRaycastFloor.point;
        }

//        return Vector3.zero;
        return DefaultFootPos;
    }

    private void OnDrawGizmos()
    {
        if(!Application.isPlaying)return;
        if(!ShowDebug)return;
        if(!this.enabled)return;
        Gizmos.color = new Color(1,1,1,.25f);
        Gizmos.DrawWireSphere(StartPoint.position,Settings.MaxReachLength);
        //Gizmos.DrawWireSphere(GroundHitLocation, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(NextStepPos, 0.1f);
        Gizmos.DrawWireSphere(NextStepPos, Settings.MaxStrideLength);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(DefaultFootPos, Vector3.one*.2f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(PredictedBodyPos, Vector3.one*.6f);
        Gizmos.DrawWireCube(PredictedFootPos, Vector3.one*.3f);

        Gizmos.DrawLine(PredictedFootPosToFloor.origin, PredictedFootPosToFloor.origin+PredictedFootPosToFloor.direction*2);
        Gizmos.DrawLine(BotBody.transform.position, BotBody.transform.position + PredictedFootPosToFloor.direction*maxLimbLength);


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
