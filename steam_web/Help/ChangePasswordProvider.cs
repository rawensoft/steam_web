using AngleSharp.Html.Parser;
using SteamWeb.Auth.Interfaces;
using SteamWeb.Extensions;
using SteamWeb.Help.Enums;
using SteamWeb.Help.Interfaces;
using SteamWeb.Help.OwnershipProviders;
using SteamWeb.Script;
using SteamWeb.Web;
using System.Web;

namespace SteamWeb.Help;
public sealed class ChangePasswordProvider : IDisposable
{
    private readonly ISessionProvider _session;
    private readonly Proxy? _proxy;
    private const Script.Enums.TypeReset _reset = Script.Enums.TypeReset.Password;

    private bool _disposed = false;
    private ulong _stoken = 0;
    private uint _account = 0;
    private uint _issueid = 0;
    private string _referer = string.Empty;
    private string _html = string.Empty;
    private string _lastHash = string.Empty;
    private string _login = string.Empty;
    private HtmlParser _parser = new();
    private bool _canSkipMobileConf = true;
    private readonly List<PROVIDER_LIST> _acceptedConfs = new(5);
    private readonly List<PROVIDER_LIST> _skipedConfs = new(5);
    public IOwnershipProofProvider? Provider { get; private set; }
    /// <summary>
    /// Какой шаг завершён
    /// </summary>
    public CHANGE_STEP Step { get; private set; } = CHANGE_STEP.Waiting;

    public ChangePasswordProvider(ISessionProvider session, Proxy? proxy)
    {
        _session = session;
        _proxy = proxy;
    }

    /// <summary>
    /// Получение stoken и referer
    /// </summary>
    /// <returns>Успешный статус будет <see cref="STEP_STATUS.Ok"/>, иначе статус ошибки</returns>
    public STEP_STATUS Step1(CancellationToken cancellationToken)
    {
        if (Step != CHANGE_STEP.Waiting)
            return STEP_STATUS.WrongStep;

        var url = "https://help.steampowered.com/en/wizard/HelpChangePassword?redir=store/account/";
        var referer = "https://store.steampowered.com/account/";
        var request = new GetRequest(url, _proxy, _session, referer)
        {
            CancellationToken = cancellationToken,
            Timeout = 25000,
        };
        var response = Downloader.Get(request);
        if (!response.Success)
            return STEP_STATUS.ResponseError;

        _referer = request.Url;
        var uri = new Uri(request.Url);
        if (uri.Query == string.Empty)
            return STEP_STATUS.EmptyQuery;

        var query = HttpUtility.ParseQueryString(uri.Query);
        if (query.Get("need_password") != null)
            return STEP_STATUS.SignIn;

        var stoken = query.Get("s");
        if (stoken == null)
            return STEP_STATUS.BadQueryData;
        _stoken = stoken.ParseUInt64();

        var account = query.Get("account");
        if (account == null)
            return STEP_STATUS.BadQueryData;
        _account = account.ParseUInt32();

        var reset = query.Get("reset");
        if (reset == null || (byte)_reset != reset.ParseByte())
            return STEP_STATUS.BadQueryData;

        var issueid = query.Get("issueid");
        if (issueid == null)
            return STEP_STATUS.BadQueryData;
        _issueid = issueid.ParseUInt32();

        _html = response.Data!;
        Step = CHANGE_STEP.Step1;
        return STEP_STATUS.Ok;
    }
    /// <summary>
    /// Подтверждение владения аккаунтов, вызывается пока OWNER_STEP_STATUS будет Done
    /// </summary>
    public OWNER_STEP_STATUS Step2(CancellationToken cancellationToken)
    {
        if (Step != CHANGE_STEP.Step1)
            return OWNER_STEP_STATUS.WrongStep;

        if (_lastHash != string.Empty)
        {
            var request = new GetRequest("https://help.steampowered.com/en/" + _lastHash, _proxy, _session, _referer)
            {
                CancellationToken = cancellationToken,
            };
            var response = Downloader.Get(request);
            if (!response.Success)
                return OWNER_STEP_STATUS.ErrorRequest;
            _html = response.Data!;
            _lastHash = string.Empty;
        }

        var document = _parser.ParseDocument(_html);
        var wizard_content_wrapper = document.GetElementsByClassName("wizard_content_wrapper").FirstOrDefault();
        if (wizard_content_wrapper == null)
            return OWNER_STEP_STATUS.ErrorHtml;

        var help_page_title = wizard_content_wrapper.GetElementsByClassName("help_page_title").FirstOrDefault()?.TextContent.GetClearWebString();
        if (help_page_title == "Remove Steam Guard Mobile Authenticator")
            return OWNER_STEP_STATUS.PageRemoveSteamGuard;
        if (help_page_title == "Password Reset")
        {
            var pwreset_account_name = wizard_content_wrapper.GetElementsByClassName("pwreset_account_name").FirstOrDefault();
            if (pwreset_account_name == null)
                return OWNER_STEP_STATUS.ErrorHtml;

            _login = pwreset_account_name.TextContent!.GetClearWebString()!;
            Step = CHANGE_STEP.Step2;
            return OWNER_STEP_STATUS.Done;
        }

        var help_wizard_buttons = wizard_content_wrapper.GetElementsByClassName("help_wizard_button");
        if (!help_wizard_buttons.Any())
            return OWNER_STEP_STATUS.ErrorHtml;

        var accept = help_wizard_buttons[0];
        var accept_text = accept.Children[0].TextContent.GetClearWebString() ?? string.Empty;
        var accept_href = accept.GetAttribute("href")!;


        if (accept_text == "Send a confirmation to my Steam Mobile app")
        {
            var decline_text = help_wizard_buttons[1].Children[0].TextContent.GetClearWebString() ?? string.Empty;
            _canSkipMobileConf = decline_text != "I no longer have access to my Steam Guard Mobile Authenticator";

            var provider = new SteamMobileAppProofProvider(_session, _proxy, _stoken, accept_href, accept_href, _reset)
            {
                CanSkip = _canSkipMobileConf,
            };
            provider.OnProviderStatusChanged += OnProviderStatusChanged;
            Provider = provider;
            return OWNER_STEP_STATUS.NeedTakeAction;
        }
        else if (accept_text.StartsWith("Text an account verification code to my phone number ending in "))
        {
            var provider = new PhoneProofProvider(_session, _proxy, _stoken, accept_href, accept_href, _reset);
            provider.OnProviderStatusChanged += OnProviderStatusChanged;
            Provider = provider;
            return OWNER_STEP_STATUS.NeedTakeAction;
        }
        else if (accept_text.StartsWith("Email an account verification code to "))
        {
            var provider = new EmailProofProvider(_session, _proxy, _stoken, accept_href, accept_href, _reset);
            provider.OnProviderStatusChanged += OnProviderStatusChanged;
            Provider = provider;
            return OWNER_STEP_STATUS.NeedTakeAction;
        }
        if (help_page_title == "Verify Password")
        {
            var provider = new LoginProofProvider(_session, _proxy, _stoken, accept_href, accept_href, _reset);
            provider.OnProviderStatusChanged += OnProviderStatusChanged;
            Provider = provider;
            return OWNER_STEP_STATUS.NeedTakeAction;
        }

        return OWNER_STEP_STATUS.ErrorUnknownPage;
    }
    /// <summary>
    /// Выполняет изменение пароля на новый
    /// </summary>
    /// <param name="new_password">Пароль должен быть от 7 символов<br/>Рекомендация от Steam: Include a-z, A-Z, 0-9 or symbols for a stronger password</param>
    public CHANGE_PASSWORD_STEP Step3(string new_password, CancellationToken cancellationToken)
    {
        if (Step != CHANGE_STEP.Step2)
            return CHANGE_PASSWORD_STEP.WrongStep;

        if (new_password.Length <= 6)
            return CHANGE_PASSWORD_STEP.BadPassword;
        var request = new Models.DefaultRequest(_session, _proxy, cancellationToken);
        var response = AjaxHelp.AjaxCheckPasswordAvailable(request, new_password, _referer);
        if (!response.Available)
        {
            return CHANGE_PASSWORD_STEP.BadPassword;
        }

        var rsa = AjaxHelp.GetRSAKey(request, _login, _referer);
        if (!rsa.Success)
        {
            return CHANGE_PASSWORD_STEP.ErrorRSA;
        }

        var wizardRequest = request.CreateWizard(_stoken, _referer);
        var resultPassword = AjaxHelp.AjaxAccountRecoveryChangePassword(wizardRequest, _account, new_password, rsa);
        if (resultPassword.IsError)
            return CHANGE_PASSWORD_STEP.PasswordNotChanged;
        _lastHash = resultPassword.Hash!;
        Step = CHANGE_STEP.Done;

        var requestHash = new GetRequest("https://help.steampowered.com/en/" + _lastHash, _proxy, _session, _referer)
        {
            CancellationToken = cancellationToken,
        };
        var responseHash = Downloader.Get(requestHash);
        if (!responseHash.Success)
            return CHANGE_PASSWORD_STEP.Ok;

        _lastHash = string.Empty;
        return CHANGE_PASSWORD_STEP.Done;
    }

    public ChangeEmailProvider? CreateChangeEmailProvider()
    {
        if (Step != CHANGE_STEP.Done)
            return null;

        string hash = $"wizard/HelpWithLoginInfoReset/?s={_stoken}&account={_account}&reset=2";
        var emailProvider = new ChangeEmailProvider(_session, _proxy, _stoken, _account, "https://help.steampowered.com/en/" + hash, hash);
        return emailProvider;
    }

    private void OnProviderStatusChanged(PROVIDER_LIST provider, bool accepted, string hash)
    {
        if (accepted)
            _acceptedConfs.Add(provider);
        else
            _skipedConfs.Add(provider);
        _lastHash = hash;

        if (Provider != null)
        {
            Provider.OnProviderStatusChanged -= OnProviderStatusChanged;
            Provider = null;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            if (Provider != null)
                Provider.OnProviderStatusChanged -= OnProviderStatusChanged;
        }
    }
}