using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Windows.Controls;
using System.ComponentModel;

namespace VMS.TPS
{
  public class Script
  {
    public Script()
    {
    }

    //-----------------------------------------------------------------------------------------------
    public void Execute(ScriptContext context, System.Windows.Window window)
    {
      var structureset = context.StructureSet;
      if (structureset == null)
        return;

      StackPanel rootPanel = new StackPanel();

      m_grid = new DataGrid();
      m_grid.ItemsSource = m_structureInfos;
      m_grid.AutoGenerateColumns = true;

      m_grid.FontFamily = new System.Windows.Media.FontFamily("Calibri"); 
      m_grid.FontSize = 16.0;
      m_grid.ColumnHeaderHeight = 30.0;
      rootPanel.Children.Add(m_grid);

      Button btn = new Button();
      btn.Content = "Calculate...";
      btn.Click += new RoutedEventHandler(OnCalculateButtonClick);
      btn.Margin = new Thickness(5.0);
      rootPanel.Children.Add(btn);

      window.Content = rootPanel;
      window.Width = 640;
      window.Height = 480;

      foreach (Structure structure in structureset.Structures)
      {
        m_structureInfos.Add(new StructureInfo(structure, structureset.Image));
      }
    }

    //-----------------------------------------------------------------------------------------------
    void OnCalculateButtonClick(object sender, RoutedEventArgs e)
    {
      StructureInfo selectedRow = m_grid.SelectedItem as StructureInfo;
      if (selectedRow != null)
      {
        selectedRow.Calculate();
      }
    }
    List<StructureInfo> m_structureInfos = new List<StructureInfo>();
    DataGrid m_grid;
  }

  //-----------------------------------------------------------------------------------------------
  /// <summary>
  /// Class holding information of one structure
  /// </summary>
  //-----------------------------------------------------------------------------------------------
  class StructureInfo : INotifyPropertyChanged
  {
    Structure m_structure;
    VMS.TPS.Common.Model.API.Image m_image;

    //-----------------------------------------------------------------------------------------------
    public StructureInfo(Structure structure, VMS.TPS.Common.Model.API.Image image)
    {
      m_structure = structure;
      m_image = image;
    }

    public string Id { get { return m_structure.Id; } }
    public string Volume { get { return m_volume.ToString("F2") + " cm3"; } }
    public string AverageHU { get { return m_isCalculated ? m_averageHU.ToString("F2") : "-"; } }
    public string MinHU { get { return m_isCalculated ? m_minHU.ToString("F0") : "-"; } }
    public string MaxHU { get { return m_isCalculated ? m_maxHU.ToString("F0") : "-"; } }
    public int VoxelCount { get { return m_voxelCount; } }
    //public string VolumeByVoxels { get { return m_volumeByVoxels.ToString("F2") + " cm3"; } }
    //public string VolumeDiff { get { return m_volumeDiff.ToString("F2") + " %"; } }
    public void Calculate()
    {
      if (!m_isCalculated) 
      { 
        CalculateStatistics(m_image, m_structure); 
        RaisePropertyChanged(); 
      } 
    }

    bool m_isCalculated = false;
    double m_averageHU, m_minHU, m_maxHU;
    double m_volume;
    // double m_volumeByVoxels;
    // double m_volumeDiff;
    int m_voxelCount;

    //-----------------------------------------------------------------------------------------------
    private void CalculateStatistics(VMS.TPS.Common.Model.API.Image image, Structure structure)
    {
      double sum = 0.0;
      m_voxelCount = 0;
      if (image == null || structure == null)
        return;

      if (image.Series.Modality != SeriesModality.CT || !m_structure.HasSegment)
        return;
      m_volume = m_structure.Volume;
      if (m_volume < Double.Epsilon)
        return;

      m_minHU = Double.MaxValue;
      m_maxHU = Double.MinValue;
      m_averageHU = Double.NaN;
      m_isCalculated = true;

      int xcount = image.XSize;
      System.Collections.BitArray segmentArray = new System.Collections.BitArray(xcount);
      int[,] voxelPlane = new int[image.XSize, image.YSize];
      bool voxelPlaneExists = false;

      double intercept = image.VoxelToDisplayValue(0);
      double slope = image.VoxelToDisplayValue(1) - image.VoxelToDisplayValue(0);

      for (int z = 0; z < image.ZSize; z++)
      {
        for (int y = 0; y < image.YSize; y++)
        {
          VVector start = image.Origin +
                          image.YDirection * (y * image.YRes) +
                          image.ZDirection * (z * image.ZRes);
          VVector end = start + image.XDirection * image.XRes * image.XSize;

          SegmentProfile segmentProfile = structure.GetSegmentProfile(start, end, segmentArray);

          for (int x = 0; x < image.XSize; x++)
          {
            if (segmentArray[x])
            {
              if (!voxelPlaneExists)
              {
                image.GetVoxels(z, voxelPlane);
                voxelPlaneExists = true;
              }
              double HU = (voxelPlane[x, y] * slope) + intercept;

              if (!Double.IsNaN(HU))
              {
                sum += HU;
                m_voxelCount++;
                if (HU > m_maxHU) m_maxHU = HU;
                if (HU < m_minHU) m_minHU = HU;
              }
            }
          }
        }
        voxelPlaneExists = false;
      }
      m_averageHU = sum / ((double)m_voxelCount);
      // m_volumeByVoxels = (image.XRes * image.YRes * image.ZRes * m_voxelCount) * 0.001;
      // m_volumeDiff = (m_volume - m_volumeByVoxels) / m_volumeByVoxels * 100.0;
    }

    void RaisePropertyChanged()
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(null));
    }
    public event PropertyChangedEventHandler PropertyChanged;
  }
}
