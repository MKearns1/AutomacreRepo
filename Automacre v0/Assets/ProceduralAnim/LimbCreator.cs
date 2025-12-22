using UnityEngine;
using System.Collections.Generic;

public class LimbCreator : MonoBehaviour
{

    public int NumberOfJoints = 2;
    public float Length;
    public float JointSize = .5f;
    public List<Transform> Joints = new List<Transform>();
    public bool HasPole;
    public Transform Pole;

    [ExecuteAlways]
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
       
    }
    [ContextMenu("Create")]

    public void CreateJoints()
    {
        DestroyOldJoints();

        CreateBone(transform.position, transform, "Base");

        float DistanceBetweenJoints = (float)(Length / (float)(NumberOfJoints+1f));

        for (int i = 1; i < NumberOfJoints+1; i++)
        {
             CreateBone(transform.position + transform.forward * i * DistanceBetweenJoints, Joints[i-1], "Bone" + i.ToString());        
        }
        //Joints.Add(end.transform);
        CreateBone(transform.position + transform.forward * Length, Joints[Joints.Count - 1], "End");

        if(Pole != null)
        DestroyImmediate(Pole.gameObject);

        if (HasPole)
        {
            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Vector3 MidWayPoint = Vector3.Lerp(Joints[Joints.Count - 1].position, Joints[0].position, 0.5f);
            pole.transform.position = transform.up * 6 + MidWayPoint;
            pole.transform.SetParent(transform, true);
            Pole = pole.transform;
            Pole.name = "Pole";
        }
    }

    public void CreateBone(Vector3 Position, Transform Parent, string name = "Bone")
    {
        GameObject newJoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        newJoint.transform.position = Position;
        newJoint.transform.localScale = Vector3.one * JointSize;
        newJoint.transform.SetParent(Parent, true);
        newJoint.name = name;
        Joints.Add(newJoint.transform);

    }

    public void DestroyOldJoints()
    {
        foreach(Transform t in Joints)
        {
            if(t != null)
            DestroyImmediate(t.gameObject);
        }
        Joints.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
