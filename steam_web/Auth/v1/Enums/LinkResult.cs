namespace SteamWeb.Auth.v1.Enums;

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
