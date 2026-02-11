using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PlanarDoseIn3D
{
  public class SimpleSurface
  {
    public delegate Point3D Function(double x, double z);

    public double Xmin { get; set; }
    public double Xmax { get; set; }
    public double Ymin { get; set; }
    public double Ymax { get; set; }
    public double Zmin { get; set; }
    public double Zmax { get; set; }
    public int Nx { get; set; }
    public int Nz { get; set; }
    public Point3D Center { get; set; }
    public Viewport3D Viewport3d { get; set; }
    public bool ShowWireframe { get; set; }
    public bool ShowSurface { get; set; }

    private Color surfaceColor = Colors.White;

    public Color SurfaceColor
    {
      get { return surfaceColor; }
      set { surfaceColor = value; m_material = null; }
    }
    DiffuseMaterial m_material = null;
    private Material Material
    {
      get
      {
        if (m_material == null)
        {
          LinearGradientBrush brush = new LinearGradientBrush();
          brush.StartPoint = new Point(0.5, 0.0);
          brush.EndPoint = new Point(0.5, 1.0);
          brush.GradientStops.Add(new GradientStop(Colors.LightBlue, 0.0));
          brush.GradientStops.Add(new GradientStop(Colors.Blue, 0.2));
          brush.GradientStops.Add(new GradientStop(Colors.LightGreen, 0.4));
          brush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.6));
          brush.GradientStops.Add(new GradientStop(Colors.Red, 1.0));
          brush.Freeze();
          m_material = new DiffuseMaterial(brush);
        }
        return m_material;
      }

    }
    MeshGeometry3D m_mesh;

    public void CreateSurface(Function f)
    {
      if (m_mesh == null)
      {
        ShowSurface = true;
        ShowWireframe = true;
        m_mesh = new MeshGeometry3D();
        //m_wireframe = new ScreenSpaceLines3D();
        GeometryModel3D geometry = new GeometryModel3D(m_mesh, this.Material);
        ModelVisual3D model = new ModelVisual3D();
        model.Content = geometry;

        // Some performance gains (see Maximize WPF 3D Performance in MSDN)
        Viewport3d.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Unspecified);
        Viewport3d.Children.Add(model);

        //m_wireframe.Color = Colors.Black;
        //m_wireframe.Thickness = 1;
        //Viewport3d.Children.Add(m_wireframe);
      }
      m_mesh.Positions.Clear();
      //m_wireframe.Points.Clear();
      m_mesh.TriangleIndices.Clear();
      m_mesh.TextureCoordinates.Clear();

      double dx = (Xmax - Xmin) / Nx;
      double dz = (Zmax - Zmin) / Nz;
      if (Nx < 2 || Nz < 2)
        return;

      Point3D[,] pts = new Point3D[Nx, Nz];

      // Invoke given delegate on our x,z grid to get y-values (heights)
      for (int i = 0; i < Nz; i++)
      {
        double z = Zmin + i * dz;
        for (int j = 0; j < Nx; j++)
        {
          double x = Zmin + j * dx;
          Point3D fxz = f(x, z);
          pts[i, j] = fxz;
          if (!Double.IsNaN(fxz.Y))
          {
            pts[i, j] += (Vector3D)Center;
          }
        }
      }

      // Construct mesh points
      for (int z = 0; z < Nz; z++)
      {
        for (int x = 0; x < Nx; x++)
        {
          m_mesh.Positions.Add(pts[x, z]);
          m_mesh.TextureCoordinates.Add(new Point(0.5, pts[x, z].Y));
        }
      }

      // Construct triangle indexes and wireframe
      for (int z = 0; z < Nz - 1; z++)
      {
        for (int x = 0; x < Nx - 1; x++)
        {
          int[] indexes = new int[6];
          indexes[0] = z * Nz + x;
          indexes[1] = z * Nz + (x + 1);
          indexes[2] = (z + 1) * Nz + (x + 1);

          indexes[3] = indexes[2];
          indexes[4] = (z + 1) * Nz + x;
          indexes[5] = indexes[0];

          foreach (int index in indexes)
            m_mesh.TriangleIndices.Add(index);

          //if (ShowWireframe)
          //{
          //  if (z == 0)
          //  {
          //    m_wireframe.Points.Add(m_mesh.Positions[indexes[0]]);
          //    m_wireframe.Points.Add(m_mesh.Positions[indexes[1]]);
          //  }
          //  m_wireframe.Points.Add(m_mesh.Positions[indexes[1]]);
          //  m_wireframe.Points.Add(m_mesh.Positions[indexes[2]]);
          //  m_wireframe.Points.Add(m_mesh.Positions[indexes[2]]);
          //  m_wireframe.Points.Add(m_mesh.Positions[indexes[4]]);
          //  if (x == 0)
          //  {
          //    m_wireframe.Points.Add(m_mesh.Positions[indexes[4]]);
          //    m_wireframe.Points.Add(m_mesh.Positions[indexes[0]]);
          //  }
          //}
        }
      }
      if (!ShowSurface)
      {
        m_mesh.Positions.Clear();
        m_mesh.TriangleIndices.Clear();
      }
    }
  }
}
