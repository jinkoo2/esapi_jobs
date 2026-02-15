using System;
using System.ComponentModel;
using System.Text;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

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
    public class RegistrationViewModel
    {
        private readonly VMSRegistration _registration;

        public RegistrationViewModel(VMSRegistration registration)
        {
            _registration = registration;
        }
        public VMSRegistration VMSObject => _registration;


        public override string ToString() => _registration.Id;

        [Category("Identification")]
        public string Id => _registration.Id;

        [Category("Identification")]
        public string Name => _registration.Name;

        [Category("Identification")]
        public string UID => _registration.UID;

        [Category("Coordinate Systems")]
        public string SourceFOR => _registration.SourceFOR;

        [Category("Coordinate Systems")]
        public string RegisteredFOR => _registration.RegisteredFOR;

        [Category("Approval")]
        public string Status
        {
            get
            {
                switch (_registration.Status)
                {
                    case RegistrationApprovalStatus.Approved: return "Approved";
                    case RegistrationApprovalStatus.Unapproved: return "Unapproved";
                    case RegistrationApprovalStatus.Retired: return "Retired";
                    case RegistrationApprovalStatus.Reviewed: return "Reviewed";
                    default: return "Unknown";
                }
            }
        }

        [Category("Approval")]
        public DateTime? StatusDateTime => _registration.StatusDateTime;

        [Category("Approval")]
        public string StatusUserName => _registration.StatusUserName;

        [Category("Approval")]
        public string StatusUserDisplayName => _registration.StatusUserDisplayName;

        [Category("Audit")]
        public string HistoryUserName => _registration.HistoryUserName;

        [Category("Audit")]
        public string HistoryUserDisplayName => _registration.HistoryUserDisplayName;

        [Category("Audit")]
        public DateTime HistoryDateTime => _registration.HistoryDateTime;

        [Category("Creation")]
        public DateTime? CreationDateTime => _registration.CreationDateTime;

        [Category("Metadata")]
        public string Comment => _registration.Comment;

        [Category("Transformation")]
        public string TransformationMatrix => FormatMatrix(_registration.TransformationMatrix);

        private string FormatMatrix(double[,] matrix)
        {
            var sb = new StringBuilder();
            for (int r = 0; r < 4; r++)
            {
                sb.Append("[ ");
                for (int c = 0; c < 4; c++)
                {
                    sb.AppendFormat("{0,8:F2}", matrix[r, c]);
                    if (c < 3) sb.Append(", ");
                }
                sb.AppendLine(" ]");
            }
            return sb.ToString();
        }
    }
}
