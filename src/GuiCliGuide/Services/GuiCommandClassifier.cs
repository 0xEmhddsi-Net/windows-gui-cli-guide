using GuiCliGuide.Models;

namespace GuiCliGuide.Services;

/// <summary>
/// Produces educational examples, not claims that a GUI control literally runs a shell command.
/// </summary>
public static class GuiCommandClassifier
{
    public static GuiActionInsight Classify(HoverSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var displayName = FirstNonEmpty(snapshot.ElementName, snapshot.ClassName, snapshot.ControlType, "Unnamed UI element");
        var processName = string.IsNullOrWhiteSpace(snapshot.ProcessName) ? "Unknown process" : snapshot.ProcessName;
        var pid = snapshot.ProcessId;
        var processSelector = pid > 0 ? $"Get-Process -Id {pid}" : "Get-Process";

        if (HasAny(snapshot.ElementName, "close", "exit", "quit") ||
            snapshot.AutomationId.Equals("Close", StringComparison.OrdinalIgnoreCase))
        {
            return new GuiActionInsight
            {
                ControlDisplayName = displayName,
                ControlType = snapshot.ControlType,
                ProcessName = processName,
                ProcessId = pid,
                Risk = "High",
                Command = "# No exact CLI equivalent: a window close usually requests an orderly app shutdown.",
                Explanation = $"This appears to be a close/exit control for {processName}. A GUI close commonly sends a window-close request so the app can save work or show prompts. `taskkill` is not the same operation and can discard work; this app will not execute a suggested close command."
            };
        }

        if (HasAny(snapshot.ElementName, "minimize", "minimise") ||
            snapshot.AutomationId.Equals("Minimize", StringComparison.OrdinalIgnoreCase))
        {
            return new GuiActionInsight
            {
                ControlDisplayName = displayName,
                ControlType = snapshot.ControlType,
                ProcessName = processName,
                ProcessId = pid,
                Risk = "Low",
                Command = "# Windows API equivalent: ShowWindow(hwnd, SW_MINIMIZE)",
                Explanation = "Minimize changes the window's display state; it normally leaves the process running. The target window handle is not necessarily the same as the process's main window, so a generic CLI command would be misleading."
            };
        }

        if (HasAny(snapshot.ElementName, "restore") ||
            snapshot.AutomationId.Equals("Restore", StringComparison.OrdinalIgnoreCase))
        {
            return new GuiActionInsight
            {
                ControlDisplayName = displayName,
                ControlType = snapshot.ControlType,
                ProcessName = processName,
                ProcessId = pid,
                Risk = "Low",
                Command = "# Windows API equivalent: ShowWindow(hwnd, SW_RESTORE)",
                Explanation = "Restore changes a window's display state. It is a window-manager operation, not a process launch or termination."
            };
        }

        return new GuiActionInsight
        {
            ControlDisplayName = displayName,
            ControlType = snapshot.ControlType,
            ProcessName = processName,
            ProcessId = pid,
            Risk = "Low",
            Command = $"{processSelector} | Select-Object ProcessName, Id",
            Explanation = $"No reliable one-to-one CLI equivalent was identified. This read-only PowerShell example only displays the process associated with the UI element; many buttons invoke application code or Windows APIs rather than a shell command."
        };
    }

    private static bool HasAny(string value, params string[] terms) =>
        terms.Any(term => value.Contains(term, StringComparison.OrdinalIgnoreCase));

    private static string FirstNonEmpty(params string[] values) =>
        values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))?.Trim() ?? "Unnamed UI element";
}
