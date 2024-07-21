using Spectre.Console;
using Spectre.Console.Cli;

namespace Stleganographer.Console
{
    public class DecodeCommand : Command<DecodeSettings>
    {
        public override int Execute(CommandContext context, DecodeSettings settings)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(settings.InputPath);
            ArgumentNullException.ThrowIfNullOrEmpty(settings.EncryptionKey);

            if (!File.Exists(settings.InputPath))
            {
                AnsiConsole.MarkupLine("[red]Input path does not exist![/]");
                return 1;
            }

            try
            {
                var payload = Steganographer.Decode(settings.InputPath, settings.InputFormat, settings.EncryptionKey);

                AnsiConsole.MarkupLine("[underline green]Successfully decoded STL![/]");
                AnsiConsole.WriteLine($"Payload: {payload}");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[red]Failed to decode STL![/]");
                AnsiConsole.WriteException(ex);
                return 1;
            }

            return 0;
        }
    }
}
