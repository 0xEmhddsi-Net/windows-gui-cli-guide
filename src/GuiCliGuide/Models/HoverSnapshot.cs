using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Automation;
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

        var defaultInsight = GuiCommandClassifier.Classify(new HoverSnapshot
        {
            ElementName = "Waiting for mouse movement",
            ClassName = "System",
            ControlType = "None",
            ProcessName = "System",
            ProcessId = 0,
            ProcessPath = string.Empty,
            WindowHandle = IntPtr.Zero
        });

        UpdateUi(defaultInsight);
    }

    private void OnHovered(HoverSnapshot snapshot)
    {
        var insight = GuiCommandClassifier.Classify(snapshot);
        Dispatcher.BeginInvoke(() => UpdateUi(insight));
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
            "Medium" => new SolidColorBrush(Color.FromRgb(0xFDE68A)),
            _ => new SolidColorBrush(Color.FromRgb(0xDB, 0xF4, 0xE8))
        };

        RiskText.Foreground = insight.Risk switch
        {
            "High" => new SolidColorBrush(Color.FromRgb(0x7F, 0x1D, 0x1D)),
            "Medium" => new SolidColorBrush(Color.FromRgb(0x7C, 0x4A, 0x00)),
            _ => new SolidColorBrush(Color.FromRgb(0x0E, 0x5B, 0x3A))
        };
    }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentInsight is null)
        {
            return;
        }

        Clipboard.SetText(_currentInsight.Command);
    }

    private void ExecuteButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentInsight is null)
        {
            return;
        }

        var result = CommandExecutor.Execute(_currentInsight);
        if (!result)
        {
            MessageBox.Show("This command could not be run. Check the generated command and try again.", "Execution failed", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void ToggleAlwaysOnTop_Click(object sender, RoutedEventArgs e)
    {
        Topmost = !Topmost;
    }
}
