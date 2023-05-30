using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TMCprojekt
{
    class Program
    {
        static decimal[,] ReadDataFromFile(string filePath)
        {
            int ncols, nrows;
            string[] lines = File.ReadAllLines(filePath);

            //Get the ncols from the first row
            if (!int.TryParse(lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1], out ncols))
            {
                Console.WriteLine("Unable to parse ncols as an integer.");
                return null;
            }

            //Get the nrows from the second row 
            if (!int.TryParse(lines[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1], out nrows))
            {
                Console.WriteLine("Unable to parse nrows as an integer.");
                return null;
            }

            // Create a two-dimensional array to store values
            decimal[,] values = new decimal[nrows, ncols];

            // Store values
            for (int i = 6; i < lines.Length; i++)
            {
                string[] numbers = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < numbers.Length; j++)
                {
                    if (decimal.TryParse(numbers[j], out decimal value))
                    {
                        values[i - 6, j] = value;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid value at row {i - 6}, column {j}: {numbers[j]}");
                    }
                }
            }
            return values;
        }

        static int[,] ApplyContouringAlgorithm(decimal[,] values, decimal[] contourLevels)
        {
            int height = values.GetLength(0);
            int width = values.GetLength(1);

            int[,] contours = new int[height, width]; // 2D array to store contour information

            for (int i = 1; i < height - 1; i++)
            {
                for (int j = 1; j < width - 1; j++)
                {
                    bool isContour = false;
                    decimal currentValue = values[i, j];

                    // Check if the current value is a contour point
                    foreach (decimal level in contourLevels)
                    {
                        if (currentValue > level && (
                                values[i - 1, j - 1] <= level ||
                                values[i - 1, j] <= level ||
                                values[i - 1, j + 1] <= level ||
                                values[i, j - 1] <= level ||
                                values[i, j + 1] <= level ||
                                values[i + 1, j - 1] <= level ||
                                values[i + 1, j] <= level ||
                                values[i + 1, j + 1] <= level))
                        {
                            isContour = true;
                            break;
                        }
                    }
                    if (isContour)
                        contours[i, j] = 1; // Set contour pixel to 1
                }
            }
            return contours;
        }

        static void SaveContourPositionsToFile(int width, int height,  int[,] contours, string inputFilename, string outputFilename)
        {
            // Save contour information to output file
            using (StreamWriter writer = new StreamWriter(outputFilename))
            {
                string[] headerLines = File.ReadLines(inputFilename).Take(6).ToArray();
                string header = string.Join("\r\n", headerLines);

                writer.WriteLine(header);
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        writer.Write(contours[i, j] + " ");
                    }
                    writer.WriteLine();
                }
            }

            Console.WriteLine("Contour information saved to: " + outputFilename);
        }

        static void Main(string[] args)
        {
            string filename = @"D:\Magisterka\Semestr 1\TMC\Projekt\TMCprojekt\src\example4.asc";
            string outputFilename = @"D:\Magisterka\Semestr 1\TMC\Projekt\TMCprojekt\results\result4.asc";
            //Debug to see the results
            decimal[,] values = ReadDataFromFile(filename);
            
            // Define the contour levels you want to track
            decimal[] contourLevels = { 25, 50, 75, 100 };

            int[,] contourPositions = ApplyContouringAlgorithm(values, contourLevels);

            // Get the dimensions of the contourPositions array
            int height = contourPositions.GetLength(0);
            int width = contourPositions.GetLength(1);

            // Save the contour positions to a file
            SaveContourPositionsToFile(width, height, contourPositions,filename, outputFilename);
        }
    }
}