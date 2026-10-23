using System.ComponentModel;
using Spectre.Console.Cli;

// =======================
// DOWNLOAD COMMAND
// =======================

internal class DownloadSettings : CommandSettings
{
    [CommandArgument(0, "<model>")]
    [Description("Model identifier in format 'author/model:variant'")]
    public string Model { get; set; } = "";

    [CommandArgument(1, "[targetDir]")]
    [Description("Destination directory (default: current directory)")]
    public string TargetDir { get; set; } = ".";

    [CommandOption("--token")]
    [Description("Hugging Face API token (for private models)")]
    public string? Token { get; set; }

    [CommandOption("--threads")]
    [Description("Number of parallel download threads (default: 4)")]
    public int Threads { get; set; } = 4;

    [CommandOption("-f|--folder")]
    [Description("Create a folder with the model name")]
    public bool Folder { get; set; } = false;
}