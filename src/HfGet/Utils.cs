using Spectre.Console;
using System.Net.Http.Headers;
using System.Text.Json;
// =======================
// HELPERS
// =======================

static class Utils
{
    public static (string repo, string variant) ParseModel(string input)
    {
        var parts = input.Split(':');
        return (parts[0], parts.Length > 1 ? parts[1] : "");
    }

    public static HttpClient CreateHttpClient(string? token)
    {
        var client = new HttpClient();

        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        client.Timeout = TimeSpan.FromHours(24);
        return client;
    }

    public static string FormatSize(long bytes)
    {
        if (bytes <= 0) return "-";

        var mb = bytes / 1024d / 1024d;
        var gb = mb / 1024d;

        return gb >= 1
            ? $"{gb:F2} GB"
            : $"{mb:F2} MB";
    }

    public static async Task<List<RepoFile>> GetRepoFiles(HttpClient client, string repo)
    {
        var json = await client.GetStringAsync(
            $"https://huggingface.co/api/models/{repo}");

        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("siblings")
            .EnumerateArray()
            .Select(x => new RepoFile(
                x.GetProperty("rfilename").GetString()!,
                x.TryGetProperty("size", out var s) ? s.GetInt64() : 0
            ))
            .ToList();
    }
}