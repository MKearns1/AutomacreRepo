using UnityEngine;

public abstract class ComponentOptionDetails : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public virtual ComponentOptionDetails Clone() { return null; }
    public abstract ComponentOptionDetails Clone();

}

public class OptionsData
{

}
