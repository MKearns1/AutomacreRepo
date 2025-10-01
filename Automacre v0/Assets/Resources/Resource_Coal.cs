using TMPro;
using UnityEngine;

public class Resource_Coal : ResourceScript
{

    public GameObject ParticleEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Find("DebugInfo").Find("Quantity").GetComponent<TextMeshPro>().text = Quantity.ToString();
    }

    public override void Harvest(BotScript bot)
    {
        Instantiate(ParticleEffect,transform.position,Quaternion.identity);
        base.Harvest(bot);
    }
}
