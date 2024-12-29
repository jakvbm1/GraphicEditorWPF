﻿using Emgu.CV.Face;
using Microsoft.Win32;
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

    public partial class MainWindow : Window
    {
        public Image loadedImage = new Image();
        int drawStyle = 1;
        Point? lineStart = null;
        Point currentPoint = new Point();

        private Image draggedImage;
        private bool isDragging = false;
        private Point clickPosition;

        private Color selectedColor = Colors.Black;
        public Color SelectedColor { set { this.selectedColor = value; } get { return selectedColor; } }

        private Ellipse end1 = null; //Ends of line while editing it
        private Ellipse end2 = null;
        private Ellipse selectedEnd = null; //End of the modified line
        public MainWindow()
        {
            InitializeComponent();
            ColorSelector.Fill = new SolidColorBrush(selectedColor);
            ColorSelector.MouseLeftButtonDown += changeColor;
        }

        private void changeColor(object sender, RoutedEventArgs e)
        {
            ColorPickerWindow cpw = new ColorPickerWindow();
            cpw.Show();
            selectedColor = Color.FromRgb(170, 80, 80);
            ColorSelector.Fill = new SolidColorBrush(selectedColor);
        }

        private void ButtonDraw_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 1;
            removeTemporaryObjects();
        }

        private void ButtonPointClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 2;
            removeTemporaryObjects();
        }

        private void ButtonLineClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 3;
            removeTemporaryObjects();
        }

        private void ButtonCircleClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 4;
            removeTemporaryObjects();
        }

        private void ButtonRectangleClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 5;
            removeTemporaryObjects();
        }

        private void ButtonPolygonClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 6;
            removeTemporaryObjects();
        }

        private void ButtonEditLineClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 7;
            removeTemporaryObjects();
        }

        private void ButtonArrowClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 8;
            removeTemporaryObjects();
        }

        private void ButtonRhombusClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 9;
            removeTemporaryObjects();
        }

        private void ButtonTrapeziumClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 10;
            removeTemporaryObjects();
        }

        private void paintSurface_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {

                switch (drawStyle)
                {
                    case 1:
                        Line line = new Line();
                        line.Stroke = new SolidColorBrush(selectedColor);
                        line.X1 = currentPoint.X - window.Width/3.5;
                        line.Y1 = currentPoint.Y;
                        line.X2 = e.GetPosition(this).X - window.Width / 3.5;
                        line.Y2 = e.GetPosition(this).Y;
                        currentPoint = e.GetPosition(this);
                        paintSurface.Children.Add(line);
                        break;


                    case 7:
                        if (selectedEnd != null)
                        {
                        Canvas.SetTop(selectedEnd, e.GetPosition(this).Y - 3);
                        Canvas.SetLeft(selectedEnd, e.GetPosition(this).X - window.Width / 3.5 - 3);
                        Line parentLine = (Line)selectedEnd.Tag;
                        if (selectedEnd.Stroke == SystemColors.WindowFrameBrush)
                            {
                                parentLine.X1 = e.GetPosition(this).X - window.Width / 3.5;
                                parentLine.Y1 = e.GetPosition(this).Y; 
                            }
                            else if (selectedEnd.Stroke == Brushes.Blue)
                            {
                                parentLine.X2 = e.GetPosition(this).X - window.Width / 3.5;
                                parentLine.Y2 = e.GetPosition(this).Y;
                            }
                        }
                        break;

                }
            }

            else if (e.LeftButton == MouseButtonState.Released)
            {
                if (selectedEnd != null && drawStyle == 7)
                {
                    removeTemporaryObjects();
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
                        ellipse.Stroke = new SolidColorBrush(selectedColor);
                        ellipse.Fill = new SolidColorBrush(selectedColor);
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
                            lineLong.Stroke = new SolidColorBrush(selectedColor);
                            lineLong.X1 = start.X - window.Width / 3.5;
                            lineLong.Y1 = start.Y;
                            lineLong.X2 = e.GetPosition(this).X - window.Width / 3.5;
                            lineLong.Y2 = e.GetPosition(this).Y;
                            lineLong.MouseLeftButtonDown += Line_MouseLeftButtonDown;
                            paintSurface.Children.Add(lineLong);
                            lineStart = null;
                           
                        }
                        break;
                    }

                case 4:
                    {
                        Ellipse ellipse = new Ellipse();
                        ellipse.Stroke = new SolidColorBrush(selectedColor);
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
                        rect.Stroke = new SolidColorBrush(selectedColor);
                        Canvas.SetTop(rect, e.GetPosition(this).Y - 20);
                        Canvas.SetLeft(rect, e.GetPosition(this).X - window.Width / 3.5 - 30);

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

                        polygon.Points = new System.Windows.Media.PointCollection() { p1, p2, p3, p4, p5, p6 };
                        Brush brushColor = new SolidColorBrush(selectedColor);
                        polygon.Stroke = brushColor;
                        paintSurface.Children.Add(polygon);
                        break;
                    }

                case 8:
                    {
                        if (lineStart is null)
                        {
                            lineStart = e.GetPosition(this);
                        }
                        else
                        {
                            Point start = (Point)lineStart;
                            Line lineLong = new Line
                            {
                                Stroke = new SolidColorBrush(selectedColor),
                                X1 = start.X - window.Width / 3.5,
                                Y1 = start.Y,
                                X2 = e.GetPosition(this).X - window.Width / 3.5,
                                Y2 = e.GetPosition(this).Y
                            };
                            paintSurface.Children.Add(lineLong);
                            lineStart = null;

                            var nstart = new Point(start.X - window.Width / 3.5, start.Y);
                            var end = new Point(lineLong.X2, lineLong.Y2);
                            var direction = end - nstart;
                            direction.Normalize();
                            var arrowBase = end - direction * 10;
                            var normal = new Vector(-direction.Y, direction.X) * 2.5;

                            var arrowHead = new PathFigure
                            {
                                StartPoint = end,
                                Segments = new PathSegmentCollection
        {
            new LineSegment(arrowBase + normal, true),
            new LineSegment(arrowBase - normal, true),
            new LineSegment(end, true)
        }
                            };

                            Path path = new Path
                            {
                                Stroke = new SolidColorBrush(selectedColor),
                                StrokeThickness = 2
                            };

                            PathGeometry pg = new PathGeometry();
                            pg.Figures.Add(arrowHead);
                            path.Data = pg;

                            paintSurface.Children.Add(path);
                        }

                        break;
                    }


                case 9:
                    {
                        Polygon polygon = new Polygon();
                        double mouseX = e.GetPosition(this).X - window.Width / 3.5;
                        double mouseY = e.GetPosition(this).Y;

                        double polygonSize = 30;

                        Point p1 = new Point(mouseX -1.5*polygonSize , mouseY + Math.Sqrt(3) * polygonSize -0.5*polygonSize);
                        Point p2 = new Point(mouseX + 0.5* polygonSize, mouseY + Math.Sqrt(3) * polygonSize - 0.5 * polygonSize);
                        Point p3 = new Point(mouseX + 1.5 * polygonSize, mouseY - 0.5 * polygonSize);
                        Point p4 = new Point(mouseX - 0.5*polygonSize, mouseY - 0.5 * polygonSize);


                        polygon.Points = new System.Windows.Media.PointCollection() { p1, p2, p3, p4 };
                        Brush brushColor = new SolidColorBrush(selectedColor);
                        polygon.Stroke = brushColor;
                        paintSurface.Children.Add(polygon);

                        break;
                    }


                case 10:
                    {
                        Polygon polygon = new Polygon();
                        double mouseX = e.GetPosition(this).X - window.Width / 3.5;
                        double mouseY = e.GetPosition(this).Y;

                        double polygonSize = 30;

                        Point p1 = new Point(mouseX - 1.5 * polygonSize, mouseY + Math.Sqrt(3) * polygonSize - 0.5 * polygonSize);
                        Point p2 = new Point(mouseX + 2.5 * polygonSize, mouseY + Math.Sqrt(3) * polygonSize - 0.5 * polygonSize);
                        Point p3 = new Point(mouseX + 1.5 * polygonSize, mouseY - 0.5 * polygonSize);
                        Point p4 = new Point(mouseX - 0.5 * polygonSize, mouseY - 0.5 * polygonSize);


                        polygon.Points = new System.Windows.Media.PointCollection() { p1, p2, p3, p4 };
                        Brush brushColor = new SolidColorBrush(selectedColor);
                        polygon.Stroke = brushColor;
                        paintSurface.Children.Add(polygon);

                        break;
                    }
            }
        }

        private void Line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (drawStyle == 7)
            {
                end1 = new Ellipse();
                end1.Stroke = SystemColors.WindowFrameBrush;
                end1.Fill = SystemColors.WindowFrameBrush;
                end1.Width = 6;
                end1.Height = 6;
                end1.Tag = (Line)sender;
                Canvas.SetTop(end1, ((Line)sender).Y1 - 3);
                Canvas.SetLeft(end1, ((Line)sender).X1 - 3);
                paintSurface.Children.Add(end1);

                end2 = new Ellipse();
                end2.Stroke = Brushes.Blue;
                end2.Fill = Brushes.Blue;
                end2.Width = 6;
                end2.Height = 6;
                end2.Tag = (Line)sender;
                Canvas.SetTop(end2, ((Line)sender).Y2 - 3);
                Canvas.SetLeft(end2, ((Line)sender).X2 - 3);

                paintSurface.Children.Add(end2);
                end1.MouseLeftButtonDown += LineEndSelected;
                end2.MouseLeftButtonDown += LineEndSelected;
            }
        }

        private void LineEndSelected(object sender, MouseButtonEventArgs e)
        {
            selectedEnd = (Ellipse)sender;
        }

        private void removeTemporaryObjects()
        {
            if(end1 != null)
            {
                paintSurface.Children.Remove(end1);
            }
            if (end2 != null)
            {
                paintSurface.Children.Remove(end2);
            }
            selectedEnd = null;
        }

        private void paintSurface_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) 
            {
             currentPoint =   e.GetPosition(this);
            }
        }

        private void ButtonUploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.bmp) | *.jpg; *.jpeg; *.png; *.bmp";
            if (od.ShowDialog() == true) 
            {
                Uri fileUri = new Uri(od.FileName);
                loadedImage.Source = new BitmapImage(fileUri);

                Canvas.SetTop(loadedImage, 200);
                Canvas.SetLeft(loadedImage, 200);
                //paintSurface.Children.Add(loadedImage);

                var iuw = new ImageUploadWindow(new BitmapImage(fileUri));
                iuw.Show();
            }
        }

        public void uploadImage(BitmapImage bp)
        {
            Image uploaded = new Image();
            uploaded.Source = bp;

            paintSurface.Children.Add(uploaded);
            uploaded.MouseLeftButtonDown += ((MainWindow)Application.Current.MainWindow).Image_MouseLeftButtonDown;
            uploaded.MouseMove += ((MainWindow)Application.Current.MainWindow).Image_MouseMove;
            uploaded.MouseLeftButtonUp += ((MainWindow)Application.Current.MainWindow).Image_MouseLeftButtonUp;
        }

        public void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (drawStyle == 7)
            {
                isDragging = true;
                draggedImage = sender as Image;
                clickPosition = e.GetPosition(paintSurface);
                draggedImage.CaptureMouse();
            }
        }

        public void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && drawStyle == 7)
            {
                Point currentPosition = e.GetPosition(paintSurface);
                double offsetX = currentPosition.X - clickPosition.X;
                double offsetY = currentPosition.Y - clickPosition.Y;

                double newLeft = Canvas.GetLeft(loadedImage) + offsetX;
                double newTop = Canvas.GetTop(loadedImage) + offsetY;
                Canvas.SetLeft(draggedImage, e.GetPosition(this).X - window.Width / 3.5);
                Canvas.SetTop(draggedImage, e.GetPosition(this).Y);

                clickPosition = currentPosition;
            }
        }

        public void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            isDragging = false;
            if (draggedImage != null && drawStyle == 7)
            {
                draggedImage.ReleaseMouseCapture();
                draggedImage = null;
            }
            }
    }
}
