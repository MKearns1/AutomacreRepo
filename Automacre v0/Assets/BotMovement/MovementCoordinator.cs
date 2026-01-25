using System.Collections.Generic;
using UnityEngine;

public class MovementCoordinator : MonoBehaviour
{
    public List<ProceduralWalker> ProceduralComponents = new();
    public List<ProceduralWalker> QueueCopy = new();

    public Queue<ProceduralWalker> StepQueue = new();

    public int maxActiveSteps;
    public int curActiveSteps;
    public int actualmovementallowednum;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetAllProceduralComponents();
        maxActiveSteps = 1;

        if (ProceduralComponents.Count > 1)
        {
            maxActiveSteps = ProceduralComponents.Count / 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
      QueueCopy.Clear();
        actualmovementallowednum = 0;
        foreach(var proceduralComponent in StepQueue)
        {
            QueueCopy.Add(proceduralComponent);
            if(proceduralComponent.MovementAllowed) actualmovementallowednum++;
        }


    }

    public void GetAllProceduralComponents()
    {
        ProceduralComponents.Clear();
        foreach (Transform Attachpoint in transform.parent.GetComponentInChildren<BotBodyBase>().transform)
        {
            if (Attachpoint.gameObject.GetComponent<AttatchPoint>() == null) continue;

            if (Attachpoint.gameObject.GetComponent<AttatchPoint>().botComponent == null) continue;

            if (Attachpoint.gameObject.GetComponent<AttatchPoint>().botComponent.GetComponentInChildren<ProceduralWalker>(false) == null) continue;

            ProceduralWalker walker = Attachpoint.gameObject.GetComponent<AttatchPoint>().botComponent.GetComponentInChildren<ProceduralWalker>(false);

            ProceduralComponents.Add(walker);

            //if(StepQueue.Count < maxActiveSteps)
            //StepQueue.Enqueue(walker);
            //walker.MovementAllowed=false;
        }
        //ProceduralComponents[Random.Range(0, ProceduralComponents.Count)].MovementAllowed = true;
        
    }

    public void AllowStep(ProceduralWalker walker)
    {
        walker.HasStepToken = true;
        walker.MovementAllowed = true;
        curActiveSteps++;
    }

    public void CompFinishStep(ProceduralWalker walker)
    {
        curActiveSteps--;
        walker.MovementAllowed = false;
        //StepQueue.Enqueue(walker);

        if(StepQueue.Count > 0 && curActiveSteps < maxActiveSteps)
        {
            AllowStep(StepQueue.Dequeue());
        }
    }

    public void RequestStep(ProceduralWalker walker)
    {
        if (walker.HasStepToken) return;

        if (curActiveSteps < maxActiveSteps && actualmovementallowednum < maxActiveSteps)
        {
            AllowStep(walker);
        }
        else
        {
            if (StepQueue.Contains(walker)) return;
            StepQueue.Enqueue(walker);
        }
    }


}
