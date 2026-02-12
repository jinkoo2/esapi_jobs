# esapi_job_processor

Standalone console application that processes pending jobs from the `_data/jobs/` directory. For each job, it dynamically compiles and executes a C# script file using `CSharpCodeProvider` (CodeDOM).

**Namespace**: `esapi_job_processor`

## Build & Run

```
/esapi build esapi_job_processor [debug|release]
/esapi run esapi_job_processor [debug|release]
```

## How It Works

1. Scans `_data/jobs/` for subdirectories containing `job.json`
2. Finds jobs with `"status": "pending"`, ordered by `created_at` (oldest first)
3. Updates status to `"running"`
4. Compiles `job.cs` in-memory using CodeDOM
5. Invokes `EsapiScript.run(Application app)` on the compiled class
6. Updates status to `"completed"` or `"failed"` with error message

## Job Structure

Each job lives in `_data/jobs/<job_folder>/` and contains:
- `job.json` -- Job metadata (id, status, created_at, src_file, etc.)
- `job.cs` -- C# source code to compile and execute (must have `public class EsapiScript` with `public void run(Application app)`)

## Dependencies

- Varian ESAPI 16.1
- Microsoft.CSharp / CodeDOM (for runtime compilation)
