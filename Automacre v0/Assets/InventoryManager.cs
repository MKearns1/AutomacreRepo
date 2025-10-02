using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddResource(ResourceType resource, int Amount)
    {
        CurrentInventory[resource] += Amount;


        switch (resource)
        {
            case ResourceType.Coal:
                
                GameObject.Find("ResourceCounts").transform.Find("Coal").transform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = CurrentInventory[ResourceType.Coal].ToString();

                break;

            case ResourceType.Wood:

                GameObject.Find("ResourceCounts").transform.Find("Wood").transform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = CurrentInventory[ResourceType.Wood].ToString();

                break;
            case ResourceType.Metal:

                GameObject.Find("ResourceCounts").transform.Find("Metal").transform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = CurrentInventory[ResourceType.Metal].ToString();

                break;
        }

    }

    Dictionary<ResourceType, int> CurrentInventory = new Dictionary<ResourceType, int>()
    {
        { ResourceType.Coal, 0 },
        { ResourceType.Wood, 0 },
        { ResourceType.Stone, 0 },
        { ResourceType.Metal, 0 }
    };

}
