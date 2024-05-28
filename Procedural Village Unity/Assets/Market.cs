using System.Collections;
using System.Collections.Generic;
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
    
    struct SellRequest
    {
        public NPCInfo client;
        public Item item;
        public int amount;

        public SellRequest(NPCInfo client, Item item, int amount)
        {
            this.client = client;
            this.item = item;
            this.amount = amount;
        }
    }

    // Enum
    public enum Item { WOOD, CRAFTS }

    // References 
    NPCInfo npcAttendingShop = null;
    NPCInfo client = null;

    // Parameters
    [SerializeField] MarketPrices sellPrices;

    // Variables
    Queue<SellRequest> queue = new Queue<SellRequest>();
    bool processingRequest = false;

    // Own Methods
    public override bool AddNPC(NPCInfo npc)
    {
        return base.AddNPC(npc);
    }
    public void AddNPCToQueue(NPCInfo client, Item item, int amount)
    {
        queue.Enqueue(new SellRequest(client, item, amount));
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

    public bool Sell(Item item, int amount)
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
    }

    public NPCInfo GetNPCAttending() { return npcAttendingShop; }

    private void OnValidate()
    {
        sellPrices = new MarketPrices(sellPrices.wood, sellPrices.crafts);
    }

    protected override void Start()
    {
        maxNpcs = 1;
        type = BuildingType.MARKET;
        base.Start();
    }

    private void Update()
    {
        if(queue.Count > 0 && !processingRequest && npcAttendingShop)
        {
            SellRequest request = queue.Dequeue();
            Buy(request.client, request.item, request.amount);
        }
    }
}
