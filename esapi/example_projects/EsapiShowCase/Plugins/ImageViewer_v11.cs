using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;

namespace VMS.TPS
{
  //---------------------------------------------------------------------------------------------
  /// <summary>
  /// Script class
  /// </summary>
  //---------------------------------------------------------------------------------------------
  public class Script
  {
    //---------------------------------------------------------------------------------------------
    public Script()
    {
    }

    //---------------------------------------------------------------------------------------------
    public void Execute(ScriptContext context, System.Windows.Window window)
    {
      if (context.Image == null)
      {
        MessageBox.Show("No image in context!");
        return;
      }
      m_image = context.Image;

      // Root panel 
      DockPanel layoutRoot = new DockPanel();
      layoutRoot.Margin = new Thickness(4, 4, 4, 4);
      layoutRoot.MinWidth = m_image.XSize;
      layoutRoot.MinHeight = m_image.YSize;
      window.Title = "Image Viewer";
      window.Content = layoutRoot;
      window.SizeToContent = SizeToContent.WidthAndHeight;
      window.Background = Brushes.Azure;

      // Renderer for an Image
      m_imageRender = new ImageRenderer(context.Image);

      // Visualization elements for slice and structure
      InitVisualizationElements(layoutRoot);

      // Panel for controls in the top
      InitControlsPanel(context.StructureSet, layoutRoot);

      Render();
    }

    //---------------------------------------------------------------------------------------------
    private void InitVisualizationElements(DockPanel layoutRoot)
    {
      // Canvas for image and structure shape
      m_canvas = new Canvas();
      DockPanel.SetDock(m_canvas, Dock.Top);
      layoutRoot.Children.Add(m_canvas);

      // WPF Image control
      {
        System.Windows.Controls.Image imageControl = new System.Windows.Controls.Image();
        imageControl.Width = m_image.XSize;
        imageControl.Height = m_image.YSize;
        imageControl.Source = m_imageRender.Bitmap;
        m_canvas.Children.Add(imageControl);
      }

      // Structure shape
      {
        m_structureContour = new System.Windows.Shapes.Path();
        m_canvas.Children.Add(m_structureContour);
      }

      // Structure intersection points
      {
        m_structureIntersectionPoints = new System.Windows.Shapes.Path();
        m_structureIntersectionPoints.Stroke = Brushes.Red;
        m_canvas.Children.Add(m_structureIntersectionPoints);
      }
    }

    //---------------------------------------------------------------------------------------------
    private void InitControlsPanel(StructureSet ss, Panel layoutRoot)
    {
      // Stack panel for other controls
      WrapPanel controlsPanel = new WrapPanel();
      controlsPanel.MinHeight = 22.0;
      controlsPanel.Width = layoutRoot.Width;
      controlsPanel.VerticalAlignment = VerticalAlignment.Top;
      controlsPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
      controlsPanel.Background = Brushes.Azure;
      DockPanel.SetDock(controlsPanel, Dock.Top);
      layoutRoot.Children.Add(controlsPanel);

      // Slice position text
      {
        m_slicePosText = new TextBlock();
        m_slicePosText.Width = 120.0;

        GroupBox group = new GroupBox();
        group.Header = "Slice";
        group.Content = m_slicePosText;
        controlsPanel.Children.Add(group);
      }

      // Slice slider
      {
        Slider slider = new Slider();
        slider.Minimum = 0.0;
        slider.Maximum = m_image.ZSize;
        slider.SmallChange = 1.0;
        slider.LargeChange = 1.0;
        slider.Width = 120.0;
        slider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(OnSliceValueChanged);
        GroupBox group = new GroupBox();
        group.Header = "Position";
        group.Content = slider;
        controlsPanel.Children.Add(group);
      }

      // Structure selection combo
      {
        ComboBox structuresCombo = new ComboBox();
        structuresCombo.ItemsSource = ss != null ? ss.Structures : null;
        structuresCombo.SelectionChanged += new SelectionChangedEventHandler(OnStructuresComboSelectionChanged);
        structuresCombo.MinWidth = 120.0;
        GroupBox group = new GroupBox();
        group.Header = "Structure";
        group.Content = structuresCombo;
        controlsPanel.Children.Add(group);
      }

      // Checkbox to select structure visualization
      {
        CheckBox showStructureCheckBox = new CheckBox();
        showStructureCheckBox.Content = " ";
        showStructureCheckBox.IsChecked = true;
        showStructureCheckBox.VerticalAlignment = VerticalAlignment.Center;
        showStructureCheckBox.Margin = new Thickness(4, 0, 4, 0);
        showStructureCheckBox.Checked += new RoutedEventHandler(OnStructuresCheckBoxChecked);
        showStructureCheckBox.Unchecked += new RoutedEventHandler(OnStructuresCheckBoxChecked);
        GroupBox group = new GroupBox();
        group.Header = "Show";
        group.Content = showStructureCheckBox;
        controlsPanel.Children.Add(group);
      }

      // Magnifier combo
      {
        ComboBox magnifierCombo = new ComboBox();
        List<string> magnifyRatios = new List<string>();
        magnifyRatios.Add("1"); magnifyRatios.Add("1.5"); magnifyRatios.Add("2"); magnifyRatios.Add("3"); magnifyRatios.Add("4"); 
        magnifierCombo.ItemsSource = magnifyRatios;
        magnifierCombo.SelectionChanged += new SelectionChangedEventHandler(OnMagnifierComboSelectionChanged);
        GroupBox group = new GroupBox();
        group.Header = "Zoom";
        group.Content = magnifierCombo;
        controlsPanel.Children.Add(group);
      }

      // Show intersections
      {
        CheckBox showIntersectionsCheckBox = new CheckBox();
        showIntersectionsCheckBox.Content = " ";
        showIntersectionsCheckBox.IsChecked = false;
        showIntersectionsCheckBox.VerticalAlignment = VerticalAlignment.Center;
        showIntersectionsCheckBox.Margin = new Thickness(4, 0, 4, 0);
        showIntersectionsCheckBox.Checked += new RoutedEventHandler(OnIntersectionsCheckBoxChecked);
        showIntersectionsCheckBox.Unchecked += new RoutedEventHandler(OnIntersectionsCheckBoxChecked);
        GroupBox group = new GroupBox();
        group.Header = "Intersections";
        group.Content = showIntersectionsCheckBox;
        controlsPanel.Children.Add(group);
      }
    }
    
    double m_magnification = 1.0;
    //---------------------------------------------------------------------------------------------
    void OnMagnifierComboSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems != null && e.AddedItems.Count == 1)
      {
        string str = e.AddedItems[0] as string;
        double ratio = 1.0;
        Double.TryParse(str, out ratio);
        ScaleTransform tr = new ScaleTransform(ratio, ratio);
        m_canvas.RenderTransform = tr;
        m_magnification = ratio;
        Render();
      }
    }

    //---------------------------------------------------------------------------------------------
    void OnStructuresComboSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems != null && e.AddedItems.Count == 1)
      {
        m_structure = e.AddedItems[0] as Structure;
        if (m_structure != null)
        {
          m_structureContour.Stroke = new SolidColorBrush(m_structure.Color);
        }
        Render();
      }
    }
    //---------------------------------------------------------------------------------------------
    void OnIntersectionsCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      CheckBox showIntersectionsCheckBox = sender as CheckBox;
      m_showIntersections = showIntersectionsCheckBox.IsChecked.HasValue && showIntersectionsCheckBox.IsChecked.Value;
      Render();
    }

    //---------------------------------------------------------------------------------------------
    void OnStructuresCheckBoxChecked(object sender, RoutedEventArgs e)
    {
      CheckBox showStructuresCheckBox = sender as CheckBox;
      m_showStructure = showStructuresCheckBox.IsChecked.HasValue && showStructuresCheckBox.IsChecked.Value;
      Render();
    }

    //---------------------------------------------------------------------------------------------
    void OnSliceValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      int slice = (int)((sender as Slider).Value);
      if (slice != m_imageRender.CurSlice && slice >= 0 && slice < m_image.ZSize)
      {
        m_imageRender.CurSlice = slice;
        Render();
      }
    }

    //---------------------------------------------------------------------------------------------
    private void Render()
    {
      m_imageRender.DrawImage();
      DrawStructure();
      UpdateSlicePositionText();
    }

    //---------------------------------------------------------------------------------------------
    void UpdateSlicePositionText()
    {
      VVector pos = (m_image.Origin + m_image.ZDirection * m_image.ZRes * m_imageRender.CurSlice);

      string text;
      VVector xunit = new VVector(1.0, 0.0, 0.0);
      VVector yunit = new VVector(0.0, 1.0, 0.0);
      VVector zunit = new VVector(0.0, 0.0, 1.0);

      if (Math.Abs(m_image.ZDirection.ScalarProduct(xunit)) > 1.0 - 1e-9)
        text = "[X: " + (pos.x * 0.10).ToString("F2") + " cm]";
      else if (Math.Abs(m_image.ZDirection.ScalarProduct(yunit)) > 1.0 - 1e-9)
        text = "[Y: " + (m_image.ZDirection.y * 0.10).ToString("F2") + " cm]";
      else if (Math.Abs(m_image.ZDirection.ScalarProduct(zunit)) > 1.0 - 1e-9)
        text = "[Z: " + (pos.z * 0.10).ToString("F2") + " cm]";
      else
        text = "[" +
                (pos.x * 0.10).ToString("F2") + " ," +
                (pos.y * 0.10).ToString("F2") + " ," +
                (pos.z * 0.10).ToString("F2") +
                " cm]";

      m_slicePosText.Text = text;
    }

    //---------------------------------------------------------------------------------------------
    private void DrawStructure()
    {
      if (!m_showStructure || m_structure == null)
      {
        m_structureContour.Data = null;
        m_structureIntersectionPoints.Data = null;
        return;
      }

      m_structureContour.StrokeThickness = 1.0 / m_magnification;
      m_structureIntersectionPoints.StrokeThickness = 1.0 / m_magnification;

      if (m_structure.HasSegment)
      {
        DrawSegment();
        DrawStructureIntersections();
      }
      else
      {
        DrawPointStructure();
      }
    }

    BitArray m_bitBufferX = new BitArray(2048);
    BitArray m_bitBufferY = new BitArray(2048);

    //---------------------------------------------------------------------------------------------
    private void DrawStructureIntersections()
    {
      m_structureIntersectionPoints.Data = null;
      
      if (!m_showIntersections || m_structure == null || !m_structure.HasSegment)
        return;
      GeometryGroup geometryGroup = new GeometryGroup();

      {
        VVector xlineStart = m_imageRender.MapToDICOM(new Point(0.0, m_image.YSize * 0.5));
        VVector xlineEnd = m_imageRender.MapToDICOM(new Point(m_image.XSize, m_image.YSize * 0.5));
        SegmentProfile profile = m_structure.GetSegmentProfile(xlineStart, xlineEnd, m_bitBufferX);
        DrawStructureIntersectionsFromSegmentProfile(profile, geometryGroup);
      }

      {
        VVector ylineStart = m_imageRender.MapToDICOM(new Point(m_image.XSize * 0.5, 0.0));
        VVector ylineEnd = m_imageRender.MapToDICOM(new Point(m_image.XSize * 0.5, m_image.YSize));
        SegmentProfile profile = m_structure.GetSegmentProfile(ylineStart, ylineEnd, m_bitBufferY);
        DrawStructureIntersectionsFromSegmentProfile(profile, geometryGroup);
      }

      geometryGroup.Freeze();
      m_structureIntersectionPoints.Data = geometryGroup;
    }

    //---------------------------------------------------------------------------------------------
    private Geometry DrawCross(Point center, GeometryGroup geometryGroup)
    {
      double crossHalfWidth = 5.0 / m_magnification;

      LineGeometry line1 = new LineGeometry(new Point(center.X - crossHalfWidth, center.Y - crossHalfWidth),
        new Point(center.X + crossHalfWidth, center.Y + crossHalfWidth));
      LineGeometry line2 = new LineGeometry(new Point(center.X - crossHalfWidth, center.Y + crossHalfWidth),
        new Point(center.X + crossHalfWidth, center.Y - crossHalfWidth));
      line1.Freeze();
      line2.Freeze();

      geometryGroup.Children.Add(line1);
      geometryGroup.Children.Add(line2);
      return geometryGroup;
    }

    //---------------------------------------------------------------------------------------------
    private void DrawStructureIntersectionsFromSegmentProfile(SegmentProfile profile, GeometryGroup geometryGroup)
    {
      foreach (VVector intersection in profile.EdgeCoordinates)
      {
        Point pt = m_imageRender.MapToScreen(intersection);
        DrawCross(pt, geometryGroup);
        //string str = "Intersection : " + intersection.x.ToString("F2") + ", " +
        //                                 intersection.y.ToString("F2") + ", " +
        //                                 intersection.z.ToString("F2") + ", " + "\n" +
        //              "Maps to : " + pt.X.ToString("F0") + ", " +
        //                             pt.Y.ToString("F0");
        //MessageBox.Show(str);
      }
    }


    //---------------------------------------------------------------------------------------------
    private void DrawPointStructure()
    {
      VVector pos = m_structure.CenterPoint;
      m_structureContour.Data = null;
      if (!Double.IsNaN(pos.x) && !Double.IsNaN(pos.y) && !Double.IsNaN(pos.z))
      {
        VVector planeOrigin = m_image.Origin + m_image.ZDirection * m_image.ZRes * m_imageRender.CurSlice;
        double distanceOfPointToCurrentPlane = (pos - planeOrigin).ScalarProduct(m_image.ZDirection);
        if (Math.Abs(distanceOfPointToCurrentPlane) < 1e-6)
        {
          Point pt = m_imageRender.MapToScreen(pos);
          GeometryGroup geometryGroup = new GeometryGroup();
          DrawCross(pt, geometryGroup);
          geometryGroup.Freeze();
          m_structureContour.Data = geometryGroup;
        }
      }
    }

    //---------------------------------------------------------------------------------------------
    private void DrawSegment()
    {
      VVector[][] contours = m_structure.GetContoursOnImagePlane(m_imageRender.CurSlice);
      StreamGeometry pathGeometry = new StreamGeometry();
      using (StreamGeometryContext ctx = pathGeometry.Open())
      {
        for (int i = 0; i < contours.GetLength(0); i++)
        {
          VVector[] curve = contours[i];
          Point[] pointArray = new Point[curve.Length];
          int pointIndex = 0;
          foreach (VVector contourPoint in curve)
          {
            pointArray[pointIndex++] = m_imageRender.MapToScreen(contourPoint);
          }
          ctx.BeginFigure(pointArray[0], false, true);
          ctx.PolyLineTo(pointArray, true, false);
        }
      }
      pathGeometry.Freeze();
      m_structureContour.Data = pathGeometry;
    }

    // Members
    VMS.TPS.Common.Model.API.Image m_image;
    VMS.TPS.Common.Model.API.Structure m_structure;
    ImageRenderer m_imageRender;

    // Controls
    System.Windows.Controls.Canvas m_canvas;
    System.Windows.Shapes.Path m_structureContour;
    System.Windows.Shapes.Path m_structureIntersectionPoints;
    System.Windows.Controls.TextBlock m_slicePosText;

    // Flags
    bool m_showStructure = true;
    bool m_showIntersections = false;
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
    public Point MapToScreen(VVector pointInDICOM)
    {
      VVector planeOrigin = m_image.Origin + m_image.ZDirection * m_image.ZRes * m_curslice;
      VVector v = pointInDICOM - planeOrigin;

      // Project to screen X and Y axis
      double projectedOnX = v.ScalarProduct(m_image.XDirection);
      double projectedOnY = v.ScalarProduct(m_image.YDirection);

      // First pixel goes to the top left corner of screen. The coordinate
      // we have is the CENTER of the first voxel, therefor adjust 
      double adjustedOnX = projectedOnX + m_image.XRes * 0.5;
      double adjustedOnY = projectedOnY + m_image.YRes * 0.5;

      double scaledOnX = adjustedOnX / m_image.XRes;
      double scaledOnY = adjustedOnY / m_image.YRes;

      Point pt = new Point(scaledOnX, scaledOnY);
      return pt;
    }
    //---------------------------------------------------------------------------------------------
    public VVector MapToDICOM(Point pt)
    {
      VVector planeOrigin = m_image.Origin + m_image.ZDirection * m_image.ZRes * m_curslice;
      VVector xadd = m_image.XDirection * m_image.XRes * pt.X;
      VVector yadd = m_image.YDirection * m_image.YRes * pt.Y;
      VVector position = planeOrigin + xadd + yadd;
      return position;
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
