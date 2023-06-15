public enum EAuthSessionGuardType
{
    Unknown = 0,
    None = 1,
    /// <summary>
    /// Guard с письма на почту
    /// </summary>
    EmailCode = 2,
    /// <summary>
    /// 2FA с мобильного приложения
    /// </summary>
    DeviceCode = 3,
    DeviceConfirmation = 4,
    EmailConfirmation = 5,
    MachineToken = 6
}
