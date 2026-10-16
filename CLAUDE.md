# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project state

This is a .NET 10 MAUI app (`TelemetryMobile/`) generated from the stock MAUI "sample content" template. The current domain code (projects, tasks, categories, tags) is template scaffolding, not telemetry functionality yet. There are no test projects.

## Commands

Run from the repo root (solution is `TelemetryMobile.slnx`, single project `TelemetryMobile/TelemetryMobile.csproj`):

```powershell
dotnet build TelemetryMobile/TelemetryMobile.csproj -f net10.0-windows10.0.19041.0   # fastest local build on Windows
dotnet build TelemetryMobile/TelemetryMobile.csproj -f net10.0-android
dotnet build TelemetryMobile/TelemetryMobile.csproj -f net10.0-windows10.0.19041.0 -t:Run   # build and launch on Windows
dotnet build TelemetryMobile/TelemetryMobile.csproj -f net10.0-android -t:Run               # deploy to running emulator/device
```

Target frameworks are OS-conditional: Android always; iOS/MacCatalyst on non-Linux; Windows only on Windows. Always pass `-f` — building without it builds every TFM. `WindowsPackageType=None` means Windows runs unpackaged.

## Architecture

- **MVVM with CommunityToolkit.Mvvm**: `Pages/*.xaml` are views; `PageModels/*PageModel.cs` are `ObservableObject` partial classes using `[ObservableProperty]` (private `_camelCase` fields) and `[RelayCommand]`. Page lifecycle (`NavigatedTo`, `NavigatedFrom`, `Appearing`) is wired to page-model commands via `toolkit:EventToCommandBehavior` in XAML, not code-behind.
- **Compiled bindings / XAML source gen**: the csproj sets `MauiXamlInflator=SourceGen` and compiled bindings with source; every page/template needs `x:DataType`, and bindings across a `x:Reference` need an explicit `x:DataType` on the binding.
- **DI and navigation** are all in `MauiProgram.cs`. Repositories, `SeedDataService`, `ModalErrorHandler`, and the top-level page models (`MainPageModel`, `ProjectListPageModel`, `ManageMetaPageModel`) are singletons. Detail pages are registered with `AddTransientWithShellRoute<Page, PageModel>("route")` (`project`, `task`), and receive parameters via query strings (`GoToAsync($"project?id={id}")`) handled in `IQueryAttributable.ApplyQueryAttributes`. Top-level Shell routes (`//main`, `//projects`, `//manage`) are defined in `AppShell.xaml`.
- **Data layer**: hand-written ADO.NET over `Microsoft.Data.Sqlite` (no ORM). Each repository in `Data/` opens a new `SqliteConnection(Constants.DatabasePath)` per call and lazily runs `CREATE TABLE IF NOT EXISTS` in a private `Init()`. Schema lives in those SQL strings — changing a model means updating the repository's table DDL and queries. `ProjectRepository` composes `TaskRepository` and `TagRepository` to populate `Project.Tasks`/`Tags`; tags are many-to-many via the `ProjectsTags` table owned by `TagRepository`.
- **Seed data**: on first launch `MainPageModel` calls `SeedDataService.LoadSeedDataAsync()`, which drops all tables and loads `Resources/Raw/SeedData.json` (deserialized with the source-generated `JsonContext` — add new serialized types there). Completion is tracked by the `is_seeded` key in `Preferences`; clear it (or app data) to reseed.
- **Global usings** (`GlobalUsings.cs`) import `TelemetryMobile.Data`, `PageModels`, `Pages`, `Services`, `Utilities`, and `Fonts`, so those namespaces are usually not imported per file.
- **UI libraries**: CommunityToolkit.Maui and Syncfusion.Maui.Toolkit (charts, segmented control, pull-to-refresh). Icons are Fluent System Icons glyphs exposed as constants in `Resources/Fonts/FluentUI.cs`; shared colors/styles are in `Resources/Styles/` (`Colors.xaml`, `Styles.xaml`, `AppStyles.xaml`). Windows-specific handler tweaks live in `MauiProgram.cs` under `#if WINDOWS`.
- **Errors**: page models catch exceptions and route them to `ModalErrorHandler.HandleError`, which shows a Shell alert.
