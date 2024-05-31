using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Indica cuándo se ha llegado a un destino dado un margen
/// </summary>
public class T_ReachDestination : Transition
{
    // References
    NavMeshAgent agent;

    // Parameters
    float threshold;

    public T_ReachDestination(State nextState, float threshold) : base(nextState)
    {
        this.threshold = threshold;
    }

    public override void Init(GameObject g, StateMachine fsm)
    {
        base.Init(g, fsm);

        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    public override bool Check()
    {
        return agent.remainingDistance < threshold;
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Reach Destination";
    }
}
