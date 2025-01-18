using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace GraphicEditorWPF
{
    /// <summary>
    /// Logika interakcji dla klasy ColorPickerWindow.xaml
    /// </summary>
    public partial class ColorPickerWindow : Window
    {
        private bool isUpdatingRGB = false;
        private bool isUpdatingHSV = false;

        public ColorPickerWindow()
        {
            InitializeComponent();
        }
            
     

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Byte redN = Byte.Parse(RedValue.Text);
                Byte blueN = Byte.Parse(BlueValue.Text);
                Byte GreenN = Byte.Parse(GreenValue.Text);


                Color color = Color.FromRgb(redN, GreenN, blueN);
                ((MainWindow)Application.Current.MainWindow).SelectedColor = color;
                ((MainWindow)Application.Current.MainWindow).ColorSelector.Fill = new SolidColorBrush(color);
            }
            catch
            {
                MessageBox.Show("Wprowadzono nieprawidłowe dane, upewnij się, że podane liczby są z zakresu od 0 do 255");
            }
            finally { this.Close(); }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RGBtoHSV(object sender, TextChangedEventArgs e)
        {
            if (RedValue != null && GreenValue != null && BlueValue != null && SaturationValue != null && HueValue != null && ValueValue != null)
            {
                if (!isUpdatingRGB)
                {
                    isUpdatingHSV = true;
                    try
                    {
                        double rp = (double)int.Parse(RedValue.Text) / 255;
                        double gp = (double)int.Parse(GreenValue.Text) / 255;
                        double bp = (double)int.Parse(BlueValue.Text) / 255;

                        double cMax = Math.Max(Math.Max(rp, gp), bp);
                        double cMin = Math.Min(Math.Min(rp, gp), bp);

                        double delta = cMax - cMin;

                        double hue = 0;

                        if (cMax == rp)
                        {
                            hue = 60 * (((gp - bp) / delta) % 6);
                        }
                        else if (cMax == gp)
                        {
                            hue = 60 * (((bp - rp) / delta) + 2);
                        }
                        else
                        {
                            hue = 60 * (((rp - gp) / delta) + 4);
                        }

                        double saturation = 0;
                        if (cMax > 0)
                        {
                            saturation = delta / cMax;
                        }
                        double value = cMax;

                        SaturationValue.Text = saturation.ToString();
                        HueValue.Text = hue.ToString();
                        ValueValue.Text = value.ToString();
                        updateColorDisplayer();
                    }
                    catch
                    {
                        MessageBox.Show("Wprowadzono nieprawidłowe dane, upewnij się, że wprowadzono poprawne liczby całkowite");
                    }
                    finally
                    {
                        isUpdatingHSV = false;
                    }
                }
            }
        }


        private void HSVtoRGB(object sender, TextChangedEventArgs e)
        {
            if (RedValue != null && GreenValue != null && BlueValue != null && SaturationValue != null && HueValue != null && ValueValue != null)
            {
                if (!isUpdatingHSV)
                {
                    isUpdatingRGB = true;
                    try
                    {
                        double hue = double.Parse(HueValue.Text) / 60;
                        double sat = double.Parse(SaturationValue.Text);
                        double v = double.Parse(ValueValue.Text);

                        double chroma = sat * v;
                        double x = chroma * (1 - Math.Abs(hue % 2 - 1));

                        double r = 0, g = 0, b = 0;
                        if (hue < 1) { r = chroma; g = x; b = 0; }
                        else if (hue < 2) { r = x; g = chroma; b = 0; }
                        else if (hue < 3) { r = 0; g = chroma; b = x; }
                        else if (hue < 4) { r = 0; g = x; b = chroma; }
                        else if (hue < 5) { r = x; g = 0; b = chroma; }
                        else if (hue < 6) { r = chroma; g = 0; b = x; }

                        double m = v - chroma;

                        RedValue.Text = ((int)((r + m)*255)).ToString();
                        GreenValue.Text = ((int)((g + m)*255)).ToString();
                        BlueValue.Text = ((int)((b + m) * 255)).ToString();
                        updateColorDisplayer();
                    }
                    catch
                    {
                        MessageBox.Show("Wprowadzono nieprawidłowe dane, upewnij się, że wprowadzono poprawne liczby");
                    }
                    finally
                    {
                        isUpdatingRGB = false;
                    }
                }
            }
        }


        private void updateColorDisplayer()
        {
            try
            {
                if (RedValue != null && GreenValue != null && BlueValue != null && colorDisplayer != null) {
                    colorDisplayer.Fill = new SolidColorBrush(Color.FromRgb(byte.Parse(RedValue.Text), byte.Parse(GreenValue.Text), byte.Parse(BlueValue.Text)));
                }
            }
            catch { }
            
        }
    }
}
