namespace GuiCliGuide.Models;

public sealed class HoverSnapshot
{
    public string ElementName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ParentClassName { get; set; } = string.Empty;
    public string ControlType { get; set; } = string.Empty;
    public string AutomationId { get; set; } = string.Empty;
    public string FrameworkId { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public int ProcessId { get; set; }
    public string ProcessPath { get; set; } = string.Empty;
    public IntPtr WindowHandle { get; set; }
    public string Bounds { get; set; } = string.Empty;
}
