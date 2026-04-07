using UnityEngine;
using System.Collections.Generic;

public class DeploymentScript : MonoBehaviour
{
    public List<GameObject> BotDeployPrefabs;
    public Bot_Workshop WorkshopBot;
    public GameObject DeployPlayerPrefab;

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
        GameObject Bot = Instantiate(GetBodyTypeByName(WorkshopBot.BodyType),WorkshopBot.transform.position,Quaternion.identity);
        WorkshopBot.gameObject.SetActive(false);

        // BotRuntimeData BotData = new BotRuntimeData();
        // BotData.AttachPoints = WorkshopBot.DesignData.AttachPoints;

        foreach (var ap in WorkshopBot.DesignData.AttachPoints.Keys)
        {
            Debug.Log(ap + WorkshopBot.DesignData.AttachPoints[ap].botComponent);

        }

        Bot.GetComponent<BotController_Procedural>().AssembleBot(WorkshopBot.DesignData);

        Camera WorkshopCam = GameObject.FindGameObjectWithTag("PlayerWorkshop").transform.GetChild(0).GetChild(0).GetComponent<Camera>();WorkshopCam.enabled = false;
        GameObject.FindGameObjectWithTag("PlayerWorkshop").transform.GetComponent<WorkshopMovement>().enabled = false;
        //GameObject.FindGameObjectWithTag("PlayerDeploy").transform.GetChild(0).GetComponent<Camera>().enabled = true;
        Instantiate(DeployPlayerPrefab, GameObject.Find("DeploySpawnPos").transform.position,Quaternion.identity);
        Destroy(FindFirstObjectByType<CanvasManager>().gameObject);
        LevelEventsManager.instance.BotDeployed();

        GameObject.FindFirstObjectByType<SplitscreenManager>().CopyBot(Bot.GetComponent<BotController_Procedural>());
        Destroy(GameObject.Find("Canvas"));

    }

    GameObject GetBodyTypeByName(string name)
    {
        foreach (var body in BotDeployPrefabs)
        {
            if(body.GetComponent<BotController>().BodyType == name) return body;
        }
        return null;
    }

    public void PreviewBot()
    {
        PreviewScript preview = GameObject.Find("PreviewArea").GetComponent<PreviewScript>();
        Vector3 Spawnpos = GameObject.Find("PreviewArea").transform.Find("SpawnPos").transform.position;

        GameObject Bot = Instantiate(GetBodyTypeByName(WorkshopBot.BodyType), Spawnpos, Quaternion.identity);
        Bot.GetComponent<BotController_Procedural>().AssembleBot(WorkshopBot.DesignData);

        preview.StartPreview(Bot.GetComponent<BotController_Procedural>());

        GameObject.Find("PreviewCam").GetComponent<Camera>().enabled = true;
        GameObject.FindGameObjectWithTag("PlayerWorkshop").transform.GetChild(0).GetChild(0).GetComponent<Camera>().enabled = false;

    }
}
