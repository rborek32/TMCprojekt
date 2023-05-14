using System;
using OSGeo.GDAL;
using System.IO;
using DataType = OSGeo.GDAL.DataType;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class Program
    {
        static void TiffToDtm(string inputFilePath, string outputFilePath)
        {
            Gdal.AllRegister(); // Register all known GDAL drivers

            // Open the input TIFF file
            Dataset inputDataset = Gdal.Open(inputFilePath, Access.GA_ReadOnly);
            Band inputBand = inputDataset.GetRasterBand(1);

            // Get the geotransform (coordinate system) information from the input file
            double[] geotransform = new double[6];
            inputDataset.GetGeoTransform(geotransform);

            // Allocate memory for the output dataset
            Dataset outputDataset = Gdal.GetDriverByName("DTM").Create(outputFilePath,
                inputDataset.RasterXSize, inputDataset.RasterYSize, 1, DataType.GDT_Float32, null);
            Band outputBand = outputDataset.GetRasterBand(1);

            // Set the geotransform information for the output file
            outputDataset.SetGeoTransform(geotransform);

            // Perform the conversion by copying data from the input to the output dataset
            float[] data = new float[inputDataset.RasterXSize * inputDataset.RasterYSize];
            inputBand.ReadRaster(0, 0, inputDataset.RasterXSize, inputDataset.RasterYSize,
                data, inputDataset.RasterXSize, inputDataset.RasterYSize, 0, 0);
            outputBand.WriteRaster(0, 0, inputDataset.RasterXSize, inputDataset.RasterYSize,
                data, inputDataset.RasterXSize, inputDataset.RasterYSize, 0, 0);

            // Close the input and output files
            inputDataset.Dispose();
            outputDataset.Dispose();
        }

        static void Main(string[] args)
        {
            string filename = @"D:\Magisterka\Semestr 1\geo1\v3\ConsoleApp1\Projekt\result.asc";
            string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "results", "DigitalTerrainModelFormatTIFF.tif");

            //string text = File.ReadAllText(filename);
            //Console.WriteLine(text);


            //string[] lines = File.ReadAllLines(filename);
            //string[] lines = File.ReadLines(filename);

            string[] lines = File.ReadAllLines(filename);

            //string[] firstLineParts = lines[0].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            //int ncols = int.Parse(firstLineParts[1]);
            //string[] secondLine = lines[1].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            //int nrows = int.Parse(secondLine[1]);

            int ncols, nrows;

            if (!lines[0].StartsWith("ncols") || !lines[1].StartsWith("nrows"))
            {
                Console.WriteLine("Invalid file format. Unable to parse ncols and nrows.");
                return;
            }

            if (!int.TryParse(lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1], out ncols))
            {
                Console.WriteLine("Invalid file format. Unable to parse ncols as an integer.");
                return;
            }

            if (!int.TryParse(lines[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1], out nrows))
            {
                Console.WriteLine("Invalid file format. Unable to parse nrows as an integer.");
                return;
            }

            // Create a two-dimensional array
            decimal[,] data = new decimal[nrows, ncols];

            // Extract the decimal numbers from the lines and store them in the array
            int dataIndex = 0;
            for (int i = 6; i < lines.Length; i++)
            {
                string[] numbers = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < numbers.Length; j++)
                {
                    if (decimal.TryParse(numbers[j], out decimal value))
                    {
                        data[dataIndex, j] = value;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid value at row {dataIndex}, column {j}: {numbers[j]}");
                    }
                }
                dataIndex++;
            }

            // Print last 3 elements of the array
            Console.WriteLine(data[nrows - 1, ncols - 3]);
            Console.WriteLine(data[nrows - 1, ncols - 2]);
            Console.WriteLine(data[nrows - 1 ,ncols - 1]);
        }
    }
}
