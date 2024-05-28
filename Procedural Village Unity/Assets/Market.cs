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

    // Enum
    public enum Item { WOOD, CRAFTS }

    // Parameters
    [SerializeField] MarketPrices sellPrices;

    // Own Methods
    public float Sell(Item item, int amount)
    {
        return sellPrices.prices[(int)item] * amount;
    }

    private void OnValidate()
    {
        sellPrices = new MarketPrices(sellPrices.wood, sellPrices.crafts);
    }

    protected override void Start()
    {
        maxNpcs = 100000000;
        type = BuildingType.MARKET;
        base.Start();
    }
}
