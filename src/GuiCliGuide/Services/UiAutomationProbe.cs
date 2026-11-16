using System.Diagnostics;
using System.Windows.Automation;
using GuiCliGuide.Interop;
using GuiCliGuide.Models;

namespace GuiCliGuide.Services;

public static class UiAutomationProbe
{
    public static HoverSnapshot? GetSnapshotFromCursor(int x, int y)
    {
        try
        {
            var element = AutomationElement.FromPoint(new System.Windows.Point(x, y));
            if (element is null)
            {
                return null;
            }

            var nativeWindowHandle = element.Current.NativeWindowHandle;
            var processId = 0;

            if (nativeWindowHandle != IntPtr.Zero)
            {
                User32.GetWindowThreadProcessId(nativeWindowHandle, out processId);
            }

            var processName = string.Empty;
            var processPath = string.Empty;

            if (processId > 0)
            {
                try
                {
                    using var process = Process.GetProcessById(processId);
                    processName = process.ProcessName;
                    processPath = process.MainModule?.FileName ?? string.Empty;
                }
                catch
                {
                    processName = string.Empty;
                    processPath = string.Empty;
                }
            }

            return new HoverSnapshot
            {
                ElementName = element.Current.Name,
                ClassName = element.Current.ClassName ?? string.Empty,
                ControlType = element.Current.LocalizedControlType ?? string.Empty,
                ProcessName = processName,
                ProcessId = processId,
                ProcessPath = processPath,
                WindowHandle = nativeWindowHandle
            };
        }
        catch
        {
            return null;
        }
    }
}
