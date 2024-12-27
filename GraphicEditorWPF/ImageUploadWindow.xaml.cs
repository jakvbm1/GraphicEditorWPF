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
            float[,] filter = generateFilter();
            Bitmap converted = BitmapImage2Bitmap(bip);

            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(0, 0, converted.Width, converted.Height);//System.Drawing
            BitmapData bmpData = converted.LockBits(rectangle, ImageLockMode.ReadWrite, converted.PixelFormat);//System.Drawing.Imag
            Image<Bgr, Byte> prefiltered = new Image<Bgr, Byte>(converted.Width, converted.Height, bmpData.Stride, bmpData.Scan0);

            ConvolutionKernelF kernel = new ConvolutionKernelF(filter);
            Image<Bgr, float> filtered = prefiltered.Convolution(kernel);
            Bitmap temp = filtered.ToBitmap();
            this.bip = Bitmap2BitmapImage(temp);
            ImageSpace.Source = bip;



        }

        private float[,] generateFilter()
        {
            float[,] filter = new float[3, 3];

            try
            {
                filter[0, 0] = float.Parse(Matrix11.Text);
                filter[0, 1] = float.Parse(Matrix12.Text);
                filter[0, 2] = float.Parse(Matrix13.Text);

                filter[1, 0] = float.Parse(Matrix21.Text);
                filter[1, 1] = float.Parse(Matrix22.Text);
                filter[1, 2] = float.Parse(Matrix23.Text);

                filter[2, 0] = float.Parse(Matrix31.Text);
                filter[2, 1] = float.Parse(Matrix32.Text);
                filter[2, 2] = float.Parse(Matrix33.Text);

                return filter;
            }

            catch(Exception ex) 
            {
                MessageBox.Show("Wprowadzono nieprawidłowe dane, upewnij się, że wprowadzono poprawne liczby");
                for(int i = 0; i < 3;)
                {
                    for (int j = 0; j < 3;)
                        filter[i, j] = 1;
                }
                return filter;
            }
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = bip;
            ((MainWindow)Application.Current.MainWindow).loadedImage.Source = bip;
            ((MainWindow)Application.Current.MainWindow).paintSurface.Children.Add(image);
            ((MainWindow)Application.Current.MainWindow).loadedImage.MouseLeftButtonDown += ((MainWindow)Application.Current.MainWindow).Image_MouseLeftButtonDown;
            ((MainWindow)Application.Current.MainWindow).loadedImage.MouseMove += ((MainWindow)Application.Current.MainWindow).Image_MouseMove;
            ((MainWindow)Application.Current.MainWindow).loadedImage.MouseLeftButtonUp += ((MainWindow)Application.Current.MainWindow).Image_MouseLeftButtonUp;
            this.Close();
        }

        private void ScrapeImageButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    }
