using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Sample3DViewer;
using System.Windows.Threading;
using System.IO;

namespace VolumeRendering
{
  //---------------------------------------------------------------------------------------------
  /// <summary>
  /// Interaction logic for MainControl.xaml
  /// </summary>
  //---------------------------------------------------------------------------------------------
  public partial class MainControl : UserControl
  {
    //---------------------------------------------------------------------------------------------
    public MainControl(VMS.TPS.Common.Model.API.Image image, VMS.TPS.Common.Model.API.Dose dose)
    {
      m_image = image;
      m_dose = dose;
      InitializeComponent();

      m_dataGrid.Visibility = System.Windows.Visibility.Collapsed;

      m_timer.Interval = TimeSpan.FromMilliseconds(20);
      m_timer.Tick += new EventHandler(OnDispatcherTimerTick);
      m_timer.Start();
    }

    //---------------------------------------------------------------------------------------------
    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      m_isMouseDown = true;
      m_oldPoint = e.GetPosition(m_imageControl);
      Mouse.Capture(m_imageControl);
    }

    //---------------------------------------------------------------------------------------------
    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      m_isMouseDown = false;
      Mouse.Capture(null);
    }

    //---------------------------------------------------------------------------------------------
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed && m_isMouseDown)
      {
        if (m_vievModel != null)
        {
          Point p = e.GetPosition(m_imageControl);
          m_vievModel.CameraMoveDeltaX = (float)(p.X - m_oldPoint.X);
          m_vievModel.CameraMoveDeltaY = (float)(p.Y - m_oldPoint.Y);

          m_vievModel.Render();
          m_oldPoint = p;
        }
      }
    }

    //---------------------------------------------------------------------------------------------
    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (m_vievModel != null)
      {
        float delta = (float)e.Delta / 25f;

        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
          delta = e.Delta;

        m_vievModel.CameraZoomDelta = delta;
        m_vievModel.Render();
      }
    }

    //---------------------------------------------------------------------------------------------
    private void OnButton3DClick(object sender, RoutedEventArgs e)
    {
      if (m_vievModel == null)
      {
        m_vievModel = new VolumeRenderinglVM(m_image, m_dose, new TransferFunction(GetNextTransferFuncName()));
        m_vievModel.Render();
      }

      if (m_vievModel == null)
      {
        return;   // Failed but message shown already
      }

      m_dataGrid.ItemsSource = m_vievModel.TransferFunction.Points;
      m_dataGrid.Visibility = System.Windows.Visibility.Visible;

      m_imageControl.Source = m_vievModel.Bitmap;

      //m_messageTB.Text = "Drag with mouse to position camera. Mouse Scroll - Zoom In/Out. Shift + Mouse Scroll - Larger Zoom steps";
    }

    //---------------------------------------------------------------------------------------------
    public void Exit(object sender, EventArgs e)
    {
      m_autoRotating = false;
      m_timer.Stop();

      if (m_vievModel != null)
      {
        m_vievModel.Exit();
        m_vievModel.Dispose();
      }
    }

    //---------------------------------------------------------------------------------------------
    private void GeMyLocationOnDisk()
    {
      if (m_myLocationOnDisk == null)
      {
        m_myLocationOnDisk = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
      }
    }
    //---------------------------------------------------------------------------------------------
    private string GetNextTransferFuncName()
    {
      m_curFunc++;

      const int numberOfTransferFuncs = 3;
      if (m_curFunc >= numberOfTransferFuncs)
        m_curFunc = 0;
      GeMyLocationOnDisk();
      return m_myLocationOnDisk + "\\" + "tf000" + m_curFunc.ToString() + ".xml";
    }

    //---------------------------------------------------------------------------------------------
    private void OnTransferFuncClick(object sender, RoutedEventArgs e)
    {
      if (m_vievModel != null)
      {
        m_vievModel.TransferFunction = new TransferFunction(GetNextTransferFuncName());
        m_vievModel.Render();
        m_dataGrid.ItemsSource = m_vievModel.TransferFunction.Points;
      }
    }

    //---------------------------------------------------------------------------------------------
    private void OnApplyButtonClick(object sender, RoutedEventArgs e)
    {
      TransferFunction trasferFunction = m_vievModel.TransferFunction;
      trasferFunction.UpdateFromPoints();
      m_vievModel.TransferFunction = trasferFunction;
      m_vievModel.Render();
    }

    //---------------------------------------------------------------------------------------------
    private void OnEditColorClick(object sender, RoutedEventArgs e)
    {
      int index = m_dataGrid.SelectedIndex;
      if (index >= 0 && index < m_vievModel.TransferFunction.Points.Count)
      {
        Color current = m_vievModel.TransferFunction.Points[index].m_color;
        byte currentA = current.A;
        current.A = 255;
        ColourPickerDialog dialog = new ColourPickerDialog(current);
        bool? dialogResult = dialog.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
          Color newColor = dialog.SelectedColour;
          TransferFunction trasferFunction = m_vievModel.TransferFunction;
          trasferFunction.Points[index].Red = newColor.R;
          trasferFunction.Points[index].Green = newColor.G;
          trasferFunction.Points[index].Blue = newColor.B;
          trasferFunction.Points[index].Alpha = currentA;
          trasferFunction.UpdateFromPoints();
          m_vievModel.TransferFunction = trasferFunction;
          m_vievModel.Render();
        }
      }
    }

    //---------------------------------------------------------------------------------------------
    private void OnAutoRotateClick(object sender, RoutedEventArgs e)
    {
      m_autoRotating = m_autoRotating ? false : true;
    }

    //---------------------------------------------------------------------------------------------
    void OnDispatcherTimerTick(object sender, EventArgs e)
    {
      if (m_autoRotating)
      {
        m_vievModel.CameraMoveDeltaX = 2.0f;
        m_vievModel.CameraMoveDeltaY = 0.0f;
        m_vievModel.Render();
      }
    }

    //---------------------------------------------------------------------------------------------
    private void OnResetViewButtonClick(object sender, RoutedEventArgs e)
    {
      if (m_vievModel != null)
      {
        m_vievModel.CameraReset();
        m_vievModel.Render();
      }
    }

    //---------------------------------------------------------------------------------------------
    private void OnSaveButtonClick(object sender, RoutedEventArgs e)
    {
      FrameworkElement element = m_rootGrid;
      Size size = new Size(element.ActualWidth + element.Margin.Left + element.Margin.Right, element.ActualHeight + element.Margin.Top + element.Margin.Bottom);

      RenderTargetBitmap bmp = new RenderTargetBitmap((int)(size.Width), (int)size.Height, 96.0, 96.0, PixelFormats.Default);
      bmp.Render(element);
      Clipboard.SetImage(bmp);

      const string basePath = @"c:\temp\VolumeRenderingScreenshots\";
      DateTime now = DateTime.Now;
      string filename = basePath +
                        now.Year.ToString("D4") + '-' +
                        now.Month.ToString("D2") + '-' +
                        now.Day.ToString("D2") +
                        'T' +
                        now.Hour.ToString("D2") + '-' +
                        now.Minute.ToString("D2") + '-' +
                        now.Second.ToString("D2") + ".png";

      if (!Directory.Exists(basePath))
        Directory.CreateDirectory(basePath);

      using (Stream file = new FileStream(filename, FileMode.CreateNew))
      {
        PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
        pngEncoder.Frames.Add(BitmapFrame.Create(bmp));
        pngEncoder.Save(file);
      }
      System.Diagnostics.Process.Start(filename);
    }

    //---------------------------------------------------------------------------------------------
    private void OnHelpButtonClick(object sender, RoutedEventArgs e)
    {
      GeMyLocationOnDisk();
      System.Diagnostics.Process.Start(m_myLocationOnDisk + "\\Introduction-to-CT-Physics.pdf");
    }

    //---------------------------------------------------------------------------------------------
    Point m_oldPoint = new Point();
    bool m_isMouseDown = false;

    string m_myLocationOnDisk;
    int m_curFunc = -1;

    DispatcherTimer m_timer = new DispatcherTimer();
    bool m_autoRotating = false;

    VMS.TPS.Common.Model.API.Image m_image;
    VMS.TPS.Common.Model.API.Dose m_dose;

    VolumeRenderinglVM m_vievModel;
  }
}
