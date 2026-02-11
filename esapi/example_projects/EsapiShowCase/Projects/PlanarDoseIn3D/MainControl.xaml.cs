using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanarDoseIn3D
{
  /// <summary>
  /// Interaction logic for MainControl.xaml
  /// </summary>
  public partial class MainControl : System.Windows.Controls.UserControl
  {
    public MainControl()
    {
      InitializeComponent();

      // set up the trackball
      var trackball = new Wpf3DTools.Trackball();
      trackball.EventSource = m_background;
      m_viewport.Camera.Transform = trackball.Transform;

    }
    public void SetDoseAndImage(Dose dose, VMS.TPS.Common.Model.API.Image image)
    {
      m_image = image;
      m_sliceSlider.Minimum = 0.0;
      m_sliceSlider.Maximum = image.ZSize;
      m_sliceSlider.SmallChange = 1.0;
      m_sliceSlider.LargeChange = 1.0;
      m_sliceSlider.Width = 120.0;

      m_dose3D = new DoseRenderer3D(m_viewport, m_planeMaterial, m_planeMesh,image, dose);
    }
    private void OnSliceHeight3DSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (m_dose3D != null)
      {
        m_dose3D.Slice3DLocation = ((System.Windows.Controls.Slider)sender).Value;
      }
    }
    private void OnSliceChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      int slice = (int)m_sliceSlider.Value;
      if (m_dose3D != null && slice >= 0 && slice < m_image.ZSize)
      {
        m_dose3D.CurSlice = slice;
      }
    }

    DoseRenderer3D m_dose3D;
    VMS.TPS.Common.Model.API.Image m_image;
  }
}

