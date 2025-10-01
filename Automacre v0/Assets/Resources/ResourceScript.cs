using UnityEngine;

public class ResourceScript : MonoBehaviour
{

    public int Quantity, MaxQuantity;

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


    }



}
