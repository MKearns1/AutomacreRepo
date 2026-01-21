using System;
using System.Net;
using UnityEngine;

[Serializable]
public class MotionPlayer
{
    public float time;
    public Vector3 start;
    public Vector3 end;
    public MovementMotion GeneralMotion;
    public bool isPlaying => time < GeneralMotion.duration;

    public void Play(Vector3 from, Vector3 to, MovementMotion motion)
    {
        start = from;
        end = to;
        GeneralMotion = motion;
        time = 0;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void update(Transform Part)
    {
        if(!isPlaying)return;
        Debug.Log("UPDATE");

        time += Time.deltaTime;
        float t = Mathf.Clamp01(time / GeneralMotion.duration);
       // t = Mathf.SmoothStep(0, 1, t);

        if (GeneralMotion.animCurve.length > 0)
            t = GeneralMotion.animCurve.Evaluate(t);

        Vector3 pos = Vector3.Lerp(start, end, t);

        float height = 4f * GeneralMotion.arcHeight * t * (1 - t);

        pos.y += height;

        Part.position = pos;

        return;
/*
        float Fulldist = Vector3.Distance(PrevPos, EndPos);
        float curdist = Vector3.Distance(EndPoint.position, EndPos);

        float maxJumpHeight = 1;
        float height = 4 * maxJumpHeight * t * (1 - t);
        flatPos.y += height;
        EndPoint.position = flatPos;

        Vector3 Direction = new Vector3(MoveToPos.x, 0, MoveToPos.z) - new Vector3(PrevPos.x, 0, PrevPos.z);
        Direction = Vector3.Normalize(Direction);

        var rotation = Quaternion.LookRotation(Direction);
        EndPoint.rotation = Quaternion.Lerp(EndPoint.rotation, rotation, moveProgress);
        EndPoint.rotation = NormalRotation(t);*/
    }
}
[Serializable]
public class MovementMotion
{
    public float duration;
    public AnimationCurve animCurve;
    public float arcHeight;

    public MovementMotion(float duration = 1, AnimationCurve animCurve = null)
    {
        this.duration = duration;
        this.animCurve = animCurve;
    }
}
