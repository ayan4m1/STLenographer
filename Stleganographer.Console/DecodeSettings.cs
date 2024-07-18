using Spectre.Console.Cli;
using System.ComponentModel;

namespace Stleganographer.Console
{
    public class DecodeSettings : CommandSettings
    {
        [CommandArgument(1, "<input>")]
        public string? InputPath { get; set; }

        [CommandOption("-f|--format")]
        [TypeConverter(typeof(EnumConverter<StlFormat>))]
        public StlFormat? InputFormat { get; set; }

        [CommandOption("-k|--key")]
        public string? EncryptionKey { get; set; }
    }
}
