using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

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
        GameObject Bot = null;

        if (WorkshopGeneral.instance.PrebuiltMode)
        {
            // WorkshopGeneral.instance.DeployPrebuilt("Boxy1");
            Bot = WorkshopGeneral.instance.curPrebuilt;
        }
        else
        {
            Bot = Instantiate(GetBodyTypeByName(WorkshopBot.BodyType), WorkshopBot.transform.position, Quaternion.identity);
            WorkshopBot.gameObject.SetActive(false);

            foreach (var ap in WorkshopBot.DesignData.AttachPoints.Keys)
            {
                Debug.LogWarning("THIS " + ap + WorkshopBot.DesignData.AttachPoints[ap].botComponent);
            }

            Bot.GetComponent<BotController_Procedural>().AssembleBot(WorkshopBot.DesignData);
        }


        Camera WorkshopCam = GameObject.FindGameObjectWithTag("PlayerWorkshop").transform.GetChild(0).GetChild(0).GetComponent<Camera>();WorkshopCam.enabled = false;
        GameObject.FindGameObjectWithTag("PlayerWorkshop").transform.GetComponent<WorkshopMovement>().enabled = false;
        PlayerScript deployplayer = Instantiate(DeployPlayerPrefab, GameObject.Find("DeploySpawnPos").transform.position,Quaternion.identity).GetComponent<PlayerScript>();
        deployplayer.CurrentSelectedBots.Add(Bot.GetComponent<BotController>());

        Destroy(WorkshopGeneral.instance.CurTransformGizmo);

        LevelEventsManager.instance.BotDeployed();

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

        GameObject Bot = null;

        if (WorkshopGeneral.instance.PrebuiltMode)
        {
            // WorkshopGeneral.instance.DeployPrebuilt("Boxy1");
            //Bot = WorkshopGeneral.instance.curPrebuilt;
            Bot = Instantiate(WorkshopGeneral.instance.curPrebuilt, Spawnpos, Quaternion.identity);
        }
        else
        {
            Bot = Instantiate(GetBodyTypeByName(WorkshopBot.BodyType), Spawnpos, Quaternion.identity);
            Bot.GetComponent<BotController_Procedural>().AssembleBot(WorkshopBot.DesignData);
        }

        preview.StartPreview(Bot.GetComponent<BotController_Procedural>());

        GameObject.Find("PreviewCam").GetComponent<Camera>().enabled = true;
        GameObject.FindGameObjectWithTag("PlayerWorkshop").transform.GetChild(0).GetChild(0).GetComponent<Camera>().enabled = false;

    }
}
