using Xunit;

namespace GuiCliGuide.Tests;

public class GuiCommandClassifierTests
{
    [Fact]
    public void Classify_WhenCloseTextPresent_ReturnsHighRiskAndCloseGuidance()
    {
        var snapshot = new GuiCliGuide.Models.HoverSnapshot
        {
            ElementName = "Close",
            ControlType = "Button",
            ProcessName = "notepad",
            ProcessId = 123,
            AutomationId = "Close"
        };

        var result = GuiCliGuide.Services.GuiCommandClassifier.Classify(snapshot);

        Assert.Equal("High", result.Risk);
        Assert.Equal("High", result.Confidence);
        Assert.Contains("close", result.Explanation, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("close", result.DetailedExplanation, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Classify_WhenGenericElementPresent_ReturnsProcessInspectionExample()
    {
        var snapshot = new GuiCliGuide.Models.HoverSnapshot
        {
            ElementName = "Save As",
            ControlType = "Button",
            ProcessName = "winword",
            ProcessId = 456,
            AutomationId = "SaveAs"
        };

        var result = GuiCliGuide.Services.GuiCommandClassifier.Classify(snapshot);

        Assert.Equal("Low", result.Risk);
        Assert.Equal("Medium", result.Confidence);
        Assert.Contains("Get-Process", result.Command, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("inspect", result.DetailedExplanation, StringComparison.OrdinalIgnoreCase);
    }
}
