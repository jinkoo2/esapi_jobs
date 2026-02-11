# esapi_job_processor

ESAPI standalone application that processes pending jobs from the `_data/jobs/` directory.

## Overview

This application scans the `_data/jobs/` folder for job subdirectories, reads each `job.json` to find jobs with `"status": "pending"`, and processes them in order of `created_at` (oldest first).

For each pending job, it dynamically compiles and executes the `job.cs` source file using `CSharpCodeProvider` (CodeDOM). The compiled script must contain a public class `EsapiScript` with a `public void run(Application app)` method.

## Job Structure

Each job lives in `_data/jobs/<job_folder>/` and contains:
- `job.json` - Job metadata (id, status, created_at, src_file, etc.)
- `job.cs` - C# source code to compile and execute

## Job Lifecycle

1. Processor reads `job.json` and checks `status == "pending"`
2. Status is updated to `"running"`
3. `job.cs` is compiled in-memory and `EsapiScript.run(app)` is invoked
4. On success: status is updated to `"completed"` with `completed_at` timestamp
5. On failure: status is updated to `"failed"` with `error_message`

## Build & Run

```
/esapi build esapi_job_processor debug
/esapi run esapi_job_processor debug
```
