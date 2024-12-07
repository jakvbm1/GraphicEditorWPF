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

namespace GraphicEditorWPF
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int drawStyle = 1;
        Point? lineStart = null;
        Point currentPoint = new Point();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonDraw_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 1;
        }

        private void ButtonPointClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 2;
        }

        private void ButtonLineClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 3;
        }

        private void ButtonCircleClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 4;
        }

        private void ButtonRectangleClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 5;
        }

        private void ButtonPolygonClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 6;
        }

        private void ButtonEditLineClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 7;
        }

        private void paintSurface_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {

                switch (drawStyle)
                {
                    case 1:
                        Line line = new Line();
                        line.Stroke = SystemColors.WindowFrameBrush;
                        line.X1 = currentPoint.X - window.Width/3.5;
                        line.Y1 = currentPoint.Y;
                        line.X2 = e.GetPosition(this).X - window.Width / 3.5;
                        line.Y2 = e.GetPosition(this).Y;
                        currentPoint = e.GetPosition(this);
                        paintSurface.Children.Add(line);
                        break;

                    //case 2:
                    //    Ellipse ellipse = new Ellipse();
                    //    ellipse.Stroke = SystemColors.WindowFrameBrush;
                    //    ellipse.Width = 6;
                    //    ellipse.Height = 6;
                    //    Canvas.SetTop(ellipse, e.GetPosition(this).Y - 3);
                    //    Canvas.SetLeft(ellipse, e.GetPosition(this).X- window.Width / 3.5-3);
                    //    paintSurface.Children.Add(ellipse);
                    //    break;

                }
            }
        }

        private void paintSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (drawStyle)
            {
                case 2:
                    {
                        Ellipse ellipse = new Ellipse();
                        ellipse.Stroke = SystemColors.WindowFrameBrush;
                        ellipse.Fill = SystemColors.WindowFrameBrush;
                        ellipse.Width = 6;
                        ellipse.Height = 6;
                        Canvas.SetTop(ellipse, e.GetPosition(this).Y - 3);
                        Canvas.SetLeft(ellipse, e.GetPosition(this).X - window.Width / 3.5 - 3);
                        paintSurface.Children.Add(ellipse);
                        break;
                    }

                    case 3:
                    {
                        if (lineStart is null)
                        { lineStart = e.GetPosition(this); }
                        else
                        {
                            Point start = (Point)lineStart;
                            Line lineLong = new Line();
                            lineLong.Stroke = SystemColors.WindowFrameBrush;
                            lineLong.X1 = start.X - window.Width / 3.5;
                            lineLong.Y1 = start.Y;
                            lineLong.X2 = e.GetPosition(this).X - window.Width / 3.5;
                            lineLong.Y2 = e.GetPosition(this).Y;
                            paintSurface.Children.Add(lineLong);
                            lineStart = null;
                           
                        }
                        break;
                    }

                case 4:
                    {
                        Ellipse ellipse = new Ellipse();
                        ellipse.Stroke = SystemColors.WindowFrameBrush;
                        ellipse.Width = 40;
                        ellipse.Height = 20;
                        Canvas.SetTop(ellipse, e.GetPosition(this).Y - 10);
                        Canvas.SetLeft(ellipse, e.GetPosition(this).X - window.Width / 3.5 - 20);
                        paintSurface.Children.Add(ellipse);
                        break;
                    }

                    case 5:
                    {
                        var rect = new Rectangle();
                        rect.Width=60; rect.Height=40;
                        rect.Stroke = SystemColors.WindowFrameBrush;
                        Canvas.SetTop(rect, e.GetPosition(this).Y - 20);
                        Canvas.SetLeft(rect, e.GetPosition(this).Y - window.Width / 3.5 - 30);

                        Brush brushColor = new SolidColorBrush(Colors.CornflowerBlue);
                        rect.Stroke = brushColor;
                        paintSurface.Children.Add((rect));
                        break;
                    }

                case 6:
                    {
                        Polygon polygon = new Polygon();
                        double mouseX = e.GetPosition(this).X - window.Width/3.5;
                        double mouseY = e.GetPosition(this).Y;

                        double polygonSize = 30;

                        Point p1 = new Point(mouseX - polygonSize, mouseY + 2*polygonSize);
                        Point p2 = new Point(mouseX + polygonSize, mouseY + 2*polygonSize);
                        Point p3 = new Point(mouseX + 2*polygonSize, mouseY + 0);
                        Point p4 = new Point(mouseX + polygonSize, mouseY - polygonSize*2);
                        Point p5 = new Point(mouseX - polygonSize, mouseY - polygonSize*2);
                        Point p6 = new Point(mouseX - 2*polygonSize, mouseY - 0);

                        polygon.Points = new PointCollection() { p1, p2, p3, p4, p5, p6 };
                        Brush brushColor = new SolidColorBrush(Colors.CornflowerBlue);
                        polygon.Stroke = brushColor;
                        paintSurface.Children.Add(polygon);
                        break;

                        break;
                    }
            }
        }

        private void paintSurface_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) 
            {
             currentPoint =   e.GetPosition(this);
            }
        }
    }
}
