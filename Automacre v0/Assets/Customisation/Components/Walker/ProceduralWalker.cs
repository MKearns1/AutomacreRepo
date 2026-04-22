using System;
using Unity.VisualScripting;
using UnityEngine;

public class ProceduralWalker : ProceduralPart
{
    public ProceduralSettings Settings;
    public BotBodyBase BotBody;

    Transform StartPoint;
    public Vector3 NextStepPos;

    public LayerMask GroundLayer;

    public bool IsMoving;
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
    public AnimationCurve NoGroundCurve;
    [DoNotSerialize ]public Vector3 SurfaceNormal;
    Vector3 PredictedFootPos;
    Ray PredictedFootPosToFloor;
    [HideInInspector] public float DistBetweenStartAndEnd;
    [HideInInspector] public float ExtensionRatio;
    [DoNotSerialize] public float maxLimbLength { get { return GetComponent<LimbCreator>().Length;  } set { } }
    public bool HasStepToken;
    bool onGround;
    public bool WantsToStep;
    MovementCoordinator Coordinator;

    Vector3 BotForward;

    public AudioClip StepLandSound;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPoint = transform;
        maxLimbLength = GetComponent<LimbCreator>().Length;
        onGround = true;
        BotForward = transform.GetComponentInParent<BotBodyBase>().transform.forward;
        Settings.MaxStrideLength = maxLimbLength/2;
        Coordinator = BotBody.transform.parent.GetComponentInChildren<MovementCoordinator>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        DefaultFootPos = BotBody.transform.TransformPoint(DefaultFootPlacementOffset);
        DefaultFootPos.y = transform.position.y;
        Vector3 DefaultFootPosDirection = DefaultFootPos - StartPoint.position;

        //PredictedBodyPos = BotBody.transform.position + BotBody.transform.parent.GetComponentInChildren<BotAI>().NavAgent.velocity / 2;
        PredictedFootPos = BotBody.PredictedBodyPos + BotBody.transform.TransformVector(DefaultFootPlacementOffset);
        PredictedFootPos.y = transform.position.y;

        RaycastHit DefaultPosBlockedHit;
        Vector3 DefaultBlockDir = DefaultFootPos - StartPoint.position;
        Ray CandidateBlockedRay = new Ray(StartPoint.position, DefaultBlockDir.normalized);
        bool DefaultPosBlocked = Physics.Raycast(CandidateBlockedRay, out DefaultPosBlockedHit, DefaultBlockDir.magnitude, GroundLayer);

        RaycastHit PredictedFootRaycastFloor;
        PredictedFootPosToFloor = new(PredictedFootPos, 
            //transform.up*-1);
            Vector3.down);
        bool PredictedHasGround = Physics.Raycast(PredictedFootPosToFloor, out PredictedFootRaycastFloor, maxLimbLength, GroundLayer);

        RaycastHit PredictedPosBlockedHit;
        Vector3 PredictedPosBlockDir = PredictedFootRaycastFloor.point - StartPoint.position;
        Ray PredictedPosBlockRay = new Ray(StartPoint.position, PredictedPosBlockDir.normalized);
        bool PredictedPosBlocked = Physics.Raycast(PredictedPosBlockRay, out PredictedPosBlockedHit, PredictedPosBlockDir.magnitude, GroundLayer);

        DistBetweenStartAndEnd = Vector3.Distance(StartPoint.position, EndPoint.position);
        bool FootToofar = DistBetweenStartAndEnd > maxLimbLength;
        ExtensionRatio = DistBetweenStartAndEnd / maxLimbLength;

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


        float DistToNextPos = Vector3.Distance(EndPoint.position, NextStepPos);
        float DistFromBasePos = Vector3.Distance(EndPoint.Find("Joint").position, StartPoint.position);


            StepUpdate();

        return;

        if (!IsMoving)
        {
            Vector3 socket = StartPoint.position;
            Vector3 toFoot = EndPoint.position - socket;
            if (toFoot.magnitude > maxLimbLength)
                EndPoint.position = socket + toFoot.normalized * maxLimbLength;
        }
        return;

        Vector3 dir = (-StartPoint.position + EndPoint.position).normalized;

        if(FootToofar && !IsMoving)
        EndPoint.position = StartPoint.position + dir * maxLimbLength;

    }

    void StepUpdate()
    {
        WantsToStep = !IsMoving && ShouldStep();

        if (!IsMoving && IsFootCriticallyTooFar())
        {
            Coordinator.RequestEmergencyStep(this);
            return;
        }

        if (IsMoving)
        {
            StepAnimationTick();
        }
    }

    void StepAnimationTick()
    {
        MoveTransition(PrevPos, MoveToPos, MovementCurve);
        moveProgress += Time.deltaTime * StepSpeed;

        bool progressedEnough = moveProgress > 0.5f;
        bool nearTarget = Vector3.Distance(EndPoint.position, MoveToPos) < 0.1f;

        if(progressedEnough && nearTarget)
        {
           OnStepFinished();
        }
    }

    public void ExecuteStep()
    {
        if ((IsMoving))
        {
            return;
        }
        if (Vector3.Distance(NextStepPos, EndPoint.position) < 0.01f)
        {
            return;
        }

        IsMoving = true;
        WantsToStep = false;
        moveProgress = 0;
        PrevPos = EndPoint.position;
        MoveToPos = NextStepPos;
    }

    void OnStepFinished()
    {
        EndPoint.position = MoveToPos;
        EndPoint.rotation = Quaternion.FromToRotation(transform.up, SurfaceNormal) * BotBody.transform.rotation;

        moveProgress = 0;
        IsMoving = false;

        Destroy(Instantiate(VFXManager.instance.StepParticleVFX.gameObject, EndPoint.position, Quaternion.identity), 1);
        SoundManager.instance.PlaySound(StepLandSound);

        Coordinator.NotifyStepFinished(this);
    }

    bool IsFootCriticallyTooFar()
    {
        float distFromBase = Vector3.Distance(EndPoint.Find("Joint").position, transform.position);
        float distToTarget = Vector3.Distance(EndPoint.position, NextStepPos);

        return distFromBase > maxLimbLength * 1.1
            //&& distToTarget > .5f
            ;
    }


    public void MoveTransition(Vector3 startPos, Vector3 EndPos, AnimationCurve movementCurve)
    {
        float t = Mathf.Clamp01(moveProgress);
        t = Mathf.SmoothStep(0, 1, t);
        if (movementCurve.length > 0)
            t = movementCurve.Evaluate(t);

        Vector3 flatPos = Vector3.Lerp(startPos, EndPos, t);

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

    public void BeginStep()
    {
        IsMoving = true;
        HasStepToken = false;
        onGround = false;

        PrevPos = EndPoint.position;
        MoveToPos = NextStepPos;
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
        bool BodyRotatedTooMuch = FootAngleError > 30;
        float DistToNextPos = Vector3.Distance(EndPoint.position, NextStepPos);


        return (FootFarAway || BodyRotatedTooMuch)
            && DistToNextPos >.5f
            ;
    }

    public Vector3 GetFootPlacementPos()
    {

        RaycastHit PredictedFootRaycastFloor;
        bool HasGround = Physics.Raycast(PredictedFootPosToFloor, out PredictedFootRaycastFloor, maxLimbLength, GroundLayer);

        if (HasGround)
        {
            return PredictedFootRaycastFloor.point;
        }

        float Iterations = 10;
        for (int i = (int)Iterations; i > 0; i--)
        {
            float lerpAmount = i / Iterations;
            Vector3 RayOrigin = Vector3.Lerp(BotBody.transform.position, PredictedFootPos, lerpAmount);
            Ray newRay = new(RayOrigin, Vector3.down);
            //HasGround = Physics.Raycast(newRay, out PredictedFootRaycastFloor, maxLimbLength, GroundLayer);
            HasGround = Physics.SphereCast(newRay.origin, 1, newRay.direction, out PredictedFootRaycastFloor, maxLimbLength, GroundLayer);

            if(!HasGround)continue;

            return PredictedFootRaycastFloor.point;
        }

        return PredictedFootPosToFloor.origin + PredictedFootPosToFloor.direction.normalized;
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
       // Gizmos.DrawWireSphere(NextStepPos, Settings.MaxStrideLength);
        Gizmos.DrawWireSphere(DefaultFootPos, Settings.MaxStrideLength);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(DefaultFootPos, Vector3.one*.2f);
        Gizmos.color = Color.yellow;
       // Gizmos.DrawWireCube(PredictedBodyPos, Vector3.one*.6f);
        Gizmos.DrawWireCube(PredictedFootPos, Vector3.one*.3f);

        Gizmos.DrawLine(PredictedFootPosToFloor.origin, PredictedFootPosToFloor.origin+PredictedFootPosToFloor.direction*2);
        Gizmos.DrawLine(BotBody.transform.position, BotBody.transform.position + PredictedFootPosToFloor.direction*maxLimbLength);


        RaycastHit nextstepHit;

        Ray NextStepRay = new Ray(StartPoint.position, DefaultFootPos - StartPoint.position);

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
