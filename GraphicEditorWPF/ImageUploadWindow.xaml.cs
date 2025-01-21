using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Shapes;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Interop;

namespace GraphicEditorWPF
{

    public partial class ImageUploadWindow : Window
    {
        BitmapImage bip;
        public ImageUploadWindow(BitmapImage bi)
        {
            this.bip = bi;
            InitializeComponent();
            ImageSpace.Source = bi;
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            Bitmap converted = BitmapImage2Bitmap(bip);

            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(0, 0, converted.Width, converted.Height);//System.Drawing
            BitmapData bmpData = converted.LockBits(rectangle, ImageLockMode.ReadWrite, converted.PixelFormat);//System.Drawing.Imaging

            if (FilterSelection.SelectedIndex == 0)
            {

                Image<Bgr, Byte> sobelBip = new Image<Bgr, Byte>(converted.Width, converted.Height, bmpData.Stride, bmpData.Scan0);
                Image<Bgr, float> sobeled = sobelBip.Sobel(0, 1, 3);

                Bitmap temp = sobeled.ToBitmap();
                this.bip = Bitmap2BitmapImage(temp);
                ImageSpace.Source = bip;

            }
        }

        public Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // To make it cross-thread accessible

                return bitmapImage;
            }
        }

        private void MatrixFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                float[,] filter = generateFilter();
                Bitmap converted = BitmapImage2Bitmap(bip);

                // Ensure correct pixel format
                if (converted.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                {
                    Bitmap temp2 = new Bitmap(converted.Width, converted.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    using (Graphics g = Graphics.FromImage(temp2))
                    {
                        g.DrawImage(converted, 0, 0);
                    }
                    converted = temp2;
                }

                System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(0, 0, converted.Width, converted.Height);
                BitmapData bmpData = converted.LockBits(rectangle, ImageLockMode.ReadWrite, converted.PixelFormat);

                // Process image
                Image<Bgr, Byte> prefiltered = new Image<Bgr, Byte>(converted.Width, converted.Height, bmpData.Stride, bmpData.Scan0);
                ConvolutionKernelF kernel = new ConvolutionKernelF(filter);
                Image<Bgr, float> filtered = prefiltered.Convolution(kernel);

                // Normalize pixel values and convert back to Bitmap
                Image<Bgr, Byte> normalized = filtered.Convert<Bgr, Byte>();
                Bitmap temp = normalized.ToBitmap();

                // Unlock image data
                converted.UnlockBits(bmpData);

                this.bip = Bitmap2BitmapImage(temp);
                ImageSpace.Source = bip;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Zastosowanie filtru nie powiodło się, wystąpił błąd: {ex.Message}");
            }
        }


        private float[,] generateFilter()
        {
            float[,] filter = new float[3, 3];

            // A default filter (identity matrix)
            float[,] defaultFilter = {
        { 0, 0, 0 },
        { 0, 1, 0 },
        { 0, 0, 0 }
    };

            try
            {
                bool valid = true;

                valid &= float.TryParse(Matrix11.Text, out filter[0, 0]);
                valid &= float.TryParse(Matrix12.Text, out filter[0, 1]);
                valid &= float.TryParse(Matrix13.Text, out filter[0, 2]);

                valid &= float.TryParse(Matrix21.Text, out filter[1, 0]);
                valid &= float.TryParse(Matrix22.Text, out filter[1, 1]);
                valid &= float.TryParse(Matrix23.Text, out filter[1, 2]);

                valid &= float.TryParse(Matrix31.Text, out filter[2, 0]);
                valid &= float.TryParse(Matrix32.Text, out filter[2, 1]);
                valid &= float.TryParse(Matrix33.Text, out filter[2, 2]);

                if (!valid)
                {
                    throw new FormatException("Wykryto niepoprawne dane.");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Wprowadzono niepoprawne dane, upewnij się, że w każdej komórce macierzy znajduje się poprawna liczba .");
                filter = defaultFilter;
            }

            return filter;
        }


        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).uploadImage(bip);
            //System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            //image.Source = bip;
            //((MainWindow)Application.Current.MainWindow).loadedImage.Source = bip;
            //((MainWindow)Application.Current.MainWindow).paintSurface.Children.Add(image);
            //((MainWindow)Application.Current.MainWindow).loadedImage.MouseLeftButtonDown += ((MainWindow)Application.Current.MainWindow).Image_MouseLeftButtonDown;
            //((MainWindow)Application.Current.MainWindow).loadedImage.MouseMove += ((MainWindow)Application.Current.MainWindow).Image_MouseMove;
            //((MainWindow)Application.Current.MainWindow).loadedImage.MouseLeftButtonUp += ((MainWindow)Application.Current.MainWindow).Image_MouseLeftButtonUp;
            this.Close();
        }

        private void ScrapeImageButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    }
