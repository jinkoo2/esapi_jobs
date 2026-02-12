using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using VMSPlanSetup = VMS.TPS.Common.Model.API.PlanSetup;



namespace esapi.ViewModel
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CourseViewModel
    {
        private readonly VMSCourse _course;

        public CourseViewModel(VMSCourse course)
        {
            _course = course;
        }

        public VMSCourse VMSObject => _course;

        public override string ToString() => _course.Id;

        [Category("Identification")]
        public string Id => _course.Id;

        [Category("Identification")]
        public string Name => _course.Name;

        [Category("Clinical")]
        public string ClinicalStatus
        {
            get
            {
                switch (_course.ClinicalStatus)
                {
                    case CourseClinicalStatus.Active:
                        return "Active";
                    case CourseClinicalStatus.Completed:
                        return "Completed";
                    case CourseClinicalStatus.Restored:
                        return "Restored";
                    default:
                        return "Not Specified";
                }
            }
        }


        [Category("Clinical")]
        public string Intent => _course.Intent;

        [Category("Timeline")]
        public DateTime? StartDateTime => _course.StartDateTime;

        [Category("Timeline")]
        public DateTime? CompletedDateTime => _course.CompletedDateTime;

        [Category("Audit")]
        public string HistoryUserName => _course.HistoryUserName;

        [Category("Audit")]
        public string HistoryUserDisplayName => _course.HistoryUserDisplayName;

        [Category("Audit")]
        public DateTime HistoryDateTime => _course.HistoryDateTime;

        [Category("Metadata")]
        public string Comment => _course.Comment;

        [Category("Content")]
        public int ExternalPlanCount => _course.ExternalPlanSetups.Count();

        [Category("Content")]
        public int BrachyPlanCount => _course.BrachyPlanSetups.Count();

        [Category("Content")]
        public int IonPlanCount => _course.IonPlanSetups.Count();

        [Category("Content")]
        public int PlanSumCount => _course.PlanSums.Count();

        [Category("Content")]
        public int TreatmentPhaseCount => _course.TreatmentPhases.Count();

        [Category("Content")]
        public int TreatmentSessionCount => _course.TreatmentSessions.Count();
    }
}
