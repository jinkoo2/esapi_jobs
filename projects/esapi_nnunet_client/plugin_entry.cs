using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;

namespace VMS.TPS
{
    public class Script
    {
        public Script() { }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(ScriptContext context)
        {
            nnunet_client.global.isPluginMode = true;
            nnunet_client.global.scriptContext = context;
            nnunet_client.global.vmsPatient = context.Patient;
            nnunet_client.global.load_config();

            var window = new nnunet_client.MainWindow(null);
            window.ShowDialog();
        }
    }
}
