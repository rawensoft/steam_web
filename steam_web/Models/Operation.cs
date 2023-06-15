namespace SteamWeb.Models;
public class Operation
{
    public string Name { get; internal set; }
    public int Missions_Completed { get; internal set; } = 0;
    public int Stars_Progress { get; internal set; } = 0;
    public int Stars_Purchased { get; internal set; } = 0;
    public int Redeemable_Balance { get; internal set; } = 0;
    public int Active_Mission_Card { get; internal set; } = 0;
    public string Activation_Time { get; internal set; } = "None";
}
