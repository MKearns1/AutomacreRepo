using System;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralPart : MonoBehaviour
{
    //[Serializable]
    public MotionPlayer motionPlayer = new MotionPlayer();
    public Queue<System.Action> Actions = new Queue<System.Action>();
    public Transform EndPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
       // motionPlayer.update2(EndPoint);  
    }

}


