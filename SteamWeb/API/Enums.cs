namespace SteamWeb.API.Enums;
public enum VANITY_TYPE : byte { IndividualProfile = 1, Group = 2, OfficialGameGroup = 3 }
public enum ETradeOfferState : byte
{
    /// <summary>
    /// Invalid
    /// </summary>
    k_ETradeOfferStateInvalid = 1,
    /// <summary>
    /// This trade offer has been sent, neither party has acted on it yet.
    /// </summary>
    k_ETradeOfferStateActive = 2,
    /// <summary>
    /// The trade offer was accepted by the recipient and items were exchanged.
    /// </summary>
    k_ETradeOfferStateAccepted = 3,
    /// <summary>
    /// The recipient made a counter offer
    /// </summary>
    k_ETradeOfferStateCountered = 4,
    /// <summary>
    /// The trade offer was not accepted before the expiration date
    /// </summary>
    k_ETradeOfferStateExpired = 5,
    /// <summary>
    /// The sender cancelled the offer
    /// </summary>
    k_ETradeOfferStateCanceled = 6,
    /// <summary>
    /// The recipient declined the offer
    /// </summary>
    k_ETradeOfferStateDeclined = 7,
    /// <summary>
    /// Some of the items in the offer are no longer available (indicated by the missing flag in the output)
    /// </summary>
    k_ETradeOfferStateInvalidItems = 8,
    /// <summary>
    /// The offer hasn't been sent yet and is awaiting email/mobile confirmation. The offer is only visible to the sender.
    /// </summary>
    k_ETradeOfferStateCreatedNeedsConfirmation = 9,
    /// <summary>
    /// Either party canceled the offer via email/mobile. The offer is visible to both parties, even if the sender canceled it before it was sent.
    /// </summary>
    k_ETradeOfferStateCanceledBySecondFactor = 10,
    /// <summary>
    /// The trade has been placed on hold. The items involved in the trade have all been removed from both parties' inventories and will be automatically delivered in the future.
    /// </summary>
    k_ETradeOfferStateInEscrow = 11,
}
public enum ETradeOfferConfirmationMethod : byte
{
    /// <summary>
    /// Invalid
    /// </summary>
    k_ETradeOfferConfirmationMethod_Invalid = 0,
    /// <summary>
    /// An email was sent with details on how to confirm the trade offer
    /// </summary>
    k_ETradeOfferConfirmationMethod_Email = 1,
    /// <summary>
    /// The trade offer may be confirmed via the mobile app
    /// </summary>
    k_ETradeOfferConfirmationMethod_MobileApp = 2
}
public enum ETradeStatus : byte
{
    /// <summary>
    /// Trade has just been accepted/confirmed, but no work has been done yet
    /// </summary>
    k_ETradeStatus_Init,
    /// <summary>
    /// Steam is about to start committing the trade
    /// </summary>
    k_ETradeStatus_PreCommitted,
    /// <summary>
    /// The items have been exchanged
    /// </summary>
    k_ETradeStatus_Committed,
    /// <summary>
    /// All work is finished
    /// </summary>
    k_ETradeStatus_Complete,
    /// <summary>
    /// Something went wrong after Init, but before Committed, and the trade has been rolled back
    /// </summary>
    k_ETradeStatus_Failed,
    /// <summary>
    /// A support person rolled back the trade for one side
    /// </summary>
    k_ETradeStatus_PartialSupportRollback,
    /// <summary>
    /// A support person rolled back the trade for both sides
    /// </summary>
    k_ETradeStatus_FullSupportRollback,
    /// <summary>
    /// A support person rolled back the trade for some set of items
    /// </summary>
    k_ETradeStatus_SupportRollback_Selective,
    /// <summary>
    /// We tried to roll back the trade when it failed, but haven't managed to do that for all items yet
    /// </summary>
    k_ETradeStatus_RollbackFailed,
    /// <summary>
    /// We tried to roll back the trade, but some failure didn't go away and we gave up
    /// </summary>
    k_ETradeStatus_RollbackAbandoned,
    /// <summary>
    /// Trade is in escrow
    /// </summary>
    k_ETradeStatus_InEscrow,
    /// <summary>
    /// A trade in escrow was rolled back
    /// </summary>
    k_ETradeStatus_EscrowRollback,
}
