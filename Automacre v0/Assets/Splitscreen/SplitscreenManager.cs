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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Offset = MainDeployCam.transform.position - AltDeployCam.transform.position ;
        GameObject.Instantiate(MapObjects, Offset, Quaternion.identity);
       // AltDeployCam = GameObject.Instantiate(MainDeployCam, MainDeployCam.transform.position+Offset, MainDeployCam.transform.rotation);

    }

    // Update is called once per frame
    void Update()
    {
        AltDeployCam.transform.position = MainDeployCam.transform.position + Offset;
        AltDeployCam.transform.rotation = MainDeployCam.transform.rotation;

        if(CopyBots.Count ==0) return;
        foreach (var pair in CopyBots)
        {
            pair.Value.transform.position = pair.Key.transform.position + Offset;
            pair.Value.transform.rotation = pair.Key.transform.rotation;
        }
        Debug.Log(CopyBots.Count);
    }

    public void CopyBot(BotController bot)
    {
        //GameObject newBot = GameObject.Instantiate(bot, bot.transform.position + Offset, bot.transform.rotation);
        GameObject newBot = GameObject.Instantiate(CopyTemplate, bot.transform.position + Offset, bot.transform.rotation);
        CopyBots.Add(new KeyValuePair<GameObject,GameObject>(bot.transform.GetChild(0).gameObject, newBot));
        CopyBots2.Add(newBot);
        OGBots.Add(bot.gameObject);
    }
}
