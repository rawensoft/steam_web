using System.ComponentModel;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using SteamWeb.Extensions;

namespace SteamWeb.Web;
public class Proxy : IWebProxy, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private bool _useProxy = true;
    private string? _ip;
    private int _port;
    private string? _username;
    private string? _password;
    private readonly ProxyType _type = ProxyType.HTTP;

    /// <summary>
    /// True если IP.IsNoneEmpty && Port > 0 && UseProxy == True
    /// </summary>
    public bool UseProxy
    {
        get
        {
            if (_ip.IsEmpty() || _port == 0 || !_useProxy || IsBadCredentials) return false;
            return true;
        }
        set { _useProxy = value; Property("UseProxy"); }
    }
    public string? IP
    {
        get => _ip;
        set
        {
            _ip = !string.IsNullOrEmpty(value) && IsValidIPv4(value) ? value : null;
            Property("IP");
        }
    }
    public int Port
    {
        get => _port;
        set
        {
            if (value > 65535)
                _port = 65535;
            else if (0 > value)
                _port = 0;
            else _port = value;
            Property("Port");
        }
    }
    public string? Username
    {
        get => _username;
        set
        {
            _username = value;
            Property("Username");
            Property("IsBadCredentials");
            Property("IsCredentials");
            Property("Credentials");
        }
    }
    public string? Password
    {
        get => _password;
        set
        {
            _password = value;
            Property("Password");
            Property("IsBadCredentials");
            Property("IsCredentials");
            Property("Credentials");
        }
    }
    [JsonIgnore]
    public bool IsCredentials => !string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password);
    /// <summary>
    /// Показывает установлен ли только логин или только пароль
    /// </summary>
    [JsonIgnore]
    public bool IsBadCredentials => (string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password)) || (!string.IsNullOrEmpty(_username) && string.IsNullOrEmpty(_password));
    [JsonIgnore]
    public ICredentials? Credentials
    {
        get => !IsCredentials ? null : new NetworkCredential(_username, _password);
        set
        {
            var cred = value as NetworkCredential;
            if (cred != null)
            {
                _username = cred.UserName;
                _password = cred.Password;
            }
            else
            {
                _username = null;
                _password = null;
            }
        }
    }
    public ProxyType Type => _type;

    public Proxy() { }
    public Proxy(string ip, int port)
    {
        IP = ip;
        Port = port;
    }
    public Proxy(string ip, int port, ProxyType type)
    {
        IP = ip;
        Port = port;
        _type = type;
    }

    public Proxy(string ip, string port) : this(ip, port.GetOnlyDigit().ParseInt32()) { }
    public Proxy(string ip, string port, ProxyType type) : this(ip, port.GetOnlyDigit().ParseInt32(), type) { }

    public Proxy(string ip, int port, string username, string password) : this(ip, port)
    {
        _username = !string.IsNullOrEmpty(username) ? username : null;
        _password = !string.IsNullOrEmpty(password) ? password : null;
    }
    public Proxy(string ip, string port, string username, string password) : this(ip, port)
    {
        _username = !string.IsNullOrEmpty(username) ? username : null;
        _password = !string.IsNullOrEmpty(password) ? password : null;
    }

    public Proxy(string ip, int port, string username, string password, ProxyType type) : this(ip, port, type)
    {
        _username = !string.IsNullOrEmpty(username) ? username : null;
        _password = !string.IsNullOrEmpty(password) ? password : null;
    }
    public Proxy(string ip, string port, string username, string password, ProxyType type) : this(ip, port, type)
    {
        _username = !string.IsNullOrEmpty(username) ? username : null;
        _password = !string.IsNullOrEmpty(password) ? password : null;
    }

    public Uri? GetProxy(Uri destination)
    {
        if (!UseProxy || string.IsNullOrEmpty(IP) || Port == 0)
            return null;
        switch (_type)
        {
            case ProxyType.Socks5:
                return new($"socks5://{IP}:{Port}");
            case ProxyType.Socks4:
                return new($"socks4://{IP}:{Port}");
            default:
                return new($"http://{IP}:{Port}");
        }
    }
    public bool IsBypassed(Uri host) => !UseProxy || string.IsNullOrEmpty(IP) || Port == 0;
    private void Property(string name) => PropertyChanged?.Invoke(this, new(name));

    public static bool IsLocalIPv4(string ip) => new Regex(@"(^127\.\d{0,255})|(^10\.\d{0,255})|(^172\.16)|(^192\.168)|(^169\.254)\.\d{0,255}\.\d{0,255}$", RegexOptions.Compiled).IsMatch(ip);
    public static bool IsValidIPv4(string ip) => IPAddress.TryParse(ip, out var address) && address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
}
