using UnityEngine;

public class TransformGizmo : MonoBehaviour
{
    public Transform LinkedTransform;
    public Transformable LinkedTransformable;

    public GameObject X;
    public GameObject Y;
    public GameObject Z;
    public Vector3 Bounds = new Vector3(4, 5, 4);
    Vector3 MaxBound;
    Vector3 MinBound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MaxBound = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(LinkedTransform == null)Destroy(gameObject);
        LinkedTransform.position = transform.position;
    }

    public void Initialise(Transform linkedT, Transformable transformable = null, Vector3 startPos = default, Vector3 Bounds = default)
    {
        LinkedTransform = linkedT;
        transform.position = LinkedTransform.position;
        
        if(transformable==null)return;

        LinkedTransformable = transformable;

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

        this.Bounds = Bounds;
        MaxBound = startPos + Bounds;
        MinBound = startPos - Bounds;
    }

    public void IncrementMove(Vector3 MoveAmount)
    {
        //if (!validMove(MoveAmount, 0.5f)) return;

        Vector3 newPos = transform.position + MoveAmount;

        if(LinkedTransformable != null)
        {
            if (!LinkedTransformable.ValidPosition(newPos)) return;
        }

        transform.position += MoveAmount;
    }

    public bool validMove(Vector3 Dir, float MinDist = 0)
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Dir, out hit, float.MaxValue, LayerMask.GetMask("Ground")))
        {
            if(hit.distance < MinDist)
            return false;
        }

        Vector3 newPos = transform.position + Dir;

        if(newPos.x > MaxBound.x || newPos.y > MaxBound.y || newPos.z > MaxBound.z)return false;
        if(newPos.x < MinBound.x || newPos.y < MinBound.y || newPos.z < MinBound.z)return false;

        return true;
    }
}
