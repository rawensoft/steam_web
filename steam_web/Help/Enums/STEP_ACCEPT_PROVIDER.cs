namespace SteamWeb.Help.Enums;

public enum STEP_ACCEPT_PROVIDER : byte
{
    /// <summary>
    /// Ожидается <see cref="Interfaces.IOwnershipProofProvider.Accept(CancellationToken)"/> или <see cref="Interfaces.IOwnershipProofProvider.Decline(CancellationToken)"/>
    /// </summary>
    WaitingAcceptOrDecline,
    /// <summary>
    /// Можно выполнить <see cref="Interfaces.IOwnershipProofProvider.Poll(CancellationToken)"/>, либо вызвать <see cref="Interfaces.IOwnershipProofProvider.Verify(string, CancellationToken)"/>
    /// </summary>
    Poll,
    /// <summary>
    /// Ожидается вызов <see cref="Interfaces.IOwnershipProofProvider.Verify(string, CancellationToken)"/>
    /// </summary>
    Verify,
    /// <summary>
    /// Этот провайдер закончил работу
    /// </summary>
    Finished,
}
