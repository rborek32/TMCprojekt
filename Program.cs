using System;
using System.IO;
namespace ConsoleApp1
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

        static void Main(string[] args)
        {
            string filename = @"D:\Magisterka\Semestr 1\geo1\v3\ConsoleApp1\Projekt\result.asc";

            //Debug to see the results
            decimal[,] values = ReadDataFromFile(filename);
        }
    }
}
