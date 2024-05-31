
/// <summary>
/// Indica cu�ndo es por la ma�ana
/// </summary>
public class T_Morning : Transition
{
    public T_Morning(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return SimulationManager.Instance.GetCurrentPeriod() == SimulationManager.TimePeriods.MORNING;
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Morning";
    }
}
