using UnityEngine;

public class parenttest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.AddComponent<childtest>();



            GameObject g2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g2.AddComponent<childtest>();

            g.GetComponent<childtest>().FF(this);
            g2.GetComponent<childtest>().FF(g.GetComponent<childtest>());

        }
        ;
    }

    public virtual void FF(parenttest parent)
    {
        Debug.Log(this.GetType());
        Debug.Log("this is " + (this.GetType() == parent.GetType()).ToString());
        this.GetType();
    }

}
