# ESAPI Projects Workspace

Monorepo for Varian Eclipse Scripting API (ESAPI v16.1) applications. All projects target .NET Framework 4.7.2, x64, Windows only.

## Directory Structure

```
.
├── esapi/              # Shared ESAPI library source, templates, examples, docs
│   └── templates/      # Project templates (standalone, binary_plugin, single_file_plugin)
├── packages/           # Shared dependency packages (NewtonSoft, SimpleITK, etc.)
├── plugins/            # Output directory for ESAPI plugin DLLs
├── projects/           # All project source code
│   ├── esapi_autocontour/           # ESAPI plugin: CT image export to ITK formats
│   ├── esapi_job_processor/         # Standalone: dynamic C# job executor
│   ├── esapi_nnunet_client/         # Standalone WPF: main nnU-Net client app
│   └── esapi_nnunet_submit_dataset/ # Standalone: background worker for dataset submission
├── scripts/            # C# script templates for job processor
└── _data/              # Runtime data (jobs, configs)
    └── jobs/           # Job queue for esapi_job_processor
```

## Build System

All projects use MSBuild. The `/esapi` slash command provides build/run shortcuts:

```
/esapi build <project_name> [debug|release]
/esapi run <project_name> [debug|release]
/esapi create <project_type> <project_name>
```

MSBuild is located at:
`C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe`

**Do NOT use** the .NET Framework 4.0 MSBuild (`C:\Windows\Microsoft.NET\Framework64\v4.0.30319\`) as it doesn't support C# 7.0+.

When building from Git Bash, use `//p:` (double slash) instead of `/p:` to avoid path interpretation.

## Shared Dependencies

All located in `./packages/` (root level):
- **Varian ESAPI 16.1** -- `C:\Program Files (x86)\Varian\RTM\16.1\esapi\API\`
- **Newtonsoft.Json** -- `packages/NewtonSoft/`
- **SimpleITK 1.2.4** -- `packages/SimpleITK-1.2.4-CSharp-win64-x64/`
- **MaterialDesignThemes** -- via NuGet in project-local `packages/`

## Constraints

- x64 only (SimpleITK and ESAPI requirement)
- .NET Framework 4.7.2 (ESAPI compatibility -- cannot migrate to .NET Core)
- Windows only (WPF + ESAPI)
- No test framework configured
