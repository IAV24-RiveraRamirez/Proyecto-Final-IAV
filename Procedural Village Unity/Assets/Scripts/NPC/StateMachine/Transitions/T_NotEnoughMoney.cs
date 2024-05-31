
/// <summary>
/// Indica cu�ndo no se ha tenido suficiente dinero para comprar algo 
/// </summary>
public class T_NotEnoughMoney : Transition
{
    protected NPCInfo info;
    public T_NotEnoughMoney(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return info.GetLastBuyResult() == Market.BuyRequestOutput.CLIENT_HAS_NO_MONEY;
    }

    public override void Enter()
    {
        info = gameObject.GetComponent<NPCInfo>();
    }

    public override void Exit()
    {
        info.SetLastBuyResult(Market.BuyRequestOutput.RESTING_STATE);
    }

    public override string ID()
    {
        return "Not enough money";
    }
}
