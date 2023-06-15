using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SteamWeb.Web;
using SteamWeb.Extensions;

namespace SteamWeb.Auth
{
    public class SteamGuardAccount
    {
        [JsonPropertyName("shared_secret")] public string SharedSecret { get; init; }
        [JsonPropertyName("serial_number")] public string SerialNumber { get; init; }
        [JsonPropertyName("revocation_code")] public string RevocationCode { get; init; }
        [JsonPropertyName("uri")] public string URI { get; init; }
        [JsonPropertyName("server_time")] public int ServerTime { get; init; }
        [JsonPropertyName("account_name")] public string AccountName { get; init; }
        [JsonPropertyName("token_gid")] public string TokenGID { get; init; }
        [JsonPropertyName("identity_secret")] public string IdentitySecret { get; init; }
        [JsonPropertyName("secret_1")] public string Secret1 { get; init; }
        [JsonPropertyName("status")] public int Status { get; init; }
        [JsonPropertyName("device_id")] public string DeviceID { get; set; }
        /// <summary>
        /// Не сериализируется и не десерилизируется. Нужно устанавливать каждый раз при создании экземпляра класса.
        /// </summary>
        [JsonIgnore] public IWebProxy Proxy { get; set; } = null;
        /// <summary>
        /// Set to true if the authenticator has actually been applied to the account.
        /// </summary>
        [JsonPropertyName("fully_enrolled")] public bool FullyEnrolled { get; set; } = false;
        public SessionData Session { get; set; }
        private static byte[] steamGuardCodeTranslations = new byte[] { 50, 51, 52, 53, 54, 55, 56, 57, 66, 67, 68, 70, 71, 72, 74, 75, 77, 78, 80, 81, 82, 84, 86, 87, 88, 89 };

        public (RESPONSE_STATUS, bool) DeactivateAuthenticator(int scheme = 2)
        {
            if (Session == null) return (RESPONSE_STATUS.Error, false);
            if (Session.Platform != SignInPlatform.Mobile) return (RESPONSE_STATUS.WrongPlatform, false);

            var postRequest = new PostRequest(APIEndpoints.STEAMAPI_BASE + "/ITwoFactorService/RemoveAuthenticator/v0001", Downloader.AppFormUrlEncoded)
            {
                Session = Session,
                Proxy = Proxy,
                UserAgent = Downloader.UserAgentOkHttp,
                IsAjax = true,
                IsMobile = true
            }
            .AddPostData("steamid", Session.SteamID).AddPostData("steamguard_scheme", scheme)
            .AddPostData("revocation_code", RevocationCode).AddPostData("access_token", Session.OAuthToken);
            var response = Downloader.Post(postRequest);
            try
            {
                if (response.LostAuth) return (RESPONSE_STATUS.WGTokenExpired, false);
                if (!response.Success || response.Data.IsEmpty()) return (RESPONSE_STATUS.Error, false);
                var removeResponse = JsonSerializer.Deserialize<RemoveAuthenticatorResponse>(response.Data);

                if (removeResponse == null || removeResponse.Response == null || !removeResponse.Response.Success) return (RESPONSE_STATUS.Error, false);
                return (RESPONSE_STATUS.Success, true);
            }
            catch (Exception)
            {
                return (RESPONSE_STATUS.Error, false);
            }
        }

        /// <summary>
        /// Generate Steam Guard Code from Steam Time Request
        /// </summary>
        /// <returns>Null Or Code</returns>
        public string GenerateSteamGuardCode() => GenerateSteamGuardCodeForTime(TimeAligner.GetSteamTime());
        /// <summary>
        /// Generate Steam Guard Code from your time
        /// </summary>
        /// <returns>Null Or Code</returns>
        public string GenerateSteamGuardCodeForTime(long time)
        {
            if (SharedSecret == null || SharedSecret.Length == 0) return null;

            byte[] timeArray = new byte[8];
            time /= 30L;
            for (int i = 8; i > 0; i--)
            {
                timeArray[i - 1] = (byte)time;
                time >>= 8;
            }

            HMACSHA1 hmacGenerator = new HMACSHA1();
            hmacGenerator.Key = Convert.FromBase64String(Regex.Unescape(SharedSecret));
            byte[] hashedData = hmacGenerator.ComputeHash(timeArray);
            byte[] codeArray = new byte[5];
            try
            {
                byte b = (byte)(hashedData[19] & 0xF);
                int codePoint = (hashedData[b] & 0x7F) << 24 | (hashedData[b + 1] & 0xFF) << 16 | (hashedData[b + 2] & 0xFF) << 8 | (hashedData[b + 3] & 0xFF);

                for (int i = 0; i < 5; ++i)
                {
                    codeArray[i] = steamGuardCodeTranslations[codePoint % steamGuardCodeTranslations.Length];
                    codePoint /= steamGuardCodeTranslations.Length;
                }
            }
            catch (Exception)
            {
                return null; //Change later, catch-alls are bad!
            }
            return Encoding.UTF8.GetString(codeArray);
        }

        public (RESPONSE_STATUS, Confirmation[]) FetchConfirmations()
        {
            if (Session == null) return (RESPONSE_STATUS.Error, new Confirmation[0]);
            var getRequest = new GetRequest(GenerateConfirmationURL())
            {
                Session = Session,
                Proxy = Proxy,
                UserAgent = Downloader.UserAgentOkHttp,
                IsMobile = true,
                IsAjax = true
            };
            var response = Downloader.GetMobile(getRequest);
            if (response.LostAuth) return (RESPONSE_STATUS.WGTokenExpired, null);
            if (!response.Success) return (RESPONSE_STATUS.Error, null);
            return FetchConfirmationInternal(response.Data);
        }
        public async Task<(RESPONSE_STATUS, Confirmation[])> FetchConfirmationsAsync()
        {
            if (Session == null) return (RESPONSE_STATUS.Error, new Confirmation[0]);
            var getRequest = new GetRequest(GenerateConfirmationURL())
            {
                Session = Session,
                Proxy = Proxy,
                UserAgent = Downloader.UserAgentOkHttp,
                IsMobile = true,
                IsAjax = true
            };
            var response = await Downloader.GetMobileAsync(getRequest);
            if (response.LostAuth) return (RESPONSE_STATUS.WGTokenExpired, null);
            if (!response.Success) return (RESPONSE_STATUS.Error, null);
            return FetchConfirmationInternal(response.Data);
        }
        private (RESPONSE_STATUS, Confirmation[]) FetchConfirmationInternal(string response)
        {
            /*So you're going to see this abomination and you're going to be upset.
              It's understandable. But the thing is, regex for HTML -- while awful -- makes this way faster than parsing a DOM, plus we don't need another library.
              And because the data is always in the same place and same format... It's not as if we're trying to naturally understand HTML here. Just extract strings.
              I'm sorry. */

            var confRegex = new Regex("<div class=\"mobileconf_list_entry\" id=\"conf[0-9]+\" data-confid=\"(\\d+)\" data-key=\"(\\d+)\" data-type=\"(\\d+)\" data-creator=\"(\\d+)\"");

            if (response == null || !confRegex.IsMatch(response))
            {
                if (response == null || !response.Contains("<div>Nothing to confirm</div>"))
                {
                    return (RESPONSE_STATUS.WGTokenInvalid, new Confirmation[0]);
                }

                return (RESPONSE_STATUS.Success, new Confirmation[0]);
            }

            var confirmations = confRegex.Matches(response);

            var ret = new List<Confirmation>(confirmations.Count);
            foreach (Match confirmation in confirmations)
            {
                if (confirmation.Groups.Count != 5) continue;

                if (!ulong.TryParse(confirmation.Groups[1].Value, out ulong confID) ||
                    !ulong.TryParse(confirmation.Groups[2].Value, out ulong confKey) ||
                    !int.TryParse(confirmation.Groups[3].Value, out int confType) ||
                    !ulong.TryParse(confirmation.Groups[4].Value, out ulong confCreator))
                {
                    continue;
                }
                ret.Add(new Confirmation(confID, confKey, confType, confCreator));
            }

            return (RESPONSE_STATUS.Success, ret.ToArray());
        }

        public RESPONSE_STATUS AcceptMultipleConfirmations(Confirmation[] confs) => _sendMultiConfirmationAjax(confs, "allow");
        public RESPONSE_STATUS DenyMultipleConfirmations(Confirmation[] confs) => _sendMultiConfirmationAjax(confs, "cancel");
        public RESPONSE_STATUS AcceptConfirmation(Confirmation conf) => _sendConfirmationAjax(conf, "allow");
        public RESPONSE_STATUS DenyConfirmation(Confirmation conf) => _sendConfirmationAjax(conf, "cancel");

        public async Task<RESPONSE_STATUS> AcceptMultipleConfirmationsAsync(Confirmation[] confs) => await _sendMultiConfirmationAjaxAsync(confs, "allow");
        public async Task<RESPONSE_STATUS> DenyMultipleConfirmationsAsync(Confirmation[] confs) => await _sendMultiConfirmationAjaxAsync(confs, "cancel");
        public async Task<RESPONSE_STATUS> AcceptConfirmationAsync(Confirmation conf) => await _sendConfirmationAjaxAsync(conf, "allow");
        public async Task<RESPONSE_STATUS> DenyConfirmationAsync(Confirmation conf) => await _sendConfirmationAjaxAsync(conf, "cancel");

        private (RESPONSE_STATUS, ConfirmationDetailsResponse) _getConfirmationDetails(Confirmation conf)
        {
            if (Session == null) return (RESPONSE_STATUS.Error, null);
            if (Session.Platform != SignInPlatform.Mobile) return (RESPONSE_STATUS.WrongPlatform, null);

            string url = $"{APIEndpoints.COMMUNITY_BASE}/mobileconf/details/{conf.ID}?{GenerateConfirmationQueryParams("details")}";
            var getRequest = new GetRequest(url, Proxy, Session, GenerateConfirmationURL(), Downloader.UserAgentOkHttp) { IsMobile = true, IsAjax = true };
            var response = Downloader.GetMobile(getRequest);
            if (response.LostAuth) return (RESPONSE_STATUS.WGTokenExpired, null);
            if (!response.Success || response.Data.IsEmpty()) return (RESPONSE_STATUS.Error, null);

            var confResponse = JsonSerializer.Deserialize<ConfirmationDetailsResponse>(response.Data);
            if (confResponse == null) return (RESPONSE_STATUS.Error, null);
            return (RESPONSE_STATUS.Success, confResponse);
        }
        private RESPONSE_STATUS _sendConfirmationAjax(Confirmation conf, string op)
        {
            if (Session == null) return RESPONSE_STATUS.Error;
            if (Session.Platform != SignInPlatform.Mobile) return RESPONSE_STATUS.WrongPlatform;
            string url = $"{APIEndpoints.COMMUNITY_BASE}/mobileconf/ajaxop?op={op}&{GenerateConfirmationQueryParams(op)}&cid={conf.ID}&ck={conf.Key}";
            var getRequest = new GetRequest(url, Proxy, Session, GenerateConfirmationURL(), Downloader.UserAgentOkHttp);
            var response = Downloader.GetMobile(getRequest);
            if (response.LostAuth) return RESPONSE_STATUS.WGTokenExpired;
            if (!response.Success || response.Data.IsEmpty()) return RESPONSE_STATUS.Error;

            SendConfirmationResponse confResponse = JsonSerializer.Deserialize<SendConfirmationResponse>(response.Data);
            return confResponse.Success ? RESPONSE_STATUS.Success : RESPONSE_STATUS.Error;
        }
        private async Task<RESPONSE_STATUS> _sendConfirmationAjaxAsync(Confirmation conf, string op)
        {
            if (Session == null) return RESPONSE_STATUS.Error;
            if (Session.Platform != SignInPlatform.Mobile) return RESPONSE_STATUS.WrongPlatform;

            string url = $"{APIEndpoints.COMMUNITY_BASE}/mobileconf/ajaxop?op={op}&{GenerateConfirmationQueryParams(op)}&cid={conf.ID}&ck={conf.Key}";
            var getRequest = new GetRequest(url, Proxy, Session, GenerateConfirmationURL(), Downloader.UserAgentOkHttp) { IsMobile = true, IsAjax = true};
            var response = await Downloader.GetMobileAsync(getRequest);
            if (response.LostAuth) return RESPONSE_STATUS.WGTokenExpired;
            if (!response.Success || response.Data.IsEmpty()) return RESPONSE_STATUS.Error;

            SendConfirmationResponse confResponse = await JsonSerializer.DeserializeAsync<SendConfirmationResponse>(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(response.Data)));
            return confResponse.Success ? RESPONSE_STATUS.Success : RESPONSE_STATUS.Error;
        }
        private RESPONSE_STATUS _sendMultiConfirmationAjax(Confirmation[] confs, string op)
        {
            if (Session == null) return RESPONSE_STATUS.Error;
            if (Session.Platform != SignInPlatform.Mobile) return RESPONSE_STATUS.WrongPlatform;

            var postRequest = new PostRequest($"{APIEndpoints.COMMUNITY_BASE}/mobileconf/multiajaxop", Downloader.AppFormUrlEncoded)
            {
                Content = GenerateConfirmationQueryParams(op),
                Session = Session,
                Proxy = Proxy,
                UserAgent = Downloader.UserAgentOkHttp,
                IsMobile = true,
                IsAjax = true,
                Referer = GenerateConfirmationURL()
            }.AddPostData("op", op).AddPostData("op", op);
            for (int i = 0; i < confs.Length; i++)
            {
                var conf = confs[i];
                postRequest.AddPostData("cid[]", conf.ID).AddPostData("ck[]", conf.Key);
            }
            var response = Downloader.Post(postRequest);
            if (response.LostAuth) return RESPONSE_STATUS.WGTokenExpired;
            if (!response.Success || response.Data.IsEmpty()) return RESPONSE_STATUS.Error;

            var confResponse = JsonSerializer.Deserialize<SendConfirmationResponse>(response.Data);
            return confResponse.Success ? RESPONSE_STATUS.Success : RESPONSE_STATUS.Error;
        }
        private async Task<RESPONSE_STATUS> _sendMultiConfirmationAjaxAsync(Confirmation[] confs, string op)
        {
            if (Session == null) return RESPONSE_STATUS.Error;
            if (Session.Platform != SignInPlatform.Mobile) return RESPONSE_STATUS.WrongPlatform;

            var postRequest = new PostRequest($"{APIEndpoints.COMMUNITY_BASE}/mobileconf/multiajaxop", Downloader.AppFormUrlEncoded)
            {
                Content = GenerateConfirmationQueryParams(op),
                Session = Session,
                Proxy = Proxy,
                UserAgent = Downloader.UserAgentOkHttp,
                IsMobile = true,
                IsAjax = true,
                Referer = GenerateConfirmationURL()
            }.AddPostData("op", op).AddPostData("op", op);
            for (int i = 0; i < confs.Length; i++)
            {
                var conf = confs[i];
                postRequest.AddPostData("cid[]", conf.ID).AddPostData("ck[]", conf.Key);
            }
            var response = await Downloader.PostAsync(postRequest);
            if (response.LostAuth) return RESPONSE_STATUS.WGTokenExpired;
            if (!response.Success || response.Data.IsEmpty()) return RESPONSE_STATUS.Error;

            var confResponse = JsonSerializer.Deserialize<SendConfirmationResponse>(response.Data);
            return confResponse.Success ? RESPONSE_STATUS.Success : RESPONSE_STATUS.Error;
        }

        /// <summary>
        /// Refreshes the Steam session. Necessary to perform confirmations if your session has expired or changed.
        /// </summary>
        /// <returns>Доступные статусы:
        /// <code>Success</code>
        /// <code>WrongPlatform</code>
        /// <code>WGTokenExpired</code>
        /// <code>Error</code>
        /// </returns>
        public RESPONSE_STATUS RefreshSession()
        {
            if (Session == null) return RESPONSE_STATUS.Error;
            if (Session.Platform != SignInPlatform.Mobile) return RESPONSE_STATUS.WrongPlatform;

            var postRequest = new PostRequest(APIEndpoints.MOBILEAUTH_GETWGTOKEN, Downloader.AppFormUrlEncoded)
            {
                Session = Session,
                Proxy = Proxy,
                UserAgent = Downloader.UserAgentOkHttp,
                IsMobile = true,
                IsAjax = true,
            }.AddPostData("access_token", Session.OAuthToken);
            var response = Downloader.Post(postRequest);
            if (response.LostAuth) return RESPONSE_STATUS.WGTokenExpired;
            if (!response.Success && response.Data?.Contains("Access is denied. Retrying will not help. Please verify your") == true) return RESPONSE_STATUS.WGTokenExpired;
            if (!response.Success || response.Data?.IsEmpty() != false) return RESPONSE_STATUS.Error;

            try
            {
                var refreshResponse = JsonSerializer.Deserialize<RefreshSessionDataResponse>(response.Data);
                if (refreshResponse == null || refreshResponse.Response == null || refreshResponse.Response.Token.IsEmpty())
                    return RESPONSE_STATUS.Error;

                string token = Session.SteamID + "%7C%7C" + refreshResponse.Response.Token;
                string tokenSecure = Session.SteamID + "%7C%7C" + refreshResponse.Response.TokenSecure;

                Session.SteamLogin = token;
                Session.SteamLoginSecure = tokenSecure;
                return RESPONSE_STATUS.Success;
            }
            catch (Exception)
            {
                return RESPONSE_STATUS.Error;
            }
        }
        /// <summary>
        /// Refreshes the Steam session. Necessary to perform confirmations if your session has expired or changed.
        /// </summary>
        /// <returns>Доступные статусы:
        /// <code>Success</code>
        /// <code>WrongPlatform</code>
        /// <code>WGTokenExpired</code>
        /// <code>Error</code>
        /// </returns>
        public async Task<RESPONSE_STATUS> RefreshSessionAsync()
        {
            if (Session == null) return RESPONSE_STATUS.Error;
            if (Session.Platform != SignInPlatform.Mobile) return RESPONSE_STATUS.WrongPlatform;

            var postRequest = new PostRequest(APIEndpoints.MOBILEAUTH_GETWGTOKEN, Downloader.AppFormUrlEncoded)
            {
                Session = Session,
                Proxy = Proxy,
                UserAgent = Downloader.UserAgentOkHttp,
                IsMobile = true,
                IsAjax = true,
            }.AddPostData("access_token", Session.OAuthToken);
            var response = await Downloader.PostMobileAsync(postRequest);
            if (response.LostAuth) return RESPONSE_STATUS.WGTokenExpired;
            if (!response.Success && response.Data?.Contains("Access is denied. Retrying will not help. Please verify your") == true) return RESPONSE_STATUS.WGTokenExpired;
            if (!response.Success || response.Data?.IsEmpty() != false) return RESPONSE_STATUS.Error;

            try
            {
                var refreshResponse = JsonSerializer.Deserialize<RefreshSessionDataResponse>(response.Data);
                if (refreshResponse == null || refreshResponse.Response == null || refreshResponse.Response.Token.IsEmpty())
                    return RESPONSE_STATUS.Error;

                string token = Session.SteamID + "%7C%7C" + refreshResponse.Response.Token;
                string tokenSecure = Session.SteamID + "%7C%7C" + refreshResponse.Response.TokenSecure;

                Session.SteamLogin = token;
                Session.SteamLoginSecure = tokenSecure;
                return RESPONSE_STATUS.Success;
            }
            catch (Exception)
            {
                return RESPONSE_STATUS.Error;
            }
        }

        public string GenerateConfirmationURL(string tag = "conf")
        {
            string endpoint = APIEndpoints.COMMUNITY_BASE + "/mobileconf/conf?";
            string queryString = GenerateConfirmationQueryParams(tag);
            return endpoint + queryString;
        }
        public string GenerateConfirmationQueryParams(string tag)
        {
            if (string.IsNullOrEmpty(DeviceID))
                throw new ArgumentException("Device ID is not present");

            long time = TimeAligner.GetSteamTime();
            return $"p={DeviceID}&a={Session.SteamID}&k={_generateConfirmationHashForTime(time, tag)}&t={time}&m=android&tag={tag}";
        }
        private string _generateConfirmationHashForTime(long time, string tag)
        {
            byte[] decode = Convert.FromBase64String(IdentitySecret);
            int n2 = 8;
            if (tag != null)
            {
                if (tag.Length > 32) n2 = 8 + 32;
                else n2 = 8 + tag.Length;
            }
            byte[] array = new byte[n2];
            int n3 = 8;
            while (true)
            {
                int n4 = n3 - 1;
                if (n3 <= 0) break;
                array[n4] = (byte)time;
                time >>= 8;
                n3 = n4;
            }
            if (tag != null) Array.Copy(Encoding.UTF8.GetBytes(tag), 0, array, 8, n2 - 8);
            try
            {
                HMACSHA1 hmacGenerator = new HMACSHA1();
                hmacGenerator.Key = decode;
                byte[] hashedData = hmacGenerator.ComputeHash(array);
                string encodedData = Convert.ToBase64String(hashedData, Base64FormattingOptions.None);
                string hash = WebUtility.UrlEncode(encodedData);
                return hash;
            }
            catch
            {
                return null;
            }
        }
    }
    internal class RefreshSessionDataResponse
    {
        [JsonPropertyName("response")] public RefreshSessionDataInternalResponse Response { get; init; } = new();
        internal class RefreshSessionDataInternalResponse
        {
            [JsonPropertyName("token")] public string Token { get; init; }
            [JsonPropertyName("token_secure")] public string TokenSecure { get; init; }
        }
    }
    internal class RemoveAuthenticatorResponse
    {
        [JsonPropertyName("response")] public RemoveAuthenticatorInternalResponse Response { get; init; } = new();

        internal class RemoveAuthenticatorInternalResponse
        {
            [JsonPropertyName("success")] public bool Success { get; init; } = false;
        }
    }
    internal class ConfirmationDetailsResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; } = false;

        [JsonPropertyName("html")]
        public string HTML { get; init; }
    }
    internal class SendConfirmationResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; } = false;
    }
}
