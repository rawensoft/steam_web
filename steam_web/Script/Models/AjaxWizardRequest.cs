using SteamWeb.Models;

namespace SteamWeb.Script.Models;
public class AjaxWizardRequest : DefaultRequest
{
    public string S { get; init; }
    public string? Referer { get; init; }
}