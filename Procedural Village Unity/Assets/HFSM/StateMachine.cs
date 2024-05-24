using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class StateMachine : State
{
    private State current;

    public Blackboard blackboard;

    protected bool changedState = false;

    string lastTransitionID = "";
    public StateMachine()
    {
        blackboard = new Blackboard();
    }

    public void SetContext(GameObject gO)
    {
        gameObject = gO;
    }

    public void StartMachine(State initial)
    {
        current = initial;
        EnterCurrentState();
    }

    public override void Update(float dt)
    {
        current.Update(dt);
        List<Transition> transitions = current.GetTransitions();
        bool changeState = false;
        for(int i = 0; !changeState && i < transitions.Count; ++i)
        {
            changeState = ChangeState(transitions[i]);
        }
        if (changedState && fsm != null)
        {
            fsm.changedState = true;
        }
    }

    private bool ChangeState(Transition t)
    {
        bool changeState = t.Check();
        if(changeState)
        {
            lastTransitionID = t.ID();
            t.Exit();
            current.Exit();
            current = t.NextState();
            EnterCurrentState();
            
            changedState = true;
        }
        return changeState;
    }

    private void EnterCurrentState()
    {
        if(current is StateMachine)
        {
            current.Init(gameObject, this);
        }
        current.Enter();
        List<Transition> transitions = current.GetTransitions();
        foreach (Transition t in transitions)
        {
            t.Enter();
        }
    }

    public StateMachine AddState(State s)
    {
        s.Init(gameObject, this);
        List<Transition> transitions = s.GetTransitions();
        for(int i = 0; i < transitions.Count; ++i)
        {
            transitions[i].Init(gameObject, this);
        }
        return this;
    }

    public State GetActiveState() { return current; }
    public string GetLastTransitionID() { return lastTransitionID; }

    public bool HasChangedState() { return changedState; }

    /// <summary>
    /// won't be implemented, not necessary
    /// </summary>
    public override void Enter()
    {
        lastTransitionID = "";
    }

    /// <summary>
    /// won't be implemented, not necessary
    /// </summary>
    public override void Exit()
    {
        return;
    }

    public abstract override string ID();
}
