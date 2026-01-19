using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;

public class IKSolver : MonoBehaviour
{
    public List<Transform> Bones = new List<Transform>();
    public List<IKJoint> Joints = new List<IKJoint>();
    public List<IKJoint> DefaultJoints = new List<IKJoint>();
    protected Vector3 TargetPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        GetAllBones();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("GetBones")]
    public void GetAllBones()
    {
        Joints.Clear();
        /*bool remainingchildren = true;
        Transform current = transform;

        while (remainingchildren)
        {
            Transform nextJoint = GetNextJoint(current);

            if (nextJoint == null){ remainingchildren = false;break; }

            float distToNextJoint = 0f;
            distToNextJoint = Vector3.Distance(current.position, nextJoint.position);
            IKJoint newJoint = new IKJoint(nextJoint, distToNextJoint);
            Joints.Add(newJoint);
            current = nextJoint;
            *//*

            if (current.childCount > 0)
            {
                //Bones.Add(current.GetChild(0));

                float distToNextJoint = 0f;

                Transform nextJoint = GetNextJoint(current);

                if (nextJoint == null) remainingchildren=false;

                //if (GetNextJoint(nextJoint) == null) remainingchildren=false;
                
                   // Transform nextjoint = current.GetChild(0).GetChild(0);
                distToNextJoint = Vector3.Distance(current.position, nextJoint.position);
               
                IKJoint newJoint = new IKJoint(current, distToNextJoint);
                Joints.Add(newJoint);
                current = nextJoint;
            }
            else
            {
                remainingchildren = false;
            }*//*
        }*/

        Transform current = transform;

        while (true)
        {
            Transform next = GetNextJoint(current);
            if (next == null) break;

            float length = Vector3.Distance(current.position, next.position);
            Joints.Add(new IKJoint(current, length ));

            current = next;
        }

        // add end effector with length 0
        Joints.Add(new IKJoint(current, 0));



        DefaultJoints = Joints;
    }

    public Transform GetNextJoint(Transform current)
    {
        foreach(Transform child in current)
        {
            if(child.GetComponent<LimbJoint>()!= null)return child;
        }
        return null;
    }

    public Transform GetChildSegment(Transform Joint)
    {
        foreach (Transform child in Joint)
        {
            if (child.GetComponent<LimbSegment>() != null) return child;
        }
        return null;
    }
}

[Serializable]
public class IKJoint
{
    public Transform Joint;
    public float Length;

    public IKJoint(Transform t, float l)
    {
        Joint = t;
        Length = l;
    }
}
