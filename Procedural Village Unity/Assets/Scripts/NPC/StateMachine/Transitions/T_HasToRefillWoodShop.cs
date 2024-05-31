
/// <summary>
/// Indica que un carpintero debe rellenar el almacén de su carpintería
/// </summary>
public class T_HasToRefillWoodShop : Transition
{
    public T_HasToRefillWoodShop(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return (bool)fsm.blackboard.Get("Craft_GoRefill", typeof(bool)) && 
            (int)fsm.blackboard.Get(Market.Item.WOOD.ToString(), typeof(int)) > 0;
    }

    public override void Enter()
    {
        object refill = fsm.blackboard.Get("Craft_GoRefill", typeof(bool));
        if(refill == null)
        {
            fsm.blackboard.Set("Craft_GoRefill", typeof(bool), false);
        }
        object wood = fsm.blackboard.Get(Market.Item.WOOD.ToString(), typeof(int));
        if (wood == null)
        {
            fsm.blackboard.Set(Market.Item.WOOD.ToString(), typeof(int), 0);
        }
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Has to Refill wood shop";
    }
}
