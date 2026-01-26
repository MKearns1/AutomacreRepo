using UnityEngine;

public class WorkshopMovement : MonoBehaviour
{
    Transform CamXRoot;
    Transform CamYRoot;
    Camera MainCam;
    float pitch = 0;

    float ZoomSpeed = .2f;

    

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

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit rayhit = Interfaces.CastMouseOverObject(MainCam);

            if (rayhit.collider == null) return;

            Debug.Log(rayhit.collider.gameObject);

            if(rayhit.collider.gameObject.tag == "AttatchPoint")
            {
                if(WorkshopGeneral.instance.CurrentSelectedComponentToPlace == null) return ;
                ClickAttachPoint(rayhit);
            }

            if(rayhit.collider.transform.GetComponentInParent<BotComponent>() != null)
            {
                WorkshopGeneral.instance.SelectBotsComponent(rayhit.collider.transform.GetComponentInParent<BotComponent>());
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
}
