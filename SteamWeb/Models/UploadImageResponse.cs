namespace SteamWeb.Models;
public record UploadImageResponse
{
    public bool success { get; init; } = false;
    public ImageItem images { get; init; }
    public string hash { get; init; }
    public string message { get; init; } = "Неизвестная ошибка";
}
