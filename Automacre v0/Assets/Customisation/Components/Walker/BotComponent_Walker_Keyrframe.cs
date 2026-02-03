using UnityEngine;

public class BotComponent_Walker_Keyrframe : BotComponent
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnAttached()
    {
        if(transform.parent.localPosition.x > 0)
        {
            transform.localScale = new Vector3(-1* transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        
    }
}
