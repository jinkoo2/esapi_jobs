using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using VMS.TPS.Common.Model.Types;
using VMS.TPS.Common.Model.API;


namespace VMS.TPS
{
  public class Script
  {
    public Script()
    {
    }

    public void Execute(ScriptContext context, System.Windows.Window window)
    {
      window.Width = 400;
      window.Height = 600;
      window.Title = "PTV Centroid";

      ListView view = new ListView();
      window.Content = view;

      StructureSet structureSet = context.StructureSet;
      if (structureSet == null)
      {
        MessageBox.Show("No structureset in context, cannot continue");
        return;
      }

      const string structureToFind = "PTV";

      Structure target = (from s in structureSet.Structures where s.Id == structureToFind select s).FirstOrDefault();
      if (target == null)
      {
        MessageBox.Show("Structure with id " + structureToFind + " was not found in structureset " + structureSet.Id);
        return;
      }

      var currentStudy = structureSet.Image.Series.Study;
      var allVolumeImagesOfStudy = from series in currentStudy.Series
                                   from image in series.Images
                                   where image.ZSize > 1
                                   select image;

      view.Items.Add("Center point (mm) of structure " + structureToFind + " in current study:");

      foreach (var ss in context.Patient.StructureSets)
      {
        if (allVolumeImagesOfStudy.Contains(ss.Image))
        {
          string header = "Structure set " + ss.Id + ":";
          view.Items.Add(header);
          Structure otherPtv = (from s in ss.Structures where s.Id == structureToFind select s).FirstOrDefault();
          if (otherPtv != null)
          {
            VVector center = otherPtv.CenterPoint;
            string msg = "\t" + center.x.ToString("F2") + ", "
                              + center.y.ToString("F2") + ", "
                              + center.z.ToString("F2");
            view.Items.Add(msg);
          }
        }
      }
    }
  }
}
