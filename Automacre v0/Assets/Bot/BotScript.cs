using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class BotScript : MonoBehaviour
{

    NavMeshAgent agent;

    public float MovementSpeed;

    public enum State
    {
        Idle,
        MovingToTarget,
        HarvestingResource
    }

    public State BotState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = MovementSpeed;
       
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void MoveTo(Vector3 Position)
    {
        BotState = State.MovingToTarget; 
        agent.SetDestination(Position);
    }
    public void Direct(DirectType DirectionType)
    {
        switch (DirectionType)
        {
            case DirectType.Move:


                break;

        }
    }

   
}
