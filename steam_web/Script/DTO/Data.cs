using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;

public class Data<T> : Data
{
	[JsonPropertyName("Data")]
	public T? data { get; set; } = default;
}
public class Data
{
	[JsonIgnore]
	public bool IsSuccess => Success == EResult.OK;

	[JsonPropertyName("Data")]
	public EResult Success { get; set; } = EResult.Invalid;
}