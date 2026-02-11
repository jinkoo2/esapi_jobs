using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using VMS.TPS.Common.Model.API;

namespace VMS.TPS
{
  public class Script
  {
    public Script()
    {
    }

    public void Execute(ScriptContext context /*, System.Windows.Window window*/)
    {
      PlanSetup plan = context.PlanSetup;
      if (plan == null)
      {
        MessageBox.Show("No plan in context, cannot continue.");
      }
      else
      {
        string filename = MakeValidFilenameFromId(plan.Id);
        XmlSerializer serializer = new XmlSerializer(plan.GetType());
        using (TextWriter textWriter = new StreamWriter(filename))
        {
          serializer.Serialize(textWriter, plan);
        }
        System.Diagnostics.Process.Start(filename);
      }
    }
    string MakeValidFilenameFromId(string id)
    {
      string directory = "c:\\temp\\";
      if (!Directory.Exists(directory))
      {
        Directory.CreateDirectory(directory);
      }
      char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
      foreach (char ch in invalidChars)
      {
        if (id.Contains(ch))
        {
          id = id.Replace(ch, '_');
        }
      }
      string name = directory + id + ".xml";
      return name;
    }
  }
}
