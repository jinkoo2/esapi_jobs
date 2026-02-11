using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
// [assembly: ESAPIScript(IsWriteable = true)]

namespace esapi_job_processor
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      try
      {
        using (Application app = Application.CreateApplication())
        {
          Execute(app);
        }
      }
      catch (Exception e)
      {
        Console.Error.WriteLine(e.ToString());
      }
    }

    static void Execute(Application app)
    {
      string jobsDir = Path.GetFullPath(Path.Combine(
          Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
          @"..\..\..\..\_data\jobs"));

      if (!Directory.Exists(jobsDir))
      {
        Console.Error.WriteLine("Jobs directory not found: " + jobsDir);
        return;
      }

      // Collect all pending jobs with their created_at timestamps
      var pendingJobs = new List<Tuple<string, DateTime>>();

      foreach (string jobFolder in Directory.GetDirectories(jobsDir))
      {
        string jsonPath = Path.Combine(jobFolder, "job.json");
        if (!File.Exists(jsonPath))
          continue;

        string json = File.ReadAllText(jsonPath);

        string status = ParseJsonStringValue(json, "status");
        if (status != "pending")
          continue;

        string createdAt = ParseJsonStringValue(json, "created_at");
        DateTime createdDate;
        if (!DateTime.TryParse(createdAt, out createdDate))
          createdDate = DateTime.MaxValue;

        pendingJobs.Add(Tuple.Create(jobFolder, createdDate));
      }

      // Sort by created_at (oldest first)
      pendingJobs.Sort((a, b) => a.Item2.CompareTo(b.Item2));

      if (pendingJobs.Count == 0)
      {
        Console.WriteLine("No pending jobs found.");
        return;
      }

      Console.WriteLine("Found {0} pending job(s). Processing...", pendingJobs.Count);

      foreach (var job in pendingJobs)
      {
        string jobFolder = job.Item1;
        string jobName = Path.GetFileName(jobFolder);
        string jsonPath = Path.Combine(jobFolder, "job.json");
        string json = File.ReadAllText(jsonPath);

        string srcFile = ParseJsonStringValue(json, "src_file");
        string srcPath = Path.Combine(jobFolder, srcFile);

        Console.WriteLine();
        Console.WriteLine("=== Processing: {0} ===", jobName);

        if (!File.Exists(srcPath))
        {
          Console.Error.WriteLine("Source file not found: " + srcPath);
          UpdateJobStatus(jsonPath, json, "failed", "Source file not found: " + srcFile);
          continue;
        }

        string code = File.ReadAllText(srcPath);

        try
        {
          UpdateJobStatus(jsonPath, json, "running", null);
          json = File.ReadAllText(jsonPath);

          CompileAndRun(code, app);

          UpdateJobStatus(jsonPath, json, "completed", null);
          Console.WriteLine("=== Completed: {0} ===", jobName);
        }
        catch (Exception ex)
        {
          Console.Error.WriteLine("Error executing {0}: {1}", jobName, ex.Message);
          UpdateJobStatus(jsonPath, json, "failed", ex.ToString());
        }
      }

      Console.WriteLine();
      Console.WriteLine("All pending jobs processed.");
    }

    static void CompileAndRun(string code, Application app)
    {
      var provider = new CSharpCodeProvider();
      var parameters = new CompilerParameters
      {
        GenerateInMemory = true,
        GenerateExecutable = false
      };

      // Add references from loaded assemblies
      foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
      {
        if (!asm.IsDynamic && !string.IsNullOrEmpty(asm.Location))
          parameters.ReferencedAssemblies.Add(asm.Location);
      }

      CompilerResults result = provider.CompileAssemblyFromSource(parameters, code);

      if (result.Errors.HasErrors)
      {
        foreach (CompilerError error in result.Errors)
          Console.Error.WriteLine(error.ToString());
        throw new Exception("Compilation failed.");
      }

      Assembly assembly = result.CompiledAssembly;
      Type type = assembly.GetType("EsapiScript");
      if (type == null)
        throw new Exception("Class 'EsapiScript' not found in job script.");

      var method = type.GetMethod("run");
      if (method == null)
        throw new Exception("Method 'run' not found in EsapiScript class.");

      object instance = Activator.CreateInstance(type);
      method.Invoke(instance, new object[] { app });
    }

    static void UpdateJobStatus(string jsonPath, string json, string newStatus, string errorMessage)
    {
      json = ReplaceJsonStringValue(json, "status", newStatus);
      json = ReplaceJsonStringValue(json, "updated_at", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));

      if (newStatus == "completed")
        json = ReplaceJsonStringValue(json, "completed_at", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));

      if (errorMessage != null)
        json = ReplaceJsonStringValue(json, "error_message", errorMessage);

      File.WriteAllText(jsonPath, json);
    }

    static string ParseJsonStringValue(string json, string key)
    {
      string pattern = "\"" + key + "\"";
      int idx = json.IndexOf(pattern);
      if (idx < 0) return null;

      int colonIdx = json.IndexOf(':', idx + pattern.Length);
      if (colonIdx < 0) return null;

      // Find the value after the colon
      int start = colonIdx + 1;
      while (start < json.Length && json[start] == ' ') start++;

      if (start >= json.Length) return null;

      if (json[start] == '"')
      {
        int end = json.IndexOf('"', start + 1);
        if (end < 0) return null;
        return json.Substring(start + 1, end - start - 1);
      }

      if (json.Substring(start, 4) == "null")
        return null;

      return null;
    }

    static string ReplaceJsonStringValue(string json, string key, string newValue)
    {
      string pattern = "\"" + key + "\"";
      int idx = json.IndexOf(pattern);
      if (idx < 0) return json;

      int colonIdx = json.IndexOf(':', idx + pattern.Length);
      if (colonIdx < 0) return json;

      int start = colonIdx + 1;
      while (start < json.Length && json[start] == ' ') start++;

      if (start >= json.Length) return json;

      string newValueStr = newValue != null ? "\"" + newValue + "\"" : "null";

      if (json[start] == '"')
      {
        int end = json.IndexOf('"', start + 1);
        if (end < 0) return json;
        return json.Substring(0, start) + newValueStr + json.Substring(end + 1);
      }

      if (start + 4 <= json.Length && json.Substring(start, 4) == "null")
      {
        return json.Substring(0, start) + newValueStr + json.Substring(start + 4);
      }

      return json;
    }
  }
}
