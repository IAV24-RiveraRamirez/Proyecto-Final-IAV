using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Trata de comprar un Item a un mercader
/// Solo lo consigue si ve su dinero disminuido
/// </summary>
public class S_BuyItem : State
{
    public const string GET_RANDOM_AMOUNT = "GetRandomAmountFromShop";

    float lastMoney = 0;
    NPCInfo info;
    Market.Item item;
    string itemNameOnBlackboard;
    int amount;

    public S_BuyItem(Market.Item item, string itemNameOnBlackboard)
    {
        this.item = item;
        this.itemNameOnBlackboard = itemNameOnBlackboard;
    }

    public override void Enter()
    {
        info = gameObject.GetComponent<NPCInfo>();
        gameObject.GetComponent<NavMeshAgent>().isStopped = true;

        Market market = null;
        if (info.GetLeisurePlace() == null) market = info.GetMarketPlace() as Market;
        else market = info.GetLeisurePlace() as Market;

        amount = -1;
        if (itemNameOnBlackboard != GET_RANDOM_AMOUNT) amount = (int)fsm.blackboard.Get(itemNameOnBlackboard, typeof(int));
        else
        {
            int max = market.GetMaxItemAmount(item);
            if (max == -2)
            {
                info.SetLastBuyResult(Market.BuyRequestOutput.SHOP_HAS_NO_ITEM);
                return;
            }
            amount = Random.Range(1, max + 1);
            Debug.Log("Buying: " + item + " x " + amount + "/" + max);
        }
        market.MakeRequest(info, item, amount, Market.Request.RequestType.BUY);
        lastMoney = info.GetMoney();
    }

    public override void Exit()
    {
        gameObject.GetComponent<NavMeshAgent>().isStopped = false;
    }

    public override string ID()
    {
        return "Buying " + item.ToString().ToLower();
    }

    public override void Update(float dt)
    {
        if (lastMoney > info.GetMoney())
        {
            fsm.blackboard.Set(item.ToString(), typeof(int), amount);
            if(itemNameOnBlackboard != GET_RANDOM_AMOUNT) fsm.blackboard.Set(itemNameOnBlackboard, typeof(int), 0);
        }
    }
}
