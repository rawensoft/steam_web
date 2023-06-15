namespace SteamWeb.Script.Enums;

public enum CATEGORY_730_TYPE : byte
{
    tag_CSGO_Type_Pistol,
    tag_CSGO_Type_SMG,
    tag_CSGO_Type_Rifle,
    tag_CSGO_Type_SniperRifle,
    tag_CSGO_Type_Shotgun,
    tag_CSGO_Type_Machinegun,
    tag_Type_CustomPlayer,
    tag_CSGO_Type_WeaponCase,
    tag_CSGO_Type_Knife,
    tag_CSGO_Tool_Sticker,
    tag_Type_Hands,
    tag_CSGO_Type_Spray,
    tag_CSGO_Tool_Patch,
    tag_CSGO_Type_MusicKit,
    tag_CSGO_Type_Collectible,
    tag_CSGO_Tool_WeaponCase_KeyTag,
    tag_CSGO_Type_Ticket,
    tag_CSGO_Tool_GiftTag,
    tag_CSGO_Tool_Name_TagTag,
    tag_CSGO_Type_Tool
}
public enum CATEGORY_730_WEAPON: byte
{
    tag_weapon_ak47,
    tag_weapon_aug,
    tag_weapon_awp,
    tag_weapon_bayonet,
    tag_weapon_knife_survival_bowie,
    tag_weapon_knife_butterfly,
    tag_weapon_knife_css,
    tag_weapon_cz75a,
    tag_weapon_deagle,
    tag_weapon_elite,
    tag_weapon_knife_falchion,
    tag_weapon_famas,
    tag_weapon_fiveseven,
    tag_weapon_knife_flip,
    tag_weapon_g3sg1,
    tag_weapon_galilar,
    tag_weapon_glock,
    tag_weapon_knife_gut,
    tag_weapon_knife_tactical,
    tag_weapon_knife_karambit,
    tag_weapon_m249,
    tag_weapon_m4a1_silencer,
    tag_weapon_m4a1,
    tag_weapon_knife_m9_bayonet,
    tag_weapon_mac10,
    tag_weapon_mag7,
    tag_weapon_mp5sd,
    tag_weapon_mp7,
    tag_weapon_mp9,
    tag_weapon_knife_gypsy_jackknife,
    tag_weapon_negev,
    tag_weapon_knife_outdoor,
    tag_weapon_nova,
    tag_weapon_hkp2000,
    tag_weapon_p250,
    tag_weapon_p90,
    tag_weapon_knife_cord,
    tag_weapon_bizon,
    tag_weapon_revolver,
    tag_weapon_sawedoff,
    tag_weapon_scar20,
    tag_weapon_sg556,
    tag_weapon_knife_push,
    tag_weapon_knife_skeleton,
    tag_weapon_ssg08,
    tag_weapon_knife_stiletto,
    tag_weapon_knife_canis,
    tag_weapon_knife_widowmaker,
    tag_weapon_tec9,
    tag_weapon_ump45,
    tag_weapon_knife_ursus,
    tag_weapon_usp_silencer,
    tag_weapon_xm1014
}
public enum SORT_DIRECTION:byte { Asc, Desc }
public enum SORT_COLUMN : byte { Name, Price }
public enum TypeLost : byte
{
    Default = 4,
    /// <summary>
    /// AjaxVerifyAccountRecoveryCodeAsync
    /// AjaxPollAccountRecoveryConfirmationAsync
    /// </summary>
    RecoveryConfirm = 0,
    SkipEmail = 2,
    SkipPhone = 4,
    KTGuard = 8,
    KTPhone = 12,
    KTEmail = 14,
    KTPassword = 15,
}
public enum OP_CODES : byte
{
    /// <summary>
    /// отправка кода на почту
    /// </summary>
    GetPhoneNumber,
    /// <summary>
    /// отправка смс на телефон
    /// </summary>
    EmailVerification,
    /// <summary>
    /// отправка кода из смс
    /// </summary>
    GetSMSCode,
    /// <summary>
    /// Повторить переход по ссылке
    /// </summary>
    RetryEmailVerification,
    /// <summary>
    /// Повторно отправить код на телефон
    /// </summary>
    ReSendSMS,
}
public enum FriendsAction : byte
{
    Block,
    UnBlock,
    UnFriend,
    UnFollow,
    IgnoreFriendInvite,
    AcceptFriend,
    LeaveFromGroup,
    AcceptGroup,
    IgnoreGroup,
}
public enum TYPE_HISTORY_ITEM : byte { Created, Canceled, Buyed, Selled, Unknown = 0 }
public enum AUTH_TYPE : byte
{
    /// <summary>
    /// Вход выполен
    /// </summary>
    Success,
    /// <summary>
    /// Неверный пароль
    /// </summary>
    BadPassword,
    /// <summary>
    /// Нужно ввести 2FA код
    /// </summary>
    Need2FA,
    /// <summary>
    /// Нужно ввести код с почты
    /// </summary>
    NeedEmailCode,
    /// <summary>
    /// Нужно решить каптчу
    /// </summary>
    Captcha,
    /// <summary>
    /// Ошибка при выполнении запроса
    /// </summary>
    UnknownError
}
public enum TypeMethod : byte
{
    Mobile,
    Email
}
public enum TypeReset : byte
{
    Email,
    Password,
    Phone,
    KTPassword,
    KTPhone,
    KTGuard,
    KTEmail
}
