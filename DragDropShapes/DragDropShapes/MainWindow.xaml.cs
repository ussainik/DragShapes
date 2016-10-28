using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DragDropShapes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        private Grid grid = null;
        private UserDialog dialog = null;
        private double x1, y1, x2, y2;
        private double c1, c2;
        Shape result = null;
        Grid shapetoDelete = null;
        public List<Function> strList { get; set; }
        
        bool IsEdit = false;
        
        public MainWindow()
        {
            InitializeComponent();
            strList = new List<Function>();
            strList = PopulateAllFunctions();
        }

        //public IEnumerable<Function> SelectedItems
        //{
        //    get { return strList.Where(o => o.IsSelected); }
        //}

        private ListBoxItem draggedItem;
        private ICommand _okCommand;
        private ICommand _okOnFunctionCommand;
        
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                RaisePropertyChanged("Text");
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new DragDropShapes.RelayCommand<object>(new Action<object>(ShowMessage));
                }
                return _okCommand;
            }
        }

        public ICommand OkFunctionCommand
        {
            get
            {
                if (_okOnFunctionCommand== null)
                {
                    _okOnFunctionCommand = new DragDropShapes.RelayCommand<object>(new Action<object>(FindSelectedFunction));
                }
                return _okOnFunctionCommand;
            }
        }

        

        public List<Function> PopulateAllFunctions()
        {
            List<Function> results = new List<Function>();
            results.Add(new Function()
            {
                NameSpace = "Something",
                FunctionName = "FunctionA"
            });
            results.Add(new Function()
            {
                NameSpace = "SomethingElse",
                FunctionName = "FunctionB"
            });
            results.Add(new Function()
            {
                NameSpace = "SomethingElse1",
                FunctionName = "FunctionC"
            });
            results.Add(new Function()
            {
                NameSpace = "SomethingElse2",
                FunctionName = "FunctionC"
            });
            return results;
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedItem = sender as ListBoxItem;

            if (draggedItem != null)
            {
                DragDrop.DoDragDrop(draggedItem, draggedItem.Content, DragDropEffects.Copy);
            }

        }


        private void CvsSurface_OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void CvsSurface_OnDrop(object sender, DragEventArgs e)
        {
            result = null;
            Object droppedData = e.Data; //This part is not important

            /*Translate Drop Point in reference to Stack Panel*/
            Point dropPoint = e.GetPosition(this.cvsSurface);

            //Console.WriteLine(dropPoint);
            //Label lbl = new Label();
            //lbl.Content = draggedItem.Content;
            UIElement element = draggedItem.Content as UIElement;
            Shape s = element as Shape;
            if (s is Ellipse)
            {
                Ellipse ellipse = new Ellipse()
                {
                    Height = s.Height,
                    Width = s.Width,
                    Fill = s.Fill,
                    Focusable = true
                };
                result = ellipse;
            }
            else if (s is Rectangle)
            {
                Rectangle rectangle = new Rectangle()
                {
                    Name = "rct",
                    Height = s.Height,
                    Width = s.Width,
                    Fill = s.Fill,
                    Focusable = true
                };
                result = rectangle;
            }
            else if (s is Diamond)
            {
                Diamond rhombus = new Diamond()
                {
                    Height = s.Height,
                    Width = s.Width,
                    Fill = s.Fill,
                    Focusable = true
                };
                result = rhombus;
            }
            grid = new Grid();
            //grid.Height = result.Height;
            //grid.Width = result.Width;
            grid.Children.Add(result);
            grid.MouseLeftButtonDown += Sp_MouseLeftButtonDown;
            grid.MouseLeftButtonUp += Sp_MouseLeftButtonUp;

            c1 = 0;
            c2 = 0;
            cvsSurface.Children.Add(grid);
            Canvas.SetLeft(grid, dropPoint.X);
            Canvas.SetTop(grid, dropPoint.Y);

        }



        private void Sp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsEdit = false;
            var panel = sender as Grid;
            var height = panel.Height;
            var width = panel.Width;
            //IsLine = false;
            Panel.SetZIndex(panel, 20);
            if (e.ClickCount == 2)
            {
                return;
            }
            bool mouserelease = System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed;

            if (!mouserelease)
            {
                
                x2 = e.GetPosition(this.cvsSurface).X;
                y2 = e.GetPosition(this.cvsSurface).Y;
                Point P1 = new Point();
                P1.X = x1;
                P1.Y = y1;
                Point P2 = new Point();
                P2.X = x2;
                P2.Y = y2;
                if (x1 == x2)
                {
                    return;
                }
                result = DrawLinkArrow(P1, P2);
                grid = new Grid();
                grid.Children.Add(result);
                cvsSurface.Children.Add(grid);
                grid.MouseLeftButtonDown += Grid_MouseLeftButtonDown;

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsEdit = false;
            if (shapetoDelete == null)
            {
                MessageBox.Show("Please select a shape");
            }
            else
            {
                cvsSurface.Children.Remove(shapetoDelete);
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            IsEdit = true;
            if (shapetoDelete == null)
            {
                MessageBox.Show("Please select a shape");
                return;
            }
            else
            {
                var tbl = FindChild<TextBlock>(shapetoDelete, "tblMessage");
                if (tbl != null)
                {
                    Text = tbl.Text;
                    //shapetoDelete.Children.Remove(tbl);
                }
                dialog = new UserDialog()
                {
                    DataContext = this,
                    Height = 180,
                    Width = 400,
                    MaxHeight = 180,
                    MaxWidth = 400
                };
                dialog.ShowDialog();
            }
            //var tbtext = FindChild<TextBlock>(shapetoDelete, "tblMessage");
            //if (tbtext != null)
            //{
            //    shapetoDelete.Children.Remove(tbtext);
            //}
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //var childpart = sender as Grid;
            IsEdit = false;
            if (e.ClickCount == 2)
            {
                dialog = new UserDialog()
                {
                    DataContext = this,
                    Height = 180,
                    Width = 400,
                    MaxHeight = 180,
                    MaxWidth = 400
                };
                //c1 = Canvas.GetLeft(sender as Grid);
                //c2 = Canvas.GetTop(sender as Grid);
                c1 = e.GetPosition(sender as Grid).X;
                c2 = e.GetPosition(sender as Grid).Y;
                dialog.ShowDialog();

            }
            else
            {
                (sender as Grid).Focus();
                shapetoDelete = new Grid();
                shapetoDelete = sender as Grid;
            }
        }

       


        private void Sp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            grid = sender as Grid;
            IsEdit = false;
            var height = grid.Height;
            var width = grid.Width;
            Panel.SetZIndex(grid, 20);
            if (e.ClickCount == 2)
            {
                c1 = 0;
                c2 = 0;
                var rectangle = FindChild<Rectangle>(grid, "rct");
                if (rectangle != null)
                {
                    RectangleFunctions rfunctions = new RectangleFunctions()
                    {
                        DataContext = this,
                        Height = 280,
                        Width = 300,
                        MaxHeight = 280,
                        MaxWidth = 300
                    };
                    rfunctions.ShowDialog();
                    return;
                }
                dialog = new UserDialog()
                {
                    DataContext = this,
                    Height = 180,
                    Width = 400,
                    MaxHeight = 180,
                    MaxWidth = 400
                };
                dialog.ShowDialog();
            }
            else
            {
                grid.Focus();
                x1 = e.GetPosition(this.cvsSurface).X;
                y1 = e.GetPosition(this.cvsSurface).Y;
                
                shapetoDelete = new Grid();
                shapetoDelete = sender as Grid;
            }
        }

        private void ShowMessage(object obj)
        {

            if (grid != null)
            {
                if (IsEdit)
                {
                    var tbtext = FindChild<TextBlock>(shapetoDelete, "tblMessage");
                    if (tbtext != null)
                    {
                        shapetoDelete.Children.Remove(tbtext);
                    }
                }

                TextBlock tbl = new TextBlock()
                    {
                        Name = "tblMessage",
                        Text = obj.ToString(),
                        FontSize = 12,

                    };
                    //Panel.SetZIndex(tbl, 20);
                    if (c1 == 0 && c2 == 0)
                    {
                        tbl.HorizontalAlignment = HorizontalAlignment.Center;
                        tbl.VerticalAlignment = VerticalAlignment.Center;
                        grid.Children.Add(tbl);
                    }
                    else
                    {

                    //grid.Margin = new Thickness(0, 5, 0, 0);
                    tbl.Margin = new Thickness(c1 + 4, c2 + 6, 0, 0);
                    cvsSurface.Children.Add(tbl);
                    }
                    dialog.Close();
              
            }

        }

        private void FindSelectedFunction(object obj)
        {
            //var selectedResults = (List<object>)obj;
            foreach (var f in (obj as System.Collections.IList).Cast<Function>())
            {
                MessageBox.Show(f.FunctionName);
            }
        }

        private static Shape DrawLinkArrow(Point p1, Point p2)
        {
            GeometryGroup lineGroup = new GeometryGroup();
            double theta = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            Point p = new Point(p1.X + ((p2.X - p1.X) / 1.35), p1.Y + ((p2.Y - p1.Y) / 1.35));
            pathFigure.StartPoint = p;

            Point lpoint = new Point(p.X + 6, p.Y + 15);
            Point rpoint = new Point(p.X - 6, p.Y + 15);
            LineSegment seg1 = new LineSegment();
            seg1.Point = lpoint;
            pathFigure.Segments.Add(seg1);

            LineSegment seg2 = new LineSegment();
            seg2.Point = rpoint;
            pathFigure.Segments.Add(seg2);

            LineSegment seg3 = new LineSegment();
            seg3.Point = p;
            pathFigure.Segments.Add(seg3);

            pathGeometry.Figures.Add(pathFigure);
            RotateTransform transform = new RotateTransform();
            transform.Angle = theta + 90;
            transform.CenterX = p.X;
            transform.CenterY = p.Y;
            pathGeometry.Transform = transform;
            lineGroup.Children.Add(pathGeometry);

            LineGeometry connectorGeometry = new LineGeometry();
            connectorGeometry.StartPoint = p1;
            connectorGeometry.EndPoint = p2;
            lineGroup.Children.Add(connectorGeometry);
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            path.Data = lineGroup;
            path.StrokeThickness = 2;
            path.Stroke = path.Fill = Brushes.Black;
            path.Focusable = true;

            return path;
        }


        public static T FindChild<T>(DependencyObject parent, string childName)
   where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
