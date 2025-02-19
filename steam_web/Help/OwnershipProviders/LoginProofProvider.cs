using SteamWeb.Auth.Interfaces;
using SteamWeb.Extensions;
using SteamWeb.Help.Enums;
using SteamWeb.Help.Interfaces;
using SteamWeb.Script;
using SteamWeb.Script.Models;
using SteamWeb.Web;
using static SteamWeb.Help.Interfaces.IOwnershipProofProvider;

namespace SteamWeb.Help.OwnershipProviders;

public class LoginProofProvider : IOwnershipProofProvider
{
    private readonly ISessionProvider _session;
    private readonly Proxy? _proxy;
    private readonly string _href;
    private readonly string _referer;
    private readonly ulong _stoken;
    private readonly uint _account;
    private SteamRSA? _rsa;
    private readonly Script.Enums.TypeReset _reset;

    public PROVIDER_LIST Type { get; } = PROVIDER_LIST.Login;
    public string? Hash { get; private set; }
    public ACCEPT_PROVIDER_STATUS Status { get; private set; } = ACCEPT_PROVIDER_STATUS.NoAction;
    public STEP_ACCEPT_PROVIDER Step { get; private set; } = STEP_ACCEPT_PROVIDER.WaitingAcceptOrDecline;
    public string Login { get; set; } = string.Empty;
    public OnProviderStatusChangedHandler? OnProviderStatusChanged { get; set; }

    public LoginProofProvider(ISessionProvider session, Proxy? proxy, ulong stoken, string referer, string href, Script.Enums.TypeReset reset)
    {
        _session = session;
        _proxy = proxy;
        _stoken = stoken;
        _referer = referer;
        _href = href;
        _account = session.SteamID.ToSteamId32();
        _reset = reset;
    }

    public bool Accept(CancellationToken cancellationToken)
    {
        if (Step != STEP_ACCEPT_PROVIDER.WaitingAcceptOrDecline)
            return false;

        var requestHelp = new GetRequest(_href, _proxy, _session, _referer)
        {
            CancellationToken = cancellationToken,
        };
        var responseHelp = Downloader.Get(requestHelp);

        var response = AjaxHelp.GetRSAKey(new(_session, _proxy, cancellationToken), Login, _referer);
        if (response.Success)
        {
            _rsa = response;
            Status = ACCEPT_PROVIDER_STATUS.Waiting;
            Step = STEP_ACCEPT_PROVIDER.Verify;
        }
        return response.Success;
    }
    public VERIFY_STATUS Verify(string password, CancellationToken cancellationToken)
    {
        if (Step != STEP_ACCEPT_PROVIDER.Poll && Step != STEP_ACCEPT_PROVIDER.Verify)
            return VERIFY_STATUS.Error;

        var request = new AjaxInfoRequest
        {
            Method = Script.Enums.TypeMethod.Email,
            Proxy = _proxy,
            Session = _session,
            S = _stoken,
            Account = _account,
            CancellationToken = cancellationToken,
            Referer = _referer,
            Reset = _reset,
            Lost = Script.Enums.TypeLost.SkipEmail,
        };
        var response = AjaxHelp.AjaxAccountRecoveryVerifyPassword(request, _rsa!, password);
        if (response.IsError)
            return response.ErrorMsg == AjaxDefault.Error_SessionExpired ? VERIFY_STATUS.Expired : VERIFY_STATUS.Error;

        Hash = response.Hash;
        Status = ACCEPT_PROVIDER_STATUS.Accepted;
        Step = STEP_ACCEPT_PROVIDER.Finished;
        OnProviderStatusChanged?.Invoke(Type, true, Hash!);
        return VERIFY_STATUS.Success;
    }
    public bool Decline(CancellationToken cancellationToken) => false;
    public POLL_STATUS Poll(CancellationToken cancellationToken) => POLL_STATUS.NotAvailable;
    public bool ResendCode(CancellationToken cancellationToken) => false;
}
