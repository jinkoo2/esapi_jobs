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
using VMS.TPS.Common.Model.API;
using System.Windows.Controls.DataVisualization.Charting;
using VMS.TPS.Common.Model.Types;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.Net.Mail;
using System.Windows.Controls.Primitives;

namespace WpfPlanComparer
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      this.Closing += new System.ComponentModel.CancelEventHandler(OnMainWindowClosing);
      m_app = VMS.TPS.Common.Model.API.Application.CreateApplication("superuser", "superuser");
    }

    Patient m_patient;
    void OnOpenPatientClicked(object sender, RoutedEventArgs e)
    {
      string patientId = m_patientIdTextBox.Text.Trim();
      m_patient = m_app.OpenPatientById(patientId);
      if (m_patient == null)
      {
        MessageBox.Show("Cannot find patient with id " + patientId);
        return;
      }
      m_contentGrid.Children.Clear();
      m_openPatientButton.Visibility = System.Windows.Visibility.Collapsed;
      var element = CreateComparison(m_patient);
      if (element != null)
      {
        m_contentGrid.Children.Add(element);
      }

    }

    private FrameworkElement CreateComparison(Patient patient)
    {
      // Get all plans with calculated dose grouped by the structureset they use
      var plansGroupedByStructureSet =
                  from c in patient.Courses
                  from ps in c.PlanSetups
                  where ps.Dose != null
                  group ps by ps.StructureSet into g
                  select new
                  {
                    ss = g.Key,
                    plans = g
                  };

      if (!plansGroupedByStructureSet.Any())
      {
        MessageBox.Show(patient.Id + " has no plans with calculated dose");
        return null;
      }
      StructureSet ss = plansGroupedByStructureSet.First().ss;
      var plans = plansGroupedByStructureSet.First().plans;

      // Get structures that have objectives
      var structures = (from structure in ss.Structures
                        where structure.DicomType != "EXTERNAL" &&
                        structure.Volume > 10.0 &&
                        plans.First().OptimizationSetup.Objectives.Where(o => o.Structure == structure).Any()
                        select structure).Take(3);

      if (!structures.Any())
      {
        MessageBox.Show("No structures with optimization objectives found");
        return null;
      }

      Grid grid = new Grid();
      grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30.0) });
      grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
      grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30.0) });
      TextBox headerTextBox = new TextBox { HorizontalAlignment = System.Windows.HorizontalAlignment.Center, FontFamily = new FontFamily("Calibri"), FontSize = 20.0, BorderBrush = null };
      TextBox footerTextBox = new TextBox { HorizontalAlignment = System.Windows.HorizontalAlignment.Right, FontFamily = new FontFamily("Calibri"), FontSize = 8.0,  BorderBrush = null, VerticalContentAlignment = System.Windows.VerticalAlignment.Center };
      UniformGrid chartArea = new UniformGrid();
      chartArea.Columns = 1;
      Grid.SetRow(headerTextBox, 0);
      Grid.SetRow(chartArea, 1);
      Grid.SetRow(footerTextBox, 2);
      grid.Children.Add(headerTextBox);
      grid.Children.Add(chartArea);
      grid.Children.Add(footerTextBox);

      headerTextBox.Text = "Patient : " + patient.Id + ", Structure Set : " + ss.Id;

      int x = 0;
      foreach (Structure structure in structures)
      {
        if (x++ > 10)
          break;

        Chart chart = new Chart();
        chart.Axes.Add(new LinearAxis { Title = "Gy", Orientation = AxisOrientation.X, Minimum = 0.0 });
        chart.Axes.Add(new LinearAxis { Title = "% Volume", Orientation = AxisOrientation.Y, Minimum = 0.0 });
        chart.Title = structure.Id;

        chartArea.Children.Add(chart);
        int n = 0;
        Color[] colors = { Colors.Red, Colors.Blue, Colors.Green, Colors.Beige, Colors.Cornsilk, Colors.Chartreuse, Colors.Black };
        foreach (PlanSetup plan in plans)
        {
          DVHData dvh = plan.GetDVHCumulativeData(structure, DoseValuePresentation.Absolute, VolumePresentation.Relative, 0.1);
          if (dvh != null)
          {
            LineSeries series = new LineSeries();
            series.IndependentValuePath = "DoseValue.Dose";
            series.DependentValuePath = "Volume";
            series.ItemsSource = dvh.CurveData;
            series.DataPointStyle = GetNewDataPointStyle(colors[n]);

            series.Title = plan.Id;
            chart.Series.Add(series);
            n++;
          }
        }
      }
      footerTextBox.Text = "Generated at " + DateTime.Now.ToString() + " by " + Environment.UserName;
      return grid;
    }

    private void OnSaveXpsClicked(object sender, RoutedEventArgs e)
    {
      string file = SaveAsXps();
      System.Diagnostics.Process.Start(file);
    }

    private string SaveAsXps()
    {
      if (m_patient == null)
        return null;
      var element = CreateComparison(m_patient);
      if (element == null)
        return null;

      if (!Directory.Exists(@"c:\temp"))
        Directory.CreateDirectory(@"c:\temp");

      string filename = @"c:\temp\PlanComparison.xps";
      try
      {

        FixedDocument fixedDoc = new FixedDocument();
        FixedPage fixedPage = new FixedPage();
        fixedPage.Margin = new Thickness(10.0);

        double margin = 30.0;
        var pageSize = fixedDoc.DocumentPaginator.PageSize;
        element.Width = pageSize.Width - margin * 2;
        element.Height = pageSize.Height - margin *2;
        element.SetValue(FixedPage.LeftProperty, margin);
        element.SetValue(FixedPage.TopProperty, margin);
        
        PageContent pageContent = new PageContent();
        fixedPage.Children.Add(element);
        ((System.Windows.Markup.IAddChild)pageContent).AddChild(fixedPage);
        fixedDoc.Pages.Add(pageContent);

        TmpHostWindow wnd = new TmpHostWindow();
        wnd.SetContent(fixedDoc, element);
        wnd.Show();
        wnd.Close();

        // Create an xps document and write my fixed document to it
        var package = Package.Open(filename, FileMode.Create);
        XpsDocument xpsDoc = new XpsDocument(package);

        XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
        xpsWriter.Write(fixedDoc);
        package.Flush();
        package.Close();

        return filename;
      }
      catch (IOException exception)
      {
        MessageBox.Show("Failed with I/O exception: " + exception.ToString());
        return null;
      }
    }

    private Style GetNewDataPointStyle(Color col)
    {
      Color background = col;
      Style style = new Style(typeof(DataPoint));
      Setter st1 = new Setter(DataPoint.BackgroundProperty,
                                  new SolidColorBrush(background));
      Setter st2 = new Setter(DataPoint.BorderBrushProperty,
                                  new SolidColorBrush(Colors.White));
      Setter st3 = new Setter(DataPoint.BorderThicknessProperty, new Thickness(0.1));

      Setter st4 = new Setter(DataPoint.TemplateProperty, null);
      style.Setters.Add(st1);
      style.Setters.Add(st2);
      style.Setters.Add(st3);
      style.Setters.Add(st4);
      return style;
    }
    private Style GetNewPolylineStyle(double dash)
    {
      var style = new Style(typeof(Polyline));
      style.Setters.Add(new Setter(Shape.StrokeDashArrayProperty,
                        new DoubleCollection(new[] { dash })));
      return style;
    }

    void OnMainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (m_app != null)
      {
        m_app.ClosePatient();
        m_app.Dispose();
        m_app = null;
      }
    }
    VMS.TPS.Common.Model.API.Application m_app;
  }
  class TmpHostWindow : Window
  {
    public TmpHostWindow()
    {
      this.WindowStyle = System.Windows.WindowStyle.ToolWindow | System.Windows.WindowStyle.None;
      this.Loaded += new RoutedEventHandler(TmpHostWindow_Loaded);
    }

    void TmpHostWindow_Loaded(object sender, RoutedEventArgs e)
    {
      this.Hide();
    }
    public void SetContent(object content, FrameworkElement el)
    {
      this.Content = content;
      el.Loaded += new RoutedEventHandler(el_Loaded);
   }

    void el_Loaded(object sender, RoutedEventArgs e)
    {
      
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);
      //this.Hide();
    }
    protected override void OnContentRendered(EventArgs e)
    {
      base.OnContentRendered(e);
      //this.Hide();
    }
  }
}
