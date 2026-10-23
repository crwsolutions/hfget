using Spectre.Console.Cli;

var app = new CommandApp();

app.SetDefaultCommand<DownloadCommand>();

var version = $"v{typeof(Program).Assembly.GetName().Version}";

app.Configure(config =>
{
    config.SetApplicationName("hfget");
    config.SetApplicationVersion(version);

    config.AddCommand<DownloadCommand>("download")
        .WithAlias("get");

    config.AddCommand<ListCommand>("list");
});

return app.Run(args);
