using System;
using System.ComponentModel;
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
    public class ImageViewModel
    {
        private readonly VMSImage _image;

        public ImageViewModel(VMSImage image)
        {
            _image = image;
        }

        public VMSImage VMSObject => _image;

        public override string ToString()
        {
            return _image.Id;
        }

        [Category("Identification")]
        public string Id => _image.Id;

        [Category("Identification")]
        public string Name => _image.Name;

        [Category("Identification")]
        public string UID => _image.UID;

        [Category("Acquisition")]
        public DateTime? CreationDateTime => _image.CreationDateTime;

        [Category("Acquisition")]
        public string ImageType => _image.ImageType;

        [Category("Acquisition")]
        public SeriesViewModel Series => new SeriesViewModel(_image.Series);

        [Category("Acquisition")]
        public string ImagingOrientation
        {
            get
            {
                switch (_image.ImagingOrientation)
                {
                    case PatientOrientation.HeadFirstSupine:
                        return "Head First (Supine)";
                    case PatientOrientation.HeadFirstProne:
                        return "Head First (Prone)";
                    case PatientOrientation.HeadFirstDecubitusRight:
                        return "Head First (Right Decubitus)";
                    case PatientOrientation.HeadFirstDecubitusLeft:
                        return "Head First (Left Decubitus)";
                    case PatientOrientation.FeetFirstSupine:
                        return "Feet First (Supine)";
                    case PatientOrientation.FeetFirstProne:
                        return "Feet First (Prone)";
                    case PatientOrientation.FeetFirstDecubitusRight:
                        return "Feet First (Right Decubitus)";
                    case PatientOrientation.FeetFirstDecubitusLeft:
                        return "Feet First (Left Decubitus)";
                    case PatientOrientation.Sitting:
                        return "Sitting";
                    default:
                        return "No Orientation";
                }
            }
        }

        [Category("Acquisition")]
        public string ContrastBolusAgent => _image.ContrastBolusAgentIngredientName;

        [Category("Display")]
        public double Window => _image.Window;

        [Category("Display")]
        public double Level => _image.Level;

        [Category("Display")]
        public string DisplayUnit => _image.DisplayUnit;

        [Category("Geometry")]
        public string Origin => FormatVector(_image.Origin);

        [Category("Geometry")]
        public string XDirection => FormatVector(_image.XDirection);

        [Category("Geometry")]
        public string YDirection => FormatVector(_image.YDirection);

        [Category("Geometry")]
        public string ZDirection => FormatVector(_image.ZDirection);

        [Category("Geometry")]
        public double XRes => _image.XRes;

        [Category("Geometry")]
        public int XSize => _image.XSize;

        [Category("Geometry")]
        public double YRes => _image.YRes;

        [Category("Geometry")]
        public int YSize => _image.YSize;

        [Category("Geometry")]
        public double ZRes => _image.ZRes;

        [Category("Geometry")]
        public int ZSize => _image.ZSize;

        [Category("Geometry")]
        public string SizeString => $"{XSize} x {YSize} x {ZSize}";

        [Category("Geometry")]
        public string ResolutionString => $"{XRes:F1} x {YRes:F1} x {ZRes:F1}";


        [Category("Status")]
        public bool IsProcessed => _image.IsProcessed;

        [Category("Status")]
        public bool HasUserOrigin => _image.HasUserOrigin;

        [Category("Status")]
        public string UserOrigin => FormatVector(_image.UserOrigin);

        [Category("Status")]
        public string UserOriginComments => _image.UserOriginComments;

        [Category("Audit")]
        public string HistoryUserName => _image.HistoryUserName;

        [Category("Audit")]
        public string HistoryUserDisplayName => _image.HistoryUserDisplayName;

        [Category("Audit")]
        public DateTime HistoryDateTime => _image.HistoryDateTime;

        [Category("Metadata")]
        public string Comment => _image.Comment;

        [Category("Other")]
        public string FOR => _image.FOR;

        private string FormatVector(VVector v) =>
            double.IsNaN(v.x) ? "(NaN)" : $"{v.x:F2}, {v.y:F2}, {v.z:F2}";
    }
}
