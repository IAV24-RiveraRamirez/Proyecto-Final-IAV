
/// <summary>
/// Indica cuándo se ha podido comprar un ítem de una tienda
/// </summary>
public class T_ItemWasBought : Transition
{
    NPCInfo info;
    public T_ItemWasBought(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return info.GetLastBuyResult() == Market.BuyRequestOutput.ITEM_BOUGHT;
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
        return "Item bought!";
    }
}
