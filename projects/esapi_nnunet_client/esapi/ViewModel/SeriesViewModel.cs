using System;
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


namespace esapi.ViewModel
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SeriesViewModel
    {
        private readonly VMSSeries _series;

        public SeriesViewModel(VMSSeries series)
        {
            _series = series;
        }

        public VMSSeries VMSObject => _series;


        public override string ToString() => _series.Id;

        [Category("Identification")]
        public string Id => _series.Id;

        [Category("Identification")]
        public string Name => _series.Name;

        [Category("Identification")]
        public string UID => _series.UID;

        [Category("Device")]
        public string ImagingDeviceId => _series.ImagingDeviceId;

        [Category("Device")]
        public string ImagingDeviceModel => _series.ImagingDeviceModel;

        [Category("Device")]
        public string ImagingDeviceManufacturer => _series.ImagingDeviceManufacturer;

        [Category("Device")]
        public string ImagingDeviceSerialNo => _series.ImagingDeviceSerialNo;

        [Category("Acquisition")]
        public string Modality
        {
            get
            {
                switch (_series.Modality)
                {
                    case SeriesModality.CT: return "CT";
                    case SeriesModality.MR: return "MR";
                    case SeriesModality.PT: return "PET";
                    case SeriesModality.RTIMAGE: return "RTIMAGE";
                    case SeriesModality.RTSTRUCT: return "RTSTRUCT";
                    case SeriesModality.RTPLAN: return "RTPLAN";
                    case SeriesModality.RTDOSE: return "RTDOSE";
                    case SeriesModality.REG: return "REG";
                    case SeriesModality.Other: return "Other";
                    default: return "Unknown";
                }
            }
        }


        [Category("Frame of Reference")]
        public string FOR => _series.FOR;

        [Category("Content")]
        public int ImageCount => _series.Images.Count();

        [Category("Audit")]
        public string HistoryUserName => _series.HistoryUserName;

        [Category("Audit")]
        public string HistoryUserDisplayName => _series.HistoryUserDisplayName;

        [Category("Audit")]
        public DateTime HistoryDateTime => _series.HistoryDateTime;

        [Category("Metadata")]
        public string Comment => _series.Comment;
    }
}
