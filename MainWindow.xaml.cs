using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace HelmertLSA
{
    public partial class MainWindow : Window
    {
        private HelmertSolver _solver = new HelmertSolver();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseMaster_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "CSV/TXT Files (*.csv;*.txt)|*.csv;*.txt" };
            if (dlg.ShowDialog() == true)
                MasterFileBox.Text = dlg.FileName;
        }

        private void BrowseAdjustable_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "CSV/TXT Files (*.csv;*.txt)|*.csv;*.txt" };
            if (dlg.ShowDialog() == true)
                AdjustFileBox.Text = dlg.FileName;
        }

        private async void RunAdjustment_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MasterFileBox.Text) || string.IsNullOrEmpty(AdjustFileBox.Text))
            {
                MessageBox.Show("Please select both Master and Adjustable files.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string masterFile = MasterFileBox.Text;
            string adjustFile = AdjustFileBox.Text;

            await Task.Run(() =>
            {
                var masterPoints = LoadPoints(masterFile);
                var adjPoints = LoadPoints(adjustFile);

                if (masterPoints == null || adjPoints == null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Error reading files. Please check file format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                    return;
                }

                var common = masterPoints.Join(adjPoints, m => m.Id, a => a.Id, (m, a) => (m, a)).ToList();

                if (common.Count < 3)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("At least 3 common points are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                    return;
                }

                var X_master = common.Select(p => p.m.Vector).ToList();
                var X_adj = common.Select(p => p.a.Vector).ToList();

                var result = _solver.Solve(X_adj, X_master);

                Dispatcher.Invoke(() =>
                {
                    var transformedPoints = _solver.ApplyTransformation(adjPoints, result);
                    SaveTransformedPoints(masterFile, transformedPoints);
                    MessageBox.Show("Transformed points have been saved successfully in .lsa format.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            });
        }

        private void SaveTransformedPoints(string filePath, List<Point3D> transformedPoints)
        {
            var directory = Path.GetDirectoryName(filePath);
            var lsaFilePath = Path.Combine(directory ?? string.Empty, Path.GetFileNameWithoutExtension(filePath) + ".lsa");

            using (var writer = new StreamWriter(lsaFilePath))
            {
                writer.WriteLine("ID,X,Y,Z");
                foreach (var point in transformedPoints)
                {
                    writer.WriteLine($"{point.Id},{point.X},{point.Y},{point.Z}");
                }
            }
        }

        private List<Point3D> LoadPoints(string path)
        {
            var lines = File.ReadAllLines(path);
            return lines.Skip(1)
                        .Select(line =>
                        {
                            var parts = Regex.Split(line.Trim(), "[,\t]+");
                            return new Point3D(parts[0], float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                        }).ToList();
        }
    }
}