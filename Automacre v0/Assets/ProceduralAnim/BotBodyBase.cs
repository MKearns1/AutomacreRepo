using UnityEngine;
using System.Collections.Generic;

public class BotBodyBase : MonoBehaviour
{
    public float moveSpeed;

    Ray FloorRay;

    public LayerMask GroundLayer;
    Vector3 GroundHitLocation;
    public float DesiredHeight = 1;
    public List<ProceduralAnimBase> ProceduralComponents;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ProceduralComponents[Random.Range(0,ProceduralComponents.Count)].MovementAllowed = true;
        
    }

    // Update is called once per frame
    void Update()
    {


        FloorRay = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(FloorRay.origin, FloorRay.direction);
        RaycastHit hit;

        if (Physics.Raycast(FloorRay, out hit, 2, GroundLayer))
        {
            GroundHitLocation = hit.point;
        }

        Vector3 DesiredPos = GroundHitLocation + Vector3.up * DesiredHeight;

        transform.position = Vector3.Lerp(transform.position, DesiredPos, Time.deltaTime * 5);

        Vector3 AvgHeight = Vector3.zero;
        float CompAvgHeight = 0;
        bool stepping = false;

        foreach (var comp in ProceduralComponents)
        {
            float y = comp.EndPoint.position.y;
            CompAvgHeight += y;
            if (comp.moving)
            {
                stepping = true;

                /*foreach (var comp2 in ProceduralComponents)
                {
                    if(comp2 != comp)
                    {
                        comp.MovementAllowed = false;
                    }
                    else { comp.MovementAllowed = true; }
                }*/
                
            }

        }

        foreach (var comp in ProceduralComponents)
        {
            if (comp == GetMostBehindComponent() && !comp.moving)
            {
                comp.MovementAllowed = true;
            }
            else { comp.MovementAllowed = false;} 
        }

            AvgHeight /= ProceduralComponents.Count;
        CompAvgHeight /= ProceduralComponents.Count;
        CompAvgHeight += DesiredHeight;

        Ray FaceRay = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(FaceRay, out hit, 3, GroundLayer))
        {
            //transform.Rotate(Vector3.up*Time.deltaTime * 4000);
            if(Vector3.Angle(hit.normal, Vector3.up) > 80)
            transform.rotation = Quaternion.LookRotation(Vector3.Reflect(transform.forward, hit.normal), Vector3.up);
        }
        Debug.DrawRay(FaceRay.origin, FaceRay.direction);

        // if(!stepping)
        transform.position = transform.position + transform.forward * Time.deltaTime * moveSpeed;
        transform.position = new Vector3(transform.position.x, CompAvgHeight, transform.position.z);
    }




    public ProceduralAnimBase GetMostBehindComponent()
    {
        ProceduralAnimBase furthestComponent = null;
        float furthestdist = -Mathf.Infinity;

        foreach (var comp in ProceduralComponents)
        {
            float dist = Vector3.Distance(comp.EndPoint.position, comp.NextStepPos);

            if(dist > furthestdist)
            {
                furthestdist = dist;
                furthestComponent = comp;
            }

        }

        return furthestComponent;
    }

}
