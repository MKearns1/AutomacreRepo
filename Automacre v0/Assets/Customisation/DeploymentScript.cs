using UnityEngine;

public class DeploymentScript : MonoBehaviour
{
    public GameObject BotDeployPrefab;
    public Bot_Workshop WorkshopBot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Deploy()
    {
        GameObject Bot = Instantiate(BotDeployPrefab,Vector3.right*10,Quaternion.identity);


       // BotRuntimeData BotData = new BotRuntimeData();
       // BotData.AttachPoints = WorkshopBot.DesignData.AttachPoints;

        foreach (var ap in WorkshopBot.DesignData.AttachPoints.Keys)
        {
            Debug.Log(ap + WorkshopBot.DesignData.AttachPoints[ap].botComponent);

        }
        Bot.GetComponent<BotController>().AssembleBot(WorkshopBot.DesignData);

        GameObject.FindGameObjectWithTag("PlayerWorkshop").transform.GetChild(0).GetChild(0).GetComponent<Camera>().enabled = false;
        GameObject.FindGameObjectWithTag("PlayerWorkshop").transform.GetComponent<WorkshopMovement>().enabled = false;
        GameObject.FindGameObjectWithTag("PlayerDeploy").transform.GetChild(0).GetComponent<Camera>().enabled = true;
        GameObject.FindFirstObjectByType<SplitscreenManager>().CopyBot(Bot.GetComponent<BotController>());
        Destroy(GameObject.Find("Canvas"));
    }
}
