namespace GuiCliGuide.Models;

public sealed class GuiActionInsight
{
    public string ControlDisplayName { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public string ControlType { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public int ProcessId { get; set; } = 0;
    public string Risk { get; set; } = "Low";
}
