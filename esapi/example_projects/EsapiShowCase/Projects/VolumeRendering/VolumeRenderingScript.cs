using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
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
      window.Closed += new EventHandler(OnWindowClosed);
      window.Width = 800;
      window.Height = 600;
      m_control = new VolumeRendering.MainControl(context.Image, context.PlanSetup != null ? context.PlanSetup.Dose : null);
      window.Content = m_control;
    }

    void OnWindowClosed(object sender, EventArgs e)
    {
      if (m_control != null)
      {
        m_control.Exit(sender, e);
      }
    }
    VolumeRendering.MainControl m_control;
  }
}
