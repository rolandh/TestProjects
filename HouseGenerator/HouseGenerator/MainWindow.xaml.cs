using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace HouseGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ImageToVector imageProcessor;

        public MainWindow()
        {
            //imageProcessor = new ImageToVector();
            InitializeComponent();
        }

        private void EventHandler MouseHover(object sender, RoutedEventArgs e){
            MessageBox.Hover
        }


        private void LoadBitmapButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the open file dialog box.
            var openFileDialog = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog.Filter = "Image Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;

            openFileDialog.Multiselect = true;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = openFileDialog.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                try
                {
                    imageProcessor = new ImageToVector(openFileDialog.FileName);
                    if (imageProcessor.IsLoaded())
                    {
                        //Draw raster
                        Image image = imageProcessor.GetImage();
                        image.Stretch = Stretch.None;
                        image.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        this.Raster.Children.Clear();
                        this.Raster.Children.Add(image);

                        //Draw vector version
                        List<ImageToVector.ImageVector> vectors = imageProcessor.GetVectors();
                        this.Vector.Children.Clear();
                        foreach (ImageToVector.ImageVector vector in vectors)
                        {
                            if (!vector.Alive) continue;

                            var myLine = new Line();
                            myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                            double factor = 10;
                            myLine.X1 = vector.Sx * factor;
                            myLine.X2 = vector.Ex * factor;
                            myLine.Y1 = vector.Sy * factor;
                            myLine.Y2 = vector.Ey * factor;
                            myLine.StrokeThickness = 4;
                            myLine.HorizontalAlignment = HorizontalAlignment.Center;
                            myLine.VerticalAlignment = VerticalAlignment.Center;

                            this.Vector.Children.Add(myLine);
                            Console.WriteLine("Drawing: " + vector.ToString());
                        }
                    }
                    imageProcessor.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load image due to e: " + ex.Message, "Error", MessageBoxButton.OK);
                }
            }
        }
    }
}
