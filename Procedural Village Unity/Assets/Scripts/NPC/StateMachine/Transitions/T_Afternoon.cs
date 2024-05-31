
/// <summary>
/// Indica que ha llegado la tarde
/// </summary>
public class T_Afternoon : Transition
{
    public T_Afternoon(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return SimulationManager.Instance.GetCurrentPeriod() == SimulationManager.TimePeriods.AFTERNOON;
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Afternoon";
    }
}
