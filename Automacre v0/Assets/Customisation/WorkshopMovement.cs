using NUnit.Framework;
using UnityEditor.Build.Player;
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

    SelectionHighlight currentHoverHighlight;
    SelectionHighlight currentSelectedHighlight;

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

            Click(rayhit);
        }

        SelectionHighlight newHover = null;
        RaycastHit[] hits = Interfaces.CastMouseOverObjects(GetComponentInChildren<Camera>());
        if (hits.Length == 0) return;

        newHover = hits[0].collider.transform?.GetComponentInParent<SelectionHighlight>();

        if(currentHoverHighlight != newHover)
        {
            if(currentHoverHighlight!= null) currentHoverHighlight.SetHover(false);
            if(newHover != null) newHover.SetHover(true);
            currentHoverHighlight = newHover;
        }
       // Debug.Log(hits[0].collider.transform);
    }

    public void ClickAttachPoint(RaycastHit rayhit)
    {
        Transform attatchpoint = rayhit.collider.gameObject.transform;
        AttatchPoint ap = attatchpoint.GetComponent<AttatchPoint>();

        System.Collections.Generic.List<AttatchPoint> DesiredPoints = new System.Collections.Generic.List<AttatchPoint>();
        DesiredPoints.Add(ap);

        if (WorkshopGeneral.instance.Mirror)
        {
            System.Collections.Generic.List<AttatchPoint> botAttachPoints = new System.Collections.Generic.List<AttatchPoint>();
            foreach (var aps in ap.transform.GetComponentInParent<Bot_Workshop>().DesignData.AttachPoints.Values) { botAttachPoints.Add(aps); }
            AttatchPoint MirroredPoint = WorkshopGeneral.instance.GetMirroredAttachPoint(ap, botAttachPoints);

            DesiredPoints.Add(MirroredPoint);           
        }

        foreach (var AttachPoint in DesiredPoints) 
        {
            if ((ap.UnacceptedTypes.Contains(WorkshopGeneral.instance.CurrentSelectedComponentToPlace.ComponentDefaultData.Type)))
            {
                continue;
            }
            if(AttachPoint.botComponent != null) { AttachPoint.botComponent.RemoveFromBot(); }

            GameObject newComp = Instantiate(WorkshopGeneral.instance.CurrentSelectedComponentToPlace.ComponentDefaultData.DefaultPrefab, 
                AttachPoint.transform.position, AttachPoint.transform.rotation);

            AttachPoint.AttachNewComponent(newComp.GetComponent<BotComponent>());
        }


       // WorkshopGeneral.instance.SelectMenuComponent(newComp.GetComponent<BotComponent>().CompName);
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

    public void Click(RaycastHit rayhit)
    {
        BotComponent botComponent = rayhit.collider.transform.GetComponentInParent<BotComponent>();
        Transformable transformable = rayhit.collider.gameObject.GetComponent<Transformable>();
        TransformGizmo gizmo = rayhit.collider.gameObject.GetComponentInParent<TransformGizmo>();
        SelectionHighlight newSelected = rayhit.collider.GetComponentInParent<SelectionHighlight>();

        Debug.Log(rayhit.collider.gameObject == null);

        if (rayhit.collider.gameObject.tag == "AttatchPoint")
        {
            Debug.Log("Attempt Attach");
            if (WorkshopGeneral.instance.CurrentSelectedComponentToPlace == null) return;
            ClickAttachPoint(rayhit);
            return;
        }

        if (botComponent != null)
        {
            WorkshopGeneral.instance.SelectBotsComponent(botComponent);
        }

        if (transformable != null)
        {
            transformable.Select();
        }
        else if (rayhit.collider.gameObject.GetComponentInParent<Transformable>() != null)
        {
            rayhit.collider.gameObject.GetComponentInParent<Transformable>().Select();
        }

        if (currentSelectedHighlight != newSelected)
        {
            if(currentSelectedHighlight != null) currentSelectedHighlight.SetSelected(false);
            if(newSelected != null) newSelected.SetSelected(true);
            currentSelectedHighlight = newSelected;
        }

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

