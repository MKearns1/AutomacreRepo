using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class BotScript : MonoBehaviour
{

    NavMeshAgent agent;

    public float MovementSpeed;

    public Action onPathCompleteAction;

    public enum State
    {
        Idle,
        MovingToTarget,
        HarvestingResource
    }

    public State CurrentBotState;
    List<BotDirection> botDirections = new List<BotDirection>();
    bool CurrentlyPerformingAction;
    //Vector<BotDirection> bb;
    Coroutine pathcomplete;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = MovementSpeed;

        onPathCompleteAction += GetNextAction;
        onPathCompleteAction += DirectionComplete;
       
    }

    // Update is called once per frame
    void Update()
    {
       // if((Vector3.Distance(transform.position, agent.destination) < 1) && agent.)
       if(agent.remainingDistance < 1)
        {
           // Debug.Log("Target Reached");
        }

        if (botDirections.Count > 0 && !CurrentlyPerformingAction)
        {
            PerformAction(botDirections[0]);
            Debug.Log(botDirections.Count);

        }
    }

    public void MoveTo(UnityEngine.Vector3 Position)
    {
        CurrentBotState = State.MovingToTarget; 
        agent.SetDestination(Position);


        if(pathcomplete != null)
        {
            StopCoroutine(pathcomplete);
        }
        pathcomplete = StartCoroutine(PathComplete(onPathCompleteAction));
    }
    public void PerformAction(BotDirection Direction)
    {
        CurrentlyPerformingAction = true;
        switch (Direction.Type)
        {
            case DirectType.Move:
                // Debug.Log("MovingToLocation");


                MoveTo(Direction.Location);
                break;

            case DirectType.Harvest:
                Debug.Log("Harvesting");

                StartCoroutine(BeginHarvesting(Direction.TargetObject.GetComponentInParent<ResourceScript>()));
                break;

        }
    }

    public void GiveDirection(BotDirection Direction)
    {
        switch (Direction.Type)
        {
            case DirectType.Move:

                //botDirections.Add(Direction);
                break;

            case DirectType.Harvest:
                BotDirection moveto = new BotDirection(DirectType.Move, Direction.Location,null);
                botDirections.Add(moveto);

                break;

        }


        botDirections.Add(Direction);
    }
    public void DirectionComplete()
    {
        botDirections.RemoveAt(0);
        CurrentlyPerformingAction = false;
    }

    public IEnumerator PathComplete(Action onComplete)
    {
        // Wait until path is ready
        while (agent.pathPending)
            yield return null;

        // Wait until close enough
        while (agent.remainingDistance > agent.stoppingDistance ||
               agent.velocity.sqrMagnitude > 0.01f)
            yield return null;

        onComplete?.Invoke();
    }

    void GetNextAction()
    {
         Debug.Log("Target Reached");

    }

    public IEnumerator BeginHarvesting(ResourceScript resource)
    {
        while (resource.Quantity > 0) {
            yield return new WaitForSeconds(1f);
            HarvestResource(resource);
        }
        HarvestComplete();

        yield return null;
    }

    void HarvestResource(ResourceScript resource)
    {
        botDirections[0].TargetObject.GetComponentInParent<ResourceScript>().Harvest(this);
    }
    void HarvestComplete()
    {
        DirectionComplete();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if(agent != null) 
       Gizmos.DrawSphere(agent.destination, 0.5f);
    }
}
