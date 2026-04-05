using UnityEngine;

public class RadiusConstraint : MonoBehaviour, IGizmoConstraint
{
    public Transform Origin;
    public float MaxDistance = 4f;

    public bool IsValidPosition(Vector3 position)
    {
        return Vector3.Distance(Origin.position, position) <= MaxDistance;
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
