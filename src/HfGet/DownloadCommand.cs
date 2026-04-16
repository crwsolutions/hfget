using Spectre.Console;
using Spectre.Console.Cli;

internal class DownloadCommand : AsyncCommand<DownloadSettings>
{
    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        DownloadSettings settings,
        CancellationToken cancellationToken)
    {
        var (repo, variant) = Utils.ParseModel(settings.Model);

        Directory.CreateDirectory(settings.TargetDir);

        using var client = Utils.CreateHttpClient(settings.Token);

        var files = await Utils.GetRepoFiles(client, repo);

        var modelFile = files.FirstOrDefault(f =>
            f.Name.EndsWith(".gguf") &&
            f.Name.Contains(variant, StringComparison.OrdinalIgnoreCase));

        var mmprojFile = files.FirstOrDefault(f =>
            f.Name.Contains("mmproj", StringComparison.OrdinalIgnoreCase));

        if (modelFile == null)
        {
            AnsiConsole.MarkupLine("[red]Model file not found[/]");
            return -1;
        }

        var downloads = new List<DownloadItem>
        {
            new(repo, modelFile.Name)
        };

        if (mmprojFile != null)
            downloads.Add(new(repo, mmprojFile.Name));

        // Determine the actual target directory
        string actualTargetDir = settings.TargetDir;
        if (settings.Folder)
        {
            // Sanitize the model name to create a folder name
            // Replace invalid characters with _
            var sanitizedName = repo.Replace("/", "_").Replace(":", "_")
                .Replace("\\", "_").Replace("/", "_");
            actualTargetDir = Path.Combine(settings.TargetDir, sanitizedName);
            Directory.CreateDirectory(actualTargetDir);
        }

        await DownloadManager.Run(
            downloads,
            actualTargetDir,
            client,
            settings.Threads);

        // Determine final paths for model and mmproj files
        string modelPath;
        string? mmprojPath = null;

        if (settings.Folder)
        {
            modelPath = Path.Combine(actualTargetDir, modelFile.Name);
            if (mmprojFile != null)
            {
                mmprojPath = Path.Combine(actualTargetDir, mmprojFile.Name);
            }
        }
        else
        {
            modelPath = Path.Combine(actualTargetDir, modelFile.Name);
            if (mmprojFile != null)
            {
                mmprojPath = Path.Combine(actualTargetDir, mmprojFile.Name);
            }
        }

        PrintLlamaCommand(modelPath, mmprojPath);

        return 0;
    }

    static void PrintLlamaCommand(string modelPath, string? mmprojPath)
    {
        var cmd = $".\\llama-server.exe -m \"{modelPath}\"";

        if (!string.IsNullOrWhiteSpace(mmprojPath))
        {
            cmd += $" -mm \"{mmprojPath}\"";
        }

        AnsiConsole.MarkupLine("\n[green]Run command:[/]");
        AnsiConsole.MarkupLine($"[white]{cmd}[/]");
        AnsiConsole.WriteLine();
    }
}