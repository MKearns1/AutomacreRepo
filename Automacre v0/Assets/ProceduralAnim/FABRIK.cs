using Mono.Cecil;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class FABRIK : IKSolver
{
    public int iterations = 10;
    public Transform TargetTransform;
    public Transform Pole;
    bool usePole;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
         base.Start();
        if (GetComponent<LimbCreator>() != null)
        {
            Pole = GetComponent<LimbCreator>().Pole;
        }
        else
        {
            Pole.position = transform.position + (Joints[Joints.Count - 1].Joint.position - transform.position) / 2 + Vector3.up;
        }

    }

    // Update is called once per frame
    void Update()
    {
       // TargetPoint = GameObject.Find("Target").transform.position;
        TargetPoint = TargetTransform.position;
        solver();
    }

    [ContextMenu ("RunIKSolver")]
    void solver()
    {
        TargetPoint = TargetTransform.position;
        Transform cur = Joints[Joints.Count-1].Joint;
        Vector3 BaseJointPos = Joints[0].Joint.position;
        bool GoingTowardsBase = true;

        float DistanceFromBase = Vector3.Distance(Joints[0].Joint.position, TargetPoint);
        float TotalLength = 0;

        foreach (IKJoint joint in Joints) 
        {
            TotalLength += joint.Length;
        }


        if (DistanceFromBase > TotalLength)
        {
            Vector3 Direction = (TargetPoint - Joints[0].Joint.position).normalized;

            for (int i = 1; i < Joints.Count; i++)
            {
                Joints[i].Joint.position = Joints[i - 1].Joint.position + Direction * Joints[i - 1].Length;
                
            }
            ConfigureSegmentRotations();
        }
        else
        {
            List<Vector3> JointPositions = new List<Vector3>();

            foreach (IKJoint joint in Joints) 
            {
                JointPositions.Add(joint.Joint.position);
            }

            for (int i = 0; i < iterations; i++)
            {
                for(int f = JointPositions.Count-1; f>0; f--)
                {
                    Vector3 curPos = JointPositions[f];
                    Vector3 NextPos = Vector3.zero;
                    if(f ==  JointPositions.Count-1)        // IS THIS THE LEAF NODE?
                    {
                        NextPos = TargetPoint;
                    }
                    else
                    {
                        Vector3 PrevNodePos = JointPositions[f + 1];
                        NextPos = PrevNodePos+ (curPos - PrevNodePos).normalized * Joints[f].Length;
                    }

                    JointPositions[f] = NextPos;
                }
                for (int b = 0;  b < JointPositions.Count; b++)
                {
                    Vector3 curPos = JointPositions[b];
                    Vector3 NextPos = Vector3.zero;
                    if (b - 1 < 0)
                    {
                        NextPos = BaseJointPos;
                    }
                    else
                    {
                        Vector3 PrevNodePos = JointPositions[b - 1];
                        NextPos = PrevNodePos + (curPos - PrevNodePos).normalized * Joints[b-1].Length;
                    }

                    JointPositions[b] = NextPos;
                }
            
            }

            //if (usePole)
            {
                for (int b = 1; b < JointPositions.Count - 1; b++)
                {
                    var plane = new Plane(JointPositions[b + 1] - JointPositions[b - 1], JointPositions[b - 1]);
                    var projectedpole = plane.ClosestPointOnPlane(Pole.position);
                    var projectedBone = plane.ClosestPointOnPlane(JointPositions[b]);
                    var angle = Vector3.SignedAngle(projectedBone - JointPositions[b - 1], projectedpole - JointPositions[b - 1], plane.normal);
                    JointPositions[b] = Quaternion.AngleAxis(angle, plane.normal) * (JointPositions[b] - JointPositions[b - 1]) + JointPositions[b - 1];
                }
            }

            for (int j = 0; j < Joints.Count; j++)
            {
                Joints[j].Joint.position = JointPositions[j];
            }

            ConfigureSegmentRotations();

        }
    }

    Vector3 GetPositionAlong(Vector3 currentPosition, Vector3 TargetPosition, float DistanceFromPoint)
    {
        //1
        Vector3 Location =Vector3.zero;

        Vector3 dir = currentPosition - TargetPosition;
        dir = dir.normalized;

        Vector3 offset = dir*DistanceFromPoint;
        Vector3 Position1 = TargetPosition - offset;
        Location = Position1;


/*        //2
        float DistanceBetweenPoints = Vector3.Distance(currentPosition, TargetPosition);
        float PercentAcross = Mathf.InverseLerp(0, DistanceBetweenPoints, DistanceFromPoint);

        Vector3 Position2 = Vector3.Lerp(TargetPosition, currentPosition, PercentAcross);
        Location = Position2;*/

        return Location;
    }

    void ConfigureSegmentRotations()
    {
        for (int j = 0; j < Joints.Count; j++)
        {
            if (j == Joints.Count - 1) continue;

            Vector3 relativePos = Joints[j + 1].Joint.position - Joints[j].Joint.position;

            Joints[j].Joint.transform.GetChild(1).rotation = Quaternion.LookRotation(relativePos, transform.up);


        }
    }
    private void OnDrawGizmos()
    {
        if(!Application.isPlaying) return;

        for (int i = 1; i < Joints.Count; i++)
        {
            Gizmos.DrawLine(Joints[i - 1].Joint.position, Joints[i].Joint.position);
        }
    }
}


