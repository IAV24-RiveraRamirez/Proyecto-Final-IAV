using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// M�quina de estados abstracta de la que heredan los diferentes trabajos
/// </summary>
public abstract class SM_Work : StateMachine
{
    public override void Init(GameObject g, StateMachine fsm)
    {
        base.Init(g, fsm);

        // Marca el d�a como no terminado en su comienzo
        fsm.blackboard.Set("WorkDayEnded", typeof(bool), false);
    }
}
