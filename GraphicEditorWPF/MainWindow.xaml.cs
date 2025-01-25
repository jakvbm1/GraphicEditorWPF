
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Serialization;



namespace GraphicEditorWPF
{

    public partial class MainWindow : Window
    {
        public Image loadedImage = new Image();
        int drawStyle = 1;
        Point? lineStart = null;
        Point currentPoint = new Point();
        int lineThickness = 5;

        private List<Layer> layers = new List<Layer>();
        private int activeLayerIndex = 0;

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
            InitializeLayers();
            LineThicknessDisplayer.Text = lineThickness.ToString();
            ColorSelector.Fill = new SolidColorBrush(selectedColor);
            ColorSelector.MouseLeftButtonDown += changeColor;
        }

        private void InitializeLayers()
        {
            layers.Add(new Layer("0 (tło)"));
            layers.Add(new Layer("1"));

            foreach (var layer in layers)
            {
                paintSurface.Children.Add(layer.LayerCanvas);
            }

            LayerList.ItemsSource = layers;
            LayerList.SelectedIndex = 0;
        }


        private void LayerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            activeLayerIndex = LayerList.SelectedIndex;
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

        private void ButtonDrawBrokenLine(object sender, RoutedEventArgs e)
        {
            drawStyle=11;
            removeTemporaryObjects();
        }

        private void EraserButtonClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 12;
            removeTemporaryObjects();
        }

        private void ButtonDrawStarClick(object sender, RoutedEventArgs e)
        {
            drawStyle = 13;
            removeTemporaryObjects();
        }
        private void AddNewLayer(string name)
        {
            var newLayer = new Layer(name);
            layers.Add(newLayer);
            paintSurface.Children.Add(newLayer.LayerCanvas);
            LayerList.Items.Refresh();
        }

        private void DeleteLayer(int index)
        {
            if (index >= 0 && index < layers.Count && layers.Count > 1)
            {
                paintSurface.Children.Remove(layers[index].LayerCanvas);
                layers.RemoveAt(index);
                LayerList.Items.Refresh();

                if (layers.Count > 0)
                {
                    activeLayerIndex = Math.Min(index, layers.Count - 1);
                    LayerList.SelectedIndex = activeLayerIndex;
                }
                else
                {
                    activeLayerIndex = -1;
                }
            }
        }




        private void PngSaveClick(object sender, RoutedEventArgs e)
        {
            // Create a SaveFileDialog to choose the save location and filename
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg",
                DefaultExt = "png",
                AddExtension = true
            };

            // Show the dialog and get the result
            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string filename = saveFileDialog.FileName;
                BitmapEncoder encoder;

                if (System.IO.Path.GetExtension(filename).ToLower() == ".jpg")
                {
                    encoder = new JpegBitmapEncoder();
                }
                else
                {
                    encoder = new PngBitmapEncoder();
                }

                // Set the canvas size if not already set
                if (double.IsNaN(paintSurface.Width) || double.IsNaN(paintSurface.Height))
                {
                    paintSurface.Width = paintSurface.ActualWidth;
                    paintSurface.Height = paintSurface.ActualHeight;
                }

                // Create a render bitmap for saving the layers
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                    (int)paintSurface.Width, (int)paintSurface.Height, 96d, 96d, PixelFormats.Pbgra32);

                // Use a DrawingVisual to render the layers onto the renderBitmap
                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    foreach (UIElement element in paintSurface.Children)
                    {
                        if (element is Visual visualElement)
                        {
                            // Render each layer individually
                            drawingContext.DrawRectangle(
                                new VisualBrush(visualElement),
                                null,
                                new Rect(
                                    new Point(Canvas.GetLeft(element), Canvas.GetTop(element)),
                                    new Size(element.RenderSize.Width, element.RenderSize.Height)
                                ));
                        }
                    }
                }
                renderBitmap.Render(drawingVisual);

                // Create a file stream for saving the image
                using (FileStream fileStream = new FileStream(filename, FileMode.Create))
                {
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                    encoder.Save(fileStream);
                }

                MessageBox.Show("Obraz zapisany!", "Save Image", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
                        line.StrokeThickness = lineThickness;
                        line.X1 = currentPoint.X - window.Width/3.5;
                        line.Y1 = currentPoint.Y;
                        line.X2 = e.GetPosition(this).X - window.Width / 3.5;
                        line.Y2 = e.GetPosition(this).Y;
                        currentPoint = e.GetPosition(this);
                        layers[activeLayerIndex].LayerCanvas.Children.Add(line);
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

                    case 12:
                        {
                            var clickedElement = e.Source as FrameworkElement;

                            if (clickedElement != null)
                            {
                                if (layers[activeLayerIndex].LayerCanvas.Children.Contains(clickedElement))
                                {
                                    layers[activeLayerIndex].LayerCanvas.Children.Remove(clickedElement);
                                }
                            }

                            break;
                        }


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
                        layers[activeLayerIndex].LayerCanvas.Children.Add(ellipse);
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
                            layers[activeLayerIndex].LayerCanvas.Children.Add(lineLong);
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
                        layers[activeLayerIndex].LayerCanvas.Children.Add(ellipse);
                        break;
                    }

                    case 5:
                    {
                        var rect = new Rectangle();
                        rect.Width=60; rect.Height=40;
                        rect.Stroke = new SolidColorBrush(selectedColor);
                        Canvas.SetTop(rect, e.GetPosition(this).Y - 20);
                        Canvas.SetLeft(rect, e.GetPosition(this).X - window.Width / 3.5 - 30);

                        layers[activeLayerIndex].LayerCanvas.Children.Add((rect));
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
                        layers[activeLayerIndex].LayerCanvas.Children.Add(polygon);
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
                            layers[activeLayerIndex].LayerCanvas.Children.Add(lineLong);
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

                            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path
                            {
                                Stroke = new SolidColorBrush(selectedColor),
                                StrokeThickness = 2
                            };

                            PathGeometry pg = new PathGeometry();
                            pg.Figures.Add(arrowHead);
                            path.Data = pg;

                            layers[activeLayerIndex].LayerCanvas.Children.Add(path);
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
                        layers[activeLayerIndex].LayerCanvas.Children.Add(polygon);

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
                        layers[activeLayerIndex].LayerCanvas.Children.Add(polygon);

                        break;
                    }

                case 11:
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
                            layers[activeLayerIndex].LayerCanvas.Children.Add(lineLong);
                            lineStart = e.GetPosition(this);

                        }
                        break;
                    }

                case 12:
                    {
                        var clickedElement = e.Source as FrameworkElement;
                        
                        if (clickedElement != null)
                        {
                           if(layers[activeLayerIndex].LayerCanvas.Children.Contains(clickedElement))
                            {
                                layers[activeLayerIndex].LayerCanvas.Children.Remove(clickedElement);
                            }
                        }

                        break;
                    }

                case 13:
                    {

                        Polygon polygon = new Polygon();

                        // Get mouse position relative to the canvas
                        double mouseX = e.GetPosition(this).X - window.Width / 3.5;
                        double mouseY = e.GetPosition(this).Y;

                        // Define the size of the star
                        double polygonSize = 30; // Adjust size as needed

                        // Calculate the points for the star
                        Point p1 = new Point(mouseX, mouseY - polygonSize); // Top point
                        Point p2 = new Point(mouseX + 0.363 * polygonSize, mouseY - 0.309 * polygonSize); // Top-right inner
                        Point p3 = new Point(mouseX + 0.951 * polygonSize, mouseY - 0.309 * polygonSize); // Right outer
                        Point p4 = new Point(mouseX + 0.363 * polygonSize, mouseY + 0.118 * polygonSize); // Bottom-right inner
                        Point p5 = new Point(mouseX + 0.588 * polygonSize, mouseY + 0.809 * polygonSize); // Bottom-right outer
                        Point p6 = new Point(mouseX, mouseY + 0.382 * polygonSize); // Bottom inner
                        Point p7 = new Point(mouseX - 0.588 * polygonSize, mouseY + 0.809 * polygonSize); // Bottom-left outer
                        Point p8 = new Point(mouseX - 0.363 * polygonSize, mouseY + 0.118 * polygonSize); // Bottom-left inner
                        Point p9 = new Point(mouseX - 0.951 * polygonSize, mouseY - 0.309 * polygonSize); // Left outer
                        Point p10 = new Point(mouseX - 0.363 * polygonSize, mouseY - 0.309 * polygonSize); // Top-left inner

                        // Assign points to the polygon
                        polygon.Points = new System.Windows.Media.PointCollection() { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10 };

                        // Set stroke and fill for the star
                        Brush brushColor = new SolidColorBrush(selectedColor); // Assuming selectedColor is defined
                        polygon.Stroke = brushColor;
                        

                        // Add the star to the active layer
                        layers[activeLayerIndex].LayerCanvas.Children.Add(polygon);
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
            lineStart = null;
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

            layers[activeLayerIndex].LayerCanvas.Children.Add(uploaded);
            uploaded.MouseLeftButtonDown += ((MainWindow)Application.Current.MainWindow).Image_MouseLeftButtonDown;
            uploaded.MouseMove += ((MainWindow)Application.Current.MainWindow).Image_MouseMove;
            uploaded.MouseLeftButtonUp += ((MainWindow)Application.Current.MainWindow).Image_MouseLeftButtonUp;
            if (activeLayerIndex >= layers.Count) { activeLayerIndex--; }
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


        private void AddLayerButton_Click(object sender, RoutedEventArgs e)
        {
            int newLayerIndex = layers.Count;
            AddNewLayer(newLayerIndex.ToString());
        }

        private void RemoveLayerButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteLayer(activeLayerIndex);
            LayerList.SelectedIndex = activeLayerIndex;
            LayerList.Items.Refresh();
        }

        private void LayerChanged(object sender, RoutedEventArgs e)
        {
            activeLayerIndex = LayerList.SelectedIndex;
        }

        private void ThickLineButton_Click(object sender, RoutedEventArgs e)
        {
            lineThickness++;
            LineThicknessDisplayer.Text = lineThickness.ToString();

        }

        private void ThinLineButton_Click(object sender, RoutedEventArgs e)
        {
            if (lineThickness > 1) { lineThickness--; }
            LineThicknessDisplayer.Text = lineThickness.ToString();
        }
    }
}
