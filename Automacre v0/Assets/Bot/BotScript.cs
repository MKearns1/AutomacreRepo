using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
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
    public List<BotDirection> botDirections = new List<BotDirection>();
    bool CurrentlyPerformingAction;

    public List<ResourceType> Inventory = new List<ResourceType>();

    TextMeshPro debugtext;
    //Vector<BotDirection> bb;
    Coroutine pathcomplete;
    float GiveUpTimer=0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = MovementSpeed;

        debugtext = transform.Find("State").GetComponent<TextMeshPro>();

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
            //Debug.Log(botDirections.Count);

        }

        debugtext.text = CurrentBotState.ToString();
        debugtext.gameObject.transform.rotation = UnityEngine.Quaternion.Euler(transform.rotation.x+90,0,transform.rotation.z);
        switch (CurrentBotState)
        {
            case State.Idle:
                debugtext.color = Color.white;
                break;
            case State.MovingToTarget:
                debugtext.color = Color.green;
                break;
            case State.HarvestingResource:
                debugtext.color = Color.yellow;
                break;


        }

        //Debug.Log(agent.velocity.magnitude);

        if(GiveUpTimer >=2) 
        {
            agent.isStopped = true;
            agent.ResetPath();
            //DirectionComplete();
            GiveUpTimer = 0;
            Debug.Log("GiveUp");

        }
        else
        {
            agent.isStopped = false;

            GiveUp();
        }

        debugtext.text = GiveUpTimer.ToString();
    }

    public void MoveTo(UnityEngine.Vector3 Position)
    {
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

                CurrentBotState = State.MovingToTarget;

                MoveTo(Direction.Location);
                break;

            case DirectType.Harvest:
                Debug.Log("Harvesting");

                CurrentBotState = State.HarvestingResource;

                if (Direction.TargetObject != null)
                {
                    StartCoroutine(BeginHarvesting(Direction.TargetObject.GetComponentInParent<ResourceScript>()));
                }
                else
                {
                    DirectionComplete();
                }
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

                UnityEngine.Vector3 RandomPointAroundResource = Direction.TargetObject.transform.position + UnityEngine.Random.insideUnitSphere;
                BotDirection moveto = new BotDirection(DirectType.Move, RandomPointAroundResource,null);
                botDirections.Add(moveto);

                break;

        }


        botDirections.Add(Direction);
    }
    public void DirectionComplete()
    {
        if (botDirections.Count > 0)
        {
            botDirections.RemoveAt(0);
            CurrentlyPerformingAction = false;
            CurrentBotState = State.Idle;
            GiveUpTimer = 0;

        }
        else
        {
            Debug.Log("No Directions left to remove");
        }
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

            if (resource.Quantity > 0)
            {
                HarvestResource(resource);
            }
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


    public void GiveUp()
    {
        if(agent.remainingDistance > agent.stoppingDistance)
        {
            if (agent.velocity.magnitude < 3)
            {
                GiveUpTimer += Time.deltaTime;
                //Debug.Log(GiveUpTimer);
            }
            else
            {
                GiveUpTimer = 0;
            }
        }
    }


}
