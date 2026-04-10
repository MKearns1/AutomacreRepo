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

                break;

            case "Prebuilt":
                CustomTab.SetActive(false);
                PrebuiltTab.SetActive(true);
                WorkshopGeneral.instance.PrebuiltMode = true;
                WorkshopGeneral.instance.BotWorkshopBase.gameObject.SetActive(false);
                break;
        }
    }

    public void BackToWorkshop()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
