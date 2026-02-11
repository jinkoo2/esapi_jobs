//---------------------------------------------------------------------------------------------
/// <summary>
/// Eclipse v11/v13 ESAPI standalone application that helps in the design and debugging of binary plugin scripts.
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
/// 
/// - Create your project with the Varian API Wizard as usual and add this project to the Projects folder of your solution.
/// - Add a reference to the project that contains your script
/// - Modify the btnStart method in MainWindow.xaml.cs to reflect the data your script needs.
/// - Modify your plugin script project as follows (see example project PluginScriptExample)
///      * Add a static method called Start in the VMS.TPS namespace. The btnStart method will send data to this method. This will be the entry point
///        when your script is run from PluginTester
///      * Modify your Execute method to call Start and send the same data as btnStart. This will be the entry point when your script
///        is run from Eclipse
///        
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PluginTester
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      try
      {
          Console.WriteLine("Logging in as " + args[0] + "...");
        using (VMS.TPS.Common.Model.API.Application app  = VMS.TPS.Common.Model.API.Application.CreateApplication(args[0],args[1]))
        {
          Execute(app);
        }
      }
      catch (Exception e)
      {
        Console.Error.WriteLine(e.ToString());
      }
    }
    static void Execute(VMS.TPS.Common.Model.API.Application app)
    {
        Window window = new Window();
        MainWindow mainWindow = new MainWindow(app);
        window.Title = "UM Plugin Tester";
        window.Content = mainWindow;
        window.Width = 1200;
        window.Height = 600;
        window.ShowDialog();
    }
  }
}
