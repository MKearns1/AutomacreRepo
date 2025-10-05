using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;

public class ResourceScript : MonoBehaviour, IClickable
{

    public int Quantity, MaxQuantity;
    public float HarvestTime;
    public ResourceType resourceType;
    AudioSource audiosource;
    public AudioClip HarvestSound;
    string HoverText = "Harvest";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        audiosource = transform.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Harvest(BotScript bot) 
    {
        Quantity--;

        foreach (InventoryItem i in bot.Inventory)
        {
            if (i.Resource == resourceType) i.Quantity++;
        }

        //GameObject.Find("Player").GetComponent<InventoryManager>().AddResource(resourceType, 1);
        if (audiosource != null)
        {
            //audiosource.pitch = Random.Range(.8f, 1.2f);
          //  audiosource.Play();
            AudioSource.PlayClipAtPoint(HarvestSound, transform.position);
        }
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
public enum ResourceType
{
    Coal,Wood,Stone,Metal
}