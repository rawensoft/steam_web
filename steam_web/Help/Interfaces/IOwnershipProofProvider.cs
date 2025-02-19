using SteamWeb.Help.Enums;

namespace SteamWeb.Help.Interfaces;

/// <summary>
/// Интерфейс используется для унифицирования провайдеров подтверждения владения аккаунтов<para/>
/// При получении провайдера необходимо пойти по одному из двух путей:<br/>
/// 1. Вызвать Decline для выбора другого провайдера<br/>
/// 2. Вызвать Accept и Verify, опционально Poll
/// </summary>
public interface IOwnershipProofProvider
{
    // Содержит url hash для перехода на след шаг
    internal string? Hash { get; }
    /// <summary>
    /// Текущий статус
    /// </summary>
    public ACCEPT_PROVIDER_STATUS Status { get; }
    /// <summary>
    /// Тип этого провайдера подтверждения
    /// </summary>
    public PROVIDER_LIST Type { get; }
    /// <summary>
    /// Текущий шаг
    /// </summary>
    internal STEP_ACCEPT_PROVIDER Step { get; }
    public delegate void OnProviderStatusChangedHandler(PROVIDER_LIST provider, bool accepted, string hash);
    /// <summary>
    /// Событие вызывается, когда провайдер нужно закрыть, т.к. в нём уже выбрано действие
    /// </summary>
    public OnProviderStatusChangedHandler? OnProviderStatusChanged { get; set; }

    /// <summary>
    /// Вызывается для загрузки данных с этого провайдера, обычно нужно для получения доп данных что этот провайдер выбран
    /// </summary>
    /// <returns>true если можно переходить к следующему выполнению, иначе false</returns>
    public bool Accept(CancellationToken cancellationToken);
    /// <summary>
    /// Опционально вызывается для проверки подтверждения со стороны steam
    /// </summary>
    /// <returns><see cref="POLL_STATUS.Success"/>, когда нужно выполнить <see cref="Verify(string, CancellationToken)"/><br/>
    /// <see cref="POLL_STATUS.Continue"/> для продолжения вызова <see cref="Poll(CancellationToken)"/><br/>
    /// Остальные статусы при ошибка</returns>
    public POLL_STATUS Poll(CancellationToken cancellationToken);
    /// <summary>
    /// Отправляет код на почту\телефон ещё раз
    /// </summary>
    /// <returns>true код отправлен заново</returns>
    public bool ResendCode(CancellationToken cancellationToken);
    /// <summary>
    /// Проверяет подтверждение этого провайдера
    /// </summary>
    /// <param name="code">Строка может быть пустой, содержать код с телефона\почты, либо пароль от аккаунта, в зависимости от провайдера</param>
    /// <returns><see cref="VERIFY_STATUS.Success"/> если выполнено подтверждение через этот провайдер, либо статус ошибку</returns>
    public VERIFY_STATUS Verify(string code, CancellationToken cancellationToken);
    /// <summary>
    /// Выполняет пропуск этого провайдера
    /// </summary>
    /// <returns>true отклонение этого провайдера выполнено</returns>
    public bool Decline(CancellationToken cancellationToken);
}