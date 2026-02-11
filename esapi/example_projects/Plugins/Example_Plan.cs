using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

// Do not change namespace and class name
// otherwise Eclipse will not be able to run the script.
namespace VMS.TPS
{
  class Script
  {
    public Script()
    {
    }

    // Second parameter is commented out because we do not need a window for this script
    public void Execute(ScriptContext context /*, System.Windows.Window window */)
    {
      // Retrieve list of plans displayed in Scope Window
      IEnumerable<PlanSetup> plansInScope = context.PlansInScope;
      if (plansInScope.Count() == 0)
      {
        MessageBox.Show("Scope Window does not contain any plans.");
        return;
      }

      // Retrieve plan names
      List<string> planIDs = new List<string>();
      foreach (var ps in plansInScope)
      {
        planIDs.Add(ps.Id);
      }
     
      // Construct output message
      string message = string.Format("Hello {0}, number of plans in Scope Window is {1} ({2}). \n\nWhen more than one plan are present in Scope Window a short comparison follows.",
        context.CurrentUser.Name,
        plansInScope.Count(),
        string.Join(",", planIDs));

      // If more than one plan, create a short comparison list
      if (plansInScope.Count() > 1)
      {
        // Display some additional plan information
        PlanSetup plan1 = plansInScope.ElementAt(0);
        PlanSetup plan2 = plansInScope.ElementAt(1);
        
        if (plan1.StructureSet != null && plan2.StructureSet != null)
        {
          Image image1 = plan1.StructureSet.Image;
          Image image2 = plan2.StructureSet.Image;
          var structures1 = plan1.StructureSet.Structures;
          var structures2 = plan2.StructureSet.Structures;
          var beams1 = plan1.Beams;
          var beams2 = plan2.Beams;
          Fractionation fractionation1 = plan1.UniqueFractionation;
          Fractionation fractionation2 = plan2.UniqueFractionation;
          message += string.Format("\n* Image IDs for the first two plans: {0}, {1}", image1.Id, image2.Id);
          message += string.Format("\n* Number of structures defined in plans: {0} and {1} accordingly.", structures1.Count(), structures2.Count());
          message += string.Format("\n* Number of Beams: {0} and {1}.", beams1.Count(), beams2.Count());
          message += string.Format("\n* Number of Fractions: {0} and {1}.", fractionation1.NumberOfFractions, fractionation2.NumberOfFractions);
        }
      }
      MessageBox.Show(message);
    }
  }
}
