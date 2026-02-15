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
    public class ReferencePointViewModel
    {
        private readonly VMSReferencePoint _refPoint;

        public ReferencePointViewModel(VMSReferencePoint refPoint)
        {
            _refPoint = refPoint;
        }

        public VMSReferencePoint VMSObject => _refPoint;


        public override string ToString() => _refPoint.Id;

        [Category("Identification")]
        public string Id => _refPoint.Id;

        [Category("Identification")]
        public string Name => _refPoint.Name;

        [Category("Dose Limits")]
        public string DailyDoseLimit => _refPoint.DailyDoseLimit.ToString();

        [Category("Dose Limits")]
        public string SessionDoseLimit => _refPoint.SessionDoseLimit.ToString();

        [Category("Dose Limits")]
        public string TotalDoseLimit => _refPoint.TotalDoseLimit.ToString();

        [Category("Metadata")]
        public string Comment => _refPoint.Comment;

        [Category("Audit")]
        public string HistoryUserName => _refPoint.HistoryUserName;

        [Category("Audit")]
        public string HistoryUserDisplayName => _refPoint.HistoryUserDisplayName;

        [Category("Audit")]
        public DateTime HistoryDateTime => _refPoint.HistoryDateTime;
    }
}
