using UnityEngine;

public class LevelEventsManager : MonoBehaviour
{
    public static LevelEventsManager instance;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BotDeployed()
    {
        GameObject.Find("GarageDoor").GetComponentInChildren<Animation>().Play();

    }
}
