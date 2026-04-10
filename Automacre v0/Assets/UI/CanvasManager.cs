using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class CanvasManager : MonoBehaviour
{
    RectTransform cursor;

    [SerializeField]public List<CompIcon> compIcons = new List<CompIcon>();

    public GameObject DeployUI;
    public GameObject WorkshopUI;
    public GameObject EndGamePopUp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //DeployUI = transform.Find("DeployUI").gameObject;
       // WorkshopUI = transform.Find("WorkshopUI").gameObject;

        EnterMode("Workshop");

        cursor = transform.Find("Cursor").GetComponent<RectTransform>();

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

}

[Serializable]
public class CompIcon
{
    public string name;
    public Sprite image;
}
