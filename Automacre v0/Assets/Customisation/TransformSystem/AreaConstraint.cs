using UnityEngine;

public class AreaConstraint : MonoBehaviour, IGizmoConstraint
{
    public Vector3 Origin;
    public float MaxRadius = 4f;
    public float MaxHeight = 10f;

    public bool IsValidPosition(Vector3 position)
    {

        MeshCollider collider = GameObject.Find("WorkshopBounds").GetComponent<MeshCollider>();

        Vector3 closest =
        collider.ClosestPoint(position);

        float tolerance = 0.001f;

        return Vector3.Distance(closest, position)
               <= tolerance;

        if (Vector3.Distance(Origin, position) >= MaxRadius)
        {

        }

        return Vector3.Distance(Origin, position) <= MaxRadius;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
