using Spectre.Console;
using Spectre.Console.Cli;

namespace Stleganographer.Console
{
    public class EncodeCommand : Command<EncodeSettings>
    {
        public override int Execute(CommandContext context, EncodeSettings settings)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(settings.InputPath);
            ArgumentNullException.ThrowIfNullOrEmpty(settings.OutputPath);
            ArgumentNullException.ThrowIfNullOrEmpty(settings.EncryptionKey);
            ArgumentNullException.ThrowIfNullOrEmpty(settings.EncryptionPayload);

            if (!File.Exists(settings.InputPath))
            {
                AnsiConsole.WriteLine("[red]Input path does not exist![/]");
                return 1;
            }

            if (File.Exists(settings.OutputPath))
            {
                AnsiConsole.WriteLine("[red]Output path already exists![/]");
                return 1;
            }

            if (!settings.InputFormat.HasValue)
            {
                AnsiConsole.WriteLine("[red]Input STL format was not supplied![/]");
                return 1;
            }

            if (!settings.OutputFormat.HasValue)
            {
                AnsiConsole.WriteLine("[red]Output STL format was not supplied![/]");
                return 1;
            }

            try
            {
                Steganographer.Encode(
                    settings.InputPath,
                    settings.OutputPath,
                    settings.InputFormat.GetValueOrDefault(StlFormat.ASCII),
                    settings.OutputFormat.GetValueOrDefault(StlFormat.ASCII),
                    settings.EncryptionPayload,
                    settings.EncryptionKey
                );
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine("[red]Failed to encode STL![/]");
                AnsiConsole.WriteException(ex);
                return 1;
            }

            return 0;
        }
    }
}
