﻿using SteamWeb.Extensions;
using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;
public class AjaxNext
{
    [JsonPropertyName("hash")]
    public string? Hash { get; init; } = null;

    [JsonPropertyName("errorMsg")]
    public string? ErrorMsg { get; init; } = null;

    [JsonPropertyName("title")]
    public string? Title { get; init; } = null;

    [JsonPropertyName("steamid")]
    public ulong SteamId { get; init; }

    [JsonPropertyName("html")]
    public string? Html { get; init; } = null;

    [JsonIgnore] public bool IsHash => !Hash.IsEmpty();
    /// <summary>
    /// Показывает является ли этот ответ ошибкой или нет
    /// <br/>
    /// Ошибка может быть в <see cref="ErrorMsg"/> или в <see cref="Title"/>,  <see cref="Html"/> и <see cref="SteamId"/>
    /// </summary>
    [JsonIgnore] public bool IsError => !ErrorMsg.IsEmpty() || !Html.IsEmpty();
}