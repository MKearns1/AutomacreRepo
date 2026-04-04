using UnityEngine;

public class PreviewScript : MonoBehaviour
{
    public Camera cam;
    Vector3 DefaultCamPos;
    BotController bc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DefaultCamPos = cam.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (bc == null) return;
        if(Vector3.Distance(bc.Ai.transform.position, transform.Find("EndPoint").transform.position) < 5)
        {
            EndPreview();
        }
    }

    public void StartPreview(BotController botController) 
    { 
        bc = botController;
        cam.transform.SetParent(botController.Ai.transform);
        botController.Ai.NavAgent.SetDestination(transform.Find("EndPoint").transform.position);
    }

    public void EndPreview()
    {
        cam.transform.SetParent(transform);
        cam.transform.position = DefaultCamPos;
        Destroy(bc.gameObject);

        GameObject.FindGameObjectWithTag("PlayerWorkshop").transform.GetChild(0).GetChild(0).GetComponent<Camera>().enabled = true;
        cam.enabled =false;
    }
}
