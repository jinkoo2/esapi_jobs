# esapi_autocontour

ESAPI binary plugin (DLL) for Varian Eclipse that exports CT images to ITK-compatible formats (.mha, .mhd, .nrrd, .nii, .nii.gz).

**Namespace**: `autocontour`
**Assembly**: `autocontour.esapi` (library)
**Target**: .NET Framework 4.7.2, x64

## Build

```
/esapi build esapi_autocontour [debug|release]
```

Output goes to `../../plugins/` for Eclipse plugin deployment.

## Entry Point

Eclipse invokes `VMS.TPS.Script.Execute(ScriptContext context)` in `main.cs`, which creates and displays `MainWindow` as a dialog.

## How It Works

1. User clicks "Export Image to ITK" in the WPF dialog
2. Plugin reads voxel data slice-by-slice from the active Eclipse image via `image.GetVoxels()`
3. Creates a SimpleITK image with correct spacing and origin
4. Writes to user-selected file format using `SimpleITK.WriteImage()`

## Source Files

| File | Purpose |
|------|---------|
| `main.cs` | ESAPI script entry point (`VMS.TPS.Script.Execute`) |
| `MainWindow.xaml` | WPF UI with Export and Close buttons |
| `MainWindow.xaml.cs` | Image export logic using SimpleITK |

## Dependencies

- Varian ESAPI 16.1 (`VMS.TPS.Common.Model.API`, `VMS.TPS.Common.Model.Types`)
- SimpleITK 1.2.4 (from project-local `packages/`)
