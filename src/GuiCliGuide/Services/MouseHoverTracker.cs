using System.Windows.Threading;
using GuiCliGuide.Interop;
using GuiCliGuide.Models;

namespace GuiCliGuide.Services;

/// <summary>Polls the cursor and inspects the UI Automation element beneath it.</summary>
public sealed class MouseHoverTracker : IDisposable
{
    private readonly DispatcherTimer _timer;
    private string? _lastKey;
    private bool _isPaused;

    public event Action<HoverSnapshot>? Hovered;

    public MouseHoverTracker()
    {
        _timer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(350)
        };
        _timer.Tick += OnTick;
    }

    public bool IsPaused => _isPaused;

    public void Start() => _timer.Start();

    public void Pause() => _isPaused = true;

    public void Resume() => _isPaused = false;

    public void Dispose()
    {
        _timer.Stop();
        _timer.Tick -= OnTick;
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (_isPaused)
        {
            return;
        }

        if (!User32.GetCursorPos(out var point))
        {
            return;
        }

        var snapshot = UiAutomationProbe.GetSnapshotFromCursor(point.x, point.y);
        if (snapshot is null)
        {
            return;
        }

        var key = $"{snapshot.ProcessId}|{snapshot.WindowHandle}|{snapshot.AutomationId}|{snapshot.ElementName}|{snapshot.ClassName}";
        if (key == _lastKey)
        {
            return;
        }

        _lastKey = key;
        Hovered?.Invoke(snapshot);
    }
}
