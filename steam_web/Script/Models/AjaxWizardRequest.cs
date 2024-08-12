namespace SteamWeb.Script.Models;
public class AjaxWizardRequest : AjaxDefaultRequest
{
    public string S { get; init; }
    public string? Referer { get; init; }
}