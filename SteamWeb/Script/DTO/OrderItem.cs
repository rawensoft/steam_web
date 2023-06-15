using SteamWeb.Extensions;

namespace SteamWeb.Script.DTO;

public record OrderItem
{
    public string Name { get; internal set; }
    public string Game { get; internal set; }
    public int Count { get; internal set; }
    public ulong ID { get; internal set; }
    public string PriceFull { get; internal set; }
    /// <summary>
    /// Возвращает цену в Int32 или -1, если не получилось спарсить
    /// </summary>
    public int Price
    {
        get
        {
            if (PriceFull.IsEmpty())
                return -1;
            var price = PriceFull.Split(' ')[0];
            if (int.TryParse(price.Replace(",", "").Replace(".", ""), out int result))
            {
                if (!price.Contains(",") && !price.Contains("."))
                    result *= 100;
                if (price.Contains(",0 ") || price.Contains(".0 "))
                    result *= 10;
                return result;
            }
            return -1;
        }
    }
}
