namespace SteamWeb.Script.Models;
public class AjaxValidPhone
{
    public bool success { get; set; } = false;
    public string? number { get; set; } = null;
    public bool is_valid { get; set; } = false;
    public bool is_voip { get; set; } = false;
    public bool is_fixed { get; set; } = false;
}
