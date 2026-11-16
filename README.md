# GUI CLI Guide

A Windows 11 desktop app that teaches users what the GUI is doing behind the scenes by translating hovered controls into a likely CLI equivalent.

## What it does

- Tracks the mouse while the user moves around the desktop or other Windows apps.
- Uses UI Automation and window/process metadata to inspect the element under the cursor.
- Classifies the control into patterns like:
  - Close / Exit
  - Minimize
  - Launch / Open
  - Generic app action
- Shows the likely CLI command and an explanation.
- Lets the user copy or explicitly run the equivalent command after confirmation.

## Safety model

This app does not silently execute commands. It is designed as a teaching tool.

The general pattern is:

1. Hover over a control.
2. Preview the likely CLI equivalent.
3. Review the explanation and risk.
4. Explicitly run it only after the user chooses to do so.

## Example translations

- Close button on an app window -> `taskkill /PID <pid> /T /F`
- Minimize button -> window state change, shown as a shell-friendly explanation and PowerShell example
- Launch button -> `Start-Process "<path>"`

## Build

From a Windows machine with .NET 8 SDK installed:

```powershell
dotnet restore src\WindowsGuiCliGuide.sln
dotnet build src\WindowsGuiCliGuide.sln
```

Then run:

```powershell
dotnet run --project src\GuiCliGuide\GuiCliGuide.csproj
```

## Notes

This is an MVP designed to help users understand GUI actions as system-level operations. It is intentionally conservative and educational-first.
