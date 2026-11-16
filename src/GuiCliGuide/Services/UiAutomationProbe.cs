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

            var current = element.Current;
            var processId = current.ProcessId;
            var processName = string.Empty;
            var processPath = string.Empty;

            if (processId > 0)
            {
                try
                {
                    using var process = System.Diagnostics.Process.GetProcessById(processId);
                    processName = process.ProcessName;
                    processPath = process.MainModule?.FileName ?? string.Empty;
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or System.ComponentModel.Win32Exception)
                {
                    // The process may exit or deny access between the UI Automation query and inspection.
                }
            }

            return new HoverSnapshot
            {
                ElementName = current.Name ?? string.Empty,
                ClassName = current.ClassName ?? string.Empty,
                ControlType = current.ControlType?.ProgrammaticName ?? current.LocalizedControlType ?? string.Empty,
                AutomationId = current.AutomationId ?? string.Empty,
                FrameworkId = current.FrameworkId ?? string.Empty,
                ProcessName = processName,
                ProcessId = processId,
                ProcessPath = processPath,
                WindowHandle = current.NativeWindowHandle == 0 ? IntPtr.Zero : new IntPtr(current.NativeWindowHandle)
            };
        }
        catch (ElementNotAvailableException)
        {
            return null;
        }
        catch (System.Runtime.InteropServices.COMException)
        {
            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}
