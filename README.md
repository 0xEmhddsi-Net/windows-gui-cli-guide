# GUI CLI Guide

A Windows 11 WPF teaching tool that inspects the accessible UI element beneath the pointer and explains a plausible CLI or Windows API equivalent.

> **AI disclosure:** This project was created with AI assistance (GitHub Copilot). The code and documentation were generated and iterated with AI under maintainer direction. AI-generated output may contain errors; review and test changes before relying on them.

## Features

- Inspects Windows UI Automation metadata for the control beneath the pointer.
- Displays the element name, control type, process, and PID.
- Explains common close, minimize, and restore controls; otherwise provides a read-only process-inspection example.
- Includes pause/resume, confidence and risk labels, advanced details, copy-example, and copy-full-report options.
- Supports toggling always-on-top.

## Safety and limitations

This app is educational, not an action recorder. UI controls do not generally invoke CLI commands directly, and there is no universal command for actions such as closing a browser tab or window. Suggested equivalents are illustrative, not claims about an app's internal implementation.

Hovering only reads available UI Automation metadata; it does not click or change the target app. The app does not execute generated commands. Examples can be copied and reviewed manually. UI Automation metadata varies by app, and missing or incomplete information is normal.

## Requirements

- Windows 11
- .NET 8 SDK

## Build and run

From the repository root in PowerShell:

```powershell
dotnet restore src\WindowsGuiCliGuide.sln
dotnet build src\WindowsGuiCliGuide.sln --configuration Release
dotnet run --project src\GuiCliGuide\GuiCliGuide.csproj
```

Run the tests:

```powershell
dotnet test src\WindowsGuiCliGuide.sln --configuration Release
```

A Windows GitHub Actions workflow builds the solution and runs the unit tests on pushes and pull requests.
