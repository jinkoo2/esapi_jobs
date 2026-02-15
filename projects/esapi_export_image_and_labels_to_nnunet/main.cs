using System;
using System.Reflection;
using System.Windows;

using VMS.TPS.Common.Model.API;
using VMSApplication = VMS.TPS.Common.Model.API.Application;

[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("1.0.0.1")]
[assembly: ESAPIScript(IsWriteable = false)]

namespace nnunet_client
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                using (VMSApplication app = VMSApplication.CreateApplication())
                {
                    Execute(app);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("--- ESAPI Main Exception ---");
                Console.Error.WriteLine(e.ToString());
            }
        }

        static void Execute(VMSApplication vmsApp)
        {
            try
            {
                global.load_config();
                global.vmsApplication = vmsApp;

                var wpfApp = new App();

                wpfApp.DispatcherUnhandledException += (sender, e) =>
                {
                    e.Handled = true;
                    Console.Error.WriteLine("--- WPF Dispatcher Exception!!! ---");
                    Console.Error.WriteLine(e.Exception.ToString());
                    MessageBox.Show($"An unexpected error occurred: {e.Exception.Message}",
                                    "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                };

                var mainWindow = new SubmitImageAndLabelsWindow(vmsApp);
                wpfApp.Run(mainWindow);

                Console.WriteLine("done");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("--- ESAPI Execute Exception ---");
                Console.Error.WriteLine(e.ToString());
                throw;
            }
        }
    }
}
