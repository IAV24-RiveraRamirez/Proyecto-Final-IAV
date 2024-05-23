using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transition
{
    protected GameObject gameObject;
    protected StateMachine fsm;
    protected State nextState;

    public Transition(State nextState) { this.nextState = nextState; }

    public void Init(GameObject g, StateMachine fsm)
    {
        gameObject = g;
        this.fsm = fsm;
    }

    public abstract void Enter();
    public abstract bool Check();
    public virtual State NextState() { return nextState; }

    public abstract void Exit();

}
