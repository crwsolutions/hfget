using Spectre.Console.Cli;
// =======================
// LIST COMMAND
// =======================

internal class ListSettings : CommandSettings
{
    [CommandArgument(0, "<model>")]
    public string Model { get; set; } = "";

    [CommandOption("--token")]
    public string? Token { get; set; }
}
