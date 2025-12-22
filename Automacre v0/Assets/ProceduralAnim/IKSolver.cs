using UnityEngine;
using System.Collections.Generic;
using System;

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
        bool remainingchildren = true;
        Transform current = transform;

        while (remainingchildren)
        {
            if (current.childCount > 0)
            {
                //Bones.Add(current.GetChild(0));

                float distToNextJoint = 0f;

                if (current.GetChild(0).childCount > 0)
                {
                    Transform nextjoint = current.GetChild(0).GetChild(0);
                    distToNextJoint = Vector3.Distance(current.GetChild(0).position, nextjoint.position);
                }

                IKJoint newJoint = new IKJoint(current.GetChild(0), distToNextJoint);
                Joints.Add(newJoint);
                current = current.GetChild(0);
            }
            else
            {
                remainingchildren = false;
            }
        }
        DefaultJoints = Joints;
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
