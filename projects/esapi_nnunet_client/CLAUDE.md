# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

WPF desktop application (.NET Framework 4.7.2, x64) that integrates with **Varian Eclipse Scripting API (ESAPI) v16.1** for medical image segmentation and treatment planning. Provides auto-contouring via nnU-Net server, dose limit checking, and job submission capabilities.

**Namespace**: `nnunet_client`

## Build Commands

```batch
# Build (defaults to Debug x64)
build.bat

# Build with specific configuration
build.bat Debug
build.bat Release
```

The build script auto-detects MSBuild from Visual Studio and builds dependencies first:
1. `../esapi/esapi.csproj` (shared ESAPI wrapper library)
2. `../MahApps.Metro-develop-net48/` (UI framework, local source)
3. Main project (`esapi_nnunet_client.csproj`)

Post-build copies SimpleITK native DLLs and JSON config files to output.

**Debug output**: `\\VarianFS\VA_DATA$\ProgramData\Vision\PublishedScripts\bladder_art\bin\`
**Release output**: `bin\release\esapi_nnunet_client.exe`

## Architecture

### MVVM Pattern

- **ViewModels** (`viewmodels/`): Inherit from `BaseViewModel` which provides `SetProperty<T>()` for change notification and auto-save with 3-second debounce. Commands use `RelayCommand`/`RelayCommand2`.
- **Views** (`views/`): XAML UserControls. Value converters in `views/Converters.cs`.
- **Models** (`models/`): Domain objects (DoseLimit, SubmitJob, SegmentationTemplate, etc.).
- **Windows** (`windows/`): Top-level WPF windows launched from MainWindow.

### Entry Point & Global State

- `main.cs`: `Program.Main()` creates `VMSApplication` (ESAPI connection), sets up WPF exception handler, launches `MainWindow`.
- `global.cs`: Static class holding `VMSApplication`, `VMSPatient`, and `AppConfig`. Config loaded from `config.json` in execution folder via `load_config()`.

### Key Modules

| Module | Window | ViewModel | Purpose |
|--------|--------|-----------|---------|
| Auto-Contour | `AutoContourWindow` | `AutoContourViewModel` | Submit images to nnU-Net server for segmentation |
| Bladder ART | `BladderART` | `BladderAutoPlanViewModel` | Automated treatment planning (PTV creation, beam setup) |
| Dose Limits | `DoseLimitChecker` | `DoseLimitCheckerViewModel` | Check dose constraints against structure sets |
| Submit Jobs | `SubmitImageAndLabelsWindow` | `SubmitImageAndLabelsViewModel` | Queue-based image+label submission to nnU-Net |

### Service Layer

- `nnUNetServiceClient.cs`: HTTP REST client for nnU-Net server v2 API. Auth via email+token from config.
- `services/JobQueueService.cs`: File-based job queue storing JSON in `{app_data_dir}/submit_queue/`. Manages job lifecycle (Pending → Processing → Completed/Failed).
- `SubmitJobWorker.cs` / `SubmitJobWorkerMain.cs`: Background workers for processing queued jobs.

### Configuration

`AppConfig.cs` defines config shape loaded from `config.json`:
- `nnunet_server_url`, `nnunet_server_auth_email`, `nnunet_server_auth_token` — server connection
- `data_root_secure` — secure data path
- `app_data_dir` — local app data directory
- User identity fields for job attribution

## Key Dependencies

- **Varian ESAPI 16.1** — `C:\Program Files (x86)\Varian\RTM\16.1\esapi\API\`
- **MahApps.Metro** — WPF UI framework (built from local source)
- **SimpleITK** — Medical image processing (native x64 DLLs)
- **Newtonsoft.Json** — JSON serialization
- **MaterialDesignThemes** — UI styling
- **Fleck** — WebSocket support

## Constraints

- x64 only (SimpleITK requirement)
- .NET Framework 4.7.2 (ESAPI compatibility — cannot migrate to .NET Core)
- Windows only (WPF + ESAPI)
- No test framework configured; `DoseLimitEvaluator.RunTests()` is the only test-like method
