using TMPro;
using UnityEngine;

public class Resource_Wood : ResourceScript, IClickable
{

    public GameObject ParticleEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //for (int i = 0; i < MaxQuantity; i++)
        //{
        //    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    cube.transform.position = transform.position + Random.insideUnitSphere;
        //   // cube.transform.rotation = Random.rotation;
        //    cube.gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
        //    Destroy(cube.GetComponent<Collider>());

        //    cube.gameObject.transform.SetParent(transform.Find("Visual"), true);
        //    cube.name = "Coal";
        //}

        float randomrot = Random.Range(0, 360);
        transform.rotation = Quaternion.Euler(new Vector3(0, randomrot, 0));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Find("DebugInfo").Find("Quantity").GetComponent<TextMeshPro>().text = Quantity.ToString();
    }

    public override void Harvest(BotScript bot)
    {

        GameObject BurstEffect = Instantiate(ParticleEffect,transform.position,Quaternion.identity);
        BurstEffect.GetComponent<ParticleEffectScript>().color = new Color(.5f, .25f, .25f);
        Destroy(BurstEffect,1);
        base.Harvest(bot);



        if (Quantity <= 0)
        {
            Destroy(this.gameObject);
        }

    }


}
