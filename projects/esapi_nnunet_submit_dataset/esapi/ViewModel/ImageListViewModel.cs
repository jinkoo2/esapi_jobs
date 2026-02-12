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
using System.Collections.ObjectModel;


namespace esapi.ViewModel
{
    public class ImageListViewModel : BaseViewModel
    {
        private ObservableCollection<ViewModel.ImageViewModel> _imageList;
        public ObservableCollection<ViewModel.ImageViewModel> ImageList
        {
            get=>_imageList;
            set
            {
                if (_imageList == value) return;


                SetProperty(ref _imageList, value, nameof(ImageList));

                SelectedImage = null;
            }
        }

        private ViewModel.ImageViewModel _selectedImage;
        public ViewModel.ImageViewModel SelectedImage
        {
            get => _selectedImage;
            set => SetProperty(ref _selectedImage, value, nameof(SelectedImage));  
        }
    }
}
