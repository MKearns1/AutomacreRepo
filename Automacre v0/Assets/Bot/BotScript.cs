using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class BotScript : MonoBehaviour
{

    NavMeshAgent agent;

    public float MovementSpeed;

    public Action onPathCompleteAction;

    public enum State
    {
        Idle,
        MovingToTarget,
        HarvestingResource,
        DepositingItems
    }

    public State CurrentBotState;
    public List<BotDirection> botDirections = new List<BotDirection>();
    bool CurrentlyPerformingAction;

    public List<InventoryItem> Inventory = new List<InventoryItem>() { };

    TextMeshPro debugtext;
    //Vector<BotDirection> bb;
    Coroutine pathcomplete;
    float GiveUpTimer=0;
    float DepositTime = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = MovementSpeed;

        debugtext = transform.Find("State").GetComponent<TextMeshPro>();

        onPathCompleteAction += GetNextAction;
        onPathCompleteAction += DirectionComplete;


        Inventory.Clear(); // just in case
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            Inventory.Add(new InventoryItem(type, 0));
        }
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
            case State.DepositingItems:
                debugtext.color = Color.grey;
                break;


        }

        //Debug.Log(agent.velocity.magnitude);

        if (GiveUpTimer >=2) 
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
                Interfaces.DrawDebugCircle(Direction.Location,agent.stoppingDistance,16,Color.yellow,20);
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

                case DirectType.Deposit:

                if(Direction.TargetObject.GetComponentInParent<StorageScript>()!=null)
                {
                    StartCoroutine(BeginDepositing(Direction.TargetObject.GetComponentInParent<StorageScript>()));

                }

                break;

        }
    }

    public void GiveDirection(BotDirection Direction)
    {
        BotDirection moveto = new BotDirection(DirectType.Move,Direction.Location,null);
        UnityEngine.Vector3 RandomPointAroundResource;

        switch (Direction.Type)
        {
            case DirectType.Move:

                //botDirections.Add(Direction);
                break;

            case DirectType.Harvest:

                RandomPointAroundResource = Direction.TargetObject.transform.position + UnityEngine.Random.insideUnitSphere;
                moveto.Location = RandomPointAroundResource;
                botDirections.Add(moveto);

                break;

            case DirectType.Deposit:
                RandomPointAroundResource = Direction.TargetObject.transform.position + UnityEngine.Random.insideUnitSphere;
                moveto.Location = RandomPointAroundResource;
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
            yield return new WaitForSeconds(resource.HarvestTime);

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


    public IEnumerator BeginDepositing(StorageScript Storage)
    {
        foreach (InventoryItem item in Inventory)
        {
            while(item.Quantity > 0)
            {
                yield return new WaitForSeconds(1f);

                Deposit(item, Storage);
            }

        }
        DepositComplete();

        yield return null;
    }

    public void Deposit(InventoryItem depositItem, StorageScript Storage)
    {
        //botDirections[0].TargetObject.GetComponentInParent<StorageScript>().DepositItem(depositItem, 1, this);
        Storage.DepositItem(depositItem, 1, this);
    }

    public void DepositComplete()
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
