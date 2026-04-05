using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class CanvasManager : MonoBehaviour
{
    RectTransform cursor;

    [SerializeField]public List<CompIcon> compIcons = new List<CompIcon>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
}

[Serializable]
public class CompIcon
{
    public string name;
    public Sprite image;
}
