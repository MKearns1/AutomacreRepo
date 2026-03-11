using UnityEngine;

public class TransformGizmo : MonoBehaviour
{
    public Transform LinkedTransform;
    public GameObject X;
    public GameObject Y;
    public GameObject Z;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LinkedTransform.position = transform.position;
    }

    public void Initialise(Transform linkedT)
    {
        LinkedTransform = linkedT;
        transform.position = LinkedTransform.position;
    }
}
