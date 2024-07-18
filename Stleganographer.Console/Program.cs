using Spectre.Console.Cli;
using Stleganographer.Console;

var app = new CommandApp();

app.Configure(config =>
{
    config.AddCommand<EncodeCommand>("encode").WithDescription("Encodes an STL with an encrypted payload.");
    config.AddCommand<DecodeCommand>("decode").WithDescription("Decodes an encrypted payload from an STL, if present.");
});

await app.RunAsync(args);