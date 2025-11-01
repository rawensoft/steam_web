using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;

public class DataResponse<T> : DataResponse
{
	[JsonPropertyName("data")]
	public T? Data { get; set; } = default;
}
public class DataResponse
{
	[JsonIgnore]
	public bool IsSuccess => Success == EResult.OK;

	[JsonPropertyName("success")]
	public EResult Success { get; set; } = EResult.Invalid;
}