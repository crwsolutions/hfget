using Spectre.Console;
using Spectre.Console.Cli;

internal class ListCommand : AsyncCommand<ListSettings>
{
    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        ListSettings settings,
        CancellationToken cancellationToken)
    {
        var repo = settings.Model.Split(':')[0];

        using var client = Utils.CreateHttpClient(settings.Token);

        var files = await Utils.GetRepoFiles(client, repo);

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Type")
            .AddColumn("Name")
            .AddColumn("Size");

        foreach (var f in files)
        {
            var type =
                f.Name.EndsWith(".gguf") ? "GGUF" :
                f.Name.Contains("mmproj") ? "mmproj" :
                "other";

            table.AddRow(type, f.Name, Utils.FormatSize(f.Size));
        }

        AnsiConsole.Write(table);

        return 0;
    }
}
