﻿using SteamWeb.Web;
using System.Text.Json;
using SteamWeb.API.Models.IPlayerService;
using SteamWeb.API.Models;

namespace SteamWeb.API;
public static class IPlayerService
{
    public static ResponseData<ProfileCustomization> GetProfileCustomization(ApiRequest apiRequest, ulong steamid,
        bool include_inactive_customizations = false, bool include_purchased_customizations = false)
    {
        var request = new GetRequest(SteamApiUrls.IPlayerService_GetProfileCustomization_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid);
        if (include_inactive_customizations)
            request.AddQuery("include_inactive_customizations", 1);
        if (include_purchased_customizations)
            request.AddQuery("include_purchased_customizations", 1);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<ProfileCustomization>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static async Task<ResponseData<ProfileCustomization>> GetProfileCustomizationAsync(ApiRequest apiRequest, ulong steamid,
        bool include_inactive_customizations = false, bool include_purchased_customizations = false)
    {
        var request = new GetRequest(SteamApiUrls.IPlayerService_GetProfileCustomization_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid);
        if (include_inactive_customizations)
            request.AddQuery("include_inactive_customizations", 1);
        if (include_purchased_customizations)
            request.AddQuery("include_purchased_customizations", 1);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<ProfileCustomization>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static bool SetProfileTheme(ApiRequest apiRequest, string theme_id)
    {
        var request = new PostRequest(SteamApiUrls.IPlayerService_SetProfileTheme_v1, Downloader.AppFormUrlEncoded)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddPostData("theme_id", theme_id);
        var response = Downloader.Post(request);
        return response.Success;
    }
    public static async Task<bool> SetProfileThemeAsync(ApiRequest apiRequest, string theme_id)
    {
        var request = new PostRequest(SteamApiUrls.IPlayerService_SetProfileTheme_v1, Downloader.AppFormUrlEncoded)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddPostData("theme_id", theme_id);
        var response = await Downloader.PostAsync(request);
        return response.Success;
    }

    /// <summary>
    /// Gets badges that are owned by a specific user
    /// </summary>
    /// <param name="steamid">The player we're asking about</param>
    /// <returns></returns>
    public static ResponseData<PlayerBadges> GetBadges(ApiRequest apiRequest, ulong steamid)
    {
        var request = new GetRequest(SteamApiUrls.IPlayerService_GetBadges_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<PlayerBadges>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    /// <summary>
    /// Gets badges that are owned by a specific user
    /// </summary>
    /// <param name="steamid">The player we're asking about</param>
    /// <returns></returns>
    public static async Task<ResponseData<PlayerBadges>> GetBadgesAsync(ApiRequest apiRequest, ulong steamid)
    {
        var request = new GetRequest(SteamApiUrls.IPlayerService_GetBadges_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<PlayerBadges>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    /// <summary>
    /// Gets all the quests needed to get the specified badge, and which are completed
    /// </summary>
    /// <param name="steamid">The player we're asking about</param>
    /// <param name="badgeid">The badge we're asking about</param>
    /// <returns></returns>
    public static ResponseData<PlayerQuests> GetCommunityBadgeProgress(ApiRequest apiRequest, ulong steamid, int? badgeid = null)
    {
        var request = new GetRequest(SteamApiUrls.IPlayerService_GetCommunityBadgeProgress_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid);
        if (badgeid.HasValue)
            request.AddQuery("badgeid", badgeid.Value);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<PlayerQuests>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    /// <summary>
    /// Gets all the quests needed to get the specified badge, and which are completed
    /// </summary>
    /// <param name="steamid">The player we're asking about</param>
    /// <param name="badgeid">The badge we're asking about</param>
    /// <returns></returns>
    public static async Task<ResponseData<PlayerQuests>> GetCommunityBadgeProgressAsync(ApiRequest apiRequest, ulong steamid, int? badgeid = null)
    {
        var request = new GetRequest(SteamApiUrls.IPlayerService_GetCommunityBadgeProgress_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid);
        if (badgeid.HasValue)
            request.AddQuery("badgeid", badgeid.Value);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<PlayerQuests>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static ResponseData<FavoriteBadge> GetFavoriteBadge(ApiRequest apiRequest, ulong steamid)
    {
        var request = new GetRequest(SteamApiUrls.IPlayerService_GetFavoriteBadge_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<FavoriteBadge>>(response.Data!)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static async Task<ResponseData<FavoriteBadge>> GetFavoriteBadgeAsync(ApiRequest apiRequest, ulong steamid)
    {
        var request = new GetRequest(SteamApiUrls.IPlayerService_GetFavoriteBadge_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<FavoriteBadge>>(response.Data!)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    /// <summary>
    /// Return a list of games owned by the player
    /// </summary>
    /// <param name="steamid">The player we're asking about</param>
    /// <param name="include_appinfo">true if we want additional details (name, icon) about each game</param>
    /// <param name="include_played_free_games">Free games are excluded by default. If this is set, free games the user has played will be returned.</param>
    /// <param name="include_free_sub">Some games are in the free sub, which are excluded by default.</param>
    /// <param name="skip_unvetted_apps">if set, skip unvetted store apps</param>
    /// <param name="appids_filter">if set, restricts result set to the passed in apps</param>
    /// <returns></returns>
    public static ResponseData<PlayerOwnedGames> GetOwnedGames(ApiRequest apiRequest, ulong steamid, bool include_appinfo = false,
        bool include_played_free_games = false, bool include_free_sub = false, bool skip_unvetted_apps = false, uint? appids_filter = null)
    {
        var request = new GetRequest(SteamApiUrls.IPlayerService_GetOwnedGames_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid);
        if (include_appinfo)
            request.AddQuery("include_appinfo", 1);
        if (include_played_free_games)
            request.AddQuery("include_played_free_games", 1);
        if (appids_filter.HasValue)
            request.AddQuery("appids_filter", appids_filter.Value);
        if (include_free_sub)
            request.AddQuery("include_free_sub", 1);
        if (skip_unvetted_apps)
            request.AddQuery("skip_unvetted_apps", 1);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<PlayerOwnedGames>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    /// <summary>
    /// Return a list of games owned by the player
    /// </summary>
    /// <param name="steamid">The player we're asking about</param>
    /// <param name="include_appinfo">true if we want additional details (name, icon) about each game</param>
    /// <param name="include_played_free_games">Free games are excluded by default. If this is set, free games the user has played will be returned.</param>
    /// <param name="include_free_sub">Some games are in the free sub, which are excluded by default.</param>
    /// <param name="skip_unvetted_apps">if set, skip unvetted store apps</param>
    /// <param name="appids_filter">if set, restricts result set to the passed in apps</param>
    /// <returns></returns>
    public static async Task<ResponseData<PlayerOwnedGames>> GetOwnedGamesAsync(ApiRequest apiRequest, ulong steamid, bool include_appinfo = false,
        bool include_played_free_games = false, bool include_free_sub = false, bool skip_unvetted_apps = false, uint? appids_filter = null)
    {
        var request = new GetRequest(SteamApiUrls.IPlayerService_GetOwnedGames_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid);
        if (include_appinfo)
            request.AddQuery("include_appinfo", 1);
        if (include_played_free_games)
            request.AddQuery("include_played_free_games", 1);
        if (appids_filter.HasValue)
            request.AddQuery("appids_filter", appids_filter.Value);
        if (include_free_sub)
            request.AddQuery("include_free_sub", 1);
        if (skip_unvetted_apps)
            request.AddQuery("skip_unvetted_apps", 1);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<PlayerOwnedGames>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    /// <summary>
    /// Returns the Steam Level of a user
    /// </summary>
    /// <param name="steamid">The player we're asking about</param>
    /// <returns></returns>
    public static ResponseData<PlayerSteamLevel> GetSteamLevel(ApiRequest apiRequest, ulong steamid)
    {
        var request = new GetRequest(SteamApiUrls.IPlayerService_GetSteamLevel_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<PlayerSteamLevel>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    /// <summary>
    /// Returns the Steam Level of a user
    /// </summary>
    /// <param name="steamid">The player we're asking about</param>
    /// <returns></returns>
    public static async Task<ResponseData<PlayerSteamLevel>> GetSteamLevelAsync(ApiRequest apiRequest, ulong steamid)
    {
        var request = new GetRequest(SteamApiUrls.IPlayerService_GetSteamLevel_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<PlayerSteamLevel>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static ResponseData<NicknamesModel> GetNicknameList(ApiRequest apiRequest)
    {
        var request = new GetRequest(SteamApiUrls.IPlayerService_GetNicknameList_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<NicknamesModel>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static async Task<ResponseData<NicknamesModel>> GetNicknameListAsync(ApiRequest apiRequest)
    {
        var request = new GetRequest(SteamApiUrls.IPlayerService_GetNicknameList_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<NicknamesModel>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}