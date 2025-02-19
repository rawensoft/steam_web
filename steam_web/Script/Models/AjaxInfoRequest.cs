using SteamWeb.Script.Enums;

namespace SteamWeb.Script.Models;
public class AjaxInfoRequest : AjaxWizardRequest
{
    public TypeMethod Method { get; init; }
    public TypeReset Reset { get; init; }
    public TypeLost Lost { get; init; }
    public uint Account { get; init; }
}