using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    public class PatientViewModel
    {
        private readonly VMSPatient _patient;

        public PatientViewModel(VMSPatient patient)
        {
            _patient = patient;
        }

        public VMSPatient VMSObject => _patient;

        public override string ToString() => $"{_patient.LastName}, {_patient.FirstName}, {_patient.Id}";

        [Category("Identification")]
        public string Id => _patient.Id;

        [Category("Identification")]
        public string Id2 => _patient.Id2;

        [Category("Identification")]
        public string Name => _patient.Name;

        [Category("Personal Info")]
        public string FirstName => _patient.FirstName;

        [Category("Personal Info")]
        public string MiddleName => _patient.MiddleName;

        [Category("Personal Info")]
        public string LastName => _patient.LastName;

        [Category("Personal Info")]
        public string Sex => _patient.Sex;

        [Category("Personal Info")]
        public DateTime? DateOfBirth => _patient.DateOfBirth;

        [Category("Medical Info")]
        public string SSN => _patient.SSN;

        [Category("Medical Info")]
        public HospitalViewModel Hospital => new HospitalViewModel(_patient.Hospital);

        [Category("Medical Info")]
        public string PrimaryOncologistId => _patient.PrimaryOncologistId;

        [Category("Metadata")]
        public string Comment => _patient.Comment;

        [Category("Audit")]
        public bool HasModifiedData => _patient.HasModifiedData;

        [Category("Audit")]
        public DateTime HistoryDateTime => _patient.HistoryDateTime;

        [Category("Audit")]
        public string HistoryUserName => _patient.HistoryUserName;

        [Category("Audit")]
        public string HistoryUserDisplayName => _patient.HistoryUserDisplayName;

        [Category("Metadata")]
        public DateTime? CreationDateTime => _patient.CreationDateTime;

        [Category("Collections")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public List<CourseViewModel> Courses => _patient.Courses.Select(c => new CourseViewModel(c)).ToList();

        [Category("Collections")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public List<StructureSetViewModel> StructureSets => _patient.StructureSets.Select(ss => new StructureSetViewModel(ss)).ToList();

        [Category("Collections")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public List<StudyViewModel> Studies => _patient.Studies.Select(st => new StudyViewModel(st)).ToList();

        [Category("Collections")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public List<RegistrationViewModel> Registrations => _patient.Registrations.Select(reg => new RegistrationViewModel(reg)).ToList();

        [Category("Collections")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public List<ReferencePointViewModel> ReferencePoints => _patient.ReferencePoints.Select(rp => new ReferencePointViewModel(rp)).ToList();
    }
}
