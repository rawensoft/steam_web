using SteamWeb.Auth.Interfaces;
using SteamWeb.Extensions;
using SteamWeb.Help.Enums;
using SteamWeb.Help.Interfaces;
using SteamWeb.Script;
using SteamWeb.Script.Models;
using SteamWeb.Web;
using static SteamWeb.Help.Interfaces.IOwnershipProofProvider;

namespace SteamWeb.Help.OwnershipProviders;

public class SteamMobileAppProofProvider : IOwnershipProofProvider
{
    private readonly ISessionProvider _session;
    private readonly Proxy? _proxy;
    private readonly string _referer;
    private readonly string _href;
    private readonly ulong _stoken;
    private readonly uint _account;
    private readonly Script.Enums.TypeReset _reset;

    public PROVIDER_LIST Type { get; } = PROVIDER_LIST.SteamMobileApp;
    public string? Hash { get; private set; }
    public ACCEPT_PROVIDER_STATUS Status { get; private set; } = ACCEPT_PROVIDER_STATUS.NoAction;
    public STEP_ACCEPT_PROVIDER Step { get; private set; } = STEP_ACCEPT_PROVIDER.WaitingAcceptOrDecline;
    public OnProviderStatusChangedHandler? OnProviderStatusChanged { get; set; }
    /// <summary>
    /// Можно ли пропустить подтверждение текущего провайдера
    /// </summary>
    public bool CanSkip { get; internal set; } = false;

    public SteamMobileAppProofProvider(ISessionProvider session, Proxy? proxy, ulong stoken, string referer, string href, Script.Enums.TypeReset reset)
    {
        _session = session;
        _proxy = proxy;
        _stoken = stoken;
        _referer = referer;
        _href = href;
        _account = session.SteamID.ToSteamId32();
        _reset = reset;
    }

    /// <summary>
    ///  Вызывается для загрузки данных с этого провайдера, обычно нужно для получения доп данных что этот провайдер выбран
    /// </summary>
    /// <returns>true запрос выполнен и подтверждение отправлено в Steam Mobile App; false в других случаях</returns>
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
            Method = Script.Enums.TypeMethod.Mobile,
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
            Step = STEP_ACCEPT_PROVIDER.Poll;
        }
        return response.Success;
    }
    /// <summary>
    /// Проверяет выполнено ли подтверждение
    /// <code>
    /// using var mres = new ManualResetEventSlim(false);
    /// for (var g_datePollStart = DateTime.UtcNow; (DateTime.UtcNow - g_datePollStart).TotalSeconds >= 600000; mres.Wait(3000))
    /// {
    ///     var status = Poll();
    ///     if (status == POLL_STATUS.Success)
    ///         break;
    ///     else if (status == POLL_STATUS.Expired)
    ///         return;
    /// }
    /// </code>
    /// </summary>
    /// <returns>Статус poll'инга</returns>
    public POLL_STATUS Poll(CancellationToken cancellationToken)
    {
        if (Step == STEP_ACCEPT_PROVIDER.Verify)
            return POLL_STATUS.Success;

        if (Step != STEP_ACCEPT_PROVIDER.Poll)
            return POLL_STATUS.Error;

        var request = new AjaxInfoRequest
        {
            Method = Script.Enums.TypeMethod.Mobile,
            Proxy = _proxy,
            Session = _session,
            S = _stoken,
            Account = _account,
            CancellationToken = cancellationToken,
            Referer = _referer,
            Reset = _reset,
            Lost = Script.Enums.TypeLost.RecoveryConfirm,
        };
        var response = AjaxHelp.AjaxPollAccountRecoveryConfirmation(request);
        if (response.Continue)
            return POLL_STATUS.Continue;
        if (response.Success)
        {
            Step = STEP_ACCEPT_PROVIDER.Verify;
            return POLL_STATUS.Success;
        }

        return response.ErrorMsg == AjaxDefault.Error_SessionExpired ? POLL_STATUS.Expired : POLL_STATUS.Error;
    }
    public VERIFY_STATUS Verify(string code, CancellationToken cancellationToken)
    {
        if (Step != STEP_ACCEPT_PROVIDER.Poll && Step != STEP_ACCEPT_PROVIDER.Verify)
            return VERIFY_STATUS.Error;

        var request = new AjaxInfoRequest
        {
            Method = Script.Enums.TypeMethod.Mobile,
            Proxy = _proxy,
            Session = _session,
            S = _stoken,
            Account = _account,
            CancellationToken = cancellationToken,
            Referer = _referer,
            Reset = _reset,
            Lost = Script.Enums.TypeLost.RecoveryConfirm,
        };
        var response = AjaxHelp.AjaxVerifyAccountRecoveryCodeAsync(request, string.Empty).Result;
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
        if (!CanSkip)
            return false;
        if (Step != STEP_ACCEPT_PROVIDER.WaitingAcceptOrDecline)
            return false;
        var request = new AjaxInfoRequest
        {
            Method = Script.Enums.TypeMethod.Mobile,
            Proxy = _proxy,
            Session = _session,
            S = _stoken,
            Account = _account,
            CancellationToken = cancellationToken,
            Referer = _referer,
            Reset = _reset,
            Lost = Script.Enums.TypeLost.RecoveryConfirm,
        };
        var response = AjaxHelp.AjaxAccountRecoveryGetNextStepAsync(request).Result;
        if (response.Redirect.IsEmpty())
            return false;
        Hash = response.Redirect;
        Status = ACCEPT_PROVIDER_STATUS.Declined;
        Step = STEP_ACCEPT_PROVIDER.Finished;
        OnProviderStatusChanged?.Invoke(Type, false, Hash!);
        return true;
    }
    public bool ResendCode(CancellationToken cancellationToken) => true;
}
