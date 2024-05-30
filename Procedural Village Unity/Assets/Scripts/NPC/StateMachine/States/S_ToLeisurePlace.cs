using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class S_ToLeisurePlace : State
{
    NavMeshAgent agent;
    GameObject destination;

    public S_ToLeisurePlace()
    {

    }

    public override void Enter()
    {
        destination = gameObject.GetComponent<NPCInfo>().GetLeisurePlace().gameObject;
        agent = gameObject.GetComponent<NavMeshAgent>();

        agent.SetDestination(destination.transform.position);
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "To Leisure Place";
    }

    public override void Update(float dt)
    {

    }
}
