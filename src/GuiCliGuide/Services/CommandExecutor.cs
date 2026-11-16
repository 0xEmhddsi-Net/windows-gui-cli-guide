using System.Diagnostics;
using System.Runtime.InteropServices;
using GuiCliGuide.Interop;
using GuiCliGuide.Models;
using GuiCliGuide.Services;

namespace GuiCliGuide.Services;

public sealed class MouseHoverTracker : IDisposable
{
    private readonly LowLevelMouseProc _callback;
    private readonly object _syncRoot = new();
    private IntPtr _hookHandle;
    private long _lastNotificationTick;

    public event Action<HoverSnapshot>? Hovered;

    public MouseHoverTracker()
    {
        _callback = HookCallback;
    }

    public void Start()
    {
        if (_hookHandle != IntPtr.Zero)
        {
            return;
        }

        _hookHandle = User32.SetWindowsHookEx(
            User32.WH_MOUSE_LL,
            _callback,
            IntPtr.Zero,
            0);

        if (_hookHandle == IntPtr.Zero)
        {
            throw new InvalidOperationException("Unable to install the mouse hook.");
        }
    }

    public void Dispose()
    {
        if (_hookHandle != IntPtr.Zero)
        {
            User32.UnhookWindowsHookEx(_hookHandle);
            _hookHandle = IntPtr.Zero;
        }
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)User32.WM_MOUSEMOVE)
        {
            var hookStruct = Marshal.PtrToStructure<User32.MSLLHOOKSTRUCT>(lParam);
            var snapshot = UiAutomationProbe.GetSnapshotFromCursor(hookStruct.pt.x, hookStruct.pt.y);
            if (snapshot is not null)
            {
                var now = DateTime.UtcNow.Ticks;
                if (now - _lastNotificationTick > TimeSpan.FromMilliseconds(150).Ticks)
                {
                    _lastNotificationTick = now;
                    Hovered?.Invoke(snapshot);
                }
            }
        }

        return User32.CallNextHookEx(_hookHandle, nCode, wParam, lParam);
    }

    public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
}
