using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StorageScript : MonoBehaviour, IClickable
{
    public List<InventoryItem> StoredItems = new List<InventoryItem>();
    public int StoredAmount, MaxCapacity;
    string HoverText = "Storage";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StoredItems.Clear(); // just in case
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            StoredItems.Add(new InventoryItem(type, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick(Vector3 hitPoint, List<BotScript> AffectedBots)
    {
        BotDirection MoveTodirection = new BotDirection(DirectType.Move, hitPoint, gameObject);
        BotDirection DepositDirection = new BotDirection(DirectType.Deposit, hitPoint, gameObject);

        foreach (var bot in AffectedBots)
        {
            //  bot.GiveDirection(MoveTodirection);
            bot.GiveDirection(DepositDirection);
        }

        //throw new System.NotImplementedException();
    }

    public void DepositItem(InventoryItem Item, int DepositAmount, BotScript bot)
    {

        if (StoredAmount + DepositAmount <= MaxCapacity)
        {
            InventoryItem StoredItem = Interfaces.FindItemInList(StoredItems, Item.Resource);
            InventoryItem botItem = Interfaces.FindItemInList(bot.Inventory, Item.Resource);

            StoredItem.Quantity += DepositAmount;
            StoredAmount += DepositAmount;
            botItem.Quantity -= DepositAmount;

            GameObject.Find("Player").GetComponent<InventoryManager>().AddResource(Item.Resource, DepositAmount);

        }
        SetFullness();
    }

    public void SetFullness()
    {
        GameObject FullnessVisual = transform.Find("Visual").Find("Fullness").gameObject;

        float FullnessPercent = (float)StoredAmount/(float)MaxCapacity;

        float Height = Mathf.Lerp(-.5f,.9f,FullnessPercent);

        Vector3 NewPos = FullnessVisual.transform.localPosition;
        NewPos.y = Height;

        FullnessVisual.transform.localPosition = NewPos;

        transform.Find("Debug").Find("FullnessTxt").GetComponent<TextMeshPro>().text = (FullnessPercent*100).ToString() + "%";

    }

    public void OnHover()
    {
        throw new System.NotImplementedException();
    }

    public string GetHoverText()
    {
        return HoverText;
        throw new System.NotImplementedException();
    }
}
