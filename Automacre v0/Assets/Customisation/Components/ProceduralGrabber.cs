using UnityEngine;

public class ProceduralGrabber : MonoBehaviour
{
    public Transform RestingPosition;
    public Vector3 RestingPosition2;
    public Transform EndPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<FABRIK>().TargetTransform = RestingPosition;

    }

    // Update is called once per frame
    void Update()
    {
       //GetComponent<FABRIK>().TargetTransform = RestingPosition;
       RestingPosition2 = transform.position + transform.forward;
        EndPoint.position = RestingPosition2;
    }
}
