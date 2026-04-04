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
        if(LinkedTransform == null)Destroy(gameObject);
        LinkedTransform.position = transform.position;
    }

    public void Initialise(Transform linkedT, Transformable transformable = null)
    {
        LinkedTransform = linkedT;
        transform.position = LinkedTransform.position;
        
        if(transformable==null)return;

        if (!transformable.hasX)
        {
            Destroy(X);
        }
        if (!transformable.hasY)
        {
            Destroy(Y);
        }
        if (!transformable.hasZ)
        {
            Destroy(Z);
        }
    }

    public void IncrementMove(Vector3 MoveAmount)
    {
        transform.position += MoveAmount;
    }
}
