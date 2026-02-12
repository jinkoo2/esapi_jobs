using System;
using System.ComponentModel;
using VMS.TPS.Common.Model.API;

using VMSPatient = VMS.TPS.Common.Model.API.Patient;
using VMSStructureSet = VMS.TPS.Common.Model.API.StructureSet;
using VMSStructure = VMS.TPS.Common.Model.API.Structure;
using VMSImage = VMS.TPS.Common.Model.API.Image;
using VMSCourse = VMS.TPS.Common.Model.API.Course;
using VMSStudy = VMS.TPS.Common.Model.API.Study;
using VMSSeries = VMS.TPS.Common.Model.API.Series;
using VMSRegistration = VMS.TPS.Common.Model.API.Registration;
using VMSReferencePoint = VMS.TPS.Common.Model.API.ReferencePoint;
using VMSHospital = VMS.TPS.Common.Model.API.Hospital;


namespace esapi.ViewModel
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HospitalViewModel
    {
        private readonly VMSHospital _hospital;

        public HospitalViewModel(VMSHospital hospital)
        {
            _hospital = hospital;
        }

        public VMSHospital VMSObject => _hospital;

        public override string ToString() => _hospital?.Name ?? "(No Hospital)";

        [Category("Identification")]
        public string Id => _hospital.Id;

        [Category("Identification")]
        public string Name => _hospital.Name;

        [Category("Metadata")]
        public string Comment => _hospital.Comment;

        [Category("Location")]
        public string Location => _hospital.Location;

        [Category("Audit")]
        public DateTime? CreationDateTime => _hospital.CreationDateTime;

        [Category("Audit")]
        public DateTime HistoryDateTime => _hospital.HistoryDateTime;

        [Category("Audit")]
        public string HistoryUserName => _hospital.HistoryUserName;

        [Category("Audit")]
        public string HistoryUserDisplayName => _hospital.HistoryUserDisplayName;
    }
}
