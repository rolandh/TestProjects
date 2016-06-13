using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace HouseGenerator
{
    class ImageToVector : IDisposable
    {
        private readonly string fileName;
        private readonly Boolean loaded = false;
        private readonly Image image;
        private readonly System.Drawing.Bitmap bitmap;
        static private readonly double vectorTolerance = 0.01;
        private List<ImageVector> imageVectors = new List<ImageVector>();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) if (bitmap != null) bitmap.Dispose();
        }

        public List<ImageVector> GetVectors()
        {
            return imageVectors;
        }

        public Image GetImage()
        {
            return image;
        }

        public string GetFileName()
        {
            if (loaded && !String.IsNullOrEmpty(fileName)) return fileName;
            return "";
        }

        public bool IsLoaded()
        {
            return loaded;
        }

        public BitmapSource DecodeImage(string fileName)
        {
            BitmapSource source = null;

            PngBitmapDecoder decoder;
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);

                if (decoder.Frames != null && decoder.Frames.Count > 0)
                    source = decoder.Frames[0];
            }
            return source;
        }

        public ImageToVector(string fileName)
        {
            if (!File.Exists(fileName)) return;

            image = new Image();

            byte[] imageBytes = LoadImageData(fileName);
            ImageSource imageSource = CreateImage(imageBytes);
            image.Source = imageSource;

            bitmap = new System.Drawing.Bitmap(fileName);

            Raster2Vector();

            this.fileName = fileName;
            loaded = true;
            

        }

        public void ProcessImage()
        {
            if (!IsLoaded()) return;
        }

        public ImageToVector()
        {
            loaded = false;
            
        }

        public void Raster2Vector()
        {
            //Turn all pixels into vectors of length 0
            CreateInitialVectors();
            Console.WriteLine("Created " + imageVectors.Count + " vectors");
            //Join all vectors that are on the same plane
            JoinVectors();
            Console.WriteLine(imageVectors.Count + " Vectors left after joining");

            //Extend adjacent verticies so they are now overlapping
            ExtendVerticies();

            //Delete any vectors which are a subset of others
            CombineVectorSubsets();
            Console.WriteLine(imageVectors.Count + " Vectors left after removing subsets");
        }

        public void JoinVectors()
        {
            for(int i = 0; i < imageVectors.Count; i++)
            {
                if (!imageVectors[i].Alive) continue;

                for (int j = 0; j < imageVectors.Count; j++)
                {
                    ImageVector ivec = imageVectors[i];
                    ImageVector jvec = imageVectors[j];
                    if (!imageVectors[j].Alive || (i== j)) continue;
                    //if vector has a start or end position within 1 unit of us and it is in the same plane, then join the vectors
                    if (imageVectors[i].WithinSamePlane(imageVectors[j]))
                    {
                        //if v.Sx,Sy is adjacent to n.Sx,Sy OR
                        //if v.Sx,Sy is adjacent to n.Ex,Ey OR
                        //if v.Ex,Ey is adjacent to n.Sx,Sy OR
                        //if v.Ex,Ey is adjacent to n.Ex,Ey OR
                        //Join the vectors by updating the vector and deleting the neighbor
                        Console.WriteLine("i: " + i + " j: " + j + " in the same plane");
                        String prevVector = imageVectors[i].ToString();
                        if (PointsAreAdjacent(imageVectors[i].Sx, imageVectors[i].Sy, imageVectors[j].Sx, imageVectors[j].Sy))
                        {
                            imageVectors[i].Update(imageVectors[i].Ex, imageVectors[i].Ey, imageVectors[j].Ex, imageVectors[j].Ey);
                            imageVectors[j].Alive = false;
                            Console.WriteLine("1 i: " + i + " j: " + j + " Updated vector " + prevVector + " to:          " + imageVectors[i].ToString());
                            Console.WriteLine("Removed vector " + imageVectors[j].ToString());
                        }
                        else if (PointsAreAdjacent(imageVectors[i].Sx, imageVectors[i].Sy, imageVectors[j].Ex, imageVectors[j].Ey))
                        {
                            imageVectors[i].Update(imageVectors[i].Ex, imageVectors[i].Ey, imageVectors[j].Sx, imageVectors[j].Sy);
                            imageVectors[j].Alive = false;
                            Console.WriteLine("2 i: " + i + " j: " + j + " Updated vector " + prevVector + " to:          " + imageVectors[i].ToString());
                            Console.WriteLine("Removed vector " + imageVectors[j].ToString());
                        }
                        else if(PointsAreAdjacent(imageVectors[i].Ex, imageVectors[i].Ey, imageVectors[j].Sx, imageVectors[j].Sy))
                        {
                            imageVectors[i].Update(imageVectors[i].Sx, imageVectors[i].Sy, imageVectors[j].Ex, imageVectors[j].Ey);
                            imageVectors[j].Alive = false;
                            Console.WriteLine("3 i: " + i + " j: " + j + " Updated vector " + prevVector + " to:          " + imageVectors[i].ToString());
                            Console.WriteLine("Removed vector " + imageVectors[j].ToString());
                        }
                        else if(PointsAreAdjacent(imageVectors[i].Ex, imageVectors[i].Ey, imageVectors[j].Ex, imageVectors[j].Ey))
                        {
                            imageVectors[i].Update(imageVectors[i].Sx, imageVectors[i].Sy, imageVectors[j].Sx, imageVectors[j].Sy);
                            imageVectors[j].Alive = false;
                            Console.WriteLine("4 i: " + i + " j: " + j + " Updated vector " + prevVector + " to:          " + imageVectors[i].ToString());
                            Console.WriteLine("Removed vector " + imageVectors[j].ToString());
                        }
                    }
                }
            }

            //Delete the dead vectors
            var newVectors = new List<ImageVector>();
            foreach (ImageVector vector in imageVectors) if (vector.Alive) newVectors.Add(vector);
            imageVectors = newVectors;

        }

        //Iterate all of the vectors checking if they are a subset of another vector
        public void CombineVectorSubsets()
        {
            for (int i = 0; i < imageVectors.Count; i++)
            {
                if (!imageVectors[i].Alive) continue;

                for (int j = 0; j < imageVectors.Count; j++)
                {
                    if (!imageVectors[j].Alive || (i == j)) continue;

                    String prevVector = imageVectors[i].ToString();

                    //If we are in the same plane and our length is less than the master vector then lets check if we are a subset of the vector
                    bool withinSamePlane = imageVectors[i].WithinSamePlane(imageVectors[j]);
                    if ((imageVectors[j].Length <= imageVectors[i].Length) && withinSamePlane)
                    {
                        //If our start and end lie within the master vector then we are a subset to be deleted
                        //Get the most positive point of the master vector to make our check more readable
                        int iex, isx, jsx, jex, iey, isy, jey, jsy;
                        if (imageVectors[i].Sx > imageVectors[i].Ex)
                        {
                            iex = imageVectors[i].Sx;
                            isx = imageVectors[i].Ex;
                            iey = imageVectors[i].Sy;
                            isy = imageVectors[i].Ey;
                        }
                        else
                        {
                            iex = imageVectors[i].Ex;
                            isx = imageVectors[i].Sx;
                            iey = imageVectors[i].Ey;
                            isy = imageVectors[i].Sy;
                        }

                        if (imageVectors[j].Sx > imageVectors[j].Ex)
                        {
                            jex = imageVectors[j].Sx;
                            jsx = imageVectors[j].Ex;
                            jey = imageVectors[j].Sy;
                            jsy = imageVectors[j].Ey;
                        }
                        else
                        {
                            jex = imageVectors[j].Ex;
                            jsx = imageVectors[j].Sx;
                            jey = imageVectors[j].Ey;
                            jsy = imageVectors[j].Sy;
                        }


                        if (((jsx > isx) && (jsx < iex)) && ((jex > isx) && (jex < iex)))
                        {
                            //If j is a complete subset of i it is deleted
                            //isx--------jsx======jex-----iex
                            imageVectors[j].Alive = false;
                            Console.WriteLine("Removed vector " + imageVectors[j].ToString());
                        }
                        else if ((isx >= jsx) && (isx <= jex))
                        {
                            //if j is a partial subset of i it is deleted and i is extended
                            //jsx===========jex
                            //       isx-----------iex
                            //jsx===========jex
                            //              isx-----------iex
                            imageVectors[i].Update(jsx, jsy, iex, iey);
                            imageVectors[j].Alive = false;
                            Console.WriteLine("i: " + i + " j: " + j + " Updated vector " + prevVector + " to:         " + imageVectors[i].ToString());
                            Console.WriteLine("Removed vector " + imageVectors[j].ToString());
                        }
                        else if ((jsx >= isx) && (jsx <= iex))
                        {
                            //if j is a partial subset of i it is deleted and i is extended
                            //isx===========iex
                            //       jsx-----------jex
                            //isx===========iex
                            //              jsx-----------jex

                            imageVectors[i].Update(isx, isy, jex, jey);
                            imageVectors[j].Alive = false;
                            Console.WriteLine("i: " + i + " j: " + j + " Updated vector " + prevVector + " to:         " + imageVectors[i].ToString());
                            Console.WriteLine("Removed vector " + imageVectors[j].ToString());
                        }
                    }
                }
            }

            //Delete the dead vectors
            var newVectors = new List<ImageVector>();
            foreach (ImageVector vector in imageVectors) if (vector.Alive) newVectors.Add(vector);
            imageVectors = newVectors;
        }

        public void ExtendVerticies()
        {
            for (int i = 0; i < imageVectors.Count; i++)
            {
                if (!imageVectors[i].Alive) continue;

                for (int j = 0; j < imageVectors.Count; j++)
                {
                    if (!imageVectors[j].Alive || (i == j)) continue;
                    //If we are not within the same plane however a set of our vertexs are adjacent, then extend the appropriate vector.
                    if (!imageVectors[i].WithinSamePlane(imageVectors[j]))
                    {
                        String prevJVector = imageVectors[j].ToString();

                        //Only update the vector if we are making it longer in the same plane
                        if (PointsAreAdjacent(imageVectors[i].Sx, imageVectors[i].Sy, imageVectors[j].Sx, imageVectors[j].Sy))
                        {
                            ImageVector newVector = new ImageVector(imageVectors[i].Sx, imageVectors[i].Sy, imageVectors[j].Ex, imageVectors[j].Ey);
                            if ((newVector.Length > imageVectors[j].Length) && (newVector.Slope == imageVectors[j].Slope) && (newVector.Intercept == imageVectors[j].Intercept))
                            {
                                imageVectors[j] = newVector;
                                Console.WriteLine("i: " + i + " j: " + j + " Updated vector " + prevJVector + " to:         " + imageVectors[j].ToString());
                            }
                        }
                        else if (PointsAreAdjacent(imageVectors[i].Sx, imageVectors[i].Sy, imageVectors[j].Ex, imageVectors[j].Ey))
                        {
                            ImageVector newVector = new ImageVector(imageVectors[j].Sx, imageVectors[j].Sy, imageVectors[i].Sx, imageVectors[i].Sy);
                            if ((newVector.Length > imageVectors[j].Length) && (newVector.Slope == imageVectors[j].Slope) && (newVector.Intercept == imageVectors[j].Intercept))
                            {
                                imageVectors[j] = newVector;
                                Console.WriteLine("i: " + i + " j: " + j + " Updated vector " + prevJVector + " to:         " + imageVectors[j].ToString());
                            }
                        }
                        else if (PointsAreAdjacent(imageVectors[i].Ex, imageVectors[i].Ey, imageVectors[j].Sx, imageVectors[j].Sy))
                        {
                            ImageVector newVector = new ImageVector(imageVectors[i].Ex, imageVectors[i].Ey, imageVectors[j].Ex, imageVectors[j].Ey);
                            if ((newVector.Length > imageVectors[j].Length) && (newVector.Slope == imageVectors[j].Slope) && (newVector.Intercept == imageVectors[j].Intercept))
                            {
                                imageVectors[j] = newVector;
                                Console.WriteLine("i: " + i + " j: " + j + " Updated vector " + prevJVector + " to:         " + imageVectors[j].ToString());
                            }
                        }
                        else if (PointsAreAdjacent(imageVectors[i].Ex, imageVectors[i].Ey, imageVectors[j].Ex, imageVectors[j].Ey))
                        {
                            ImageVector newVector = new ImageVector(imageVectors[j].Sx, imageVectors[j].Sy, imageVectors[i].Ex, imageVectors[i].Ey);
                            if ((newVector.Length > imageVectors[j].Length) && (newVector.Slope == imageVectors[j].Slope) && (newVector.Intercept == imageVectors[j].Intercept))
                            {
                                imageVectors[j] = newVector;
                                Console.WriteLine("i: " + i + " j: " + j + " Updated vector " + prevJVector + " to:         " + imageVectors[j].ToString());
                            }
                        }
                    }
                }
            }
        }

        public void CreateInitialVectors()
        {
            //Create a 1x1 vector to represent each pixel
            for(int x = 0; x < bitmap.Width; x++) for (int y = 0; y < bitmap.Height; y++) if (GetPixel(x, y)) imageVectors.Add(new ImageVector(x, y, x, y));

        }
        public class ImageVector : System.Windows.Shapes
        {
            public int Sx;
            public int Sy;
            public int Ex;
            public int Ey;
            public double Slope;
            public double Intercept;
            public double Length;
            public bool Alive = false;

            public ImageVector(int sx, int sy, int ex, int ey)
            {
                this.Sx = sx;
                this.Sy = sy;
                this.Ex = ex;
                this.Ey = ey;
                ReCalculate();
                Alive = true;
            }

            public void Update(int sx, int sy, int ex, int ey)
            {
                this.Sx = sx;
                this.Sy = sy;
                this.Ex = ex;
                this.Ey = ey;
                ReCalculate();
                Alive = true;
            }

            public void ReCalculate()
            {
                double x2 = Ex;
                double x1 = Sx;
                double y2 = Ey;
                double y1 = Sy;

                //Calculate the length from 0
                double Length1 = Math.Sqrt(((x1 - 0) * (x1 - 0)) + ((y1 - 0) * (y1 - 0)));
                double Length2 = Math.Sqrt(((x2 - 0) * (x2 - 0)) + ((y2 - 0) * (y2 - 0)));

                //Make sure the Start point is the closest point to 0,0
                if (Length1 >= Length2)
                {
                    this.Sx = Ex;
                    this.Sy = Ey;
                    this.Ex = Sx;
                    this.Ey = Sy;
                    x2 = Ex;
                    x1 = Sx;
                    y2 = Ey;
                    y1 = Sy;
                } 

                //m = rise/run
                Slope = ((x1 - x2) / (y1 - y2));
                //y/mx = c
                Intercept = (y1 / (Slope * x1));

                Length = Length(x1, y1, x2, y2);
            }

                public bool WithinSamePlane(ImageVector vector, double tolerance = 0.0) {
                if (tolerance == 0.0) tolerance = vectorTolerance;

                //if both our points have length zero then we are on the same plane as our slopes are NaN
                if (Length == 0.0 && vector.Length == 0.0) return true;

                //If all of the X values are the same or all of the Y values are the same then we must be in the same plane
                if (MatchFour(Sx, Ex, vector.Sx, vector.Ex)) return true;
                if (MatchFour(Sy, Ey, vector.Sy, vector.Ey)) return true;

                //Check for diagonal vectors
                //If our slope and intercept are both valid numbers and the same within the tolerance these vectors are considered in the same plane as each other
                bool range1 = TestRange(Slope, vector.Slope * (1.0 - tolerance), vector.Slope * (1.0 + tolerance));
                bool range2 = TestRange(Intercept, vector.Intercept * (1.0 - tolerance), vector.Intercept * (1.0 + tolerance));
                return (range1 && range2);
            }

            public override String ToString()
            {
                String slope = Slope.ToString();
                if (slope.Equals("-∞")) slope = "NaN";
                return "Sx: " + Sx + " Sy: " + Sy + " Ex: " + Ex + " Ey: " + Ey + " m: " + slope + " Int: " + Intercept + " L: " + Length + " Alive: " + Alive;
            }

        }

        static double Length(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt((((double)x2 - (double)x1) * ((double)x2 - (double)x1)) + (((double)y2 - (double)y1) * ((double)y2 - (double)y1)));
        }
        static double Length(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
        }

        static bool MatchTwo(int a, int b)
        {
            return a == b;
        }

        static bool MatchFour(int a, int b, int c, int d)
        {
            return MatchTwo(a, b) && MatchTwo(a, c) && MatchTwo(a, d) && MatchTwo(b, c) && MatchTwo(b, d) && MatchTwo(c, d);
        }

        static public bool IsValidNumber(double d) {
            return !(Double.IsNaN(d) || Double.IsPositiveInfinity(d) || Double.IsNegativeInfinity(d) || Double.IsInfinity(d));
        }

        public bool PointsAreAdjacent(int x1, int y1, int x2, int y2, double distance = 1.0)
        {
            int distanceBetweenPoints = (int)Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
            return (distanceBetweenPoints <= distance);
        }

        static private bool TestRange(double numberToCheck, double bottom, double top)
        {
            if (!IsValidNumber(numberToCheck)) return false;
            return (numberToCheck >= bottom && numberToCheck <= top);
        }

        public bool GetPixel(int x, int y)
        {
            System.Drawing.Color pixel = bitmap.GetPixel(x, y);
            if (pixel.R == 0 && pixel.G == 0 && pixel.B == 0) return true;
            return false;
        }



        private static byte[] LoadImageData(string filePath)
        {
            byte[] imageBytes;
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var br = new BinaryReader(fs);
            imageBytes = br.ReadBytes((int)fs.Length);
            br.Close();
            fs.Close();

            return imageBytes;

        }
        internal byte[] GetEncodedImageData(ImageSource image, string preferredFormat)

        {

            byte[] result = null;

            BitmapEncoder encoder = null;

            switch (preferredFormat.ToLower())

            {

                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;

                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;
                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;
                case ".tif":
                case ".tiff":
                    encoder = new TiffBitmapEncoder();
                    break;
                case ".gif":
                    encoder = new GifBitmapEncoder();
                    break;
                case ".wmp":
                    encoder = new WmpBitmapEncoder();
                    break;
            }
            if (image is BitmapSource)
            {
                using (var stream = new MemoryStream())
                {
                    encoder.Frames.Add(BitmapFrame.Create(image as BitmapSource));
                    encoder.Save(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    result = new byte[stream.Length];
                    var br = new BinaryReader(stream);
                    br.Read(result, 0, (int)stream.Length);
                }
            }

            return result;

        }

        private static ImageSource CreateImage(byte[] imageData, int decodePixelWidth = 0, int decodePixelHeight = 0)
        {
            if (imageData == null) return null;

            var result = new BitmapImage();
            result.BeginInit();
            if (decodePixelWidth > 0)
            {
                result.DecodePixelWidth = decodePixelWidth;
            }

            if (decodePixelHeight > 0)
            {
                result.DecodePixelHeight = decodePixelHeight;
            }

            result.StreamSource = new MemoryStream(imageData);
            result.CreateOptions = BitmapCreateOptions.None;
            result.CacheOption = BitmapCacheOption.Default;
            result.EndInit();

            return result;

        }


    }
}
