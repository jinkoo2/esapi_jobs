# esapi_nnunet_submit_dataset

Standalone console application (.NET Framework 4.7.2, x64) that runs as a background worker, processing jobs from a file-based queue to export medical images and structure labels from Eclipse (ESAPI) and submit them to an nnU-Net training server.

**Namespace**: `nnunet_client`

## Build & Run

```
/esapi build esapi_nnunet_submit_dataset [debug|release]
/esapi run esapi_nnunet_submit_dataset [debug|release]
```

## Entry Point

`SubmitJobWorkerMain.Main()` in `SubmitJobWorkerMain.cs`:
1. Creates ESAPI `VMSApplication`
2. Loads `config.json`
3. Starts `SubmitJobWorker` continuous loop
4. Handles Ctrl+C for graceful shutdown

## Worker Loop

The worker polls the queue directory every 60 seconds:
1. Finds pending jobs (oldest first)
2. Opens patient in Eclipse via ESAPI
3. Exports base CT image as `.mha` using `esapi.exporter.export_image()`
4. Exports each structure as mask image using `esapi.exporter.export_structure_as_mask_image()`
5. Combines masks into single Int32 label image via SimpleITK
6. Submits image + labels to nnU-Net server via REST API (multipart POST)
7. Updates job status (Pending -> Processing -> Completed/Failed)
8. Cleans up temp folders older than 7 days

## Source Files

| File | Purpose |
|------|---------|
| `SubmitJobWorkerMain.cs` | Entry point, ESAPI app init, exception handling |
| `SubmitJobWorker.cs` | Core worker loop, image export, label combining, server submission |
| `nnUNetServiceClient.cs` | HTTP client for nnU-Net REST API with bearer token auth |
| `AppConfig.cs` | Configuration model loaded from `config.json` |
| `global.cs` | Global state (VMSApplication, AppConfig) |
| `helper.cs` | Logging and utility functions |
| `models/SubmitJob.cs` | Job data model (PatientId, DatasetId, LabelMappings, status) |
| `services/JobQueueService.cs` | File-based queue management (enqueue, dequeue, status updates) |

## Inlined Dependencies

- `esapi/` -- Shared ESAPI utility library (image export, structure helpers)
- `variandb/` -- Varian database query utilities

## Configuration

`config.json` (copied to output on build):
- `nnunet_server_url`, `nnunet_server_auth_email`, `nnunet_server_auth_token`
- `data_root_secure`, `app_data_dir` (must point to valid, accessible paths)

## Dependencies

- Varian ESAPI 16.1
- SimpleITK 1.2.4 (from `../../packages/`)
- Newtonsoft.Json (from `../../packages/`)
