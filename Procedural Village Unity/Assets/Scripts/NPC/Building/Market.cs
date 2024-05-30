using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class Market : NPCBuilding
{
    // Structures 
    [System.Serializable]
    struct MarketPrices
    {
        public float wood;
        public float crafts;

        [HideInInspector]
        public float[] prices;

        public MarketPrices(float wood, float crafts) 
        {
            this.wood = wood;
            this.crafts = crafts;

            prices = new float[2];
            prices[(int)Item.WOOD] = wood;
            prices[(int)Item.CRAFTS] = crafts;
        }
    }
    
    public struct Request
    {
        public enum RequestType { BUY, SELL }

        public NPCInfo client;
        public Item item;
        public int amount;
        public RequestType type;

        public Request(NPCInfo client, Item item, int amount, RequestType type)
        {
            this.client = client;
            this.item = item;
            this.amount = amount;
            this.type = type;
        }
    }

    // Enum
    public enum Item { WOOD, CRAFTS }
    public enum BuyRequestOutput { RESTING_STATE, ITEM_BOUGHT, NO_CLIENT, CLIENT_HAS_NO_MONEY, SHOP_HAS_NO_ITEM }

    // References 
    NPCInfo npcAttendingShop = null;
    NPCInfo client = null;

    // Parameters
    [SerializeField] MarketPrices sellPrices;
    [SerializeField] MarketPrices buyPrices;

    // Variables
    Queue<Request> queue = new Queue<Request>();
    Dictionary<Item, int> itemAmount = new Dictionary<Item, int>();
    bool processingRequest = false;

    // Own Methods
    public void AddNPCToQueue(NPCInfo client, Item item, int amount, Request.RequestType type)
    {
        if (amount == 0)
        {
            SimulationManager.Instance.RemoveTemporalMarket(this);
            client.SetLastBuyResult(BuyRequestOutput.SHOP_HAS_NO_ITEM);
            return;
        }
        queue.Enqueue(new Request(client, item, amount, type));
    }

    public bool IsItemAvaliable(Item item, int amount)
    {
        if(itemAmount.ContainsKey(item))
        {
            if (amount <= itemAmount[item] || itemAmount[item] == -1) return true;
            else return false;
        }
        return false;
    }

    /// <summary>
    /// Coger la cantidad máxima de ítems de un tipo que se venden
    /// </summary>
    /// <param name="item">Item del que se quiere comprobar</param>
    /// <returns>-2 Si no se vende, -1 si hay infinitas unidades y >= 0 que existen X de ese ítem </returns>
    public int GetMaxItemAmount(Item item)
    {
        if (itemAmount.ContainsKey(item))
        {
            return itemAmount[item];
        }
        return -2;
    }

    public void AddItems(Item item, int amount)
    {
        if (!itemAmount.ContainsKey(item))
        {
            itemAmount.Add(item, amount);
        }
        else if (itemAmount[item] > -1) itemAmount[item] += amount;
        else if (itemAmount[item] == -1) itemAmount[item] = amount;
    }

    public void Buy(NPCInfo client, Item item, int amount)
    {
        processingRequest = true;
        this.client = client;
        Blackboard blackboard = npcAttendingShop.GetComponent<NPCBehaviourSM>().GetStateMachine().blackboard;
        blackboard.Set("Sell_Request", typeof(bool), true);
        blackboard.Set("Sell_Item", typeof(Item), item);
        blackboard.Set("Sell_ItemAmout", typeof(int), amount);
    }

    public void Sell(NPCInfo client, Item item, int amount)
    {
        processingRequest = true;
        this.client = client;
        Blackboard blackboard = npcAttendingShop.GetComponent<NPCBehaviourSM>().GetStateMachine().blackboard;
        blackboard.Set("Buy_Request", typeof(bool), true);
        blackboard.Set("Buy_Item", typeof(Item), item);
        blackboard.Set("Buy_ItemAmout", typeof(int), amount);
    }

    public BuyRequestOutput GetMoneyFromClient(Item item, int amount)
    {
        BuyRequestOutput result = BuyRequestOutput.RESTING_STATE;
        if (client != null)
        {
            if(IsItemAvaliable(item, amount))
            {
                float money = buyPrices.prices[(int)item] * amount;
                bool hasMoney = client.ChangeMoney(-money);
                result = BuyRequestOutput.ITEM_BOUGHT;
                if (!hasMoney)
                {
                    result = BuyRequestOutput.CLIENT_HAS_NO_MONEY;
                }
                else if (itemAmount[item] != -1) 
                {
                    npcAttendingShop.ChangeMoney(money);
                    itemAmount[item] -= amount;
                }
                processingRequest = false;
                client.SetLastBuyResult(result, money);
                client = null;
            }
            else
            {
                client.SetLastBuyResult(result);
                client = null;
                result = BuyRequestOutput.SHOP_HAS_NO_ITEM;
            }
        }
        else
        {
            result = BuyRequestOutput.NO_CLIENT;
        }

        return result;
    }

    public bool GiveMoneyToClient(Item item, int amount)
    {
        if (client != null)
        {
            if((int)item < sellPrices.prices.Length && item >= 0 && amount > 0)
            {
                client.ChangeMoney(sellPrices.prices[(int)item] * amount);
                processingRequest = false;
                client = null;
                return true;
            }
            else
            {
                Debug.LogError("Failure while trying to sell " + amount + " of item " + item);
                return false;
            }
        }
        else
        {
            Debug.LogError("No client waiting for sell!");
            return false;
        } 
    }

    public void SetNPCAttending(NPCInfo info) {
        npcAttendingShop = info;
        if(info == null)
        {
            while(queue.Count > 0)
            {
                Request r = queue.Dequeue();
                r.client.SetLastBuyResult(BuyRequestOutput.SHOP_HAS_NO_ITEM);
            }
        }
    }

    public NPCInfo GetNPCAttending() { return npcAttendingShop; }

    private void OnValidate()
    {
        sellPrices = new MarketPrices(sellPrices.wood, sellPrices.crafts);
        buyPrices = new MarketPrices(buyPrices.wood, buyPrices.crafts);
    }

    protected void SetUpAvaliableItems()
    {
        for (int i = 0; i < buyPrices.prices.Length; ++i)
        {
            if (buyPrices.prices[i] > 0)
            {
                itemAmount.Add((Item)i, -1); // -1 hay infinito
            }
        }
    }

    protected override void Start()
    {
        maxNpcs = 1;
        type = BuildingType.MARKET;
        SetUpAvaliableItems();
        base.Start();
    }

    protected virtual void Update()
    {
        if(queue.Count > 0 && !processingRequest && npcAttendingShop)
        {
            Request request = queue.Dequeue();
            if (request.type == Request.RequestType.SELL)
                Buy(request.client, request.item, request.amount);
            else
                Sell(request.client, request.item, request.amount);
        }
    }
}
