using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase que simboliza un mercado dentro de la simulación.
/// </summary>
public class Market : NPCBuilding
{
    // Structures 
    /// <summary>
    /// Define los precios de los materiales que tenga como variables float.
    /// </summary>
    [System.Serializable]
    struct MarketPrices
    {
        /// <summary>
        /// Precio de madera
        /// </summary>
        public float wood;
        /// <summary>
        /// Precio de Artesanía
        /// </summary>
        public float crafts;

        /// <summary>
        /// Array con los precios
        /// </summary>
        [HideInInspector]
        public float[] prices;

        /// <summary>
        /// Constructora
        /// </summary>
        /// <param name="wood"> precio de madera </param>
        /// <param name="crafts"> precio de artesanía </param>
        public MarketPrices(float wood, float crafts) 
        {
            this.wood = wood;
            this.crafts = crafts;

            prices = new float[2];
            prices[(int)Item.WOOD] = wood;
            prices[(int)Item.CRAFTS] = crafts;
        }
    }
    
    /// <summary>
    /// Estructura que da cuerpo a una petición de compra/venta en el mercado. Es el tipo
    /// de datos generado por un NPC al pedir una cantidad de un objeto concreto
    /// </summary>
    public struct Request
    {
        /// <summary>
        /// Tipo de petición
        /// </summary>
        public enum RequestType { BUY, SELL }

        /// <summary>
        /// Cliente que ha hecho petición
        /// </summary>
        public NPCInfo client;
        /// <summary>
        /// Item pedido
        /// </summary>
        public Item item;
        /// <summary>
        /// Cantidad pedida
        /// </summary>
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
    /// <summary>
    /// Tipos de objetos disponibles en tienda
    /// </summary>
    public enum Item { WOOD, CRAFTS }
    /// <summary>
    /// Tipos de Outputs que puede generar una petición de Compra.
    /// RESTING_STATE es el estado por defecto.
    /// </summary>
    public enum BuyRequestOutput { RESTING_STATE, ITEM_BOUGHT, NO_CLIENT, CLIENT_HAS_NO_MONEY, SHOP_HAS_NO_ITEM, MARKET_CLOSED }

    // References
    /// <summary>
    /// Referencia al NPC que está atendiendo en la tienda
    /// </summary>
    NPCInfo npcAttendingShop = null;
    /// <summary>
    /// Referencia al cliente cuya petición se está procesando actualmente
    /// </summary>
    NPCInfo client = null;

    // Parameters
    /// <summary>
    /// Precios de venta de objetos
    /// </summary>
    [SerializeField] MarketPrices sellPrices;
    /// <summary>
    /// Precios de compra de objetos
    /// </summary>
    [SerializeField] MarketPrices buyPrices;
    /// <summary>
    /// Tiempo que tarda el mercado en procesar una petición de un NPC
    /// </summary>
    [SerializeField] float timeToProcessRequest = 1.5f;

    // Variables
    /// <summary>
    /// Cola de peticiones de clientes
    /// </summary>
    Queue<Request> queue = new Queue<Request>();
    /// <summary>
    /// Items disponibles y su cantidad
    /// </summary>
    Dictionary<Item, int> itemAmount = new Dictionary<Item, int>();
    /// <summary>
    /// Indica si una petición está siendo procesada o no
    /// </summary>
    bool processingRequest = false;

    // Own Methods
    /// <summary>
    /// Petición de NPC al mercado
    /// </summary>
    /// <param name="client"> Cliente que ha hecho la petición </param>
    /// <param name="item"> Item que compra/vende </param>
    /// <param name="amount"> Cantidad pedida </param>
    /// <param name="type"> Tipo de petición </param>
    public void MakeRequest(NPCInfo client, Item item, int amount, Request.RequestType type)
    {
        if (amount == 0)
        {
            SimulationManager.Instance.RemoveTemporalMarket(this);
            client.SetLastBuyResult(BuyRequestOutput.SHOP_HAS_NO_ITEM);
            return;
        }
        if (npcAttendingShop == null) { client.SetLastBuyResult(BuyRequestOutput.MARKET_CLOSED); return; }
        queue.Enqueue(new Request(client, item, amount, type));
    }

    /// <summary>
    /// Comprueba si un Item, en una cantidad concreta existe en este mercado
    /// </summary>
    /// <param name="item"> Item </param>
    /// <param name="amount"> Cantidad </param>
    /// <returns></returns>
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

    /// <summary>
    /// Procesar petición de compra
    /// </summary>
    void Buy(NPCInfo client, Item item, int amount)
    {
        processingRequest = true;
        this.client = client;
        GiveMoneyToClient(item, amount);
    }

    /// <summary>
    /// Procesar petición de venta
    /// </summary>
    void Sell(NPCInfo client, Item item, int amount)
    {
        processingRequest = true;
        this.client = client;
        GetMoneyFromClient(item, amount);
    }

    /// <summary>
    /// Parte del procesado de una petición de venta.
    /// Comprueba si el cliente tiene dinero y actúa acorde
    /// </summary>
    void GetMoneyFromClient(Item item, int amount)
    {
        BuyRequestOutput result = BuyRequestOutput.RESTING_STATE;
        if (client != null)
        {
            if(IsItemAvaliable(item, amount))
            {
                float money = buyPrices.prices[(int)item] * amount;
                bool hasMoney = client.ChangeMoney(-money, true);
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
                result = BuyRequestOutput.SHOP_HAS_NO_ITEM;
                client.SetLastBuyResult(result);
                client = null;
                processingRequest = false;
            }
        }
        else
        {
            processingRequest = false;
            result = BuyRequestOutput.NO_CLIENT;
        }
    }

    /// <summary>
    /// Dar dinero al cliente de una petición de venta
    /// </summary>
    bool GiveMoneyToClient(Item item, int amount)
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

    /// <summary>
    /// Da valor al NPC que está atendiendo 
    /// </summary>
    /// <param name="info"> En caso de null, se da por entendido que se cierra el mercado </param>
    public void SetNPCAttending(NPCInfo info) {
        npcAttendingShop = info;
        if(info == null)
        {
            processingRequest = false;
            this.client = null;
            while(queue.Count > 0)
            {
                Request r = queue.Dequeue();
                r.client.SetLastBuyResult(BuyRequestOutput.MARKET_CLOSED);
            }
        }
    }

    public NPCInfo GetNPCAttending() { return npcAttendingShop; }

    /// <summary>
    /// Construyo los precios de Compra/Venta según los valores dados en el inspector
    /// </summary>
    private void OnValidate()
    {
        sellPrices = new MarketPrices(sellPrices.wood, sellPrices.crafts);
        buyPrices = new MarketPrices(buyPrices.wood, buyPrices.crafts);
    }

    /// <summary>
    /// Decide qué objetos están disponibles en función de su precio.
    /// En caso < 0, no hay de ese objeto disponible.
    /// En cualquier otro caso, hay Infinito hasta que se añada cantidad concreta
    /// </summary>
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

    /// <summary>
    /// Procesa petición de cliente después del tiempo necesario para procesar una petición
    /// </summary>
    /// <param name="request"> Petición a procesar </param>
    IEnumerator ProcessRequest(Request request)
    {
        processingRequest = true;
        yield return new WaitForSeconds(timeToProcessRequest);

        if (request.type == Request.RequestType.SELL)
            Buy(request.client, request.item, request.amount);
        else
            Sell(request.client, request.item, request.amount);

        processingRequest = false;
    }

    protected override void Start()
    {
        maxNpcs = 1;
        type = BuildingType.MARKET;
        SetUpAvaliableItems();
        base.Start();
    }

    /// <summary>
    /// Método al que llama el NPC atendiendo la tienda. 
    /// Atiende las peticiones cuando puede.
    /// </summary>
    public void Attend()
    {
        if (queue.Count > 0 && !processingRequest && npcAttendingShop)
        {
            Request request = queue.Dequeue();
            StartCoroutine(ProcessRequest(request));
        }
    }
}
