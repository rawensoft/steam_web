namespace SteamWeb.Auth;
public enum LinkResult : byte
{
    MustProvidePhoneNumber, //No phone number on the account
    MustRemovePhoneNumber, //A phone number is already on the account
    MustConfirmEmail, //User need to click link from confirmation email
    AwaitingFinalization, //Must provide an SMS code
    GeneralFailure, //General failure (really now!)
    AuthenticatorPresent,
    AuthenticatorNotAllowed, // Not allowed to have authentificator
    TooManyRequests // Too many times sms code sended
}
public enum FinalizeResult : byte
{
    AuthenticatorNotAllowed,
    BadSMSCode,
    UnableToGenerateCorrectCodes,
    Success,
    GeneralFailure
}
public enum RESPONSE_STATUS : byte { Success, Error, WGTokenInvalid, WGTokenExpired, WrongPlatform }
public enum ConfirmationType : byte
{
    Unknown,
    TradeOffer = 2,
    MarketSellTransaction = 3,
    ChangeNumber = 5,
    AccountRecovery = 6
}

public enum LoginResult : byte
{
    LoginOkay,
    GeneralFailure,
    BadRSA,
    BadCredentials,
    NeedCaptcha,
    Need2FA,
    NeedEmail,
    TooManyFailedLogins,
    SessionIDNotFound,
}
public enum SignInPlatform : byte { Desktop, Mobile }