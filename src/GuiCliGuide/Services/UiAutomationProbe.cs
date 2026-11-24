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
            var parentName = string.Empty;
            var parentClassName = string.Empty;

            var parent = TreeWalker.ControlViewWalker.GetParent(element);
            if (parent is not null)
            {
                var parentCurrent = parent.Current;
                parentName = parentCurrent.Name ?? string.Empty;
                parentClassName = parentCurrent.ClassName ?? string.Empty;
            }

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

            var bounds = current.BoundingRectangle;
            var boundsText = bounds == default ? "unknown" : $"{bounds.Left},{bounds.Top},{bounds.Width},{bounds.Height}";

            return new HoverSnapshot
            {
                ElementName = current.Name ?? string.Empty,
                ClassName = current.ClassName ?? string.Empty,
                ParentName = parentName,
                ParentClassName = parentClassName,
                ControlType = current.ControlType?.ProgrammaticName ?? current.LocalizedControlType ?? string.Empty,
                AutomationId = current.AutomationId ?? string.Empty,
                FrameworkId = current.FrameworkId ?? string.Empty,
                ProcessName = processName,
                ProcessId = processId,
                ProcessPath = processPath,
                WindowHandle = current.NativeWindowHandle == 0 ? IntPtr.Zero : new IntPtr(current.NativeWindowHandle),
                Bounds = boundsText
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
