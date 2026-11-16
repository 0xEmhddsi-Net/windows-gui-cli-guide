using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
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
            var point = new System.Windows.Point(x, y);
            var element = AutomationElement.FromPoint(point);
            if (element is null)
            {
                return null;
            }

            var name = element.Current.Name;
            var className = element.Current.ClassName ?? string.Empty;
            var controlType = element.Current.LocalizedControlType ?? string.Empty;
            var nativeWindowHandle = element.Current.NativeWindowHandle;

            int processId = 0;
            if (nativeWindowHandle != IntPtr.Zero)
            {
                User32.GetWindowThreadProcessId(nativeWindowHandle, out processId);
            }

            string processName = string.Empty;
            string processPath = string.Empty;
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
                }
            }

            return new HoverSnapshot
            {
                ElementName = name,
                ClassName = className,
                ControlType = controlType,
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
