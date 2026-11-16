# GUI CLI Guide

A Windows 11 WPF teaching tool that inspects the accessible UI element beneath the pointer and explains a plausible CLI or Windows API equivalent.

## MVP behavior

- Polls the pointer and queries Windows UI Automation metadata.
- Shows the element name, control type, process and PID.
- Explains common close/minimize/restore actions and provides a read-only process-inspection example for other controls.
- Copies examples to the clipboard.

## Important limitations and safety

This app is educational, not an action recorder. UI controls do not generally invoke CLI commands directly, and there is no universal command for actions such as closing a browser tab or window. A close button usually requests an orderly application shutdown; `taskkill` is not equivalent and may lose unsaved work.

Hovering never clicks or changes the target app. The app does not execute generated commands; examples can be copied and reviewed manually. UI Automation metadata varies across apps, and a missing or ambiguous accessible name can produce an unknown classification. Protected/elevated windows may not expose complete metadata.

## Requirements

- Windows 11
- .NET 8 SDK

## Build and run

```powershell
dotnet restore src\WindowsGuiCliGuide.sln
dotnet build src\WindowsGuiCliGuide.sln --configuration Release
dotnet run --project src\GuiCliGuide\GuiCliGuide.csproj
```

Run the classifier tests:

```powershell
dotnet test src\WindowsGuiCliGuide.sln --configuration Release
```

A Windows GitHub Actions workflow builds the solution and runs the unit tests on pushes and pull requests.
