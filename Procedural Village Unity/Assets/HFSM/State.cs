using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected GameObject gameObject;

    private StateMachine fsm;

    public void Init(GameObject g, StateMachine fsm)
    {
        this.fsm = fsm;
        gameObject = g;
    }

    // Llamado la primera vez que se entra al estado
    public abstract void Enter();

    // Llamado durante el Update de Unity
    public abstract void Update(float dt);

    // Llamado al salir del estado
    public abstract void Exit();
}
