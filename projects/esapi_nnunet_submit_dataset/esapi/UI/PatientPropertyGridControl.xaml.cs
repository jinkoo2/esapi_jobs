using System.Windows.Controls;
using VMS.TPS.Common.Model.API;
using esapi.ViewModel;

namespace esapi.UI
{
    public partial class PatientPropertyGridControl : UserControl
    {
        public PatientPropertyGridControl()
        {
            InitializeComponent();
        }

        public void SetPatient(Patient patient)
        {
            //PropertyGrid1.SelectedObject = new PatientViewModel(patient);
        }
    }
}
