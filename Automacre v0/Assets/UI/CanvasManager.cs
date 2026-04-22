using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEditor;

public class CanvasManager : MonoBehaviour
{
    RectTransform cursor;

    [SerializeField]public List<CompIcon> compIcons = new List<CompIcon>();

    public GameObject DeployUI;
    public GameObject WorkshopUI;
    public GameObject EndGamePopUp;

    public GameObject CustomTab;
    public GameObject PrebuiltTab;

    public GameObject WorkshopHowToPlay;
    public GameObject DeployHowToPlay;

    public AudioClip Button1Sound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //DeployUI = transform.Find("DeployUI").gameObject;
       // WorkshopUI = transform.Find("WorkshopUI").gameObject;

        EnterMode("Workshop");

        cursor = transform.Find("Cursor").GetComponent<RectTransform>();
        SetWorkshopBotType("Custom");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.Find("Canvas").transform as RectTransform, Input.mousePosition, null, out pos);

        cursor.anchoredPosition = pos;
    }

    public Sprite GetCompIcon(string compName)
    {
        foreach (CompIcon compIcon in compIcons)
        {
            if (compIcon.name == compName)
            {
                return compIcon.image;
            }
        }
        return null;
    }

    public void EnterMode(string mode)
    {
        switch (mode)
        {
            case "Deploy":
                WorkshopUI.SetActive(false); 
                DeployUI.SetActive(true);
                EndGamePopUp.SetActive(false);
                break;

            case "Workshop":
                WorkshopUI.SetActive(true);
                DeployUI.SetActive(false);
                EndGamePopUp.SetActive(false);
                break;

            case "EndGame":
                WorkshopUI.SetActive(false);
                DeployUI.SetActive(false);
                EndGamePopUp.SetActive(true);
                break;
        }
        playButtonSound();

    }

    public void SetWorkshopBotType(string type)
    {
        switch (type)
        {
            case "Custom":
                CustomTab.SetActive(true);
                PrebuiltTab.SetActive(false);
                WorkshopGeneral.instance.PrebuiltMode = false;
                WorkshopGeneral.instance.BotWorkshopBase.gameObject.SetActive(true);
                Destroy(GameObject.FindFirstObjectByType<BotController_Keyframe>().gameObject);

                break;

            case "Prebuilt":
                CustomTab.SetActive(false);
                PrebuiltTab.SetActive(true);
                WorkshopGeneral.instance.PrebuiltMode = true;
                WorkshopGeneral.instance.BotWorkshopBase.gameObject.SetActive(false);
                if(WorkshopGeneral.instance.CurTransformGizmo!=null) Destroy(WorkshopGeneral.instance.CurTransformGizmo);
                break;
        }
        playButtonSound();

    }

    public void SetBodyType(string type)
    {
        WorkshopGeneral.instance.SetBotBody(type);
        playButtonSound();
    }
    public void SetComponent(string compName)
    {
        WorkshopGeneral.instance.SelectMenuComponent(compName);
        playButtonSound();

    }

    public void HowToPlay(string Mode)
    {
        switch (Mode)
        {
            case "Workshop":
                WorkshopHowToPlay.SetActive(!WorkshopHowToPlay.activeInHierarchy);

                break;

            case "Deploy":
                DeployHowToPlay.SetActive(!DeployHowToPlay.activeInHierarchy);

                break;
        }
        playButtonSound();

    }

    public void AnimationSplitScreen()
    {
        if(WorkshopGeneral.instance.PrebuiltMode) return;
        SplitscreenManager splitscreenManager = FindFirstObjectByType<SplitscreenManager>();
        if (splitscreenManager.SplitScreenActive)
        {
            splitscreenManager.EndSplitScreen();

        }
        else
        {
            splitscreenManager.BeginSplitScreen();

        }
        playButtonSound();

    }

    public void BackToWorkshop()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Destroy(GameObject.FindFirstObjectByType<PlayerScript>().CurrentSelectedBots[0].gameObject);

        Destroy(GameObject.FindFirstObjectByType<PlayerScript>().gameObject);
        GameObject.FindGameObjectWithTag("PlayerWorkshop").transform.GetComponent<WorkshopMovement>().enabled = true;
        Camera WorkshopCam = GameObject.FindGameObjectWithTag("PlayerWorkshop").transform.GetChild(0).GetChild(0).GetComponent<Camera>(); 
        WorkshopCam.enabled = true;
        GameObject.FindFirstObjectByType<CanvasManager>().EnterMode("Workshop");
        SplitscreenManager splitscreenManager = FindFirstObjectByType<SplitscreenManager>();
        splitscreenManager.EndSplitScreen();


        if(WorkshopGeneral.instance.PrebuiltMode)
        {
            SetWorkshopBotType("Prebuilt");

        }
        else
        {
            SetWorkshopBotType("Custom");
            GameObject.FindFirstObjectByType<Bot_Workshop>(FindObjectsInactive.Include).gameObject.SetActive(true);

        }
        playButtonSound();
    }

    void playButtonSound()
    {
        SoundManager.instance.PlaySound(Button1Sound);
    }
    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
    }
}

[Serializable]
public class CompIcon
{
    public string name;
    public Sprite image;
}
