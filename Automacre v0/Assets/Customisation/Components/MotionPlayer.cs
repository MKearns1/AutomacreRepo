using System;
using System.Net;
using System.Xml.Linq;
using UnityEngine;

[Serializable]
public class MotionPlayer
{
    public float time;
    public Vector3 start;
    public Vector3 end;
    public MovementMotion GeneralMotion;
    public bool isPlaying => time < GeneralMotion.duration;
    public bool Completed = false;
    public float playSpeed;

    Transform MovePart;
    Transform to;
    public Action OnComplete;

    public void Play(Vector3 from, Vector3 to, MovementMotion motion, float Speed = 1)
    {
        start = from;
        end = to;
        GeneralMotion = motion;
        time = 0;
        playSpeed = Speed;
        Completed = false;
    }
    public void Play2(Transform Part, Transform to, MovementMotion motion, float Speed = 1, Action onCompleted = null)
    {
        MovePart = Part;
        start = Part.position;
        end = to.position;
        this.to = to;        
        GeneralMotion = motion;
        time = 0;
        playSpeed = Speed;
        Completed = false;
        OnComplete = onCompleted;
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

        time += Time.deltaTime * playSpeed;
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

    public void update2(Transform Part)
    {
        if (Completed) return;
        if(GeneralMotion.duration <= 0) return;
        //Debug.Log("UPDATE");

        time += Time.deltaTime;
        float t = Mathf.Clamp01(time / GeneralMotion.duration);
        // t = Mathf.SmoothStep(0, 1, t);

        if (GeneralMotion.animCurve.length > 0)
            t = GeneralMotion.animCurve.Evaluate(t);

        Vector3 midPoint = Vector3.Lerp(start, end, .5f);
        Vector3 HeightPoint = midPoint + Vector3.up * GeneralMotion.arcHeight;

        Vector3 pos;// = Vector3.Lerp(start, to.position, t);


        float height = 4f * GeneralMotion.arcHeight * t * (1 - t);

        Vector3 m1 = Vector3.Lerp(start, HeightPoint, t);
        Vector3 m2 = Vector3.Lerp(HeightPoint, to.position, t);
        pos = Vector3.Lerp(m1, m2, t);

       // pos.y = height;

        Part.position = pos;

        Vector3 Direction = new Vector3(to.position.x, 0, to.position.z) - new Vector3(start.x, 0, start.z);
        Direction = Vector3.Normalize(Direction);

        var rotation = Quaternion.LookRotation(Direction);
        Part.rotation = Quaternion.Lerp(Part.rotation, rotation, t);
        //Part.rotation = NormalRotation(t);

        if(time > GeneralMotion.duration)
        {
            Completed = true;
            OnComplete?.Invoke();
            OnComplete = null;
        }

        return;
    }

    public void Cancel()
    {
        time = GeneralMotion.duration;
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
