using Spectre.Console.Cli;
// =======================
// DOWNLOAD COMMAND
// =======================

internal class DownloadSettings : CommandSettings
{
    [CommandArgument(0, "<model>")]
    public string Model { get; set; } = "";

    [CommandArgument(1, "[targetDir]")]
    public string TargetDir { get; set; } = ".";

    [CommandOption("--token")]
    public string? Token { get; set; }

    [CommandOption("--threads")]
    public int Threads { get; set; } = 4;
}
