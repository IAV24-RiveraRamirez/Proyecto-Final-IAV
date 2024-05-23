using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StateMachine : State
{
    private State current;

    public Blackboard blackboard;

    Dictionary<State, Transition[]> fsmStructure = new Dictionary<State, Transition[]>();
    
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
        current.Enter();
        
        foreach(Transition t in fsmStructure[current])
        {
            t.Enter();
        }
    }

    public override void Update(float dt)
    {
        current.Update(dt);
        Transition[] transitions = fsmStructure[current];
        bool changeState = false;
        for(int i = 0; !changeState && i < transitions.Length; ++i)
        {
            changeState = ChangeState(transitions[i]);
        }
    }

    private bool ChangeState(Transition t)
    {
        bool changeState = t.Check();
        if(changeState)
        {
            t.Exit();
            current.Exit();
            current = t.NextState();
            current.Enter();
            foreach(Transition transition in fsmStructure[current])
            {
                transition.Enter();
            }
        }
        return changeState;
    }

    public StateMachine AddState(State s, Transition[] transitions)
    {
        s.Init(gameObject, this);
        for(int i = 0; i < transitions.Length; ++i)
        {
            transitions[i].Init(gameObject, this);
        }
        fsmStructure.Add(s, transitions);
        return this;
    }

    /// <summary>
    /// won't be implemented, not necessary
    /// </summary>
    public override void Enter()
    {
        return;
    }

    /// <summary>
    /// won't be implemented, not necessary
    /// </summary>
    public override void Exit()
    {
        return;
    }
}
