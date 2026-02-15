using itk.simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using itk.simple;
using System.Runtime.InteropServices;
using System.Windows;
using VMS.TPS.Common.Model.Types;

namespace esapi
{
    public static class simpleitk
    {

        public static void SaveImage_Int16(VMS.TPS.Common.Model.API.Image img, string path)
        {
            int[] size = new int[] { img.XSize, img.YSize, img.ZSize };
            double[] spacing = new double[] { img.XRes, img.YRes, img.ZRes };
            //VVector rowDirection = img.XDirection;
            //VVector columnDirection = img.YDirection;
            VVector org = img.Origin;

            /////////////////////
            // save pixel data
            int[,] buffer = new int[size[0], size[1]];
            //Int16[] bufferSave = new Int16[size[0] * size[1]];
            //byte[] bytesSave = new byte[sizeof(Int16) * size[0] * size[1]];

            uint[] sz = new uint[] { (uint)size[0], (uint)size[1], (uint)size[2] };
            itk.simple.VectorUInt32 itkSize = new itk.simple.VectorUInt32(sz);
            itk.simple.VectorDouble itkSpacing = new itk.simple.VectorDouble(spacing);
            itk.simple.VectorDouble itkOrigin = new itk.simple.VectorDouble(new double[] { org[0], org[1], org[2] });
            itk.simple.Image itkImage = new itk.simple.Image(itkSize, itk.simple.PixelIDValueEnum.sitkInt16);
            itkImage.SetSpacing(itkSpacing);
            itkImage.SetOrigin(new itk.simple.VectorDouble(itkOrigin));

            for (uint z = 0; z < size[2]; z++)
            {
                // get a slice 
                img.GetVoxels((int)z, buffer);

                // copy to Int16 buffer
                for (uint y = 0; y < size[1]; y++)
                {
                    for (uint x = 0; x < size[0]; x++)
                    {
                        // convert to display value
                        short value = (short)img.VoxelToDisplayValue(buffer[x, y]);

                        itk.simple.VectorUInt32 idx = new itk.simple.VectorUInt32(new UInt32[] { x, y, z });
                        itkImage.SetPixelAsInt16(idx, (short)value);
                        //bufferSave[y * size[0] + x] = (Int16)value;
                    } // x
                } // Y

                //Buffer.BlockCopy(bufferSave, 0, bytesSave, 0, bytesSave.Length);
                //System.Runtime.InteropServices.Marshal.Copy(bufferSave, 0, itkImage.GetBufferAsInt16(), )
            } // Z

            // Save the image to a file
            itk.simple.ImageFileWriter writer = new ImageFileWriter();
            writer.Execute(itkImage, path, true);
            Console.WriteLine("Images created and saved successfully. path=" + path);
        }


        public static void SaveImage_UInt8(VMS.TPS.Common.Model.API.Image img, string path)
        {
            Console.WriteLine("here1");

            int[] size = new int[] { img.XSize, img.YSize, img.ZSize };
            double[] spacing = new double[] { img.XRes, img.YRes, img.ZRes };
            //VVector rowDirection = img.XDirection;
            //VVector columnDirection = img.YDirection;
            VVector org = img.Origin;

            Console.WriteLine("here2");

            /////////////////////
            // save pixel data
            int[,] buffer = new int[size[0], size[1]];
            //Int16[] bufferSave = new Int16[size[0] * size[1]];
            //byte[] bytesSave = new byte[sizeof(Int16) * size[0] * size[1]];

            Console.WriteLine("here3");

            uint[] sz = new uint[] { (uint)size[0], (uint)size[1], (uint)size[2] };
            itk.simple.VectorUInt32 itkSize = new itk.simple.VectorUInt32(sz);
            itk.simple.VectorDouble itkSpacing = new itk.simple.VectorDouble(spacing);
            itk.simple.VectorDouble itkOrigin = new itk.simple.VectorDouble(new double[] { org[0], org[1], org[2] });
            itk.simple.Image itkImage = new itk.simple.Image(itkSize, itk.simple.PixelIDValueEnum.sitkInt16);
            itkImage.SetSpacing(itkSpacing);
            itkImage.SetOrigin(new itk.simple.VectorDouble(itkOrigin));

            Console.WriteLine("here4");

            for (uint z = 0; z < size[2]; z++)
            {
                // get a slice 
                img.GetVoxels((int)z, buffer);

                Console.WriteLine("here5");

                // copy to Int16 buffer
                for (uint y = 0; y < size[1]; y++)
                {
                    for (uint x = 0; x < size[0]; x++)
                    {
                        // convert to display value
                        short value = (short)img.VoxelToDisplayValue(buffer[x, y]);

                        itk.simple.VectorUInt32 idx = new itk.simple.VectorUInt32(new UInt32[] { x, y, z });
                        itkImage.SetPixelAsInt16(idx, (short)value);
                        //bufferSave[y * size[0] + x] = (Int16)value;
                    } // x
                } // Y

                //Buffer.BlockCopy(bufferSave, 0, bytesSave, 0, bytesSave.Length);
                //System.Runtime.InteropServices.Marshal.Copy(bufferSave, 0, itkImage.GetBufferAsInt16(), )
            } // Z

            Console.WriteLine("here6");

            // Save the image to a file
            itk.simple.ImageFileWriter writer = new ImageFileWriter();
            writer.Execute(itkImage, path, true);
            Console.WriteLine("Images created and saved successfully. path=" + path);
        }

        public static void save_itkimage(itk.simple.Image itkImage, string path, bool useCompression = true)
        {
            string fn = $"save_itkimage(itkImage, path={path}, useCompression={useCompression})";

            if (itkImage == null)
            {
                helper.error($"itkImage is null. fn={fn}");
                return;
            }

            // Save the image to a file
            itk.simple.ImageFileWriter writer = new ImageFileWriter();
            writer.Execute(itkImage, path, useCompression);
            helper.print($"Image created and saved successfully. path={path}, compressed={useCompression}. fn={fn}");
        }

        public static itk.simple.Image read_itkimage_UInt8(string image_path)
        {
            string fn = $"read_itkimage_UInt8(path={image_path})";

            itk.simple.ImageFileReader reader = new itk.simple.ImageFileReader();
            reader.SetFileName(image_path);
            return reader.Execute();

        }


        public static void test()
        {
            // Define the image size
            int width = 512;
            int height = 512;

            // Create a SimpleITK image
            uint[] size = { (uint)width, (uint)height };
            double[] org = { 0.0, 0.0 };
            double[] sp = { 1.0, 1.0 };
            Image image = new Image(new VectorUInt32(size), PixelIDValueEnum.sitkUInt8);
            
;           image.SetOrigin(new VectorDouble(org));
            image.SetSpacing(new VectorDouble(sp));

            // Populate the image with some data (e.g., a white square)
            byte[] dataArray = new byte[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;

                    if(x>100 && x<200 && y>100 && y<200)
                        dataArray[index] = 255; // Set pixel intensity to 255 (white)
                    else
                        dataArray[index] = 0;
                }
            }

            // copy data to image buffer
            Marshal.Copy(dataArray, 0, image.GetBufferAsUInt8(), width * height);

            // Save the image to a file
            string outputPath = "image.png";
            itk.simple.ImageFileWriter writer = new ImageFileWriter();
            writer.Execute(image, outputPath, true);
            Console.WriteLine("Image created and saved successfully.");
        }


        public static itk.simple.Image mult_by_const_uint8(itk.simple.Image image, double constant)
        {
            // Multiply the image by the constant (result will be float32)
            Image result = SimpleITK.Multiply(image, constant);

            // Cast the result back to unsigned char
            Image finalResult = SimpleITK.Cast(result, PixelIDValueEnum.sitkUInt8);

            return finalResult;
        }

        public static itk.simple.Image add(itk.simple.Image image1, itk.simple.Image image2)
        {
            return SimpleITK.Add(image1, image2);
        }

    }
}


