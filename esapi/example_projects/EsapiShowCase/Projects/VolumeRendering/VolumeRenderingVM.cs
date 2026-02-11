using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample3DViewer;
using System.Threading;
using System.Windows.Media.Imaging;

namespace VolumeRendering
{
  public class VolumeRenderinglVM : IDisposable
  {
    //---------------------------------------------------------------------------------------------
    public float CameraZoomDelta { get { return m_cameraZoomDelta; } set { m_cameraZoomDelta = value; } }
    public float CameraMoveDeltaX { get { return m_cameraMoveDeltaX; } set { m_cameraMoveDeltaX = value; } }
    public float CameraMoveDeltaY { get { return m_cameraMoveDeltaY; } set { m_cameraMoveDeltaY = value; } }
    public TransferFunction TransferFunction { get { return m_transferFunction; } set { m_transferFunction = value; m_transferFunctionChanged = true; } }
    public WriteableBitmap Bitmap { get { return m_bmp; } }

    //---------------------------------------------------------------------------------------------
    public VolumeRenderinglVM(VMS.TPS.Common.Model.API.Image image, VMS.TPS.Common.Model.API.Dose dose, TransferFunction transferFunction)
    {
      m_image = image;
      m_dose = dose;
      TransferFunction = transferFunction;
      CopyImageData();
    }

    Thread m_renderingThread;
    //---------------------------------------------------------------------------------------------
    public void Render()
    {
      if (m_renderingThread == null)
      {
        m_renderingThread = new Thread(RenderingThreadStartup);
        m_renderingThread.SetApartmentState(ApartmentState.MTA);
        m_renderingThread.Start();
        m_renderEndEvent.WaitOne();

        BitmapSource bitmap = BitmapSource.Create(m_render3d.Width, m_render3d.Height, 96, 96,
                System.Windows.Media.PixelFormats.Bgra32, null, m_render3d.ImageBufferPtr, m_render3d.ImageBufferSize, m_render3d.Stride);

        m_bmp = new WriteableBitmap(bitmap);
      }
      else
      {
        m_renderStartEvent.Set();
        m_renderEndEvent.WaitOne();
      }
      m_bmp.WritePixels(new System.Windows.Int32Rect(0, 0, m_render3d.Width, m_render3d.Height), m_render3d.ImageBufferPtr, m_render3d.ImageBufferSize, m_render3d.Stride);
    }

    //---------------------------------------------------------------------------------------------
    void CopyImageData()
    {
      m_renderDataMaxVal = 0;
      if (m_image != null)
      {
        m_xsize = (m_image.XSize / 2) * 2;   // must be an even value
        m_ysize = m_image.YSize;
        m_zsize = m_image.ZSize;
        m_xres = (float)m_image.XRes;
        m_yres = (float)m_image.YRes;
        m_zres = (float)m_image.ZRes;

        m_rendererData = new byte[m_xsize * m_ysize * m_zsize * sizeof(UInt16)];
        int[,] tmpBuff = new int[m_image.XSize, m_ysize]; // must be allocated based on real width

        int destIndex = 0;
        for (int z = 0; z < m_zsize; z++)
        {
          m_image.GetVoxels(z, tmpBuff);
          for (int y = 0; y < m_ysize; y++)
          {
            for (int x = 0; x < m_xsize; x++)
            {
              int val = tmpBuff[x, y];
              if (val > m_renderDataMaxVal)
                m_renderDataMaxVal = val;

              m_rendererData[destIndex] = (byte)(val & 0xff);
              m_rendererData[destIndex + 1] = (byte)((val >> 8) & 0xff);
              destIndex += 2;
            }
          }
        }
        //MessageBox.Show("Max :" + m_renderDataMaxVal.ToString());
      }
    }

    bool m_exiting = false;
    //---------------------------------------------------------------------------------------------
    public void Exit()
    {
      m_exiting = true;
      m_renderStartEvent.Set();
      m_renderEndEvent.WaitOne();
    }

    bool m_cameraReset = false;
    //---------------------------------------------------------------------------------------------
    public void CameraReset()
    {
      m_cameraReset = true;
    }

    //---------------------------------------------------------------------------------------------
    void RenderingThreadStartup(object o)
    {
      try
      {
        // Initialize
        m_render3d = new Render3D(m_rendererData, TransferFunction,
                                  m_xsize, m_ysize, m_zsize, m_xres, m_yres, m_zres);
        m_render3d.RenderVolume();
        m_renderEndEvent.Set();

        // Rendering loop
        while (!m_exiting)
        {
          m_renderStartEvent.WaitOne();
          if (!float.IsNaN(m_cameraZoomDelta))
          {
            m_render3d.CameraZoom(m_cameraZoomDelta);
            m_cameraZoomDelta = float.NaN;
          }
          if (!float.IsNaN(m_cameraMoveDeltaX) || !float.IsNaN(m_cameraMoveDeltaY))
          {
            m_render3d.CameraMove(m_cameraMoveDeltaX, m_cameraMoveDeltaY);
            m_cameraMoveDeltaX = float.NaN;
            m_cameraMoveDeltaY = float.NaN;
          }
          if (m_transferFunctionChanged)
          {
            m_render3d.TransferFunction = TransferFunction;
            m_transferFunctionChanged = false;
          }
          if (m_cameraReset)
          {
            m_render3d.CameraReset();
            m_cameraReset = false;
          }
          m_render3d.RenderVolume();
          m_renderEndEvent.Set();
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show("Rendering failed\n " +
                        "* check with 'Registercom.exe' from Microsoft Volume Rendering SDK that CUDA 3.0 or higher is installed\n" +
                        "* run 'regsvr32' for CacheManagerIP_x64.dll and GPURenderFactoryIP_x64.dll" +
                        "Details :" + ex.Message);
      }
      finally
      {
        m_renderEndEvent.Set();
      }
    }

    //---------------------------------------------------------------------------------------------
    public void Dispose()
    {
      if (m_render3d != null)
        m_render3d.Dispose();
    }

    //---------------------------------------------------------------------------------------------
    WriteableBitmap m_bmp = null;
    Render3D m_render3d = null;
    bool m_transferFunctionChanged = true;
    TransferFunction m_transferFunction;

    float m_cameraZoomDelta = float.NaN;
    float m_cameraMoveDeltaX = float.NaN;
    float m_cameraMoveDeltaY = float.NaN;

    AutoResetEvent m_renderStartEvent = new AutoResetEvent(false);
    AutoResetEvent m_renderEndEvent = new AutoResetEvent(false);

    byte[] m_rendererData;
    int m_renderDataMaxVal;
    int m_xsize, m_ysize, m_zsize;
    float m_xres, m_yres, m_zres;
    VMS.TPS.Common.Model.API.Image m_image;
    VMS.TPS.Common.Model.API.Dose m_dose;
  }
}
