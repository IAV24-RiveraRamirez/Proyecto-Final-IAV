
/// <summary>
/// Indica cuándo ha llegado el momento de trabajar
/// </summary>
public class T_HasToWork : Transition
{
    NPCInfo info;
    public T_HasToWork(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return SimulationManager.Instance.GetCurrentPeriod() == info.GetWorkingPeriod();
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
        return "Time to Work";
    }
}
