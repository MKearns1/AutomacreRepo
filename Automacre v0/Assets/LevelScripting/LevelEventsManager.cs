using TMPro;
using UnityEngine;

public class LevelEventsManager : MonoBehaviour
{
    public static LevelEventsManager instance;

    public int collectedCollectables=0;
    public int CompletionCollectables=10;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Fade f = Instantiate(VFXManager.instance.Fade, GameObject.FindFirstObjectByType<CanvasManager>().transform).GetComponent<Fade>();
        f.SetDirection(-1, Color.black);
        Destroy(f.gameObject,f.FadeLength);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BotDeployed()
    {
        GameObject.FindFirstObjectByType<CanvasManager>().EnterMode("Deploy");


        GameObject.Find("GarageDoor").GetComponentInChildren<Animation>().Play();

    }

    public void CollectedCollectable(int value)
    {
        collectedCollectables+=value;

        GameObject.FindFirstObjectByType<CanvasManager>().transform.Find("DeployUI")
            .transform.Find("Collectables")
            .transform.Find("ValueText").GetComponent<TextMeshProUGUI>().text = collectedCollectables.ToString();

        if(collectedCollectables >= CompletionCollectables)
        {
            EndDeployMode();
        }
    }

    public void EndDeployMode()
    {
        GameObject.FindFirstObjectByType<CanvasManager>().EnterMode("EndGame");
        GameObject.FindFirstObjectByType<CanvasManager>().EndGamePopUp.transform.Find("Collectables").
            Find("ValueText").GetComponent<TextMeshProUGUI>().text = collectedCollectables.ToString();
    }


}
