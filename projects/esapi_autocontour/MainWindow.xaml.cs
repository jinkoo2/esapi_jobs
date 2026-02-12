using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Win32;
using VMS.TPS.Common.Model.API;
using itk.simple;

namespace autocontour
{
  public partial class MainWindow : Window
  {
    private ScriptContext _context;

    public MainWindow(ScriptContext context)
    {
      _context = context;
      InitializeComponent();
    }

    private void ExportImageButton_Click(object sender, RoutedEventArgs e)
    {
      // Check if there's an active image in the context
      if (_context == null || _context.Image == null)
      {
        MessageBox.Show("No active CT image found in context. Please open an image first.",
                       "Export CT Image", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      VMS.TPS.Common.Model.API.Image image = _context.Image;

      // Ask user where to save the file
      SaveFileDialog saveDialog = new SaveFileDialog
      {
        Filter = "ITK MetaImage (*.mha)|*.mha|ITK MetaImage Header (*.mhd)|*.mhd|NRRD Image (*.nrrd)|*.nrrd|NIfTI (*.nii)|*.nii|NIfTI GZ (*.nii.gz)|*.nii.gz|All Files (*.*)|*.*",
        FileName = string.Format("CT_Export_{0}_{1}.mha",
                                _context.Patient != null ? _context.Patient.Id : "Unknown",
                                image.Id),
        Title = "Save Image as ITK Image"
      };

      if (saveDialog.ShowDialog() == true)
      {
        try
        {
          // Export the image to ITK format using SimpleITK
          ExportImageToITK(image, saveDialog.FileName);

          string patientId = _context.Patient != null ? _context.Patient.Id : "Unknown";
          string imageId = image.Id;

          MessageBox.Show(
            string.Format("CT image exported successfully!\n\n" +
                         "Patient ID: {0}\n" +
                         "Image ID: {1}\n" +
                         "Dimensions: {2} x {3} x {4}\n" +
                         "Output file: {5}",
                         patientId, imageId, image.XSize, image.YSize, image.ZSize, saveDialog.FileName),
            "Export CT Image",
            MessageBoxButton.OK,
            MessageBoxImage.Information
          );
        }
        catch (Exception ex)
        {
          MessageBox.Show(
            string.Format("Error exporting CT image:\n\n{0}", ex.Message),
            "Export CT Image Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error
          );
        }
      }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    //---------------------------------------------------------------------------------------------
    /// <summary>
    /// Exports image voxel data to an ITK image file using SimpleITK.
    /// </summary>
    /// <param name="image">The ESAPI image to export</param>
    /// <param name="outputFileName">Path to the output ITK image file</param>
    //---------------------------------------------------------------------------------------------
    private void ExportImageToITK(VMS.TPS.Common.Model.API.Image image, string outputFileName)
    {
      int width = image.XSize;
      int height = image.YSize;
      int depth = image.ZSize;

      // Get image spacing and origin from ESAPI image
      double spacingX = image.XRes;
      double spacingY = image.YRes;
      double spacingZ = image.ZRes;

      VMS.TPS.Common.Model.Types.VVector origin = image.Origin;
      double originX = origin.x;
      double originY = origin.y;
      double originZ = origin.z;

      // Buffer to hold one slice of voxel data
      int[,] buffer = new int[width, height];

      // Create array to hold all voxel data
      // SimpleITK expects data in x, y, z order (column, row, slice)
      short[] voxelData = new short[width * height * depth];

      // Get all voxel data from ESAPI image
      for (int z = 0; z < depth; z++)
      {
        image.GetVoxels(z, buffer);
        for (int y = 0; y < height; y++)
        {
          for (int x = 0; x < width; x++)
          {
            // Index calculation: x + y*width + z*width*height (x varies fastest)
            int index = x + y * width + z * width * height;
            int voxelValue = buffer[x, y];
            // Convert to short (16-bit signed integer)
            // Note: SimpleITK uses signed types, so we need to handle the conversion
            voxelData[index] = (short)Math.Max(short.MinValue, Math.Min(short.MaxValue, voxelValue));
          }
        }
      }

      // Create SimpleITK image
      VectorUInt32 size = new VectorUInt32(new uint[] { (uint)width, (uint)height, (uint)depth });
      VectorDouble spacingVec = new VectorDouble(new double[] { spacingX, spacingY, spacingZ });
      VectorDouble originVec = new VectorDouble(new double[] { originX, originY, originZ });

      // Create the image with Int16 pixel type
      itk.simple.Image sitkImage = new itk.simple.Image(size, PixelIDValueEnum.sitkInt16);
      sitkImage.SetSpacing(spacingVec);
      sitkImage.SetOrigin(originVec);
      
      // Get buffer from SimpleITK image and copy data using Marshal
      IntPtr bufferPtr = sitkImage.GetBufferAsInt16();
      Marshal.Copy(voxelData, 0, bufferPtr, voxelData.Length);

      // Write the image using SimpleITK
      SimpleITK.WriteImage(sitkImage, outputFileName);
    }
  }
}

