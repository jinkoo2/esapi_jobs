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
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StructureSetViewModel
    {
        private readonly VMSStructureSet _structureSet;

        public StructureSetViewModel(VMSStructureSet structureSet)
        {
            _structureSet = structureSet;
        }

        public VMSStructureSet VMSObject => _structureSet;

        public override string ToString()
        {
            return _structureSet.Id;
        }

        [Category("Identification")]
        public string Id => _structureSet.Id;

        [Category("Identification")]
        public string Name => _structureSet.Name;

        [Category("Identification")]
        public string UID => _structureSet.UID;

        [Category("Audit")]
        public DateTime HistoryDateTime => _structureSet.HistoryDateTime;

        [Category("Audit")]
        public string HistoryUserName => _structureSet.HistoryUserName;

        [Category("Audit")]
        public string HistoryUserDisplayName => _structureSet.HistoryUserDisplayName;

        [Category("Metadata")]
        public string Comment => _structureSet.Comment;

        [Category("Image")]
        public ImageViewModel Image => new ImageViewModel(_structureSet.Image);

        [Category("Structures")]
        public List<StructureViewModel> Structures =>_structureSet.Structures.Select(s => new StructureViewModel(s)).ToList();

    }
}
