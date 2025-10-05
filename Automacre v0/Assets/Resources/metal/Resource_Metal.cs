using TMPro;
using UnityEngine;

public class Resource_Metal : ResourceScript
{

    public GameObject ParticleEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        
        for (int i = 0; i < MaxQuantity; i++)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = transform.position + Random.insideUnitSphere;
            cube.transform.localScale = Vector3.one / Random.Range(1, 2);
            cube.transform.rotation = Random.rotation;
            cube.GetComponent<Renderer>().material =
                transform.Find("Visual").Find("Base").GetComponent<Renderer>().material;

            Destroy(cube.GetComponent<Collider>());

            cube.gameObject.transform.SetParent(transform.Find("Visual"), true);
            cube.name = "Coal";
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Find("DebugInfo").Find("Quantity").GetComponent<TextMeshPro>().text = Quantity.ToString();
    }

    public override void Harvest(BotScript bot)
    {

        GameObject BurstEffect = Instantiate(ParticleEffect,transform.position,Quaternion.identity);
        BurstEffect.GetComponent<ParticleEffectScript>().color = new Color(.1f, .1f, .1f);

        Destroy(BurstEffect,1);
        Destroy(transform.Find("Visual").Find("Coal").gameObject);
        base.Harvest(bot);



        if (Quantity <= 0)
        {
            Destroy(this.gameObject);
        }

    }


}
