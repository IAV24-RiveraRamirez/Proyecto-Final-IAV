using UnityEngine;

/// <summary>
/// M�quina de estado para un NPC en momento de Ocio
/// </summary>
public class SM_Leisure : StateMachine
{
    public override string ID()
    {
        return "On Leisure time";
    }

    public override void Init(GameObject g, StateMachine fsm)
    {
        base.Init(g, fsm);

        // Busca una actividad.
        // En caso de que sea de mercado, va al mercado, hace una petici�n y, da igual su output, 
        // vuelve a buscar una actividad
        // En caso de que sea una actividad corriente, va al sitio designado y trata de realizarla. 
        // Al acabarla, vuelve a buscar actividad

        State state1 = new S_SearchingActivity();
        State toMarket = new S_ToLeisurePlace();
        State toActivity = new S_ToLeisurePlace();
        State state2 = new S_HaveFun();
        State state3 = new S_BuyItem(Market.Item.CRAFTS, S_BuyItem.GET_RANDOM_AMOUNT);

        state1.AddTransition(new T_LeisureActivitySelected(toActivity));
        state1.AddTransition(new T_MarketSelectedAsLeisure(toMarket));
        
        state2.AddTransition(new T_FinishedLeisureActivity(state1));
        state2.AddTransition(new T_NotEnoughMoney(state1));

        state3.AddTransition(new T_ItemWasBought(state1));
        state3.AddTransition(new T_ShopHasNoItem(state1));
        state3.AddTransition(new T_NotEnoughMoney(state1));
        state3.AddTransition(new T_MarketClosed(state1));

        toActivity.AddTransition(new T_ReachDestination(state2, 2.5f));
        toMarket.AddTransition(new T_ReachDestination(state3, 2.5f));

        AddState(state1).AddState(toMarket).AddState(toActivity).AddState(state2).AddState(state3);

        StartMachine(state1);
    }

    public override void Exit()
    {
        base.Exit();
        gameObject.GetComponent<NPCInfo>().SetLeisurePlace(null);
    }
}
