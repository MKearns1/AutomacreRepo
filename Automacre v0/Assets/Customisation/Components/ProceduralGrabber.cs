using UnityEngine;
using UnityEngine.UIElements;

public class ProceduralGrabber : ProceduralPart
{
    public Transform RestingPosition;
    public Vector3 RestingPosition2;
    private float moveProgress;
    public float BaseSpeed;
    public float SpeedMultiplier;
    public AnimationCurve MovementCurve;
    private Vector3 PrevPos;
    private Vector3 MoveToPos;
    public bool moving;
    public GrabberStates state;

    public MovementMotion ReachMotion = new();
    public MovementMotion WithdrawMotion = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GetComponent<FABRIK>().TargetTransform = RestingPosition;

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        RestingPosition2 = transform.position + transform.forward;
       // EndPoint.position = RestingPosition2;
        // EndPoint.rotation = transform.rotation;

        if (!motionPlayer.isPlaying && Actions.Count >0)
        {
            Actions.Dequeue().Invoke();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Grab2(transform.position + Vector3.left);
        }

        if (Vector3.Distance(GameObject.Find("Box").transform.position, transform.position) < 3 && !motionPlayer.isPlaying)
        {
            Grab2(GameObject.Find("Box").transform.position);
        }

/*        if (moving)
        {
            MoveTransition(PrevPos,MoveToPos,moveProgress);

            float Dist = Vector3.Distance(EndPoint.position, MoveToPos);
            if (Dist > .1f) return;

            EndPoint.position = MoveToPos;
            moveProgress = 0;
            MoveFinished();

            
        }*/

    }

    public void Grab2(Vector3 target)
    {
        Actions.Clear();
        Actions.Enqueue(() => motionPlayer.Play(EndPoint.position, target, ReachMotion));
        Actions.Enqueue(() => motionPlayer.Play(EndPoint.position, GetComponentInParent<BotBodyBase>().transform.position+Vector3.up, WithdrawMotion));
    }

    public void Grab(Vector3 Position)
    {
        Reach(Position);
    }

    public void Reach(Vector3 Position)
    {
        state = GrabberStates.Reach;
        PrevPos = EndPoint.position;
        MoveToPos = Position;
        moving = true;
        SpeedMultiplier = 1f;
    }

    public void Hold()
    {
        Return();
    }

    public void Return()
    {
        state = GrabberStates.Withdraw;
        PrevPos = EndPoint.position;
        //MoveToPos = GetComponentInParent<BotBodyBase>().transform.position+Vector3.up*.8f;
        MoveToPos = transform.position;
        moving = true;
        SpeedMultiplier = .5f;
    }

    public void MoveTransition(Vector3 startPos, Vector3 EndPos, float amount)
    {
        moveProgress += Time.deltaTime * BaseSpeed * SpeedMultiplier;
        float t = Mathf.Clamp01(moveProgress);
        t = Mathf.SmoothStep(0, 1, t);
        if (MovementCurve.length > 0)
            t = MovementCurve.Evaluate(t);

        Vector3 flatPos = Vector3.Lerp(startPos, EndPos, t);

        float Fulldist = Vector3.Distance(PrevPos, EndPos);
        float curdist = Vector3.Distance(EndPoint.position, EndPos);

        float maxJumpHeight = 1;
        float height = 4 * maxJumpHeight * t * (1 - t);
        flatPos.y += height;
        EndPoint.position = flatPos;

       // Vector3 Direction = new Vector3(MoveToPos.x, 0, MoveToPos.z) - new Vector3(PrevPos.x, 0, PrevPos.z);
        Vector3 Direction = new Vector3(MoveToPos.x, 0, MoveToPos.z) - new Vector3(transform.position.x, 0, transform.position.z);
        Direction = Vector3.Normalize(Direction);

        var rotation = Quaternion.LookRotation(Direction);
        EndPoint.rotation = Quaternion.Lerp(EndPoint.rotation, rotation, moveProgress);
        //EndPoint.rotation = NormalRotation(t);
    }
    public void MoveFinished()
    {
        moving = false;
        switch (state)
        {
            case GrabberStates.Rest:
                break;

            case GrabberStates.Reach:

                Return();

                break;

            case GrabberStates.Withdraw:

                state = GrabberStates.Rest;
               // PrevPos = EndPoint.position;
               // MoveToPos = RestingPosition2;
                moving = false;

                break;
        }
    }


}

public enum GrabberStates
{
    Rest,
    Reach,
    Withdraw,
}
