using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

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
    public float Length;
    Vector3 HandTargetPoint;
    Transform localGrabTarget;

    public MovementMotion ReachMotion = new();
    public MovementMotion WithdrawMotion = new();

    [SerializeField] Collider[] colliders = new Collider[100];
    GameObject CurTargetGameObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RestingPosition2 = transform.position + transform.forward;
        Length = GetComponent<LimbCreator>().Length;
        colliders = new Collider[20];
        localGrabTarget = new GameObject("localGrabTarget").transform;
       // localGrabTarget.SetParent(transform);
        localGrabTarget.position = RestingPosition2;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        motionPlayer.update2(EndPoint);

        RestingPosition2 = transform.position + transform.forward;

        if (!motionPlayer.isPlaying && Actions.Count >0)
        {
            Actions.Dequeue().Invoke();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Grab2(transform.position + Vector3.left);
        }

        Physics.OverlapSphereNonAlloc(transform.position, Length, colliders);

       // List<Collider> colliders2 = Physics.OverlapSphere(transform.position, Length);
        Collider[] colliders2 = Physics.OverlapSphere(transform.position, Length);


        if (motionPlayer.isPlaying) return;

        foreach(var c in colliders2)
        {
            if(c.GetComponentInParent<ResourceScript>() != null)
            {
                Transform rs = c.GetComponentInParent<ResourceScript>().transform;
                CurTargetGameObj = rs.gameObject;
                float Dist = Vector3.Distance(transform.position, rs.position);
                if(Dist > Length)
                {
                    HandTargetPoint = c.ClosestPoint(transform.position);
                }
                else
                {
                    HandTargetPoint = rs.position;
                }
                localGrabTarget.position = HandTargetPoint;
                Grab3(EndPoint,localGrabTarget);
                break;
            }
        }

        if(Actions.Count == 0)
        {
            EndPoint.position = RestingPosition2;
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
    public void Grab3(Transform Part, Transform target)
    {
        Actions.Clear();
        Actions.Enqueue(() => motionPlayer.Play2(EndPoint, target, ReachMotion, onCompleted: ()=> UseClaw(target.gameObject)));
        //Actions.Enqueue(() => motionPlayer.Play2(EndPoint, EndPoint, ReachMotion));

        Transform returnPoint = GetBestBasket(GetBaskets()).PlacePoint;
        if(returnPoint == null)
        {
            returnPoint = transform;
        }
        Actions.Enqueue(() => motionPlayer.Play2(EndPoint, returnPoint, WithdrawMotion, onCompleted: () => PlaceItem()));
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

    public void UseClaw(GameObject targetObj)
    {
        Debug.Log("Use Claw");
    }
    public void FinishGrab()
    {

    }

    public void PlaceItem()
    {
        GameObject resource = GameObject.CreatePrimitive(PrimitiveType.Cube);
        resource.transform.position = EndPoint.position;
        resource.transform.localScale = Vector3.one*.2f;
        resource.AddComponent<SphereCollider>();
        resource.AddComponent<Rigidbody>();
        resource.GetComponent<Rigidbody>().mass = 10;
        resource.GetComponent<Rigidbody>().linearDamping = 1;
        resource.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        resource.GetComponent<Renderer>().material.color = Color.black;
        Physics.IgnoreCollision(resource.GetComponent<SphereCollider>(), EndPoint.GetComponentInChildren<BoxCollider>(false));
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

    public List<BotComponent_Basket> GetBaskets()
    {
        List<BotComponent_Basket> baskets = new List<BotComponent_Basket>();
        var body = transform.GetComponentInParent<BotComponent_Grabber>().body;
        
        foreach(BotComponent bc in body.CurComponents)
        {
            if(bc.GetType() == typeof(BotComponent_Basket))
            {
                baskets.Add(bc as BotComponent_Basket);
            }
        }

        return baskets;
    }
    public BotComponent_Basket GetBestBasket(List<BotComponent_Basket> baskets)
    {
        BotComponent_Basket curbest = baskets[Random.Range(0,baskets.Count-1)];
        foreach (BotComponent bc in baskets)
        {
            float dist = Vector3.Distance(transform.position, bc.transform.position);
            float bestdist = Vector3.Distance(transform.position, curbest.transform.position);
            if(true) // <--- CHANGE THIS TO A fullness check
            { 
            }
            if(dist < bestdist)
            {
                curbest = bc as BotComponent_Basket;
            }
        }

        return curbest;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Length);

        if(!Application.isPlaying)return;
        if(!enabled)return;
        Gizmos.DrawWireSphere(localGrabTarget.position, .1f);
    }
}

public enum GrabberStates
{
    Rest,
    Reach,
    Withdraw,
}
