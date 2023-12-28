using RestSharp;
using System.Text.Json.Serialization;

namespace SteamWeb.Web.DTO;

public class MemoryResponse : Response, IDisposable
{
    // Track whether Dispose has been called.
    private bool disposed = false;

    [JsonIgnore] public MemoryStream? Stream { get; set; } = null;

    public MemoryResponse(RestResponse res) : base(res) => Stream = res.RawBytes == null || res.RawBytes.Length == 0 || !res.IsSuccessful ? null : new MemoryStream(res.RawBytes);

    // Implement IDisposable.
    // Do not make this method virtual.
    // A derived class should not be able to override this method.
    public void Dispose()
    {
        Dispose(disposing: true);
        // This object will be cleaned up by the Dispose method.
        // Therefore, you should call GC.SuppressFinalize to
        // take this object off the finalization queue
        // and prevent finalization code for this object
        // from executing a second time.
        GC.SuppressFinalize(this);
    }
    // Dispose(bool disposing) executes in two distinct scenarios.
    // If disposing equals true, the method has been called directly
    // or indirectly by a user's code. Managed and unmanaged resources
    // can be disposed.
    // If disposing equals false, the method has been called by the
    // runtime from inside the finalizer and you should not reference
    // other objects. Only unmanaged resources can be disposed.
    protected virtual void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (!disposed)
        {
            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources.
                if (Stream != null)
                {
                    Stream.Dispose();
                }
            }

            // Note disposing has been done.
            disposed = true;
        }
    }
}
