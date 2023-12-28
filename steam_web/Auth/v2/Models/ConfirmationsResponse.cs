namespace SteamWeb.Auth.v2.Models;

public class ConfirmationsResponse
{
    public bool success { get; init; } = false;
    public bool needauth { get; init; } = false;
    public string message { get; init; }
    public string detail { get; init; }
    public Confirmation[] conf { get; init; } = new Confirmation[0];
    
    public Confirmation this[int index]
	{
		get => conf[index];
		set => conf[index] = value;
	}
}

