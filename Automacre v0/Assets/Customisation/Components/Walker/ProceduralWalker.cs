using System;
using Unity.VisualScripting;
using UnityEngine;

public class ProceduralWalker : ProceduralPart
{
   // [Serializable]
    public ProceduralSettings Settings;
    public BotBodyBase BotBody;

    Vector3 ComponentPlacementOffset;

    Transform StartPoint;
    Vector3 GroundHitLocation;
    public Vector3 NextStepPos;
    Vector3 NextStepPos2;

    Ray FloorRay;

    public LayerMask GroundLayer;

    public bool moving;
    public bool MovementAllowed;
    float moveProgress;
    public float StepSpeed;
    public float StepSpeedMultiplier = 2;
    Vector3 PrevPos;
    Vector3 MoveToPos;
    public Vector3 DefaultFootPlacementOffset;
    Vector3 DefaultFootPos;
    public bool ShowDebug;
    public AnimationCurve MovementCurve;
    [DoNotSerialize ]public Vector3 SurfaceNormal;
    Vector3 PredictedBodyPos;
    Vector3 PredictedFootPos;
    Ray PredictedFootPosToFloor;
    float DistBetweenStartAndEnd;
    [DoNotSerialize] public float maxLimbLength;
    public bool HasStepToken;
    bool onGround;

    Vector3 BotForward;

    public MovementMotion StepMotion = new();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPoint = transform;
        //DefaultFootPlacementOffset = (transform.forward).normalized*2f;
        DefaultFootPlacementOffset = Vector3.zero;
        DefaultFootPlacementOffset = (transform.right).normalized * .5f;
        DefaultFootPlacementOffset = (BotBody.transform.position - transform.position).normalized * -.5f;
        DefaultFootPlacementOffset.y = 0;
        //ComponentPlacementOffset = BotBody.transform.position - transform.position;
        maxLimbLength = GetComponent<LimbCreator>().Length;
        onGround = true;
        BotForward = transform.GetComponentInParent<BotBodyBase>().transform.forward;
        Settings.MaxStrideLength = maxLimbLength/2;
        StepMotion.arcHeight = maxLimbLength / 2;
        StepSpeed = Mathf.Lerp(1, 2, Mathf.InverseLerp(8, 1, maxLimbLength));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        DefaultFootPos = BotBody.transform.TransformPoint(DefaultFootPlacementOffset);// StartPoint.position + DefaultFootPlacementOffset;
        DefaultFootPos.y = transform.position.y;
        Vector3 DefaultFootPosDirection = DefaultFootPos - StartPoint.position;

        //StepSpeed = BotBody.transform.parent.GetComponentInChildren<BotAI>().NavAgent.velocity.magnitude * StepSpeedMultiplier;
        //StepSpeed = 2;

        NextStepPos2 = DefaultFootPos;

        float lookAheadTime = StepMotion.duration * .5f;
         lookAheadTime = StepMotion.duration;


        //PredictedBodyPos = BotBody.transform.position + BotBody.transform.parent.GetComponentInChildren<BotAI>().NavAgent.velocity / 2;
        PredictedFootPos = BotBody.PredictedBodyPos + BotBody.transform.TransformVector(DefaultFootPlacementOffset);
        PredictedFootPos.y = transform.position.y;

        RaycastHit DefaultPosBlockedHit;
        Vector3 DefaultBlockDir = DefaultFootPos - StartPoint.position;
        Ray CandidateBlockedRay = new Ray(StartPoint.position, DefaultBlockDir.normalized);
        bool DefaultPosBlocked = Physics.Raycast(CandidateBlockedRay, out DefaultPosBlockedHit, DefaultBlockDir.magnitude, GroundLayer);

        RaycastHit PredictedFootRaycastFloor;
        PredictedFootPosToFloor = new(PredictedFootPos, transform.up*-1);
        bool PredictedHasGround = Physics.Raycast(PredictedFootPosToFloor, out PredictedFootRaycastFloor, maxLimbLength, GroundLayer);

        RaycastHit PredictedPosBlockedHit;
        Vector3 PredictedPosBlockDir = PredictedFootRaycastFloor.point - StartPoint.position;
        Ray PredictedPosBlockRay = new Ray(StartPoint.position, PredictedPosBlockDir.normalized);
        bool PredictedPosBlocked = Physics.Raycast(PredictedPosBlockRay, out PredictedPosBlockedHit, PredictedPosBlockDir.magnitude, GroundLayer);

        DistBetweenStartAndEnd = Vector3.Distance(StartPoint.position, EndPoint.position);
        bool FootToofar = DistBetweenStartAndEnd > maxLimbLength;

        Vector3 chosenPos = GetFootPlacementPos();

        if (DefaultPosBlocked)
        {
            //Debug.Log("DefaultBlocked");
            chosenPos = DefaultPosBlockedHit.point;
            SurfaceNormal = DefaultPosBlockedHit.normal;
        }
        else if (PredictedHasGround == false) // IF the predicted spot didnt hit anything AND the default pos was NOT blocked
        {

        }
        else if (PredictedPosBlocked) // IF the default pos was NOT blocked AND the predicted did hit the ground
        {
            //Debug.Log("PredictedBlocked");
            chosenPos = PredictedPosBlockedHit.point;
            SurfaceNormal = PredictedPosBlockedHit.normal;
        }
        else // IF the default pos was NOT blocked AND the predicted did have a ground AND between the Predicted was NOT blocked
        {
            float DistBetweenStartAndCandidate = Vector3.Distance(StartPoint.position, PredictedFootRaycastFloor.point);
            bool CandidateToofar = DistBetweenStartAndCandidate > maxLimbLength;

            if (!CandidateToofar)
            {
                chosenPos = PredictedFootRaycastFloor.point;
                SurfaceNormal = PredictedFootRaycastFloor.normal;
            }
        }

        NextStepPos = chosenPos;

        if (FootToofar)
        {
            //NextStepPos = GetFootPlacementPos();
           // BotBody.transform.parent.GetComponentInChildren<MovementCoordinator>().AllowStep(this);
        }

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

        // moving = false;


        if (!moving && ShouldStep() && !HasStepToken)
        {
            BotBody.transform.parent.GetComponentInChildren<MovementCoordinator>().RequestStep(this);
        }
        if (!moving && HasStepToken)
        {
            BeginStep();
        }

/*        if (!moving && MovementAllowed && ShouldStep() && !motionPlayer.isPlaying)
        {
            moving = true;
            PrevPos = EndPoint.position;
            MoveToPos = NextStepPos;
           // Step(MoveToPos);
        }*/

        /*        if (!motionPlayer.isPlaying && Actions.Count > 0)
                {
                    Actions.Dequeue().Invoke();
                }*/


        /*       // FloorRay = new Ray(StartPoint.position, Vector3.down);
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
        }*/
        if (moving)
        {
             MoveTransition(PrevPos, MoveToPos, moveProgress);
            // Debug.LogWarning("FR" + gameObject.name);

            float Dist = Vector3.Distance(EndPoint.position, MoveToPos);
            if (Dist < .1f)
            {
                // Debug.LogWarning("POOOOOOOOOOOOOOOOOOOP" + gameObject.name);
                EndPoint.position = MoveToPos;
                EndPoint.rotation = Quaternion.FromToRotation(transform.up, SurfaceNormal) * transform.rotation; ;
                moveProgress = 0;
                MoveFinished();
            }
        }
    }

    public void Step(Vector3 target)
    {
        Actions.Clear();

        Vector3 start = EndPoint.position;
        float dist = Vector3.Distance(start, target);

        float bodySpeed = BotBody.transform.parent.GetComponentInChildren<BotAI>().NavAgent.velocity.magnitude;

        float footSpeed = Mathf.Max(bodySpeed * 1.5f, 0.5f);
        float duration = dist / footSpeed;
        duration = Mathf.Clamp(duration, 0.1f, 0.5f);
        StepMotion.duration = duration;
        Actions.Enqueue(() => motionPlayer.Play(EndPoint.position, target, StepMotion));
    }

    public void MoveTransition(Vector3 startPos, Vector3 EndPos, float amount)
    {
        moveProgress += Time.deltaTime * StepSpeed;
        float t = Mathf.Clamp01(moveProgress);
        t = Mathf.SmoothStep(0, 1, t);
        if (MovementCurve.length > 0)
            t = MovementCurve.Evaluate(t);

        Vector3 flatPos = Vector3.Lerp(startPos, EndPos, t);

        float Fulldist = Vector3.Distance(PrevPos, EndPos);
        float curdist = Vector3.Distance(EndPoint.position, EndPos);

        float maxJumpHeight = maxLimbLength/2;
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

    void BeginStep()
    {
        moving = true;
        HasStepToken = false;
        onGround = false;

        PrevPos = EndPoint.position;
        MoveToPos = NextStepPos;
    }
    void MoveFinished()
    {
        moving = false;
        onGround = true;
        BotBody.transform.parent.GetComponentInChildren<MovementCoordinator>().CompFinishStep(this);
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
        return FootFarAway;
    }

    public Vector3 GetFootPlacementPos()
    {
        //return DefaultFootPos;

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
        //Gizmos.DrawWireSphere(StartPoint.position,Settings.MaxReachLength);
        //Gizmos.DrawWireSphere(GroundHitLocation, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(NextStepPos, 0.1f);
        Gizmos.DrawWireSphere(NextStepPos, Settings.MaxStrideLength);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(DefaultFootPos, Vector3.one*.2f);
        Gizmos.color = Color.yellow;
       // Gizmos.DrawWireCube(PredictedBodyPos, Vector3.one*.6f);
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
