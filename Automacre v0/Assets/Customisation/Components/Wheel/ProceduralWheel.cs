using UnityEngine;

public class ProceduralWheel : ProceduralPart
{
    public BotBodyBase BotBody;
    public Transform WheelPart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BotBody = GetComponentInParent<BotComponent_Wheel>().body;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (BotBody.GetComponent<BotAI>().NavAgent.velocity.magnitude > 1)
        {


            WheelPart.rotation = Quaternion.LookRotation(BotBody.GetComponent<BotAI>().NavAgent.velocity, transform.up);
        }
    }
}
