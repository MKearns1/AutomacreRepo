using UnityEngine;

public class PreviewScript : MonoBehaviour
{
    public Camera cam;
    Vector3 DefaultCamPos;
    Vector3 CamOffset = new Vector3(7,7,7);
    BotController bc;
    bool Previewing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DefaultCamPos = cam.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Previewing) return;
        if (bc == null) return;

        cam.transform.position = Vector3.Lerp(bc.Ai.transform.position, bc.transform.Find("Base").transform.position, 0.5f) + CamOffset;

        if (Vector3.Distance(bc.Ai.transform.position, transform.Find("EndPoint").transform.position) < 5)
        {
            EndPreview();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndPreview();
        }
    }

    public void StartPreview(BotController botController) 
    { 
        Previewing = true;
        bc = botController;
        //Vector3 Middle = Vector3.Lerp(bc.Ai.transform.position, bc.transform.Find("Base").transform.position, 0.5f);bc.GetComponentInChildren<BotBodyBase>().DesiredOffsetFromGround
       // cam.transform.position = new Vector3(cam.transform.position.x,Middle.y, cam.transform.position.z);
        //cam.transform.SetParent(botController.Ai.transform);
        botController.Ai.NavAgent.SetDestination(transform.Find("EndPoint").transform.position);
    }

    public void EndPreview()
    {
        cam.transform.SetParent(transform);
        cam.transform.position = DefaultCamPos;
        Destroy(bc.gameObject);

        GameObject.FindGameObjectWithTag("PlayerWorkshop").transform.GetChild(0).GetChild(0).GetComponent<Camera>().enabled = true;
        cam.enabled =false;
        Previewing=false;
    }
}
