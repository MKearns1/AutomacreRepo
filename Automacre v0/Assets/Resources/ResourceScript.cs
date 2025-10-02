using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceScript : MonoBehaviour, IClickable
{

    public int Quantity, MaxQuantity;
    public ResourceType resourceType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Harvest(BotScript bot) 
    {
        Quantity--;
        GameObject.Find("Player").GetComponent<InventoryManager>().AddResource(resourceType, 1);


    }

    public void OnClick(Vector3 hitPoint, List<BotScript> AffectedBots)
    {

        BotDirection MoveTodirection = new BotDirection(DirectType.Move, hitPoint, gameObject);
        BotDirection Harvestdirection = new BotDirection(DirectType.Harvest, hitPoint, gameObject);

        foreach (var bot in AffectedBots)
        {
          //  bot.GiveDirection(MoveTodirection);
            bot.GiveDirection(Harvestdirection);
        }
        Debug.Log("asdfasdfasdfasdfasdf");
       // throw new System.NotImplementedException();

    }
}
public enum ResourceType
{
    Coal,Wood,Stone,Metal
}