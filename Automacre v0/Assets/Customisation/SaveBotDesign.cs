using System.IO;
using UnityEngine;
using System;

public class SaveBotDesign : MonoBehaviour
{
    public BotDesignData bbl;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BotDesignData lp = new BotDesignData();
        lp.Bodytype = "poo";
        lp.runtimeData = new();
        lp.runtimeData.OffsetFromGround = 123;
        dd(lp);

        bbl = pp(File.ReadAllText("BotDesign.txt"));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            BotDesignData lp = new BotDesignData();
            lp.runtimeData = WorkshopGeneral.instance.BotWorkshopBase.DesignData;
            lp.Bodytype = WorkshopGeneral.instance.BotWorkshopBase.BodyType;
            AttatchPoint val;
            WorkshopGeneral.instance.BotWorkshopBase.DesignData.AttachPoints.TryGetValue("frontRightBottom", out val);
            lp.ComponentDesignInfo = val.botComponent.GetDesignInfo();
            lp.www = (val.botComponent.DesignInfo as WalkerDesignInfo).LimbLength;
            dd(lp);
        }
    }


    public void dd(BotDesignData design)
    {
        Debug.Log("SaveBot");

        string potion = JsonUtility.ToJson(design);
      //  System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveBot.js", potion);
        File.WriteAllText("BotDesign.txt", potion);
    }

    public BotDesignData pp(string gg)
    {
        BotDesignData ll = JsonUtility.FromJson<BotDesignData>(gg);
        return ll;
    }

    [Serializable]
    public class BotDesignData
    {
        public BotRuntimeData runtimeData;
        public string Bodytype;

        [SerializeReference]
        public ComponentDesignInfo ComponentDesignInfo;
        public Vector3 poo;
        public float www;

    }
}
