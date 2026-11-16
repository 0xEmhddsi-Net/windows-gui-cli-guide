using GuiCliGuide.Models;

namespace GuiCliGuide.Services;

public static class GuiCommandClassifier
{
    public static GuiActionInsight Classify(HoverSnapshot snapshot)
    {
        var displayName = NormalizeDisplayName(snapshot.ElementName, snapshot.ClassName, snapshot.ControlType);

        if (LooksLikeCloseAction(snapshot))
        {
            return new GuiActionInsight
            {
                ControlDisplayName = displayName,
                ControlType = snapshot.ControlType,
                ProcessName = snapshot.ProcessName,
                ProcessId = snapshot.ProcessId,
                Risk = "High",
                Command = snapshot.ProcessId > 0 ? $"taskkill /PID {snapshot.ProcessId} /T /F" : "taskkill /PID <pid> /T /F",
                Explanation = $"This control appears to be a close or exit button for '{snapshot.ProcessName}'. In Windows, close actions often map to terminating the owning process tree."
            };
        }

        if (LooksLikeMinimizeAction(snapshot))
        {
            return new GuiActionInsight
            {
                ControlDisplayName = displayName,
                ControlType = snapshot.ControlType,
                ProcessName = snapshot.ProcessName,
                ProcessId = snapshot.ProcessId,
                Risk = "Low",
                Command = snapshot.ProcessId > 0 ? $"powershell -NoProfile -Command \"$hwnd = (Get-Process -Id {snapshot.ProcessId}).MainWindowHandle; if ($hwnd) {{ [User32]::ShowWindow($hwnd, 2) | Out-Null }}\"" : "powershell -NoProfile -Command \"$hwnd = (Get-Process -Id <pid>).MainWindowHandle; if ($hwnd) { [User32]::ShowWindow($hwnd, 2) | Out-Null }\"",
                Explanation = $"This likely minimizes the app window rather than ending the process. Many window buttons are UI-state changes, not process terminations."
            };
        }

        if (LooksLikeLaunchAction(snapshot))
        {
            var path = string.IsNullOrWhiteSpace(snapshot.ProcessPath) ? snapshot.ProcessName : snapshot.ProcessPath;
            return new GuiActionInsight
            {
                ControlDisplayName = displayName,
                ControlType = snapshot.ControlType,
                ProcessName = snapshot.ProcessName,
                ProcessId = snapshot.ProcessId,
                Risk = "Medium",
                Command = $"Start-Process \"{path}\"",
                Explanation = $"This looks like a launch/open control that starts the app or a related executable behind the GUI."
            };
        }

        var fallbackCommand = snapshot.ProcessId > 0 ? $"Get-Process -Id {snapshot.ProcessId} | Select-Object ProcessName, Id" : "Get-Process | Select-Object ProcessName, Id";
        return new GuiActionInsight
        {
            ControlDisplayName = displayName,
            ControlType = snapshot.ControlType,
            ProcessName = snapshot.ProcessName,
            ProcessId = snapshot.ProcessId,
            Risk = "Low",
            Command = fallbackCommand,
            Explanation = $"This element is tied to '{snapshot.ProcessName}' but does not map to a one-to-one shell command. Many GUI controls call system APIs or state changes instead of executing a literal terminal command."
        };
    }

    private static bool LooksLikeCloseAction(HoverSnapshot snapshot)
    {
        var text = $"{snapshot.ElementName} {snapshot.ControlType} {snapshot.ClassName}";
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        return text.Contains("close", StringComparison.OrdinalIgnoreCase)
            || text.Contains("exit", StringComparison.OrdinalIgnoreCase)
            || text.Contains("quit", StringComparison.OrdinalIgnoreCase)
            || string.Equals(snapshot.ElementName.Trim(), "X", StringComparison.OrdinalIgnoreCase)
            || string.Equals(snapshot.ElementName.Trim(), "Close", StringComparison.OrdinalIgnoreCase);
    }

    private static bool LooksLikeMinimizeAction(HoverSnapshot snapshot)
    {
        var text = $"{snapshot.ElementName} {snapshot.ControlType} {snapshot.ClassName}";
        return text.Contains("minimize", StringComparison.OrdinalIgnoreCase)
            || text.Contains("minimise", StringComparison.OrdinalIgnoreCase)
            || text.Contains("restore", StringComparison.OrdinalIgnoreCase)
            || text.Contains("hide", StringComparison.OrdinalIgnoreCase);
    }

    private static bool LooksLikeLaunchAction(HoverSnapshot snapshot)
    {
        var text = $"{snapshot.ElementName} {snapshot.ControlType} {snapshot.ClassName}";
        return text.Contains("open", StringComparison.OrdinalIgnoreCase)
            || text.Contains("launch", StringComparison.OrdinalIgnoreCase)
            || text.Contains("start", StringComparison.OrdinalIgnoreCase)
            || text.Contains("run", StringComparison.OrdinalIgnoreCase)
            || text.Contains("new", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeDisplayName(string elementName, string className, string controlType)
    {
        if (!string.IsNullOrWhiteSpace(elementName))
        {
            return elementName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(className))
        {
            return className.Trim();
        }

        return controlType.Trim();
    }
}
