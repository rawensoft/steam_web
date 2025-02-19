using SteamWeb.Auth.Interfaces;
using SteamWeb.Extensions;
using SteamWeb.Help.Enums;
using SteamWeb.Help.Interfaces;
using SteamWeb.Script;
using SteamWeb.Script.Models;
using SteamWeb.Web;
using static SteamWeb.Help.Interfaces.IOwnershipProofProvider;

namespace SteamWeb.Help.OwnershipProviders;

public class PhoneProofProvider : IOwnershipProofProvider
{
    private readonly ISessionProvider _session;
    private readonly Proxy? _proxy;
    private readonly string _href;
    private readonly string _referer;
    private readonly ulong _stoken;
    private readonly uint _account;
    private readonly Script.Enums.TypeReset _reset;

    public PROVIDER_LIST Type { get; } = PROVIDER_LIST.Phone;
    public string? Hash { get; private set; }
    public ACCEPT_PROVIDER_STATUS Status { get; private set; } = ACCEPT_PROVIDER_STATUS.NoAction;
    public STEP_ACCEPT_PROVIDER Step { get; private set; } = STEP_ACCEPT_PROVIDER.WaitingAcceptOrDecline;
    public OnProviderStatusChangedHandler? OnProviderStatusChanged { get; set; }

    public PhoneProofProvider(ISessionProvider session, Proxy? proxy, ulong stoken, string referer, string href, Script.Enums.TypeReset reset)
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

        var request = new AjaxInfoRequest
        {
            Method = Script.Enums.TypeMethod.Phone,
            Proxy = _proxy,
            Session = _session,
            S = _stoken,
            Account = _account,
            CancellationToken = cancellationToken,
            Referer = _referer,
            Reset = _reset,
        };
        var response = AjaxHelp.AjaxSendAccountRecoveryCode(request);
        if (response.Success)
        {
            Status = ACCEPT_PROVIDER_STATUS.Waiting;
            Step = STEP_ACCEPT_PROVIDER.Verify;
        }
        return response.Success;
    }
    public bool ResendCode(CancellationToken cancellationToken)
    {
        if (Step != STEP_ACCEPT_PROVIDER.Verify)
            return false;
        var request = new AjaxInfoRequest
        {
            Method = Script.Enums.TypeMethod.Phone,
            Proxy = _proxy,
            Session = _session,
            S = _stoken,
            Account = _account,
            CancellationToken = cancellationToken,
            Referer = _referer,
            Reset = _reset,
            Lost = Script.Enums.TypeLost.RecoveryConfirm,
        };
        var response = AjaxHelp.AjaxSendAccountRecoveryCode(request);
        return response.Success;
    }
    public VERIFY_STATUS Verify(string code, CancellationToken cancellationToken)
    {
        if (Step != STEP_ACCEPT_PROVIDER.Poll && Step != STEP_ACCEPT_PROVIDER.Verify)
            return VERIFY_STATUS.Error;

        var request = new AjaxInfoRequest
        {
            Method = Script.Enums.TypeMethod.Phone,
            Proxy = _proxy,
            Session = _session,
            S = _stoken,
            Account = _account,
            CancellationToken = cancellationToken,
            Referer = _referer,
            Reset = _reset,
            Lost = Script.Enums.TypeLost.RecoveryConfirm,
        };
        var response = AjaxHelp.AjaxVerifyAccountRecoveryCode(request, code);
        if (response.IsError)
            return response.ErrorMsg == AjaxDefault.Error_SessionExpired ? VERIFY_STATUS.Expired : VERIFY_STATUS.Error;

        Hash = response.Hash;
        Status = ACCEPT_PROVIDER_STATUS.Accepted;
        Step = STEP_ACCEPT_PROVIDER.Finished;
        OnProviderStatusChanged?.Invoke(Type, true, Hash!);
        return VERIFY_STATUS.Success;
    }
    public bool Decline(CancellationToken cancellationToken)
    {
        if (Step != STEP_ACCEPT_PROVIDER.WaitingAcceptOrDecline)
            return false;

        var request = new AjaxInfoRequest
        {
            Method = Script.Enums.TypeMethod.Phone,
            Proxy = _proxy,
            Session = _session,
            S = _stoken,
            Account = _account,
            CancellationToken = cancellationToken,
            Referer = _referer,
            Reset = _reset,
            Lost = Script.Enums.TypeLost.RecoveryConfirm,
        };
        var response = AjaxHelp.AjaxAccountRecoveryGetNextStep(request);
        if (response.Redirect.IsEmpty())
            return false;

        Hash = response.Redirect;
        Status = ACCEPT_PROVIDER_STATUS.Declined;
        Step = STEP_ACCEPT_PROVIDER.Finished;
        OnProviderStatusChanged?.Invoke(Type, false, Hash!);
        return true;
    }
    public POLL_STATUS Poll(CancellationToken cancellationToken) => POLL_STATUS.NotAvailable;
}
