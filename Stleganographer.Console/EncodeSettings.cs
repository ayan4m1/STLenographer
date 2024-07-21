using Spectre.Console.Cli;

namespace Stleganographer.Console
{
    public class EncodeSettings : CommandSettings
    {
        [CommandArgument(1, "<input>")]
        public string? InputPath { get; set; }

        [CommandArgument(2, "[output]")]
        public string? OutputPath { get; set; }

        [CommandOption("-i|--input-format")]
        public StlFormat InputFormat { get; set; }

        [CommandOption("-o|--output-format")]
        public StlFormat OutputFormat { get; set; }

        [CommandOption("-k|--key")]
        public string? EncryptionKey { get; set; }

        [CommandOption("-p|--payload")]
        public string? EncryptionPayload { get; set; }

        [CommandOption("-f|--force")]
        public bool ForceOverwrite { get; set; }
    }
}
