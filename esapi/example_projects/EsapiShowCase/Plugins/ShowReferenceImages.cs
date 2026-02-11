using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;

namespace VMS.TPS
{
  public class Script
  {
    public Script()
    {
    }

    public void Execute(ScriptContext context, System.Windows.Window window)
    {
      if (context.PlanSetup == null || context.PlanSetup.Beams == null)
      {
        MessageBox.Show("Cannot find plan with beams");
        return;
      }

      int beamCount = context.PlanSetup.Beams.Count();

      // Root panel 
      ScrollViewer scrollViewer = new ScrollViewer();
      UniformGrid rootGrid = new UniformGrid();
      rootGrid.Columns = beamCount > 2 ? (int)Math.Sqrt(beamCount) : 2;
      if (beamCount > 4)
        rootGrid.LayoutTransform = new ScaleTransform(0.5, 0.5);

      scrollViewer.Content = rootGrid;
      scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
      scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

      window.Title = "Show Reference Images";
      window.Content = scrollViewer;
      window.Background = Brushes.Azure;
      window.SizeToContent = SizeToContent.Width;

      InitVisualizationElements(context.PlanSetup.Beams, rootGrid);
    }

    private void InitVisualizationElements(IEnumerable<Beam> beams, Panel rootGrid)
    {
      foreach (Beam b in beams)
      {
        StackPanel panel = new StackPanel();
        panel.Margin = new Thickness(2.0);
        panel.Orientation = Orientation.Vertical;
        panel.Children.Add(new TextBlock { Text = b.Id, FontFamily = new FontFamily("Calibri"), FontSize = 18.0, TextAlignment = TextAlignment.Center });

        if (b.ReferenceImage != null)
        {
          VMS.TPS.Common.Model.API.Image refImage = b.ReferenceImage;

          System.Windows.Controls.Image imageControl = new System.Windows.Controls.Image();
          imageControl.Width = refImage.XSize;
          imageControl.Height = refImage.YSize;
          ImageRenderer renderer = new ImageRenderer(refImage);
          imageControl.Source = renderer.Bitmap;
          renderer.DrawImage();
          panel.Children.Add(imageControl);
        }
        rootGrid.Children.Add(panel);
      }
    }
  }
  //---------------------------------------------------------------------------------------------
  /// <summary>
  /// Class that renders slices of a volume image to a bitmap
  /// </summary>
  //---------------------------------------------------------------------------------------------
  public class ImageRenderer
  {
    //---------------------------------------------------------------------------------------------
    public ImageRenderer(VMS.TPS.Common.Model.API.Image image)
    {
      m_image = image;
      m_window = m_image.Window;
      m_level = m_image.Level;

      m_bitmap = new WriteableBitmap(m_image.XSize, m_image.YSize, 96.0, 96.0, System.Windows.Media.PixelFormats.Pbgra32, null);
      m_pixbuff = new byte[m_image.XSize * m_image.YSize * 4];
      m_sliceVoxels = new int[m_image.XSize, m_image.YSize];
      m_curslice = m_image.ZSize / 2;
      InitColormap(m_window, m_level);
    }

    //---------------------------------------------------------------------------------------------
    private void InitColormap(int window, int level)
    {
      int low = level - window / 2;
      int high = level + window / 2;
      double slope = 255.0 / ((double)window);

      for (int i = 0; i < m_colormap.Length; i++)
      {
        if (i < low)
          m_colormap[i] = Colors.Black;
        else if (i > high)
          m_colormap[i] = Colors.White;
        else
        {
          double value = ((i - low) * slope);
          if (value > 255)
            value = 255;
          byte greylevel = (byte)(value > 255.0 ? 255 : value);
          m_colormap[i] = Color.FromRgb(greylevel, greylevel, greylevel);
        }
      }
    }

    //---------------------------------------------------------------------------------------------
    public int CurSlice { get { return m_curslice; } set { m_curslice = value; } }

    //---------------------------------------------------------------------------------------------
    public void DrawImage()
    {
      m_image.GetVoxels(m_curslice, m_sliceVoxels);
      int stride = m_image.XSize * 4;

      for (int y = 0; y < m_image.YSize; y++)
      {
        int yoffset = y * stride;

        for (int x = 0; x < m_image.XSize; x++)
        {
          int voxel = m_sliceVoxels[x, y];

          int ptr = yoffset + x * 4;
          Color col = m_colormap[voxel];
          m_pixbuff[ptr++] = col.R;
          m_pixbuff[ptr++] = col.G;
          m_pixbuff[ptr++] = col.B;
          m_pixbuff[ptr++] = 255;
        }
      }
      m_bitmap.WritePixels(new Int32Rect(0, 0, m_image.XSize, m_image.YSize), m_pixbuff, stride, 0, 0);
    }

    //---------------------------------------------------------------------------------------------
    public WriteableBitmap Bitmap { get { return m_bitmap; } }
    public int BitmapSizeX { get { return m_image.XSize; } }
    public int BitmapSizeY { get { return m_image.YSize; } }

    WriteableBitmap m_bitmap;
    byte[] m_pixbuff;
    int[,] m_sliceVoxels;
    Color[] m_colormap = new Color[0x10000];

    VMS.TPS.Common.Model.API.Image m_image;
    int m_window, m_level;
    int m_curslice;
  }
}
