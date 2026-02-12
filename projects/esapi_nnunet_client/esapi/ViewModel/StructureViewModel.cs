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
    public class StructureViewModel
    {
        private readonly VMSStructure _structure;

        public StructureViewModel(VMSStructure structure)
        {
            _structure = structure;
        }

        public VMSStructure VMSObject => _structure;

        public override string ToString()
        {
            return _structure.Id;
        }

        [Category("Identification")]
        public string Id => _structure.Id;

        [Category("Identification")]
        public string Name => _structure.Name;

        [Category("Type")]
        public string DicomType => _structure.DicomType;

        [Category("Volume")]
        public double Volume => _structure.Volume;

        [Category("Flags")]
        public bool HasSegment => _structure.HasSegment;

        [Category("Flags")]
        public bool IsApproved => _structure.IsApproved;

        [Category("Flags")]
        public bool IsEmpty => _structure.IsEmpty;

        [Category("Flags")]
        public bool IsHighResolution => _structure.IsHighResolution;

        [Category("Audit")]
        public DateTime HistoryDateTime => _structure.HistoryDateTime;

        [Category("Audit")]
        public string HistoryUserName => _structure.HistoryUserName;

        [Category("Audit")]
        public string HistoryUserDisplayName => _structure.HistoryUserDisplayName;

        [Category("Metadata")]
        public string Comment => _structure.Comment;

        [Category("Geometry")]
        public string CenterPoint => $"{_structure.CenterPoint.x:F2}, {_structure.CenterPoint.y:F2}, {_structure.CenterPoint.z:F2}";



    }
}
