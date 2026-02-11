//---------------------------------------------------------------------------------------------
/// <summary>
/// Example project for the PluginTester ESAPI standalone application.
/// Developed by Eduardo Acosta eacosta@med.umich.edu
/// </summary>
/// <license>
// Copyright (c) 2014 University of Michigan.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in 
//  all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.
/// </license>
//---------------------------------------------------------------------------------------------
//
/// Using Instructions
///  See instructions in PluginTester.cs in PluginTester project

using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

using PluginScriptExample;

namespace VMS.TPS
{
  public class Script
  {
    public Script()
    {
    }

    /// <summary>
    /// This is the method called by Eclipse when script starts
    /// </summary>
    /// <param name="context"></param>
    /// <param name="window"></param>
    public void Execute(ScriptContext context, System.Windows.Window window)
    {
        List<PlanningItem> PItemsInScope = new List<PlanningItem>();
        foreach (var pitem in context.PlansInScope)
            PItemsInScope.Add(pitem);
        foreach (var pitem in context.PlanSumsInScope)
            PItemsInScope.Add(pitem);

        PlanningItem openedPItem = null;
        if (context.PlanSetup != null)
            openedPItem = context.PlanSetup;
        else
            openedPItem = context.PlanSumsInScope.First();

        Start(context.Patient, PItemsInScope, openedPItem, context.CurrentUser, window);
    }

    /// <summary>
    /// Starts execution of script. This method can be called directly from PluginTester or indirectly from Eclipse
    /// through the Execute method.
    /// </summary>
    /// <param name="patient">Opened patient</param>
    /// <param name="PItemsInScope">Planning Items in scope</param>
    /// <param name="pItem">Opened Planning Item</param>
    /// <param name="currentUser">Current user</param>
    /// <param name="window">WPF window</param>
    public static void Start(Patient patient, List<PlanningItem> PItemsInScope, PlanningItem pItem, User currentUser, Window window)
    {
        try
        {
            // get reference to selected plan

            MainControl main = new MainControl();
            main.pItem = pItem;
            main.patient = patient;
            main.user = currentUser;
            main.PItemsInScope = PItemsInScope;

            if (pItem is PlanSetup)
            {
                PlanSetup plan = (PlanSetup)pItem;
                main.StructureSet = plan.StructureSet;
            }
            else
            {
                PlanSum planSum = (PlanSum)pItem;
                main.StructureSet = planSum.PlanSetups.First().StructureSet;
            }
            var dockPanel = new System.Windows.Controls.DockPanel();
            dockPanel.Children.Add(main);
            window.Width = 800;
            window.Height = 350;
            window.Content = dockPanel;

        }
        catch (Exception e)
        {
            throw e;
        }
    }
  }
}
