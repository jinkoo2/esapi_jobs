using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

public class EsapiScript
{
    public void run(Application app)
    {
        int maxCount = __MAX_COUNT__;
        string outputPath = @"__OUTPUT_PATH__";

        // Collect all patient summaries with CreationDateTime
        var patients = new List<PatientSummary>();
        foreach (var ps in app.PatientSummaries)
        {
            patients.Add(ps);
        }

        // Sort by CreationDateTime descending (newest first)
        patients.Sort((a, b) =>
        {
            DateTime da = a.CreationDateTime.HasValue ? a.CreationDateTime.Value : DateTime.MinValue;
            DateTime db = b.CreationDateTime.HasValue ? b.CreationDateTime.Value : DateTime.MinValue;
            return db.CompareTo(da);
        });

        int count = Math.Min(maxCount, patients.Count);

        // Print to console
        Console.WriteLine("Latest {0} Patients (by CreationDateTime)", count);
        Console.WriteLine(new string('-', 80));
        Console.WriteLine("{0,-15} {1,-15} {2,-15} {3,-20}", "Patient ID", "Last Name", "First Name", "Created");
        Console.WriteLine(new string('-', 80));

        // Build JSON
        var sb = new StringBuilder();
        sb.AppendLine("[");

        for (int i = 0; i < count; i++)
        {
            var ps = patients[i];
            string created = ps.CreationDateTime.HasValue ? ps.CreationDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";

            Console.WriteLine("{0,-15} {1,-15} {2,-15} {3,-20}", ps.Id, ps.LastName, ps.FirstName, created);

            if (i > 0) sb.AppendLine(",");
            sb.AppendLine("  {");
            sb.AppendFormat("    \"Id\": {0},\n", JsonStr(ps.Id));
            sb.AppendFormat("    \"LastName\": {0},\n", JsonStr(ps.LastName));
            sb.AppendFormat("    \"FirstName\": {0},\n", JsonStr(ps.FirstName));
            sb.AppendFormat("    \"MiddleName\": {0},\n", JsonStr(ps.MiddleName));
            sb.AppendFormat("    \"Id2\": {0},\n", JsonStr(ps.Id2));
            sb.AppendFormat("    \"Sex\": {0},\n", JsonStr(ps.Sex));
            sb.AppendFormat("    \"DateOfBirth\": {0},\n", JsonStr(ps.DateOfBirth));
            sb.AppendFormat("    \"CreationDateTime\": {0}\n", JsonStr(ps.CreationDateTime));
            sb.Append("  }");
        }

        sb.AppendLine();
        sb.AppendLine("]");

        Console.WriteLine(new string('-', 80));
        Console.WriteLine("Total: {0} patients listed.", count);

        File.WriteAllText(outputPath, sb.ToString());
        Console.WriteLine("Saved to: " + outputPath);
    }

    static string JsonStr(object val)
    {
        if (val == null) return "null";
        string s = val.ToString();
        s = s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
        return "\"" + s + "\"";
    }
}
