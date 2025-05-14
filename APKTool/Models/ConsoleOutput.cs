namespace APKTool.Models;

public class ConsoleOutput
{
    public enum OutputType
    {
        Debug,
        Info,
        Process,
        Warning,
        Error,
        Success
    }

    public OutputType Type { get; set; }
    public string Output { get; set; }
    public string Time { get; set; } = $"[{DateTime.Now:HH:mm:ss}]";
    public string Prefix { get; set; }

    public ConsoleOutput(string output, OutputType type)
    {
        switch (type)
        {
            case OutputType.Debug:
                Prefix = "[*]";
                break;
            case OutputType.Info:
                Prefix = "[*]";
                break;
            case OutputType.Warning:
                Prefix = "[!]";
                break;
            case OutputType.Error:
                Prefix = "[!]";
                break;
            case OutputType.Success:
                Prefix = "[✓]";
                break;
            case OutputType.Process:
                Prefix = "[>]";
                break;
            default:
                break;
        }
        Type = type;
        Output = output;
    }
}
