using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
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
      Patient patient = context.Patient;
      if (patient == null)
      {
        MessageBox.Show("No patient in context, cannot continue.");
      }
      else
      {
        string directory = "c:\\temp\\";
        if (!Directory.Exists(directory))
        {
          Directory.CreateDirectory(directory);
        }
        string filename = directory + patient.Id + ".xml";
        XmlSerializer serializer = new XmlSerializer(patient.GetType());
        using (TextWriter textWriter = new StreamWriter(filename))
        {
          serializer.Serialize(textWriter, patient);
        }
        System.Diagnostics.Process.Start(filename);
      }
    }
  }
}
