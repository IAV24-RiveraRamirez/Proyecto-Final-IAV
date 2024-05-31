
/// <summary>
/// Indica que ha llegado la noche
/// </summary>
public class T_Evening : Transition
{
    public T_Evening(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return SimulationManager.Instance.GetCurrentPeriod() == SimulationManager.TimePeriods.EVENING;
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Evening";
    }
}
