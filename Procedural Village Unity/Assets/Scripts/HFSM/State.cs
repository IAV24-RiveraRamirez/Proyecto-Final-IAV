using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public abstract class State
{
    protected GameObject gameObject;

    protected StateMachine fsm = null;

    private List<Transition> transitions = new List<Transition>();

    public List<Transition> GetTransitions() { return transitions; }

    public virtual void Init(GameObject g, StateMachine fsm)
    {
        this.fsm = fsm;
        gameObject = g;
    }

    public void AddTransition(Transition transition) {
        transitions.Add(transition);
    }

    public void AddTransition(Transition[] transition)
    {
        for(int i = 0; i < transition.Length; ++i)
        {
            transitions.Add(transition[i]);
        }
    }

    public void AddTransition(List<Transition> transition)
    {
        transitions.AddRange(transition);
    }

    // Llamado la primera vez que se entra al estado
    public abstract void Enter();

    // Llamado durante el Update de Unity
    public abstract void Update(float dt);

    // Llamado al salir del estado
    public abstract void Exit();

    public abstract string ID();
}
