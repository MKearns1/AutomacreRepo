using Unity.AI.Navigation;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    float InputAxisX, InputAxisY;
    public float CamMoveSpeed = 15;

    float rotY;
    public float RotationSpeed=20;

    Camera cam;

    Vector3 RotationOrigin;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
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
        transform.position += MoveDirection;



















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

            Vector3 Rotation = new Vector3(0, transform.rotation.eulerAngles.y + rotY, 0);

        //transform.rotation = Quaternion.Euler(Rotation);
        
       


        Ray ScreenCentreRay = cam.ScreenPointToRay(new Vector2(Screen.width/2,Screen.height/2));
        RaycastHit hit3;

        Physics.Raycast(ScreenCentreRay, out hit3);

        if (hit3.point != null)
        {
            RotationOrigin = hit3.point;
        }




        transform.RotateAround(RotationOrigin,transform.up, rotY * Time.deltaTime * RotationSpeed);



        if (Input.GetMouseButtonDown(0))
        {
            Ray Mouseray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Physics.Raycast(Mouseray, out hit);

            if (hit.collider != null)
            {
                BotDirection direction = new BotDirection(DirectType.Move, hit.point, hit.collider.gameObject);

                if (hit.collider.GetComponentInParent<ResourceScript>() != null)
                {

                    ResourceScript resource = hit.collider.gameObject.GetComponent<ResourceScript>();
                    direction.Type = DirectType.Harvest;
                }
                else
                {
                    
                }
                //Debug.Log(hit.collider.gameObject.ToString());
                foreach (var bot in GameObject.FindGameObjectsWithTag("Bot"))
                {
                    if (bot != null)
                    {
                        bot.GetComponent<BotScript>().GiveDirection(direction);
                    }
                }
            }

        }




        GameObject.Find("NavMesh Surface").GetComponent<NavMeshSurface>().BuildNavMesh();

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(RotationOrigin, 1);
    }
}
