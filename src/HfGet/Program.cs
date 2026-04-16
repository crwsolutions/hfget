using Spectre.Console.Cli;

var app = new CommandApp();

app.SetDefaultCommand<DownloadCommand>();

app.Configure(config =>
{
    config.SetApplicationName("hfget");

    config.AddCommand<DownloadCommand>("download")
        .WithAlias("get");

    config.AddCommand<ListCommand>("list");
});

return app.Run(args);
