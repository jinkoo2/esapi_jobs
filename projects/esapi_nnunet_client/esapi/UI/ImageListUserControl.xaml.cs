using esapi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VMS.TPS.Common.Model.API;

using VMSImage = VMS.TPS.Common.Model.API.Image;

namespace esapi.UI
{
    public partial class ImageListUserControl : UserControl
    {
        public event EventHandler<VMSImage> SelectedItemChanged;

        private List<ImageViewModel> _images;

        public ImageListUserControl()
        {
            InitializeComponent();
        }

        public void SetImages(List<VMSImage> images)
        {
            _images = images
                .Select(img=> new ImageViewModel(img))
                .OrderByDescending(img => img.CreationDateTime)
                .ToList();

            var displayItems = _images.Select(imgVM => new
            {
                Image = imgVM,
                imgVM.Id,
                imgVM.CreationDateTime,
                imgVM.ImageType,
                imgVM.Series.Modality,
                ImagingOrientation = imgVM.ImagingOrientation.ToString(),
                Size = $"[{imgVM.XSize},{imgVM.YSize},{imgVM.ZSize}]",
                Resolution = $"[{imgVM.XRes:F2},{imgVM.YRes:F2},{imgVM.ZRes:F2}]",
                imgVM.Comment
            }).ToList();

            ImageDataGrid.ItemsSource = displayItems;
        }

        private void ImageDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ImageDataGrid.SelectedItem is null) return;

            var selectedVM = ImageDataGrid.SelectedItem.GetType()
                .GetProperty("Image")?.GetValue(ImageDataGrid.SelectedItem) as ImageViewModel;

            if (selectedVM != null)
            {
                SelectedItemChanged?.Invoke(this, selectedVM.VMSObject);
            }
        }
    }
}
