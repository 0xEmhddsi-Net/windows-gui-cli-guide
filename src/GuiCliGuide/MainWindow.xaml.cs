using System.Windows;
using System.Windows.Media;
using GuiCliGuide.Models;
using GuiCliGuide.Services;

namespace GuiCliGuide;

public partial class MainWindow : Window
{
    private readonly MouseHoverTracker _hoverTracker;
    private GuiActionInsight? _currentInsight;
    private bool _paused;
    private bool _detailsVisible = true;

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
            Explanation = "The app inspects accessible UI metadata. It does not click controls or run commands while you hover.",
            DetailedExplanation = "The app reads accessibility metadata from the element under the pointer. This allows it to explain common Windows window actions and show safe, read-only process information without performing the action itself.",
            TechnicalSummary = "Inspection ready. No specific control selected yet.",
            Confidence = "Medium",
            Risk = "Low"
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
        DetailedText.Text = insight.DetailedExplanation;
        ConfidenceText.Text = insight.Confidence;
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

        ConfidenceText.Foreground = insight.Confidence switch
        {
            "High" => new SolidColorBrush(Color.FromRgb(0x0B, 0x7A, 0x5A)),
            "Medium" => new SolidColorBrush(Color.FromRgb(0x7C, 0x3A, 0xED)),
            _ => new SolidColorBrush(Color.FromRgb(0x9A, 0x5B, 0x00))
        };

        StatusText.Text = _paused ? "Paused" : "Live inspection";
        StatusBadge.Background = _paused
            ? new SolidColorBrush(Color.FromRgb(0xF3, 0xE8, 0xCB))
            : new SolidColorBrush(Color.FromRgb(0xE6, 0xF4, 0xEA));
        StatusText.Foreground = _paused
            ? new SolidColorBrush(Color.FromRgb(0x7C, 0x2D, 0x12))
            : new SolidColorBrush(Color.FromRgb(0x0E, 0x5B, 0x3A));
    }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentInsight is not null)
        {
            Clipboard.SetText(_currentInsight.Command);
        }
    }

    private void CopyFullReport_Click(object sender, RoutedEventArgs e)
    {
        if (_currentInsight is null)
        {
            return;
        }

        var report = $"Control: {_currentInsight.ControlDisplayName}\n" +
                     $"Process: {_currentInsight.ProcessName} (PID: {_currentInsight.ProcessId})\n" +
                     $"Control Type: {_currentInsight.ControlType}\n" +
                     $"Risk: {_currentInsight.Risk}\n" +
                     $"Confidence: {_currentInsight.Confidence}\n\n" +
                     $"Example:\n{_currentInsight.Command}\n\n" +
                     $"Explanation:\n{_currentInsight.Explanation}\n\n" +
                     $"Technical summary:\n{_currentInsight.TechnicalSummary}";

        Clipboard.SetText(report);
    }

    private void TogglePause_Click(object sender, RoutedEventArgs e)
    {
        _paused = !_paused;
        if (_paused)
        {
            _hoverTracker.Pause();
        }
        else
        {
            _hoverTracker.Resume();
        }

        UpdateUi(_currentInsight ?? new GuiActionInsight
        {
            ControlDisplayName = "Move the pointer over a control in another app",
            Command = "Hover over a control to see an educational example.",
            Explanation = "The app inspects accessible UI metadata. It does not click controls or run commands while you hover.",
            DetailedExplanation = "The app reads accessibility metadata from the element under the pointer. This allows it to explain common Windows window actions and show safe, read-only process information without performing the action itself.",
            TechnicalSummary = "Inspection ready. No specific control selected yet.",
            Confidence = "Medium",
            Risk = "Low"
        });
    }

    private void ToggleDetails_Click(object sender, RoutedEventArgs e)
    {
        _detailsVisible = !_detailsVisible;
        DetailsPanel.Visibility = _detailsVisible ? Visibility.Visible : Visibility.Collapsed;
        DetailsButtonText.Text = _detailsVisible ? "Hide Details" : "Show Details";
    }

    private void ToggleAlwaysOnTop_Click(object sender, RoutedEventArgs e) => Topmost = !Topmost;
}
