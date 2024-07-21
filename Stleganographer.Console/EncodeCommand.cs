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
                AnsiConsole.MarkupLine("[red]Input path does not exist![/]");
                return 1;
            }

            if (File.Exists(settings.OutputPath) && !settings.ForceOverwrite.GetValueOrDefault(false))
            {
                AnsiConsole.MarkupLine("[red]Output path already exists![/]");
                return 1;
            }

            try
            {
                Steganographer.Encode(
                    settings.InputPath,
                    settings.OutputPath,
                    settings.InputFormat,
                    settings.OutputFormat,
                    settings.EncryptionPayload,
                    settings.EncryptionKey
                );
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[red]Failed to encode STL![/]");
                AnsiConsole.WriteException(ex);
                return 1;
            }


            AnsiConsole.MarkupLine($"[green]Successfully hid message in {settings.OutputPath}[/]");
            return 0;
        }
    }
}
