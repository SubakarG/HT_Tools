using System;
using System.Collections.Generic;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;

namespace HelmertLSA
{
    public static class PointSaver
    {
        /// <summary>
        /// Saves points to a file in the specified format (CSV, JSON, or TXT).
        /// If no filename is specified, saves the points to a default `.lsa` file in the same directory as the master file.
        /// </summary>
        /// <param name="masterFilePath">The path of the master file (used for determining the output directory).</param>
        /// <param name="masterPoints">The list of master points.</param>
        /// <param name="adjustedPoints">The list of adjusted (transformed) points.</param>
        /// <param name="format">The format to save the file in ("csv", "json", or "txt").</param>
        public static void SavePoints(string masterFilePath, List<Vector<float>> masterPoints, List<Vector<float>> adjustedPoints, string format = "lsa")
        {
            // Validate arguments and handle potential null values
            if (string.IsNullOrEmpty(masterFilePath))
                throw new ArgumentNullException(nameof(masterFilePath), "Master file path cannot be null or empty.");

            if (masterPoints == null)
                throw new ArgumentNullException(nameof(masterPoints), "Master points list cannot be null.");

            if (adjustedPoints == null)
                throw new ArgumentNullException(nameof(adjustedPoints), "Adjusted points list cannot be null.");

            if (masterPoints.Count != adjustedPoints.Count)
                throw new ArgumentException("Master and adjusted point lists must have the same number of elements.");

            // Get directory and file name safely
            string? directory = Path.GetDirectoryName(masterFilePath);
            if (directory == null)
                throw new ArgumentException("The provided master file path does not contain valid directory information.");

            string fileName = Path.GetFileNameWithoutExtension(masterFilePath) + ".lsa";
            string filePath = Path.Combine(directory, fileName);

            // Save points in the specified format
            if (format.ToLower() == "csv")
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("PointID,MasterX,MasterY,MasterZ,AdjustedX,AdjustedY,AdjustedZ");
                    for (int i = 0; i < masterPoints.Count; i++)
                    {
                        var m = masterPoints[i];
                        var a = adjustedPoints[i];
                        writer.WriteLine($"{i},{m[0]:F4},{m[1]:F4},{m[2]:F4},{a[0]:F4},{a[1]:F4},{a[2]:F4}");
                    }
                }
            }
            else if (format.ToLower() == "json")
            {
                var points = new List<object>();
                for (int i = 0; i < masterPoints.Count; i++)
                {
                    var m = masterPoints[i];
                    var a = adjustedPoints[i];
                    points.Add(new
                    {
                        PointID = i,
                        Master = new { X = m[0], Y = m[1], Z = m[2] },
                        Adjusted = new { X = a[0], Y = a[1], Z = a[2] }
                    });
                }

                File.WriteAllText(filePath, JsonConvert.SerializeObject(points, Newtonsoft.Json.Formatting.Indented));
            }
            else if (format.ToLower() == "txt")
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("PointID\tMasterX\tMasterY\tMasterZ\tAdjustedX\tAdjustedY\tAdjustedZ");
                    for (int i = 0; i < masterPoints.Count; i++)
                    {
                        var m = masterPoints[i];
                        var a = adjustedPoints[i];
                        writer.WriteLine($"{i}\t{m[0]:F4}\t{m[1]:F4}\t{m[2]:F4}\t{a[0]:F4}\t{a[1]:F4}\t{a[2]:F4}");
                    }
                }
            }
            else if (format.ToLower() == "lsa")
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("PointID,MasterX,MasterY,MasterZ,AdjustedX,AdjustedY,AdjustedZ");
                    for (int i = 0; i < masterPoints.Count; i++)
                    {
                        var m = masterPoints[i];
                        var a = adjustedPoints[i];
                        writer.WriteLine($"{i},{m[0]:F4},{m[1]:F4},{m[2]:F4},{a[0]:F4},{a[1]:F4},{a[2]:F4}");
                    }
                }
            }
            else
            {
                throw new ArgumentException("Invalid format specified. Use 'csv', 'json', 'txt', or 'lsa'.");
            }
        }
    }
}