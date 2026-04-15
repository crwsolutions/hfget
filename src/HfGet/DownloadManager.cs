using Spectre.Console;
using System.Net.Http.Headers;
// =======================
// DOWNLOAD ENGINE
// =======================

static class DownloadManager
{
    public static async Task Run(
        List<DownloadItem> items,
        string targetDir,
        HttpClient client,
        int threads)
    {
        await AnsiConsole.Progress().StartAsync(async ctx =>
        {
            var sem = new SemaphoreSlim(threads);

            var tasks = items.Select(async item =>
            {
                await sem.WaitAsync();
                try
                {
                    await DownloadSingle(item, targetDir, client, ctx);
                }
                finally
                {
                    sem.Release();
                }
            });

            await Task.WhenAll(tasks);
        });

        AnsiConsole.MarkupLine("[green]All downloads completed[/]");
    }

    static async Task DownloadSingle(
        DownloadItem item,
        string targetDir,
        HttpClient client,
        ProgressContext ctx)
    {
        var path = Path.Combine(targetDir, item.File);
        var tmp = path + ".part";

        var task = ctx.AddTask($"[cyan]{item.File}[/]");

        long existing = File.Exists(tmp) ? new FileInfo(tmp).Length : 0;

        using var req = new HttpRequestMessage(HttpMethod.Get, item.Url);

        if (existing > 0)
            req.Headers.Range = new RangeHeaderValue(existing, null);

        using var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
        res.EnsureSuccessStatusCode();

        var total = (res.Content.Headers.ContentLength ?? 0) + existing;

        task.MaxValue = total;
        task.Value = existing;

        await using var stream = await res.Content.ReadAsStreamAsync();
        await using var file = new FileStream(tmp, FileMode.Append, FileAccess.Write);

        var buffer = new byte[1024 * 1024];
        int read;

        while ((read = await stream.ReadAsync(buffer)) > 0)
        {
            await file.WriteAsync(buffer.AsMemory(0, read));
            task.Increment(read);
        }

        file.Close();
        File.Move(tmp, path, true);

        task.Value = total;
    }
}
