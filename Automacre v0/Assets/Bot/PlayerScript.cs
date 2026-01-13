using NUnit.Framework;
using Unity.AI.Navigation;
using UnityEditor.Experimental.GraphView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    float InputAxisX, InputAxisY;
    public float CamMoveSpeed = 15;

    float rotY;
    public float RotationSpeed=20;

    Camera cam;

    Vector3 CamDesiredPos;
    Quaternion CamDesiredRot;
    Vector3 RotationOrigin;
    Vector3 ClickLocation;
    float AgentStopDistance=10;

    public List<BotScript> CurrentSelectedBots1;
    public List<BotController> CurrentSelectedBots;
    HUDBase HudBase;
    RaycastHit CurrentHoveredObj;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = transform.GetChild(0).GetComponent<Camera>();
        HudBase = GameObject.Find("Canvas").GetComponent<HUDBase>();
        CamDesiredPos = transform.position;
        CamDesiredRot = transform.rotation;
        foreach (var bot in GameObject.FindGameObjectsWithTag("Bot"))
        {
            CurrentSelectedBots1.Add(bot.GetComponent<BotScript>());
        }

    }

    // Update is called once per frame
    void Update()
    {
        InputAxisX = Input.GetAxis("Horizontal");
        InputAxisY = Input.GetAxis("Vertical");



        Vector3 camForward = transform.forward;
        Vector3 camRight = transform.right;

        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRelative = InputAxisY * camForward;
        Vector3 rightRelative = InputAxisX * camRight;


       // Vector3 MoveDirection = new Vector3(InputAxisX, 0, InputAxisY) * Time.deltaTime * CamMoveSpeed;
        Vector3 MoveDirection = (forwardRelative+rightRelative) * Time.deltaTime * CamMoveSpeed;

        //transform.forward = camForward;
        CamDesiredPos += MoveDirection;


















        if (Input.GetKey(KeyCode.E))
        {
            rotY = -1;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            rotY = 1;
        }
        else
        { rotY = 0;        }

            Vector3 Rotation = new Vector3(0, transform.rotation.eulerAngles.y + rotY*10, 0);

        //transform.rotation = Quaternion.Euler(Rotation);
        
       


        Ray ScreenCentreRay = cam.ScreenPointToRay(new Vector2(Screen.width/2,Screen.height/2));
        RaycastHit hit3;

        Physics.Raycast(ScreenCentreRay, out hit3);

        if (hit3.collider != null)
        {
            RotationOrigin = hit3.point;
        }

        if (rotY != 0 && InputAxisX == 0 & InputAxisY ==0)
        {

            CamDesiredPos = Quaternion.AngleAxis(rotY * CamMoveSpeed *RotationSpeed * Time.deltaTime, Vector3.up) * (CamDesiredPos - RotationOrigin) + RotationOrigin;

            Vector3 dir = (RotationOrigin - CamDesiredPos).normalized;

            Quaternion LookRot = Quaternion.LookRotation(dir, transform.up);

            CamDesiredRot = Quaternion.Euler(new Vector3(0, LookRot.eulerAngles.y, 0));

            Debug.Log(LookRot.eulerAngles.y);
            Debug.DrawLine(CamDesiredPos, RotationOrigin, Color.red);
            Debug.DrawRay(CamDesiredPos, (RotationOrigin - CamDesiredPos).normalized, Color.green);
        }

        if (Input.GetMouseButtonDown(0))
        {
            MouseClick();

        }


        if((Input.GetAxis("Mouse ScrollWheel") > 0))
        {
            CamDesiredPos += cam.transform.forward;

        }
        else if ((Input.GetAxis("Mouse ScrollWheel") < 0))
        {
            CamDesiredPos += cam.transform.forward*-1;

        }

        transform.position = Vector3.Lerp(transform.position, CamDesiredPos, Time.deltaTime * 10);
        transform.rotation = Quaternion.Lerp(transform.rotation, CamDesiredRot, Time.deltaTime * 10);

        transform.Find("Cube").transform.position = CamDesiredPos;
        transform.Find("Cube").transform.rotation = CamDesiredRot;

        CurrentHoveredObj = Interfaces.CastMouseOverObject(cam);

        /*if (CurrentHoveredObj.collider != null)
        {
            IClickable i = CurrentHoveredObj.collider.gameObject.GetComponentInParent<IClickable>();
            HudBase.Hover(i);
        }*/

        

        //GameObject.Find("NavMesh Surface").GetComponent<NavMeshSurface>().BuildNavMesh();

    }

    public void MouseClick()
    {
        RaycastHit hit;
        hit = Interfaces.CastMouseOverObject(cam);

        if (hit.collider != null)
        {

            if (hit.collider.GetComponentInParent<BotController>() != null)
            CurrentSelectedBots.Add(hit.collider.GetComponentInParent<BotController>());

            foreach (BotController bot in CurrentSelectedBots)
            {
                bot.Ai.MoveTo(hit.point);
            }



            IClickable clickable = hit.collider.gameObject.GetComponentInParent<IClickable>();
            if (clickable != null)
            {

                clickable.OnClick(hit.point, CurrentSelectedBots1);
                //Debug.Log("2222222222222222222222222222");
            }
            else
            {
                BotDirection MoveTodirection = new BotDirection(DirectType.Move, hit.point, null);
                foreach (BotScript bot in CurrentSelectedBots1)
                {
                    bot.GiveDirection(MoveTodirection);
                }
                //Debug.Log("GGGGGGGGGGGGGG");
            }
        //    BotDirection direction = new BotDirection(DirectType.Move, hit.point, hit.collider.gameObject);

        //    if (hit.collider.GetComponentInParent<ResourceScript>() != null)
        //    {

        //        ResourceScript resource = hit.collider.gameObject.GetComponent<ResourceScript>();
        //        direction.Type = DirectType.Harvest;
        //    }
        //    else
        //    {

        //    }
        //    //Debug.Log(hit.collider.gameObject.ToString());
        //    foreach (var bot in GameObject.FindGameObjectsWithTag("Bot"))
        //    {
        //        if (bot != null)
        //        {
        //            bot.GetComponent<BotScript>().GiveDirection(direction);
        //        }
        //    }
        }
        ClickLocation = hit.point;
        DrawDebugCircle(ClickLocation, AgentStopDistance, 16, Color.red, 10);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(RotationOrigin, 1);
        //Gizmos.DrawSphere(ClickLocation, AgentStopDistance);
    }


    void DrawDebugCircle(Vector3 center, float radius, int segments, Color color, float duration = 0)
    {
        float angleStep = 360f / segments;

        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

            Debug.DrawLine(prevPoint, nextPoint, color, duration);

            prevPoint = nextPoint;
        }
    }

}
