using System.ComponentModel;
using Spectre.Console.Cli;

// =======================
// LIST COMMAND
// =======================

internal class ListSettings : CommandSettings
{
    [CommandArgument(0, "<model>")]
    [Description("Model identifier in format 'author/model'")]
    public string Model { get; set; } = "";

    [CommandOption("--token")]
    [Description("Hugging Face API token (for private models)")]
    public string? Token { get; set; }
}