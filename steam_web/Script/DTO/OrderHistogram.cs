using System.Text.Json;
using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;

public sealed record OrderHistogram
{
    /// <summary>
    /// 1 - success
    /// 16 - timeout
    /// </summary>
    [JsonPropertyName("success")] public byte Success { get; init; } = 0;
    [JsonIgnore] public bool Is_Success => Success == 1;
    /// <summary>
    /// Ошибка обычно означает TooManyRequests
    /// </summary>
    public bool E502L3 { get; init; } = false;
    /// <summary>
    /// An error occurred while sending the request.
    /// </summary>
    public string error { get; init; }
    public bool InternalError { get; init; } = false;
    public string price_suffix { get; init; }
    public string price_prefix { get; init; }
    public string highest_buy_order { get; init; }
    public string lowest_sell_order { get; init; }
    public float graph_min_x { get; init; }
    public float graph_max_y { get; init; }
    public float graph_max_x { get; init; }
    /// <summary>
    /// Ордеры на покупку предмета
    /// </summary>
    [JsonPropertyName("buy_order_graph")] public JsonElement[][] Buy_Orders { get; init; } = new JsonElement[0][];
    /// <summary>
    /// Ордеры на продажу предмета
    /// </summary>
    [JsonPropertyName("sell_order_graph")] public JsonElement[][] Sell_Orders { get; init; } = new JsonElement[0][];

    public OrderHistogram() { }
    public OrderHistogram(string error)
    {
        if (error.Contains("<div id=\"status\">E502 L3</div>"))
            E502L3 = true;
        this.error = error;
    }

    /// <summary>
    /// Выдаёт минимальную цену ордера на продажу
    /// </summary>
    /// <returns>-1 если нет ордеров</returns>
    public float GetMinPriceOrder()
    {
        if (lowest_sell_order == null)
            return -1;
        return (float)Math.Round(float.Parse(lowest_sell_order) / 100f, 2);
    }
    /// <summary>
    /// Получить указанную цену по индексу
    /// </summary>
    /// <param name="index"></param>
    /// <returns>Возвращает 0, если по индекс вышел за пределы массива, иначе цену.</returns>
    public float GetSellPrice(int index = 0)
    {
        if (Sell_Orders.Length < index + 1)
            return 0;
        ref var price = ref Sell_Orders[index][0];
        return (float)price.GetDouble();
    }
    public float[] GetSellPrices()
    {
        var length = Sell_Orders.Length;
        var arr = new float[length];
        for (int i = 0; i < length; i++)
            arr[i] = (float)Sell_Orders[i][0].GetDouble();
        return arr;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns>Возвращает -1, если по индекс вышел за пределы массива, иначе количество предметов на продаже.</returns>
    public int GetSellCount(int index = 0)
    {
        if (Sell_Orders.Length < index + 1)
            return -1;
        ref var count = ref Sell_Orders[index][1];
        return count.GetInt32();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns>Возвращает -1, если нет выставленных предметов по заданной цене или количество ордеров 0, иначе количество предметов на продаже.</returns>
    public int GetSellCount(double price)
    {
        if (Sell_Orders.Length == 0)
            return -1;
        for (int i = 0; i < Sell_Orders.Length; i++)
        {
            ref var item = ref Sell_Orders[i];
            var item_price = item[0].GetDouble();
            if (item_price == price)
                return item[1].GetInt32();
        }
        return -1;
    }
    public int GetAllSellsCount()
    {
        if (Sell_Orders.Length == 0)
            return 0;
        int count = 0;
        foreach (var item in Sell_Orders)
			count += item[1].GetInt32();
		return count;
	}
	public Dictionary<float, uint> GetSellOrders()
	{
		var dict = new Dictionary<float, uint>(Sell_Orders.Length + 1);
		foreach (var order in Sell_Orders)
		{
			var price = (float)order[0].GetDouble();
			var count = order[1].GetUInt32();
			dict.Add(price, count);
		}
		return dict;
	}

	/// <summary>
	/// Выдаёт Максимальную цену ордера на покупку
	/// </summary>
	/// <returns>-1 если нет ордеров</returns>
	public float GetMaxBuyOrder()
    {
        if (highest_buy_order == null)
            return -1;
		return (float)Math.Round(float.Parse(highest_buy_order) / 100f, 2);
    }
    /// <summary>
    /// Получить указанную цену по индексу
    /// </summary>
    /// <param name="index"></param>
    /// <returns>Возвращает 0, если по индекс вышел за пределы массива, иначе цену.</returns>
    public float GetBuyPrice(int index = 0)
    {
        if (Buy_Orders.Length < index + 1)
            return 0;
        var price = Buy_Orders[index][0];
        return (float)price.GetDouble();
    }
    public float[] GetBuyPrices()
    {
        var length = Buy_Orders.Length;
        var arr = new float[length];
        for (int i = 0; i < length; i++)
            arr[i] = (float)Buy_Orders[i][0].GetDouble();
        return arr;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns>Возвращает -1, если по индекс вышел за пределы массива, иначе количество предметов на покупку.</returns>
    public int GetBuyCount(int index = 0)
    {
        if (Buy_Orders.Length < index + 1)
            return -1;
        ref var count = ref Buy_Orders[index][1];
        return count.GetInt32();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns>Возвращает -1, если нет запросов на покупку по заданной цене или количество ордеров 0, иначе количество предметов на покупку.</returns>
    public int GetBuyCount(float price)
    {
        if (Buy_Orders.Length == 0)
            return -1;
        for (int i = 0; i < Buy_Orders.Length; i++)
        {
            ref var item = ref Buy_Orders[i];
            var item_price = item[0].GetDouble();
            if (item_price == price)
                return item[1].GetInt32();
        }
        return -1;
    }
    public int GetAllBuysCount()
    {
        if (Buy_Orders.Length == 0)
            return 0;
        int count = 0;
        foreach (var item in Buy_Orders)
            count += item[1].GetInt32();
        return count;
    }
    public Dictionary<float, uint> GetBuyOrders()
    {
        var dict = new Dictionary<float, uint>(Buy_Orders.Length + 1);
        foreach (var order in Buy_Orders)
        {
            var price = (float)order[0].GetDouble();
			var count = order[1].GetUInt32();
            dict.Add(price, count);
		}
        return dict;
    }
}