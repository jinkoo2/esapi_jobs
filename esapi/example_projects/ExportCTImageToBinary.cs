//---------------------------------------------------------------------------------------------
/// <summary>
/// ESAPI script that exports the active CT image to a binary file.
/// The binary file contains raw voxel data as 16-bit unsigned integers (UInt16).
/// Data is written in order: slice by slice (z), row by row (y), column by column (x).
/// </summary>
//---------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace VMS.TPS
{
  public class Script
  {
    public Script()
    {
    }

    //---------------------------------------------------------------------------------------------
    /// <summary>
    /// Exports the active CT image to a binary file.
    /// </summary>
    /// <param name="context">Script context containing the active image</param>
    //---------------------------------------------------------------------------------------------
    public void Execute(ScriptContext context)
    {
      // Check if there's an active image in the context
      if (context.Image == null)
      {
        MessageBox.Show("No active CT image found in context. Please open an image first.", 
                       "Export CT Image", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      Image image = context.Image;

      // Create output filename based on patient ID and image series ID
      string patientId = context.Patient != null ? context.Patient.Id : "Unknown";
      string imageId = image.Id;
      string outputPath = Path.Combine(
        //
        @"U:\temp",
        string.Format("CT_Export_{0}_{1}.bin", patientId, imageId)
      );

      try
      {
        // Export the image to binary file
        ExportImageToBinary(image, outputPath);

        MessageBox.Show(
          string.Format("CT image exported successfully!\n\n" +
                       "Patient ID: {0}\n" +
                       "Image ID: {1}\n" +
                       "Dimensions: {2} x {3} x {4}\n" +
                       "Output file: {5}",
                       patientId, imageId, image.XSize, image.YSize, image.ZSize, outputPath),
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

    //---------------------------------------------------------------------------------------------
    /// <summary>
    /// Exports image voxel data to a binary file.
    /// Data is written as 16-bit unsigned integers (UInt16) in little-endian format.
    /// </summary>
    /// <param name="image">The image to export</param>
    /// <param name="outputFileName">Path to the output binary file</param>
    //---------------------------------------------------------------------------------------------
    private void ExportImageToBinary(Image image, string outputFileName)
    {
      // Delete file if it already exists
      if (File.Exists(outputFileName))
      {
        File.SetAttributes(outputFileName, FileAttributes.Normal);
        File.Delete(outputFileName);
      }

      int width = image.XSize;
      int height = image.YSize;
      int depth = image.ZSize;

      // Buffer to hold one slice of voxel data
      int[,] buffer = new int[width, height];

      // Write binary data using BinaryWriter
      using (BinaryWriter writer = new BinaryWriter(File.Open(outputFileName, FileMode.Create)))
      {
        // Optionally write header information (dimensions) as metadata
        // Uncomment the following lines if you want to include dimensions in the file:
        // writer.Write(width);
        // writer.Write(height);
        // writer.Write(depth);

        // Iterate through all slices (z-direction)
        for (int z = 0; z < depth; z++)
        {
          // Get voxel data for this slice
          image.GetVoxels(z, buffer);

          // Write voxel data row by row (y-direction), column by column (x-direction)
          for (int y = 0; y < height; y++)
          {
            for (int x = 0; x < width; x++)
            {
              int voxelValue = buffer[x, y];
              
              // Convert to UInt16 and write as binary
              // Note: If voxel values can exceed UInt16.MaxValue (65535), 
              // you may need to handle overflow or use a different data type
              UInt16 value = (UInt16)voxelValue;
              writer.Write(value);
            }
          }
        }
      }
    }
  }
}

