using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.IO;

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
    public void Execute(ScriptContext context /*, System.Windows.Window window*/)
    {
      Patient patient = context.Patient;
      if (patient == null)
        return;

      patient.BeginModifications();
      
      // Get or create the phantom
      const string structureSetId = "TestPhantomImage";
      StructureSet phantomSS = patient.StructureSets.Where(ss => ss.Id == structureSetId).FirstOrDefault();
      if (phantomSS == null)
      {
        phantomSS = CreateBoxPhantom(patient, structureSetId);
      }

      // Create plan with one beam
      const string courseId = "C1";
      Course course = patient.Courses.Where(c => c.Id == courseId).FirstOrDefault();
      if (course == null)
      {
        course = patient.AddCourse();
        course.Id = courseId;
      }

      ExternalPlanSetup plan = course.AddExternalPlanSetup(phantomSS);
      plan.UniqueFractionation.SetPrescription(1, new DoseValue(1.0, DoseValue.DoseUnit.Gy), 1.0);


      // Create and set fluence
#if TEST10x10
      const double fieldSizeMM = 120.0;
      const float fluenceSizeMM = 100.0f;
#else
      const double fieldSizeMM = 400.0;
      const float fluenceSizeMM = 300.0f;
#endif
      const float fluenceResolutionMM = 2.5f;
      const int fluencePixelCount = (int)(fluenceSizeMM / fluenceResolutionMM);

      VRect<double> jaws = new VRect<double>(-fieldSizeMM * 0.5, -fieldSizeMM * 0.5, +fieldSizeMM * 0.5, +fieldSizeMM * 0.5);

      ExternalBeamMachineParameters beamParams = new ExternalBeamMachineParameters("D_Varian23EX", "6X", 600, "STATIC", null);

#if TEST10x10
      VVector isocenter = new VVector(0.0, 0.0, 213.0);
      float[,] pixels = new float[fluencePixelCount, fluencePixelCount];
      for (int y = 0; y < pixels.GetLength(1); y++)
        for (int x = 0; x < pixels.GetLength(0); x++)
          pixels[x, y] = 1.0f;
      Fluence fluence = new Fluence(pixels, -fluenceSizeMM * 0.5, +fluenceSizeMM * 0.5);
#else
      VVector isocenter = new VVector(0.0, 0.0, 0.0);
      Fluence fluence = new Fluence(ReadBitmapPixels(@"c:\temp\Cosmo.bmp", fluencePixelCount), -fluenceSizeMM * 0.5 + fluenceResolutionMM * 0.5, +fluenceSizeMM * 0.5 - fluenceResolutionMM * 0.5);
#endif
      Beam beam = plan.AddStaticBeam(beamParams, jaws, 0.0, 270.0, 90.0, isocenter);
      beam.SetOptimalFluence(fluence);
      MessageBox.Show("Created plan " + course.Id + " / " + plan.Id);
    }

    //---------------------------------------------------------------------------------------------
    public StructureSet CreateBoxPhantom(Patient patient, string id)
    {
      double imageXYSize = 600.0;
      int imageZSlices = 150;
      StructureSet phantomStrSet = patient.AddEmptyPhantom(id, PatientOrientation.HeadFirstSupine, 512, 512, imageXYSize, imageXYSize, imageZSlices, 3.0);
      VVector origin = phantomStrSet.Image.Origin;
      VVector xDirection = phantomStrSet.Image.XDirection;
      VVector yDirection = phantomStrSet.Image.YDirection;

      double boxSize = 500;
      VVector p1 = origin + xDirection * (imageXYSize - boxSize) * 0.5 + yDirection * (imageXYSize - boxSize) * 0.5;
      VVector p2 = p1 + xDirection * boxSize;
      VVector p3 = p2 + yDirection * boxSize;
      VVector p4 = p1 + yDirection * boxSize;

      VVector[] contour1 = { p1, p2, p3, p4 };

      Structure body = phantomStrSet.AddStructure("EXTERNAL", "Body");

      int firstPlane = 5;
      int lastPlane = imageZSlices - 5;
      for (int z = firstPlane; z <= lastPlane; ++z)
      {
        body.AddContourOnImagePlane(contour1, z);
      }
      body.SetAssignedHU(0);
      return phantomStrSet;
    }

    //---------------------------------------------------------------------------------------------
    public float[,] ReadBitmapPixels(string filename, int sizeInPizels)
    {
      float[,] buffer = new Single[sizeInPizels, sizeInPizels];
      float fluenceMaxVal = 15.0f;

      const int szBITMAPFILEHEADER = 14;
      const int szBITMAPINFOHEADER = 40;
      int szFile = szBITMAPFILEHEADER + szBITMAPINFOHEADER + sizeInPizels * 3 * sizeInPizels;

      FileInfo info = new FileInfo(filename);
      if (info.Length != szFile)
        throw new ApplicationException("File " + filename + " size is not " + szFile.ToString());

      using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      {
        using (BinaryReader reader = new BinaryReader(stream))
        {

          reader.ReadBytes(szBITMAPFILEHEADER + szBITMAPINFOHEADER);
          for (int y = 0; y < sizeInPizels; y++)
          {
            for (int x = 0; x < sizeInPizels; x++)
            {
              buffer[x, y] = (((float)reader.ReadByte()) / 255.0f) * fluenceMaxVal;
              reader.ReadBytes(2);
            }
          }
        }
      }
      return buffer;
    }
  }
}
