using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dormir. Rota al NPC
/// </summary>
public class S_OnBed : State
{
    public override void Enter()
    {
        gameObject.transform.Rotate(new Vector3(0, 0, 90));
    }

    public override void Exit()
    {
        gameObject.transform.Rotate(new Vector3(0, 0, -90));
    }

    public override string ID()
    {
        return "On Bed";
    }

    public override void Update(float dt)
    {
        Debug.Log("hoooonk mimimim");
        gameObject.transform.eulerAngles = new Vector3(0, 0, 90);
    }
}
