using System.ComponentModel;
using System.Diagnostics;
using GuiCliGuide.Models;

namespace GuiCliGuide.Services;

public static class CommandExecutor
{
    public static bool Execute(GuiActionInsight insight)
    {
        if (string.IsNullOrWhiteSpace(insight.Command))
        {
            return false;
        }

        try
        {
            var info = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{insight.Command}\"",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Normal
            };

            Process.Start(info);
            return true;
        }
        catch (Win32Exception)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}
