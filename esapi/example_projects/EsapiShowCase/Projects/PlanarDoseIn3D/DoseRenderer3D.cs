using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using VMS.TPS.Common.Model.Types;
using VMS.TPS.Common.Model.API;
using System.Windows;

namespace PlanarDoseIn3D
{
  //---------------------------------------------------------------------------------------------
  /// <summary>
  /// Class that renders image slice and corresponding planar dose in 3D
  /// </summary>
  //---------------------------------------------------------------------------------------------
  class DoseRenderer3D
  {
    public DoseRenderer3D(System.Windows.Controls.Viewport3D viewport, DiffuseMaterial planeMaterial3D, MeshGeometry3D planeMesh, Image image, Dose dose)
    {
      m_dose = dose;
      m_image = image;
      m_maxDose = dose.DoseMax3D.Dose;

      m_viewport3D = viewport;
      m_planeMaterial3D = planeMaterial3D;
      m_planeMesh = planeMesh;
      ValScaler = 1.0;
      MinDose = -0.1;
      Slice3DLocation = -0.5;

      m_surface = new SimpleSurface();
      m_surface.Xmin = 0;
      m_surface.Xmax = 1;
      m_surface.Zmin = 0;
      m_surface.Zmax = 1;
      m_surface.Ymin = 0;
      m_surface.Ymax = 1;
      m_surface.Nx = 30;
      m_surface.Nz = 30;
      m_surface.Viewport3d = m_viewport3D;
      m_surface.SurfaceColor = System.Windows.Media.Colors.Red;
      m_surface.Center = new Point3D(-0.5, 0.0, -0.5);

      m_renderer = new ImageRenderer(image);
      m_pixmapSizeX = m_renderer.BitmapSizeX;
      m_pixmapSizeY = m_renderer.BitmapSizeY;
      Update(false);
    }
    Image m_image;
    Dose m_dose;
    ImageRenderer m_renderer;

    //---------------------------------------------------------------------------------------------
    void Update(bool onlySlice3DLocationChanged)
    {
      // Reposition slice in 3D view
      Point3DCollection positions = m_planeMesh.Positions;
      if (Math.Abs(positions[0].Y - Slice3DLocation) > 0.0001)
      {
        Point3DCollection newPos = new Point3DCollection(positions.Count);

        foreach (var v in m_planeMesh.Positions)
        {
          Point3D vi = new Point3D(v.X, Slice3DLocation, v.Z);
          newPos.Add(vi);
        }
        m_planeMesh.Positions = newPos;
      }

      if (!onlySlice3DLocationChanged)
      {
        int res = m_hires ? 60 : 30;
        m_surface.Nx = res;
        m_surface.Nz = res;

        m_renderer.CurSlice = this.CurSlice;
        m_renderer.DrawImage();
        System.Windows.Media.ImageBrush imageBrush = new System.Windows.Media.ImageBrush(m_renderer.Bitmap);
        m_planeMaterial3D.Brush = imageBrush;

        if (m_dose != null)
        {
          m_surface.CreateSurface(DoseSurfaceValue);
        }
      }
    }

    //---------------------------------------------------------------------------------------------
    Point3D DoseSurfaceValue(double x, double z)
    {
      double pixelX = x / m_surface.Xmax * m_pixmapSizeX;
      double pixelY = z / m_surface.Zmax * m_pixmapSizeY;
      VVector realpos = m_renderer.MapToDICOM(new Point(pixelX, m_pixmapSizeY - pixelY));
      double doseVal = m_dose.GetDoseToPoint(realpos).Dose;
      if (Double.IsNaN(doseVal) || (doseVal / m_maxDose) < MinDose)
        doseVal = 0.0;
      if (doseVal > m_maxLocalDose)
        m_maxLocalDose = doseVal;
      return new Point3D(x, ValScaler * (doseVal / m_maxDose), z);
    }

    //---------------------------------------------------------------------------------------------
    double MinDose { get; set; }
    double ValScaler { get; set; }

    public double Slice3DLocation { get { return m_slice3DLocation; } set { if (m_slice3DLocation != value) { m_slice3DLocation = value; Update(true); } } }

    public int CurSlice { get { return m_curslice; } set { if (m_curslice != value) { m_curslice = value; Update(false); } } }

    double m_maxLocalDose = 0.0;
    bool m_hires = true;
    DiffuseMaterial m_planeMaterial3D;
    SimpleSurface m_surface;
    double m_maxDose = 1.0;
    MeshGeometry3D m_planeMesh;
    int m_pixmapSizeX, m_pixmapSizeY;
    double m_slice3DLocation = 0.0;
    int m_curslice;
    System.Windows.Controls.Viewport3D m_viewport3D;
  }
}
