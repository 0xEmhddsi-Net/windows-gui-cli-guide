namespace GuiCliGuide.Models;

public sealed class HoverSnapshot
{
    public string ElementName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string ControlType { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public int ProcessId { get; set; } = 0;
    public string ProcessPath { get; set; } = string.Empty;
    public IntPtr WindowHandle { get; set; }
}
