namespace SteamWeb.Models;
public class MatchmakingMode
{
    public ushort Wins { get; internal set; }
    public ushort Ties { get; internal set; }
    public ushort Losses { get; internal set; }
    public ushort MatchesPlayed => (ushort)(Wins + Ties + Losses);
    public short SkillGroup { get; internal set; }
    public string LastMatch { get; internal set; }

    public string GetSkillGroupAlias() => GetSkillGroupAlias(SkillGroup);
    public static string GetSkillGroupAlias(short sk)
    {
        if (sk == 0) return "EX";
        if (sk == 1) return "S1";
        if (sk == 2) return "S2";
        if (sk == 3) return "S3";
        if (sk == 4) return "S4";
        if (sk == 5) return "SE";
        if (sk == 6) return "SEM";
        if (sk == 7) return "GN1";
        if (sk == 8) return "GN2";
        if (sk == 9) return "GN3";
        if (sk == 10) return "GNM";
        if (sk == 11) return "MG1";
        if (sk == 12) return "MG2";
        if (sk == 13) return "MGE";
        if (sk == 14) return "DMG";
        if (sk == 15) return "LE";
        if (sk == 16) return "LEM";
        if (sk == 17) return "SMFC";
        if (sk == 18) return "GE";
        return "None";
    }
    public string GetSkillGroupFullName() => GetSkillGroupAlias(SkillGroup);
    public static string GetSkillGroupFullName(short sk)
    {
        if (sk == 0) return "Expired";
        if (sk == 1) return "Silver 1";
        if (sk == 2) return "Silver 2";
        if (sk == 3) return "Silver 3";
        if (sk == 4) return "Silver 4";
        if (sk == 5) return "Silver Elite";
        if (sk == 6) return "Silver Elite Master";
        if (sk == 7) return "Gold Nova 1";
        if (sk == 8) return "Gold Nova 2";
        if (sk == 9) return "Gold Nova 3";
        if (sk == 10) return "Gold Nova Master";
        if (sk == 11) return "Master Guardian 1";
        if (sk == 12) return "Master Guardian 2";
        if (sk == 13) return "Master Guardian Elite";
        if (sk == 14) return "Distinguished Master Guardian";
        if (sk == 15) return "Legendary Eagle";
        if (sk == 16) return "Legendary Eagle Master";
        if (sk == 17) return "Supreme Master First Class";
        if (sk == 18) return "Global Elite";
        return "None";
    }
}
