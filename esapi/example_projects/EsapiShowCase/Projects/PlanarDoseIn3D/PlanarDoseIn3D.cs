using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace VMS.TPS
{
  public class Script
  {
    public Script()
    {
    }

    public void Execute(ScriptContext context, System.Windows.Window window)
    {
      Dose dose = null;
      VMS.TPS.Common.Model.API.Image image = null;
      PlanSum psum = context.PlanSumsInScope.FirstOrDefault();
      if (psum != null && psum.Dose != null)
      {
        dose = psum.Dose;
        image = psum.PlanSetups.First().StructureSet.Image;
      }
      else
      {
        PlanSetup ps = context.PlanSetup;
        if (ps != null && ps.Dose != null)
        {
          dose = ps.Dose;
          image = ps.StructureSet.Image;
        }
      }

      if (dose == null || image == null)
      {
        MessageBox.Show("No plan/plansum with dose in context!");
        return;
      }

      PlanarDoseIn3D.MainControl mainControl = new PlanarDoseIn3D.MainControl();
      window.Content = mainControl;
      mainControl.SetDoseAndImage(dose, image);

    }
  }
}
