namespace SteamWeb.Models;
public class OperationProgress
{
    public bool Success { get; internal set; } = false;
    public string? Error { get; internal set; } = null;
    public Operation[] Operations { get; internal set; } = Array.Empty<Operation>();

    public OperationProgress() { }
    public OperationProgress(string error) => Error = error;
    public Operation? GetOperationByName(string Name)
    {
        Name = Name.ToLower();
        foreach (var item in Operations)
            if (item.Name.ToLower() == Name)
                return item;
        return null;
    }
}
