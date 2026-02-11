using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanarDoseIn3D
{
  //---------------------------------------------------------------------------------------------
  /// <summary>
  /// Class that renders slices of a volume image to a bitmap
  /// </summary>
  //---------------------------------------------------------------------------------------------
  public class ImageRenderer
  {
    //---------------------------------------------------------------------------------------------
    public ImageRenderer(Image image)
    {
      m_image = image;
      m_window = m_image.Window;
      m_level = m_image.Level;
      m_imageXSize = m_image.XSize;
      m_imageYSize = m_image.YSize;
      m_imageZSize = m_image.ZSize;
      m_imageOrigin = m_image.Origin;
      m_imageXRes = m_image.XRes;
      m_imageYRes = m_image.YRes;
      m_imageZRes = m_image.ZRes;
      m_imageXDir = m_image.XDirection;
      m_imageYDir = m_image.YDirection;
      m_imageZDir = m_image.ZDirection;

      m_bitmap = new WriteableBitmap(m_image.XSize, m_image.YSize, 96.0, 96.0, System.Windows.Media.PixelFormats.Pbgra32, null);
      m_pixbuff = new byte[m_image.XSize * m_image.YSize * 4];
      m_sliceVoxels = new int[m_image.XSize, m_image.YSize];
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
          double distFromLow = i - low;
          double value = (distFromLow * slope);
          if (value > 255)
            value = 255;
          byte shade = (byte)(value > 255.0 ? 255 : value);
          m_colormap[i] = Color.FromRgb(shade, shade, shade);
        }
      }
    }


    //---------------------------------------------------------------------------------------------
    public int CurSlice { get { return m_curslice; } set { m_curslice = value; } }

    //---------------------------------------------------------------------------------------------
    public void DrawImage()
    {
      m_image.GetVoxels(m_curslice, m_sliceVoxels);
      int stride = m_imageXSize * 4;

      for (int y = 0; y < m_imageYSize; y++)
      {
        int yoffset = y * stride;

        for (int x = 0; x < m_imageXSize; x++)
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
      m_bitmap.WritePixels(new Int32Rect(0, 0, m_imageXSize, m_imageYSize), m_pixbuff, stride, 0, 0);
    }
    //---------------------------------------------------------------------------------------------
    public Point MapToScreen(VVector pointInDICOM)
    {
      VVector planeOrigin = m_imageOrigin + m_imageZDir * m_imageZRes * m_curslice;
      VVector v = pointInDICOM - planeOrigin;

      // Project to screen X and Y axis
      double projectedOnX = v.ScalarProduct(m_imageXDir);
      double projectedOnY = v.ScalarProduct(m_imageYDir);

      // First pixel goes to the top left corner of screen. The coordinate
      // we have is the CENTER of the first voxel, therefor adjust by 
      double adjustedOnX = projectedOnX + m_imageXRes * 0.5;
      double adjustedOnY = projectedOnY + m_imageYRes * 0.5;

      double scaledOnX = adjustedOnX / m_imageXRes;
      double scaledOnY = adjustedOnY / m_imageYRes;

      Point pt = new Point(scaledOnX, scaledOnY);
      return pt;
    }
    //---------------------------------------------------------------------------------------------
    public VVector MapToDICOM(Point pt)
    {
      VVector planeOrigin = m_imageOrigin + m_imageZDir * m_imageZRes * m_curslice;
      VVector xadd = m_imageXDir * m_imageXRes * pt.X;
      VVector yadd = m_imageYDir * m_imageYRes * pt.Y;
      VVector position = planeOrigin + xadd + yadd;
      return position;
    }

    //---------------------------------------------------------------------------------------------
    public WriteableBitmap Bitmap { get { return m_bitmap; } }
    public int BitmapSizeX { get { return m_imageXSize; } }
    public int BitmapSizeY { get { return m_imageYSize; } }

    WriteableBitmap m_bitmap;
    byte[] m_pixbuff;
    int[,] m_sliceVoxels;
    Color[] m_colormap = new Color[0x10000];

    Image m_image;
    int m_window, m_level;
    int m_imageXSize, m_imageYSize, m_imageZSize;
    double m_imageXRes, m_imageYRes, m_imageZRes;
    VVector m_imageOrigin, m_imageXDir, m_imageYDir, m_imageZDir;
    int m_curslice;
  }
}
