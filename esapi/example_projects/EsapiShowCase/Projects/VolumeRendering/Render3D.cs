using System;
using System.Runtime.InteropServices;
using MicrosoftResearch.MedicalImaging.Visualization;
using Numerics;
using System.IO;

namespace Sample3DViewer
{
  class Render3D
  {
    GPURenderFactory _renderFactory = new GPURenderFactory();
    RenderVolume _renderVolume = null;
    Viewport _viewport = null;
    Transfer1D tf = null;

    IntPtr _backbuffer = IntPtr.Zero;
    ImageBuffer _imageBuffer;

    MatrixTransformation _world = new MatrixTransformation();
    MatrixTransformation _projection = new MatrixTransformation();

    Camera _camera = new Camera(
        new VectorN(new float[] { 0f, 0f, 0f }),
        new VectorN(new float[] { 0f, 0f, 1000f }),
        0f, 90f);

    float _desiredImageWidth = 400, _desiredImageHeight = 300;

    float _voxelWidth, _voxelHeight, _voxelDepth;

    public Render3D(byte[] inputData, TransferFunction transferFunction, int xsize, int ysize, int zsize, float xres, float yres, float zres)
    {
      

      //create a RenderVolume object

      using (VolumeData vol = new VolumeData(xsize, ysize, zsize, VoxelFormat.Scalar16U))
      {
        vol.XScale = xres;
        vol.YScale = yres;
        vol.ZScale = zres;
        using (VolumeDataLock volLock = vol.LockPtr(VolumeDataAccess.ReadWrite))
        {
          IntPtr buff = volLock.Buffer;

          for (int z = 0; z < vol.Depth; z++)
          {
            for (int y = 0; y < vol.Height; y++)
            {
              Marshal.Copy(inputData, z * xsize * ysize * sizeof(UInt16) + y * xsize * sizeof(UInt16), buff, vol.Width * sizeof(UInt16));
              buff += vol.Width * sizeof(UInt16);
            }
          }
        }
        _renderVolume = _renderFactory.CreateRenderVolume(vol);

        _voxelWidth = vol.XScale;
        _voxelHeight = vol.YScale;
        _voxelDepth = vol.ZScale;

      }

      Console.WriteLine("Volume Dimensions: WHD = {0}x{1}x{2}", _renderVolume.Width, _renderVolume.Height, _renderVolume.Depth);
      Console.WriteLine("Volume Voxel Dimensions: WHD = {0}x{1}x{2}", _voxelWidth, _voxelHeight, _voxelDepth);
      Console.WriteLine("Volume Voxel Format: {0}", Enum.GetName(typeof(VoxelFormat), (object)_renderVolume.Format));

      Width = (int)_desiredImageWidth;
      Height = (int)_desiredImageHeight;
      Stride = Width * 4;//for pixel format PixelFormat.ARGB_8U

      _viewport = _renderFactory.CreateViewport();

      //set render volume to viewport
      _viewport.Volume = _renderVolume;

      this.TransferFunction = transferFunction;

      SetRenderOptions();

      SetInitialTransforms(_voxelWidth, _voxelHeight, _voxelDepth, _desiredImageWidth, _desiredImageHeight);

      SetViewportMatrices();

      //create image back buffer
      _backbuffer = Marshal.AllocCoTaskMem(Height * Stride);
      _imageBuffer = new ImageBuffer(Width, Height, Stride, PixelFormat.ARGB_8U, _backbuffer);
    }

    private void SetRenderOptions()
    {
      //set render options
      VolumeRenderOptions ro = new VolumeRenderOptions();
      ro.Integration = IntegrationMode.Blend;
      ro.Gradient = GradientMode.SixPointPostClassified;
      ro.VoxelStep = 1.0f;
      ro.Jitter = 1.0f;
      ro.OpacityModulation = 0.5f;
      ro.DiffuseShading = 1.5f;
      ro.Shininess = 5.0f;
      ro.SpecularShading = 0.5f;
      Direction dir = new Direction();
      dir.X = 1f; dir.Y = 0f; dir.Z = -1f;
      ro.LightDirection = dir;
      _viewport.Options = ro;
    }

    void SetViewportMatrices()
    {
      _viewport.WorldTransform = _world.ToArray();
      _viewport.ViewTransform = _camera.GetTransform();
      _viewport.ProjTransform = _projection.ToArray();
    }

    TransferFunction m_transferFunction;
    public TransferFunction TransferFunction 
    { 
      get { return m_transferFunction; }
      set
      {
        m_transferFunction = value;
        tf = _renderFactory.CreateTransfer1D();
        tf.PreIntegrated = true;
        tf.SetPoints(m_transferFunction.Positions, m_transferFunction.Colors, m_transferFunction.Positions.Length, InterpolationMode.Linear);
        tf.Width = 1024;
        _viewport.TransferFunction = tf;
      }
    }


    void SetInitialTransforms(float VoxelWidth, float VoxelHeight, float VoxelDepth, float W, float H)
    {
      _world.Scale(VoxelWidth, VoxelHeight, VoxelDepth);
      _world.Translate(-0.5f * _renderVolume.Width, -0.5f * _renderVolume.Height, -0.5f * _renderVolume.Depth);//set origin

      _camera.Reset();
      float fovRad = (float)(Math.PI * 45f / 180f); //FOV =  45 degrees

      float nearViewPlane = 1f;
      float farViewPlane = 10000f;
      float viewportWidth = W;
      float viewportHeight = H;

      _projection.PerspectiveFovRH(fovRad, nearViewPlane, farViewPlane, viewportWidth, viewportHeight);

      SetViewportMatrices();
    }

    public void CameraMove(float dx, float dy)
    {
      _camera.Rotate(0.02f * dx, 0.02f * dy);
      SetViewportMatrices();
      RenderVolume();
    }

    public void CameraZoom(float delta)
    {
      _camera.DollyZoom(delta);

      SetViewportMatrices();
      RenderVolume();
    }

    public void CameraReset()
    {
      //SetInitialTransforms(_voxelWidth, _voxelHeight, _voxelDepth, _desiredImageWidth, _desiredImageHeight);
      _camera.Reset();

      SetViewportMatrices();
    }

    public IntPtr ImageBufferPtr { get { return _backbuffer; } }
    public int ImageBufferSize { get { return Height * Stride; } }
    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public int Stride { get; protected set; }

    public void RenderVolume()
    {
      _viewport.Render3D(_imageBuffer);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    private bool _disposed = false;
    protected virtual void Dispose(bool disposing)
    {
      if (!this._disposed)
      {
        //dispose managed resources
        if (_backbuffer != IntPtr.Zero)
        {
          Marshal.FreeCoTaskMem(_backbuffer);
          _backbuffer = IntPtr.Zero;
        }

        if (tf != null)
        {
          tf.Dispose();
          tf = null;
        }
        if (_viewport != null)
        {
          _viewport.Dispose();
          _viewport = null;
        }
        if (_renderVolume != null)
        {
          _renderVolume.Dispose();
          _renderVolume = null;
        }
        if (_renderFactory != null)
        {
          _renderFactory.Dispose();
          _renderFactory = null;
        }
      }

      //dispose unmanaged resources

      _disposed = true;
    }
  }
}
