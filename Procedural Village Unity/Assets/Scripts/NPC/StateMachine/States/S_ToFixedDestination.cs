using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Viaja hasta un lugar concreto del mapa ya conocido de antes
/// </summary>
public class S_ToFixedDestination : State
{
    NavMeshAgent agent;
    GameObject destination;

    public S_ToFixedDestination(GameObject dest)
    {
        destination = dest;
    }

    public override void Enter()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        agent.SetDestination(destination.transform.position);
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "To Fixed Destination";
    }

    public override void Update(float dt)
    {

    }
}
