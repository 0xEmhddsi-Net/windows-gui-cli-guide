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
        var controlSummary = BuildControlSummary(snapshot);

        if (HasAny(snapshot.ElementName, "close", "exit", "quit") ||
            snapshot.AutomationId.Equals("Close", StringComparison.OrdinalIgnoreCase))
        {
            return new GuiActionInsight
            {
                ControlDisplayName = displayName,
                ControlType = snapshot.ControlType,
                AutomationId = snapshot.AutomationId,
                FrameworkId = snapshot.FrameworkId,
                ProcessName = processName,
                ProcessPath = snapshot.ProcessPath,
                ProcessId = pid,
                Risk = "High",
                Confidence = "High",
                Command = "# No exact CLI equivalent: a close action usually requests an orderly window shutdown.",
                Explanation = $"This appears to be a close/exit control for {processName}. A GUI close usually asks the app to close its window or shutdown gracefully, which may trigger save prompts or cleanup logic.",
                DetailedExplanation = $"The hovered element appears to be a close action. In Windows, there is often no universal CLI equivalent for closing an arbitrary app control; the nearest safe educational example is to inspect the owning process and call out that the app is asking to exit gracefully. {controlSummary}",
                TechnicalSummary = $"Element: {displayName}; Type: {snapshot.ControlType}; PID: {pid}; Parent: {FirstNonEmpty(snapshot.ParentName, snapshot.ParentClassName, "unknown")}; Bounds: {snapshot.Bounds}."
            };
        }

        if (HasAny(snapshot.ElementName, "minimize", "minimise") ||
            snapshot.AutomationId.Equals("Minimize", StringComparison.OrdinalIgnoreCase))
        {
            return new GuiActionInsight
            {
                ControlDisplayName = displayName,
                ControlType = snapshot.ControlType,
                AutomationId = snapshot.AutomationId,
                FrameworkId = snapshot.FrameworkId,
                ProcessName = processName,
                ProcessPath = snapshot.ProcessPath,
                ProcessId = pid,
                Risk = "Low",
                Confidence = "High",
                Command = "# Windows API equivalent: ShowWindow(hwnd, SW_MINIMIZE)",
                Explanation = "Minimize changes the window's display state without terminating the application. It is a window-manager operation, not a process launch or shutdown command.",
                DetailedExplanation = $"This appears to be a minimize action. The app is likely changing the window state to the minimized form, which is a common Windows UI operation rather than a direct CLI task. {controlSummary}",
                TechnicalSummary = $"Element: {displayName}; Type: {snapshot.ControlType}; PID: {pid}; Parent: {FirstNonEmpty(snapshot.ParentName, snapshot.ParentClassName, "unknown")}; Bounds: {snapshot.Bounds}."
            };
        }

        if (HasAny(snapshot.ElementName, "restore") ||
            snapshot.AutomationId.Equals("Restore", StringComparison.OrdinalIgnoreCase))
        {
            return new GuiActionInsight
            {
                ControlDisplayName = displayName,
                ControlType = snapshot.ControlType,
                AutomationId = snapshot.AutomationId,
                FrameworkId = snapshot.FrameworkId,
                ProcessName = processName,
                ProcessPath = snapshot.ProcessPath,
                ProcessId = pid,
                Risk = "Low",
                Confidence = "High",
                Command = "# Windows API equivalent: ShowWindow(hwnd, SW_RESTORE)",
                Explanation = "Restore changes a window's display state and returns it to a previous size/location without launching a new process.",
                DetailedExplanation = $"This appears to be a restore action. It is normally a window-manager state change rather than a direct shell command. {controlSummary}",
                TechnicalSummary = $"Element: {displayName}; Type: {snapshot.ControlType}; PID: {pid}; Parent: {FirstNonEmpty(snapshot.ParentName, snapshot.ParentClassName, "unknown")}; Bounds: {snapshot.Bounds}."
            };
        }

        return new GuiActionInsight
        {
            ControlDisplayName = displayName,
            ControlType = snapshot.ControlType,
            AutomationId = snapshot.AutomationId,
            FrameworkId = snapshot.FrameworkId,
            ProcessName = processName,
            ProcessPath = snapshot.ProcessPath,
            ProcessId = pid,
            Risk = "Low",
            Confidence = "Medium",
            Command = $"{processSelector} | Select-Object ProcessName, Id",
            Explanation = "No reliable one-to-one CLI equivalent was identified. This read-only example inspects the process behind the UI element and avoids performing unsafe automation.",
            DetailedExplanation = $"The hovered element is not obviously a standard close/minimize/restore control. It may be a generic button, menu item, or child control with app-specific behavior. The safest teacher-friendly output is to inspect the owning process and explain that many GUI actions require app-specific logic. {controlSummary}",
            TechnicalSummary = $"Element: {displayName}; Type: {snapshot.ControlType}; PID: {pid}; Parent: {FirstNonEmpty(snapshot.ParentName, snapshot.ParentClassName, "unknown")}; Bounds: {snapshot.Bounds}."
        };
    }

    private static string BuildControlSummary(HoverSnapshot snapshot)
    {
        var parent = FirstNonEmpty(snapshot.ParentName, snapshot.ParentClassName, "unknown parent");
        var automationId = string.IsNullOrWhiteSpace(snapshot.AutomationId) ? "none" : snapshot.AutomationId;
        var framework = string.IsNullOrWhiteSpace(snapshot.FrameworkId) ? "unknown" : snapshot.FrameworkId;
        var bounds = string.IsNullOrWhiteSpace(snapshot.Bounds) ? "unknown" : snapshot.Bounds;

        return $"Control metadata: AutomationId={automationId}; Framework={framework}; Parent={parent}; Bounds={bounds}.";
    }

    private static bool HasAny(string value, params string[] terms) =>
        terms.Any(term => value.Contains(term, StringComparison.OrdinalIgnoreCase));

    private static string FirstNonEmpty(params string[] values) =>
        values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))?.Trim() ?? "Unnamed UI element";
}
