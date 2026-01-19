using UnityEngine;
using System.Collections.Generic;

public class LimbCreator : MonoBehaviour
{

    public int NumberOfJoints = 2;
    public float Length;
    public float JointSize = .5f;
    public float SegmentSize = .5f;
    public List<Transform> Joints = new List<Transform>();
    public List<Transform> Segments = new List<Transform>();
    public bool HasPole;
    public Transform Pole;
    public GameObject LimbSegmentPrefab;
    public GameObject LimbJointPrefab;

    [ExecuteAlways]
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        CreateJoints();
        CreateJoints();
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
        Joints[Joints.Count - 1].transform.GetChild(0).gameObject.SetActive(false); //removes visual of last joint
/*        if(Pole != null)
        DestroyImmediate(Pole.gameObject);

        if (HasPole)
        {
            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Vector3 MidWayPoint = Vector3.Lerp(Joints[Joints.Count - 1].position, Joints[0].position, 0.5f);
            pole.transform.position = transform.up * 6 + MidWayPoint;
            pole.transform.SetParent(transform, true);
            Pole = pole.transform;
            Pole.name = "Pole";
        }*/

        CreateSegments();
        if (Application.isPlaying)
        {
            GetComponent<FABRIK>().GetAllBones();
        }
    }

    public void CreateBone(Vector3 Position, Transform Parent, string name = "Bone")
    {
        //GameObject newJoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GameObject newJoint = GameObject.Instantiate(LimbJointPrefab);
        newJoint.transform.position = Position;
        newJoint.transform.localScale = Vector3.one * JointSize;
        //newJoint.transform.localScale = Vector3.one;
        newJoint.transform.SetParent(Parent, true);
        newJoint.name = name;
        Joints.Add(newJoint.transform);

    }

    [ContextMenu("Create Segments")]
    public void CreateSegments()
    {
        for (int i = 0; i < Joints.Count-1; i++)
        {
            CreateSegment(Joints[i].transform.position, Joints[i], Joints[i+1], "Segment" + i);
        }
    }

    public void CreateSegment(Vector3 Position, Transform Parent, Transform EndTarget, string name = "Segment")
    {
        float DistanceBetweenJoints = (float)(Length / (float)(NumberOfJoints + 1f));

        GameObject newSegment = GameObject.Instantiate(LimbSegmentPrefab, Parent);
        newSegment.transform.position = Position;

        LimbSegmentPrefab.transform.GetChild(0).transform.localScale = new Vector3(SegmentSize, SegmentSize, DistanceBetweenJoints / JointSize);
        LimbSegmentPrefab.transform.GetChild(0).transform.localPosition = new Vector3(0, 0, DistanceBetweenJoints / 2f / JointSize);

        Vector3 relativePos = EndTarget.position - Position;

        // Joints[j].Joint.transform.GetChild(1).rotation = Quaternion.LookRotation(relativePos, Joints[j].Joint.transform.GetChild(1).forward);
        newSegment.transform.rotation = Quaternion.LookRotation(relativePos, transform.up);

        //newSegment.transform.localScale = Vector3.one * JointSize;
        newSegment.transform.SetParent(Parent, true);
        newSegment.name = name;
        Segments.Add(newSegment.transform);
    }

    public void DestroyOldJoints()
    {
        foreach(Transform t in Joints)
        {
            if(t != null)
            DestroyImmediate(t.gameObject);
        }
        Joints.Clear();

        foreach(Transform t in Segments)
        {
            if (t != null)
                DestroyImmediate(t.gameObject);
        }
        Segments.Clear();

        if(transform.Find("Base") != null)
        {
            DestroyImmediate(transform.Find("Base").gameObject);
        }
        if (transform.Find("Pole") != null)
        {
            DestroyImmediate(transform.Find("Pole").gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
