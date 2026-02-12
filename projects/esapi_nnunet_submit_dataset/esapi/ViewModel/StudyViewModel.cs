using System;
using System.ComponentModel;
using System.Linq;
using VMS.TPS.Common.Model.API;
using System.Collections.Generic;

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
    public class StudyViewModel
    {
        private readonly VMSStudy _study;

        public StudyViewModel(VMSStudy study)
        {
            _study = study;
        }

        public VMSStudy VMSObject => _study;

        public override string ToString() => _study.Id;

        [Category("Identification")]
        public string Id => _study.Id;

        [Category("Identification")]
        public string Name => _study.Name;

        [Category("Identification")]
        public string UID => _study.UID;

        [Category("Creation")]
        public DateTime? CreationDateTime => _study.CreationDateTime;

        [Category("Audit")]
        public string HistoryUserName => _study.HistoryUserName;

        [Category("Audit")]
        public string HistoryUserDisplayName => _study.HistoryUserDisplayName;

        [Category("Audit")]
        public DateTime HistoryDateTime => _study.HistoryDateTime;

        [Category("Metadata")]
        public string Comment => _study.Comment;

        [Category("Content")]
        public List<SeriesViewModel> Series => _study.Series.Select(s=>new SeriesViewModel(s)).ToList();

        [Category("Content")]
        public List<ImageViewModel> Images3D => _study.Images3D.Select(img=>new ImageViewModel(img)).ToList();
    }
}
