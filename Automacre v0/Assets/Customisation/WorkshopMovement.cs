using UnityEngine;

public class WorkshopMovement : MonoBehaviour
{
    Transform CamXRoot;
    Transform CamYRoot;
    Camera MainCam;
    float pitch = 0;

    float ZoomSpeed = .2f;
    float GizmoIncrementRate = .5f;
    bool holdingCtrl;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CamXRoot = transform;
        CamYRoot = transform.GetChild(0);
        MainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        float InputX = Input.GetAxis("Horizontal");
        float InputY = Input.GetAxis("Vertical");

        Vector3 rot = new Vector3(0, -InputX, 0);

        CamXRoot.Rotate(rot);

        pitch += InputY;
        pitch = Mathf.Clamp(pitch, 0, 45);

        CamYRoot.transform.localRotation = Quaternion.Euler(pitch,0,0);

        if ((Input.GetAxis("Mouse ScrollWheel") > 0))
        {
            MainCam.transform.localPosition += Vector3.forward * ZoomSpeed;
        }
        else if ((Input.GetAxis("Mouse ScrollWheel") < 0))
        {
            MainCam.transform.localPosition -= Vector3.forward * ZoomSpeed;
        }

        holdingCtrl = false;
        if(Input.GetKey(KeyCode.LeftControl))
        {
            holdingCtrl = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit[] rayhits = Interfaces.CastMouseOverObjects(MainCam);

            RaycastHit rayhit = GetBestRaycastHit(rayhits);

            if (rayhit.collider == null) return;

            Debug.Log(rayhit.collider.gameObject);

            if(rayhit.collider.gameObject.tag == "AttatchPoint")
            {
                Debug.Log("Attempt Attach");
                if (WorkshopGeneral.instance.CurrentSelectedComponentToPlace == null) return ;
                ClickAttachPoint(rayhit);
            }

            if(rayhit.collider.transform.GetComponentInParent<BotComponent>() != null)
            {
                Debug.Log("Select Component on Bot");
                WorkshopGeneral.instance.SelectBotsComponent(rayhit.collider.transform.GetComponentInParent<BotComponent>());
            }

            if(rayhit.collider.gameObject.GetComponent<Transformable>() != null)
            {
                Debug.Log("Transform: " + rayhit.collider.gameObject);
                rayhit.collider.gameObject.GetComponent<Transformable>().Select();
            }

            TransformGizmo gizmo = rayhit.collider.gameObject.GetComponentInParent<TransformGizmo>();
            if (holdingCtrl) { GizmoIncrementRate = .25f; } else { GizmoIncrementRate = 1; }

            if (gizmo != null)
            {
                switch (rayhit.collider.gameObject.name)
                {
                    case "X":
                        gizmo.IncrementMove(Vector3.right * GizmoIncrementRate);
                        break;
                    case "Y":
                        gizmo.IncrementMove(Vector3.up * GizmoIncrementRate);
                        break;
                    case "Z":
                        gizmo.IncrementMove(Vector3.forward * GizmoIncrementRate);
                        break;

                    case "X-":
                        gizmo.IncrementMove(Vector3.left * GizmoIncrementRate);
                        break;
                    case "Y-":
                        gizmo.IncrementMove(Vector3.down * GizmoIncrementRate);
                        break;
                    case "Z-":
                        gizmo.IncrementMove(Vector3.back * GizmoIncrementRate);
                        break;
                }

            }

        }
    }

    public void ClickAttachPoint(RaycastHit rayhit)
    {
        Transform attatchpoint = rayhit.collider.gameObject.transform;
        AttatchPoint ap = attatchpoint.GetComponent<AttatchPoint>();

        if ((ap.UnacceptedTypes.Contains(WorkshopGeneral.instance.CurrentSelectedComponentToPlace.ComponentDefaultData.Type)))
        {
            return;
        }
        //Debug.LogWarning(WorkshopGeneral.instance.CurrentSelectedComponentToPlace.ComponentDefaultData.DefaultPrefab == null);

        GameObject newComp = Instantiate(WorkshopGeneral.instance.CurrentSelectedComponentToPlace.ComponentDefaultData.DefaultPrefab, attatchpoint.position, attatchpoint.rotation);

        ap.AttachNewComponent(newComp.GetComponent<BotComponent>());
        WorkshopGeneral.instance.SelectBotsComponent(newComp.GetComponent<BotComponent>());
    }

    public int GetHitPriority(RaycastHit rayhit)
    {
        if (rayhit.collider.gameObject.tag == "AttatchPoint")
        {
            if (WorkshopGeneral.instance.CurrentSelectedComponentToPlace != null) return 80;
      
        }

        if (rayhit.collider.transform.GetComponentInParent<BotComponent>() != null)
        {
            return 100;
        }

        if (rayhit.collider.gameObject.GetComponent<Transformable>() != null)
        {
            return 50;
        }

        TransformGizmo gizmo = rayhit.collider.gameObject.GetComponentInParent<TransformGizmo>();

        if (gizmo != null)
        {
            return 110;
        }
        return 0;
    }

    public RaycastHit GetBestRaycastHit(RaycastHit[] rayHits)
    {
        if (rayHits == null || rayHits.Length == 0) return default;

        RaycastHit BestHit = default;
        int BestPriority = 0;

        foreach (RaycastHit hit in rayHits)
        {
            int curPriority = GetHitPriority(hit);
            if (BestPriority < curPriority)
            {
                BestPriority = curPriority;
                BestHit = hit;
            }
        }
        if(BestPriority ==0 ) return default;

        return BestHit;
    }
}

