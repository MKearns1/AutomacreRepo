using UnityEngine;

public class ProceduralWheel : ProceduralPart
{
    public BotBodyBase BotBody;
    public Transform WheelPart;

    Vector3 DefaultWheelPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BotBody = GetComponentInParent<BotComponent_Wheel>().body;
    }

    // Update is called once per frame
    protected override void Update()
    {
        Quaternion lookrot; 

        if (BotBody.transform.parent.GetComponentInChildren<BotAI>().NavAgent.velocity.magnitude > 0)
        {
            lookrot = Quaternion.LookRotation(BotBody.transform.parent.GetComponentInChildren<BotAI>().NavAgent.velocity, transform.up);
            WheelPart.rotation = Quaternion.Slerp(WheelPart.rotation, lookrot, Time.deltaTime *5);
        }

        DefaultWheelPos = transform.position + (transform.forward*.5f) + transform.up * -1;

        RaycastHit DefaultPosBlockedHit;
        Vector3 DefaultBlockDir = DefaultWheelPos - transform.position;
        Ray CandidateBlockedRay = new Ray(transform.position, DefaultBlockDir.normalized);
        bool DefaultPosBlocked = Physics.Raycast(CandidateBlockedRay, out DefaultPosBlockedHit, DefaultBlockDir.magnitude, LayerMask.GetMask("Ground"));
        Debug.DrawLine(CandidateBlockedRay.origin, CandidateBlockedRay.origin + DefaultBlockDir);

        RaycastHit FloorHit;
        Vector3 FloorDir =  Vector3.up*-1;
        Ray FloorRay = new Ray(DefaultWheelPos, FloorDir);
        bool HitFloor = Physics.Raycast(FloorRay, out FloorHit, GetComponent<LimbCreator>().Length*1.5f, LayerMask.GetMask("Ground"));
        Debug.DrawLine(FloorRay.origin, FloorRay.origin + FloorDir);

        Vector3 DesiredPos = transform.position;

        if (DefaultPosBlocked)
        {
            DesiredPos = DefaultPosBlockedHit.point;
        }
        else if(HitFloor)
        {
            DesiredPos = FloorHit.point;
        }
        else
        {
            DesiredPos = DefaultWheelPos + Vector3.down* GetComponent<LimbCreator>().Length/1.1f;
        }

        WheelPart.transform.position = Vector3.Lerp(WheelPart.transform.position, DesiredPos, Time.deltaTime *10);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawCube(DefaultWheelPos, Vector3.one * .2f);
      
    }
}
