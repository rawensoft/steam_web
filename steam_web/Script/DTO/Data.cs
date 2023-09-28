namespace SteamWeb.Script.DTO;

public class Data<T>
{
    public bool IsSuccess => success == 1;
    public int success { get; set; } = 0;
    public T? data { get; set; } = default;
}
