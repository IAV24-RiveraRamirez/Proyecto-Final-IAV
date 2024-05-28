using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_ChoppingWood : State
{
    // Parameters
    float timeToProduce = 3;
    int amountProduced = 3;

    // Variables
    float timer = 0;

    NPCInfo info = null;
    
    public S_ChoppingWood(float _timeToProduce, int _amountProduced) : base()
    {
        timeToProduce = _timeToProduce;
        amountProduced = _amountProduced;
    }

    public override void Enter()
    {
        info = gameObject.GetComponent<NPCInfo>();
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Chopping Wood";
    }

    public override void Update(float dt)
    {
        timer += dt;
        if(timer > timeToProduce)
        {
            ProduceWood();
            timer = 0;
        }
    }

    void ProduceWood()
    {
        Sawmill sawmill = info.GetWorkPlace() as Sawmill;

        sawmill.AddWood(amountProduced);
    }
}
