namespace SteamWeb.Models;
public sealed class WebApiKey
{
    public string Key { get; init; } = null;
    /// <summary>
    /// Domain или ErrorText
    /// </summary>
    public string Domain { get; init; } = null;
    public bool IsError => Key == null && Domain != null;

    public WebApiKey() { }
    public WebApiKey(string Domain) => this.Domain = Domain;
    public WebApiKey(string Domain, string Key) : this(Domain) => this.Key = Key;
}
