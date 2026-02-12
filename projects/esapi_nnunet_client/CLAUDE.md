# esapi_nnunet_client

WPF desktop application (.NET Framework 4.7.2, x64) that integrates with Varian ESAPI v16.1 for medical image segmentation and treatment planning.

**Namespace**: `nnunet_client`

## Build

```
/esapi build esapi_nnunet_client [debug|release]
```

NuGet packages are restored to the project-local `packages/` folder. Shared dependencies (Newtonsoft.Json, SimpleITK) are in `../../packages/`.

Post-build copies SimpleITK native DLLs and JSON config files to output.

## Architecture (MVVM)

- **ViewModels** (`viewmodels/`): Inherit `BaseViewModel` with `SetProperty<T>()` for change notification and auto-save with 3-second debounce. Commands use `RelayCommand`/`RelayCommand2`.
- **Views** (`views/`): XAML UserControls. Converters in `views/Converters.cs`.
- **Models** (`models/`): Domain objects (DoseLimit, SubmitJob, SegmentationTemplate, etc.).
- **Windows** (`windows/`): Top-level WPF windows launched from MainWindow.

## Entry Point & Global State

- `main.cs`: `Program.Main()` creates `VMSApplication`, sets up WPF exception handler, launches `MainWindow`.
- `global.cs`: Static class holding `VMSApplication`, `VMSPatient`, and `AppConfig`. Config loaded from `config.json` via `load_config()`.

## Key Modules

| Module | Window | ViewModel | Purpose |
|--------|--------|-----------|---------|
| Auto-Contour | `AutoContourWindow` | `AutoContourViewModel` | Submit images to nnU-Net server for segmentation |
| Bladder ART | `BladderART` | `BladderAutoPlanViewModel` | Automated treatment planning |
| Dose Limits | `DoseLimitChecker` | `DoseLimitCheckerViewModel` | Check dose constraints against structure sets |
| Submit Jobs | `SubmitImageAndLabelsWindow` | `SubmitImageAndLabelsViewModel` | Queue-based image+label submission |

## Service Layer

- `nnUNetServiceClient.cs`: HTTP REST client for nnU-Net server v2 API. Auth via email+token from config.
- `services/JobQueueService.cs`: File-based job queue storing JSON in `{app_data_dir}/submit_queue/`.
- `SubmitJobWorker.cs`: Background worker for processing queued jobs.

## Inlined Dependencies

- `esapi/` -- Shared ESAPI utility library (image export, structure helpers, SimpleITK wrappers)
- `variandb/` -- Varian database query utilities

## Configuration

`config.json` (copied to output on build):
- `nnunet_server_url`, `nnunet_server_auth_email`, `nnunet_server_auth_token`
- `data_root_secure`, `app_data_dir`

## Key Dependencies

- Varian ESAPI 16.1
- SimpleITK 1.2.4 (native x64 DLLs)
- Newtonsoft.Json
- MaterialDesignThemes, ControlzEx, Extended.Wpf.Toolkit
- Fleck (WebSocket)
