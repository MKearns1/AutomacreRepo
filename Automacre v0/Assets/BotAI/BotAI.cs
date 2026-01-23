using UnityEngine;
using UnityEngine.AI;

public class BotAI : MonoBehaviour
{
    public NavMeshAgent NavAgent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            MoveTo(Vector3.zero);
        }
    }



    public void MoveTo(Vector3 Destination)
    {
        NavAgent.SetDestination(Destination);
    }
}
