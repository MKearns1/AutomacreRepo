using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Interfaces
{
    public static void DrawDebugCircle(Vector3 center, float radius, int segments, Color color, float duration = 0)
    {
        float angleStep = 360f / segments;

        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

            Debug.DrawLine(prevPoint, nextPoint, color, duration);

            prevPoint = nextPoint;
        }
    }

    public static InventoryItem FindItemInList(List<InventoryItem> list, ResourceType itemType)
    {
        foreach (InventoryItem item in list)
        {
            if(item.Resource == itemType)
            {
                return item;

            }

        }
        return null;
    }

    public static RaycastHit CastMouseOverObject(Camera cam)
    {
        Ray Mouseray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Physics.Raycast(Mouseray, out hit);

        return hit;
    }

}

public interface IClickable
{
    void OnClick(Vector3 hitPoint, List<BotScript> AffectedBots);
    void OnHover();
    string GetHoverText();
}

[Serializable]
public class InventoryItem
{
    public ResourceType Resource;
    public int Quantity;

    public InventoryItem(ResourceType resource, int quantity)
    {
        Resource = resource;
        Quantity = quantity;
    }
}
