using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class SplitscreenManager : MonoBehaviour
{
    public Camera MainDeployCam;
    public Camera AltDeployCam;
    public Vector3 Offset;
    public GameObject MapObjects;
    public GameObject CopyTemplate;

    public List<KeyValuePair<GameObject,GameObject>> CopyBots = new();
    public List<GameObject> OGBots = new();
    public List<GameObject> CopyBots2 = new();

    public BotController MainBot;
    public BotController AltBot;

    public bool SplitScreenActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!SplitScreenActive)return;
        AltDeployCam.transform.position = MainDeployCam.transform.position + Offset;
        AltDeployCam.transform.rotation = MainDeployCam.transform.rotation;

       // Vector3 pos = MainBot.Ai.transform.position;
       // pos.y = AltBot.Ai.transform.position.y;
        //AltBot.Ai.transform.position = pos + Offset;
        AltBot.Ai.NavAgent.destination = MainBot.Ai.NavAgent.destination + Offset;

        return;
        if(CopyBots.Count ==0) return;
        foreach (var pair in CopyBots)
        {
            pair.Value.transform.position = pair.Key.transform.position + Offset;
            pair.Value.transform.rotation = pair.Key.transform.rotation;
        }
        Debug.Log(CopyBots.Count);
    }

    public void CopyBot(BotController_Procedural bot)
    {
        //GameObject newBot = GameObject.Instantiate(bot, bot.transform.position + Offset, bot.transform.rotation);
        GameObject newBot = GameObject.Instantiate(CopyTemplate, bot.transform.position + Offset, bot.transform.rotation);
        CopyBots.Add(new KeyValuePair<GameObject,GameObject>(bot.transform.GetChild(0).gameObject, newBot));
        CopyBots2.Add(newBot);
        OGBots.Add(bot.gameObject);
    }


    public void BeginSplitScreen()
    {
        SplitScreenActive = true;
        MainDeployCam = GameObject.FindWithTag("PlayerDeploy").transform.Find("SplitCam").transform.GetComponent<Camera>();
        AltDeployCam = GameObject.FindWithTag("SplitScreenCam").transform.GetComponent<Camera>();
        GameObject.FindWithTag("PlayerDeploy").transform.Find("Main Camera").transform.GetComponent<Camera>().enabled = false;
        MainDeployCam.enabled = true;
        AltDeployCam.enabled = true;

        GameObject ActiveBot = GameObject.FindFirstObjectByType<PlayerScript>().CurrentSelectedBots[0].gameObject;
        string bottype = GameObject.FindFirstObjectByType<PlayerScript>().CurrentSelectedBots[0].BodyType;
        AltBot = Instantiate(WorkshopGeneral.instance.getPrebuiltByName(bottype), ActiveBot.transform.GetChild(0).transform.position + Offset, Quaternion.identity).GetComponent<BotController>();
        MainBot = ActiveBot.GetComponent<BotController>();
    }

    public void EndSplitScreen()
    {
        SplitScreenActive = false;
        MainDeployCam = GameObject.FindWithTag("PlayerDeploy").transform.Find("SplitCam").transform.GetComponent<Camera>();
        AltDeployCam = GameObject.FindWithTag("SplitScreenCam").transform.GetComponent<Camera>();
        GameObject.FindWithTag("PlayerDeploy").transform.Find("Main Camera").transform.GetComponent<Camera>().enabled = true;
        MainDeployCam.enabled = false;
        AltDeployCam.enabled = false;

        Destroy(AltBot.gameObject);
    }
}
