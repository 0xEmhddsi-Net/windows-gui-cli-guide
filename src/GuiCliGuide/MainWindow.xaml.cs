using System.Windows;
using System.Windows.Media;
using GuiCliGuide.Models;
using GuiCliGuide.Services;

namespace GuiCliGuide;

public partial class MainWindow : Window
{
    private readonly MouseHoverTracker _hoverTracker;
    private GuiActionInsight? _currentInsight;

    public MainWindow()
    {
        InitializeComponent();

        _hoverTracker = new MouseHoverTracker();
        _hoverTracker.Hovered += OnHovered;
        _hoverTracker.Start();
        Closing += (_, _) => _hoverTracker.Dispose();

        UpdateUi(new GuiActionInsight
        {
            ControlDisplayName = "Move the pointer over a control in another app",
            Command = "Hover over a control to see an educational example.",
            Explanation = "The app inspects accessible UI metadata. It does not click controls or run commands while you hover."
        });
    }

    private void OnHovered(HoverSnapshot snapshot)
    {
        if (snapshot.ProcessId == Environment.ProcessId)
        {
            return;
        }

        UpdateUi(GuiCommandClassifier.Classify(snapshot));
    }

    private void UpdateUi(GuiActionInsight insight)
    {
        _currentInsight = insight;
        ControlNameText.Text = insight.ControlDisplayName;
        ProcessInfoText.Text = $"Process: {insight.ProcessName} | PID: {insight.ProcessId} | Control: {insight.ControlType}";
        CommandTextBox.Text = insight.Command;
        ExplanationText.Text = insight.Explanation;
        RiskText.Text = insight.Risk;
        RiskBadge.Background = insight.Risk switch
        {
            "High" => new SolidColorBrush(Color.FromRgb(0xF3, 0x9A, 0x9A)),
            "Medium" => new SolidColorBrush(Color.FromRgb(0xFD, 0xE6, 0x8A)),
            _ => new SolidColorBrush(Color.FromRgb(0xDB, 0xF4, 0xE8))
        };
        RiskText.Foreground = insight.Risk == "High"
            ? new SolidColorBrush(Color.FromRgb(0x7F, 0x1D, 0x1D))
            : new SolidColorBrush(Color.FromRgb(0x0E, 0x5B, 0x3A));
    }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentInsight is not null)
        {
            Clipboard.SetText(_currentInsight.Command);
        }
    }

    private void ToggleAlwaysOnTop_Click(object sender, RoutedEventArgs e) => Topmost = !Topmost;
}
