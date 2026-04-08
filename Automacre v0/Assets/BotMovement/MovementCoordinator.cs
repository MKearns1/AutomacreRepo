using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovementCoordinator : MonoBehaviour
{
    public List<ProceduralWalker> ProceduralComponents = new();
    public List<ProceduralWalker> QueueCopy = new();

    public Queue<ProceduralWalker> StepQueue = new();

    public List<MovementGroup> movementGroups = new();

    public int maxActiveSteps;
    public int curActiveSteps;
    public int actualmovementallowednum;
    public float StepSpeed = 2;

    public float GaitFrequency = 1.5f;
    public float GaitClock;

    public List<ProceduralWalker> Walkers = new();

    public bool UseUnorderedMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetAllProceduralComponents();
        maxActiveSteps = 1;

        if (ProceduralComponents.Count > 1)
        {
            maxActiveSteps = ProceduralComponents.Count / 2;
        }

        StepSpeed = 
            Math.Max(2, movementGroups.Count * 0.75f);

        //UseUnorderedMovement = true;
        //Mathf.Lerp(1.5f, 4, )
    }

    // Update is called once per frame
    void Update()
    {
        if (UseUnorderedMovement)
        {
            foreach (var group in movementGroups)
            {
                group.Tick2(GaitClock, this);
            }
            return;
        }

        GaitClock = (GaitClock + Time.deltaTime * GaitFrequency) % 1f;

        foreach (var group in movementGroups)
        {
            group.Tick(GaitClock, this);
        }

        

        return;
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
        Walkers = ProceduralComponents;
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

        MovementGroup group = getGroupFor(walker);

        if (group != null)
        {
            foreach (var w in group.Walkers)
            {
                if (walker.HasStepToken) return;
                AllowStep(walker);

            }
        }
        return;
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

    public void POO(ProceduralWalker walker)
    {
        MovementGroup group = getGroupFor(walker);
        if (group != null)
        {
            if (OtherMovementGroupsMoving(group))return ;

            group.Moving = true;
            foreach (var w in group.Walkers)
            {
                if (w.HasStepToken) return;
                AllowStep(w);
                //w.StepSpeed = StepSpeed;
                //w.BeginStep();
            }
        }
    }

    public MovementGroup getGroupFor(ProceduralWalker walker)
    {
        foreach (var group in movementGroups)
        {
            if(group.Walkers.Contains(walker)) return group;
        }
        return null;
    }

    public bool OtherMovementGroupsMoving(MovementGroup group)
    {
        foreach( var c in movementGroups)
        {
            if (c == group) continue;
            if(c.IsMoving) return true;
        }
        return false;
    }

    public void RequestEmergencyStep(ProceduralWalker walker)
    {
        if(walker.IsMoving)return ;

        var group = getGroupFor(walker);

        walker.ExecuteStep();

        if(group != null) group.OnWalkerStartedStep(walker);
    }

    public void NotifyStepFinished(ProceduralWalker walker)
    {
        var group = getGroupFor(walker);
        if(group==null)return ;
        group.OnWalkerFinishedStep(walker);
    }

    public ProceduralWalker GetMostBehindWalkerGroup(MovementCoordinator coordinator)
    {
        ProceduralWalker furthest = coordinator.movementGroups[0].Walkers[0];
        foreach(var group in coordinator.movementGroups)
        {
            foreach(var w in group.Walkers)
            {
                if(w.IsMoving)continue;
                if(w.ExtensionRatio > furthest.ExtensionRatio)
                {
                    furthest = w;
                }
            }
        }
        return furthest;
    }

    [Serializable]
    public class MovementGroup
    {
        public int Group;
        public List<ProceduralWalker> Walkers;
        public bool Moving;

        public float PhaseOffset;
        public float PhaseWindow = 0.35f;
        public float StepUrgencyThreshold = 0.4f;
        public bool IsMoving;
        int activeSteppers;

        public void Tick(float clock, MovementCoordinator coordinator)
        {
            float phase = (clock - PhaseOffset + 1) % 1;
            bool inWindow = phase < PhaseWindow;

            if (!inWindow || IsMoving)
            {
                return;
            }

            if (coordinator.OtherMovementGroupsMoving(this)) return;

            float urgency = (float)Walkers.Count(w => w.WantsToStep) / MathF.Max(1, Walkers.Count);
            if(urgency < StepUrgencyThreshold) return;

            coordinator.getGroupFor( coordinator.GetMostBehindWalkerGroup(coordinator)).TriggerGroupStep(coordinator);
           // TriggerGroupStep(coordinator);
        }

        public void Tick2(float clock, MovementCoordinator coordinator)
        {
            foreach(ProceduralWalker walker in Walkers)
            {
                if (walker.WantsToStep &&  !walker.IsMoving)
                {
                    walker.ExecuteStep();
                }
            }
            return;
            float phase = (clock - PhaseOffset + 1) % 1;
            bool inWindow = phase < PhaseWindow;

            if (!inWindow || IsMoving)
            {
                return;
            }

            if (coordinator.OtherMovementGroupsMoving(this)) return;

            float urgency = (float)Walkers.Count(w => w.WantsToStep) / MathF.Max(1, Walkers.Count);
            if (urgency < StepUrgencyThreshold) return;

            coordinator.getGroupFor(coordinator.GetMostBehindWalkerGroup(coordinator)).TriggerGroupStep(coordinator);
            // TriggerGroupStep(coordinator);
        }

        void TriggerGroupStep(MovementCoordinator coordinator)
        {
            IsMoving = true;
            activeSteppers = 0;

            foreach (var walker in Walkers)
            {
                if(walker == null || walker.IsMoving) continue;
                walker.StepSpeed = coordinator.StepSpeed;
                
               //walker.Invoke(nameof(walker.ExecuteStep), UnityEngine.Random.Range(0,.2f));
                walker.ExecuteStep();
                activeSteppers++;
            }

            if(activeSteppers == 0 ) IsMoving = false;
        }

        public void OnWalkerStartedStep(ProceduralWalker walker)
        {
            IsMoving = true;
            activeSteppers++;
           // walker.StepSpeed = 43;
        }
        public void OnWalkerFinishedStep(ProceduralWalker walker)
        {
            activeSteppers = Mathf.Max(0, activeSteppers - 1);

            if(activeSteppers ==0) IsMoving = false;
        }

       // public bool Any
    }
}
