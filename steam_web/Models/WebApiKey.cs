using System.Text.Json.Serialization;

namespace SteamWeb.Models;
public sealed class WebApiKey
{
    /// <summary>
    /// Api ключа или заглушка в виде большого количества нулей
    /// </summary>
    public string? Key { get; init; } = null;
    /// <summary>
    /// Domain или ErrorText
    /// </summary>
    public string? Domain { get; init; } = null;
	/// <summary>
	/// Не null если <see cref="IsError"/>
	/// </summary>
	public string? RawHtml { get; internal set; }
	public bool IsError => Key == null && Domain != null;

    [JsonConstructor]
    public WebApiKey() { }
    public WebApiKey(string Domain) => this.Domain = Domain;
    public WebApiKey(string Domain, string Key) : this(Domain) => this.Key = Key;
}
