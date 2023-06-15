using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace SteamWeb;
public static class Helpers
{
    public static void OpenUrl(string url)
    {
        try
        {
            System.Diagnostics.Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo() { UseShellExecute = true, FileName = url });
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                System.Diagnostics.Process.Start("xdg-open", url);
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
            {
                System.Diagnostics.Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
    public static string Encrypt(string pass, string publickKey, string exponant)
    {
        using var rsa = new RSACryptoServiceProvider();
        var rsap = new RSAParameters
        {
            Modulus = HexStringToByteArray(publickKey),
            Exponent = HexStringToByteArray(exponant)
        };

        rsa.ImportParameters(rsap);
        var encryptedData = rsa.Encrypt(Encoding.ASCII.GetBytes(pass), false);
        var data = Convert.ToBase64String(encryptedData);
        return data;
    }
    public static byte[] HexStringToByteArray(string hexString)
    {
        using var stream = new MemoryStream(hexString.Length / 2);

        for (int i = 0; i < hexString.Length; i += 2)
        {
            stream.WriteByte(byte.Parse(hexString.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
        }
        return stream.ToArray();
    }
}
