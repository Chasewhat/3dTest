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
using System.Windows.Threading;
using BusinessSer;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using Visifire.Charts;
using HelixToolkit.Wpf;
using Microsoft.Win32;

namespace _3DMAX
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑  继承自定义window  详情参见样式引用
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 默认camera Z轴视线
        /// </summary>
        //private double defaultcZ = 3500;
        //private double defaultRate = 1;
        private DispatcherTimer timer;
        //private int flag = 0;
        private Point mouseP;
        private InfoShow show = null;
        private LocationSet locset = null;
        private const string MODEL_PATH = @"Model\i.obj";
        private ModelVisual3D device3D;
        private ControlMode mode = ControlMode.View;

        //委托
        private delegate void SetCameraPosition(PerspectiveCamera cameraN, System.Windows.Media.Media3D.Point3D p);
        SetCameraPosition setCameraPosition;
        private delegate void SetModelColor(GeometryModel3D model, Brush brush);
        SetModelColor setModelColor;
        private delegate void SetFocus(Viewport3D view);
        SetFocus setFocus;

        public MainWindow()
        {
            InitializeComponent();
            setCameraPosition = new SetCameraPosition(SetPosition);
            setModelColor = new SetModelColor(ViewControl.SetColor);
            setFocus = new SetFocus(SetViewportFocus);
            device3D = new ModelVisual3D();
            device3D.Content = Display3d(MODEL_PATH);
            device3D.SetName("worldModel");
            // Add to view port
            viewport.Children.Add(device3D);
        }

        /// <summary>
        /// Display 3D Model
        /// </summary>
        /// <param name="model">Path to the Model file</param>
        /// <returns>3D Model Content</returns>
        private Model3DGroup Display3d(string model)
        {
            Model3DGroup device = null;
            try
            {
                //此处用于设置控制键位  为不影响其他操作 暂时设置为右键控制
                viewport.RotateGesture = new MouseGesture(MouseAction.RightClick);

                //Import 3D model file
                ModelImporter import = new ModelImporter();

                //import. = Materials.Orange;

                //Load the 3D model file
                device = import.Load(model);

            }
            catch (Exception e)
            {
                // Handle exception in case can not file 3D model
                MessageBox.Show("Exception Error : " + e.StackTrace);
            }
            return device;
        }

        private void SetPosition(PerspectiveCamera cameraN, System.Windows.Media.Media3D.Point3D p)
        {
            cameraN.Position = p;
        }
        private void SetViewportFocus(Viewport3D view)
        {
            view.Focus();
        }

        /// <summary>
        /// 鼠标滚轴事件  缩放图形  camera范围0.1倍至2倍
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void OnPreviewMouseWheelShow(object sender, MouseWheelEventArgs e)
        //{
        //    base.OnPreviewMouseWheel(e);

        //    //if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        //    //{
        //    //Point pp = Mouse.GetPosition(viewport);

        //    if (defaultRate > 2 && e.Delta < 0)
        //    {
        //        return;
        //    }

        //    if (defaultRate < 0.1 && e.Delta > 0)
        //    {
        //        return;
        //    }
        //    defaultRate += (e.Delta > 0) ? -0.1 : 0.1;
        //    setCameraPosition(this.camera,
        //        new System.Windows.Media.Media3D.Point3D(this.camera.Position.X,
        //            this.camera.Position.Y, defaultcZ * defaultRate));
        //    //Size _viewportSize = new Size(ContentPanelShow.RenderSize.Width, ContentPanelShow.RenderSize.Height);
        //    //this.viewport.Focus();
        //    //scaleSliderShow.Value += d;
        //    //this.scrollerShow.ScrollToHorizontalOffset(((scrollerShow.HorizontalOffset + pp.X * d) < 0 ? 0 : (scrollerShow.HorizontalOffset + pp.X * d)));
        //    //this.scrollerShow.ScrollToVerticalOffset(((scrollerShow.VerticalOffset + pp.Y * d) < 0 ? 0 : (scrollerShow.VerticalOffset + pp.Y * d)));
        //    //}
        //}

        /// <summary>
        /// window加载事件  启动计时器，事件绑定，生成柱状图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += timer_Tick;
            //timer.Start();
            //defaultcZ = this.camera.Position.Z;
            //this.viewport.Focus();
            this.viewport.MouseLeftButtonDown += ViewPort_MouseLeftButtonDown;
            //defaultX = transform.OffsetX;
            //defaultY = transform.OffsetY;
            //defaultZ = transform.OffsetZ;

        }

        /// <summary>
        /// viewport3D点击事件 左键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewPort_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mouseposition = e.GetPosition(this.viewport);
            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);
            PointHitTestParameters pointparams = new PointHitTestParameters(mouseposition);
            RayHitTestParameters rayparams = new RayHitTestParameters(testpoint3D, testdirection);
            //viewport.f
            //ModelVisual3D v = viewport.FindNearestVisual(mouseposition) as ModelVisual3D;
            //if(v != null)
            //{
            //    string xx = v.GetName();
            //}
            mouseP = mouseposition;
            VisualTreeHelper.HitTest(viewport, null, HTResult, pointparams);

        }

        /// <summary>
        /// 点击命中测试事件
        /// </summary>
        /// <param name="rawresult"></param>
        /// <returns></returns>
        public HitTestResultBehavior HTResult(System.Windows.Media.HitTestResult rawresult)
        {
            RayHitTestResult rayResult = rawresult as RayHitTestResult;
            if (rayResult != null)
            {
                //rayResult.ModelHit
                RayMeshGeometry3DHitTestResult raymeshresult = rayResult as RayMeshGeometry3DHitTestResult;
                var visual3D = rayResult.ModelHit;//使用visual3D作为命中的模型
                Point3D hitPoint = rayResult.PointHit;
                if (visual3D != null) //如果不为空，即确实命中了某一个模型
                {
                    //if (hitPoint != null &&
                    //    (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                    //{
                    //    MoveControl(hitPoint);

                    //}
                    //else
                    //{
                    //if (viewport.FindName("Box009") == visual3D) //用FindName方法进行检索。如果相等，执行一些事件
                    //{
                    //    AxisRatationEvent(axisRotation, raymeshresult, new Vector3D(-1, 0, -1));//此处执行了一个小动画，可以根据需要进行修改。
                    //}
                    //else if (viewport.FindName("Box013") == visual3D)
                    //{
                    //    //MainWindow
                    //    ScaleEvent(scaleControl);
                    //}
                    //else if (viewport.FindName("Box014") == visual3D)
                    //{
                    //    AxisRatationEvent(axisRotation1, raymeshresult, new Vector3D(-1, 0, 0));
                    //}
                    //else if (viewport.FindName("Box015") == visual3D)
                    //{
                    //    AxisRatationEvent(axisRotation2, raymeshresult, new Vector3D(-1, 0, 1));
                    //}


                    //}
                    if (mode == ControlMode.Set && !string.IsNullOrEmpty(visual3D.GetName()))
                    {
                        InitSetShow(mouseP, visual3D.GetName());
                        GeometryModel3D v = visual3D as GeometryModel3D;
                        string xx = v.GetName();
                        if (v != null)
                            setModelColor(v, new SolidColorBrush(Colors.Red));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(visual3D.GetName()) &&
                            (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                        {

                            InitInfoShow(mouseP, visual3D.GetName());
                        }
                    }
                    return HitTestResultBehavior.Stop;
                }
                else
                {
                    if (show != null)
                    {
                        show.Close();
                        show = null;
                    }
                    if (locset != null)
                    {
                        locset.Close();
                        locset = null;
                    }
                }

            }
            return HitTestResultBehavior.Continue;
        }

        /// <summary>
        /// 信息提示show
        /// </summary>
        /// <param name="pt"></param>
        private void InitInfoShow(Point pt, string name)
        {
            if (show != null)
            {
                show.Close();
                show = null;
            }

            show = new InfoShow();
            if (!show.InitShow(name))
            {
                return;
            }
            show.Left = pt.X;
            show.Top = pt.Y;
            show.Show();
        }

        /// <summary>
        /// 信息提示show
        /// </summary>
        /// <param name="pt"></param>
        private void InitSetShow(Point pt, string name)
        {
            if (locset != null)
            {
                locset.Close();
                locset = null;
            }

            locset = new LocationSet(name);
            locset.Left = pt.X;
            locset.Top = pt.Y;
            locset.Show();
        }

        /// <summary>
        /// 命中模型放大动画效果
        /// </summary>
        /// <param name="scale"></param>
        public void ScaleEvent(ScaleTransform3D scale)
        {
            //scale.ScaleX = 1.2;
            scale.ScaleY = 1;
            //scale.ScaleZ = 1.2;
            DoubleAnimation animation = new DoubleAnimation();
            animation.To = 2;
            animation.DecelerationRatio = 1;
            animation.Duration = TimeSpan.FromSeconds(0.15);
            animation.AutoReverse = true;
            //scale.BeginAnimation(ScaleTransform3D.ScaleXProperty, animation);
            scale.BeginAnimation(ScaleTransform3D.ScaleYProperty, animation);
            //scale.BeginAnimation(ScaleTransform3D.ScaleZProperty, animation);
        }
        /// <summary>
        /// 命中的模型移动动画
        /// </summary>
        /// <param name="axisname"></param>
        /// <param name="rayresultname"></param>
        public void AxisRatationEvent(AxisAngleRotation3D axisname,
            RayMeshGeometry3DHitTestResult rayresultname, Vector3D vec)
        {
            ///axisname.Axis = new Vector3D(rayresultname.PointHit.X, -rayresultname.PointHit.Y, rayresultname.PointHit.Z);
            axisname.Axis = vec;
            DoubleAnimation animation = new DoubleAnimation();
            animation.To = 20;
            animation.DecelerationRatio = 1;
            animation.Duration = TimeSpan.FromSeconds(0.15);
            animation.AutoReverse = true;
            axisname.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);

        }

        /// <summary>
        /// 强制移动3D中心坐标
        /// </summary>
        /// <param name="hitPoint"></param>
        //private void MoveControl(Point3D hitPoint)
        //{
        //    ////相机位置
        //    var cameraPostion = this.camera.Position;

        //    ////相机看的方向
        //    var lookDirection = this.camera.LookDirection;

        //    //setCameraPosition(this.camera,
        //    //    new System.Windows.Media.Media3D.Point3D(this.camera.Position.X - hitPoint.X,
        //    //        this.camera.Position.Y - hitPoint.Y, this.camera.Position.Z - hitPoint.Z));

        //    /// 获取相机在3D投影的点 
        //    var x = cameraPostion.X + lookDirection.X;
        //    var y = cameraPostion.Y + lookDirection.Y;
        //    var z = cameraPostion.Z + lookDirection.Z;
        //    DoubleAnimation doubleAnimationX = new DoubleAnimation();
        //    doubleAnimationX.BeginTime = new TimeSpan(0, 0, 0);
        //    doubleAnimationX.Duration = TimeSpan.FromMilliseconds(500);
        //    doubleAnimationX.From = this.transform.OffsetX;
        //    doubleAnimationX.To = x - hitPoint.X;

        //    DoubleAnimation doubleAnimationY = new DoubleAnimation();
        //    doubleAnimationY.BeginTime = new TimeSpan(0, 0, 0);
        //    doubleAnimationY.Duration = TimeSpan.FromMilliseconds(500);
        //    doubleAnimationY.From = transform.OffsetY;
        //    doubleAnimationY.To = y - hitPoint.Y;

        //    //DoubleAnimation doubleAnimationZ = new DoubleAnimation();
        //    //doubleAnimationZ.BeginTime = new TimeSpan(0, 0, 0);
        //    //doubleAnimationZ.Duration = TimeSpan.FromMilliseconds(500);
        //    //doubleAnimationZ.From = transform.OffsetZ;
        //    //doubleAnimationZ.To = Math.Abs(hitPoint.Z);

        //    transform.BeginAnimation(TranslateTransform3D.OffsetXProperty, doubleAnimationX);
        //    transform.BeginAnimation(TranslateTransform3D.OffsetYProperty, doubleAnimationY);
        //    //transform.BeginAnimation(TranslateTransform3D.OffsetZProperty, doubleAnimationZ);
        //}

        /// <summary>
        /// 计时器处理事件  抓取库位状态--根据状态维护展示效果  此处暂时提供测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            IList<Location> locations = BusinessControler.GetLocations();
            GeometryModel3D obj = null;
            if (device3D == null || device3D.Content == null)
            {
                return;
            }
            Model3DGroup m = device3D.Content as Model3DGroup;
            if (m != null && m.Children.Count > 0)
            {
                foreach (Location loc in locations)
                {
                    if (string.IsNullOrEmpty(loc.LocationBind))
                    {
                        continue;
                    }
                    if (m.Children.Where(s => s.GetName() == loc.LocationBind).Count() <= 0)
                    {
                        continue;
                    }
                    obj = m.Children.First(s=>s.GetName()==loc.LocationBind) as GeometryModel3D;
                    if(obj != null)
                    {
                        //控制颜色展示
                        Brush b = loc.LocationStatus == "0" ? Brushes.Green : (loc.LocationStatus == "1" ? Brushes.Red : Brushes.Orange);
                        setModelColor(obj, b);
                    }
                }
            }           
        }

        /// <summary>
        /// reset3D图形位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void MenuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    setCameraPosition(this.camera,
        //        new System.Windows.Media.Media3D.Point3D(this.camera.Position.X,
        //            this.camera.Position.Y, defaultcZ));
        //    DoubleAnimation doubleAnimationX = new DoubleAnimation();
        //    doubleAnimationX.BeginTime = new TimeSpan(0, 0, 0);
        //    doubleAnimationX.Duration = TimeSpan.FromMilliseconds(500);
        //    doubleAnimationX.From = this.transform.OffsetX;
        //    doubleAnimationX.To = 0;

        //    DoubleAnimation doubleAnimationY = new DoubleAnimation();
        //    doubleAnimationY.BeginTime = new TimeSpan(0, 0, 0);
        //    doubleAnimationY.Duration = TimeSpan.FromMilliseconds(500);
        //    doubleAnimationY.From = transform.OffsetY;
        //    doubleAnimationY.To = 0;

        //    DoubleAnimation doubleAnimationZ = new DoubleAnimation();
        //    doubleAnimationZ.BeginTime = new TimeSpan(0, 0, 0);
        //    doubleAnimationZ.Duration = TimeSpan.FromMilliseconds(500);
        //    doubleAnimationZ.From = transform.OffsetZ;
        //    doubleAnimationZ.To = 0;

        //    transform.BeginAnimation(TranslateTransform3D.OffsetXProperty, doubleAnimationX);
        //    transform.BeginAnimation(TranslateTransform3D.OffsetYProperty, doubleAnimationY);
        //    transform.BeginAnimation(TranslateTransform3D.OffsetZProperty, doubleAnimationZ);
        //}

        /// <summary>
        /// 生成柱状分析图
        /// </summary>
        /// <param name="name"></param>
        /// <param name="valuex"></param>
        /// <param name="valuey"></param>
        public void CreateChartColumn(string name, List<string> valuex, List<string> valuey)
        {
            //创建一个图标
            Chart chart = new Chart();

            //设置图标的宽度和高度
            //chart.Width = 580;
            //chart.Height = 380;
            chart.Margin = new Thickness(5, 5, 5, 5);
            //是否启用打印和保持图片
            chart.ToolBarEnabled = false;
            //设置图标的属性
            chart.ScrollingEnabled = false;//是否启用或禁用滚动
            chart.View3D = true;//3D效果显示

            chart.Background = new SolidColorBrush(Colors.Black);
            chart.Opacity = 0.9;

            //创建一个标题的对象
            Title title = new Title();
            //设置标题的名称
            title.Text = name;
            title.Padding = new Thickness(0, 5, 5, 0);

            //向图标添加标题
            chart.Titles.Add(title);

            Axis yAxis = new Axis();
            //设置图标中Y轴的最小值永远为0           
            yAxis.AxisMinimum = 0;
            //设置图表中Y轴的后缀          
            yAxis.Suffix = "";
            chart.AxesY.Add(yAxis);

            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();

            // 设置数据线的格式
            dataSeries.RenderAs = RenderAs.StackedColumn;//柱状Stacked


            // 设置数据点              
            DataPoint dataPoint;
            for (int i = 0; i < valuex.Count; i++)
            {
                // 创建一个数据点的实例。                   
                dataPoint = new DataPoint();
                // 设置X轴点                    
                dataPoint.AxisXLabel = valuex[i];
                //设置Y轴点                   
                dataPoint.YValue = double.Parse(valuey[i]);
                dataPoint.LegendText = "##" + valuex[i];
                //添加一个点击事件        
                dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                //添加数据点                   
                dataSeries.DataPoints.Add(dataPoint);
            }

            // 添加数据线到数据序列。                
            chart.Series.Add(dataSeries);

            //将生产的图表增加到Grid，然后通过Grid添加到上层Grid.           
            Grid gr = new Grid();
            gr.Children.Add(chart);
            colShow.Children.Add(gr);
        }

        /// <summary>
        /// 柱状图事件  保留
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dataPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataPoint dp = sender as DataPoint;
            MessageBox.Show("保留方法，待以后扩展！\r\n" + dp.YValue.ToString());
        }

        private void ChartColumnShow(object sender, RoutedEventArgs e)
        {
            MenuItem m = sender as MenuItem;
            if (m.IsChecked)
            {
                colShow.Children.Clear();
                //List<string> strListx = new List<string> { "enpty", "partly", "full" };
                //List<string> strListy = new List<string> { "20", "100", "40" }; ;
                List<string> strListx = null;
                List<string> strListy = null; ;
                BusinessControler.GetLocationDistribution(out strListx, out strListy);
                CreateChartColumn("Location Analysis", strListx, strListy);
                chartCan.Visibility = Visibility.Visible;
            }
            else
            {
                chartCan.Visibility = Visibility.Collapsed;
            }
        }
        private void Import_Click(object sender, RoutedEventArgs e)
        {
            var d = new OpenFileDialog();
            d.Filter = Importers.Filter;
            d.DefaultExt = Importers.DefaultExtension;
            d.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            if (!d.ShowDialog().Value)
            {
                return;
            }
            //viewport.Children.Clear();
            device3D.Content = Display3d(d.FileName);
            //device3D.SetName("worldModel");
            // Add to view port
            //viewport.Children.Clear();
            //viewport
            //Viewport3DHelper.Export(viewport.Viewport, d.FileName);

            //using (var exporter = new KerkytheaExporter(d.FileName))
            //{
            //    var m1 = this.Resources["m1"] as Material;
            //    exporter.RegisterMaterial(m1, @"Materials\water.xml");
            //    exporter.Export(view1.Viewport);
            //}
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var d = new SaveFileDialog();
            d.Filter = Exporters.Filter;
            d.DefaultExt = Exporters.DefaultExtension;
            if (!d.ShowDialog().Value)
            {
                return;
            }

            Viewport3DHelper.Export(viewport.Viewport, d.FileName);

            //using (var exporter = new KerkytheaExporter(d.FileName))
            //{
            //    var m1 = this.Resources["m1"] as Material;
            //    exporter.RegisterMaterial(m1, @"Materials\water.xml");
            //    exporter.Export(view1.Viewport);
            //}
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (show != null)
            {
                show.Close();
            }
            if (locset != null)
            {
                locset.Close();
            }
            this.Close();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new PrintDialog();
            if (dlg.ShowDialog().GetValueOrDefault())
            {
                dlg.PrintVisual(viewport.Viewport, this.Title);
            }
        }

        private void Set_Click(object sender, RoutedEventArgs e)
        {
            this.viewM.IsChecked = false;
            this.setM.IsChecked = true;
            mode = ControlMode.Set;
            IList<string> locs = BusinessControler.GetHasSetList();
            GeometryModel3D obj = null;
            if (device3D == null || device3D.Content == null)
            {
                return;
            }
            Model3DGroup m = device3D.Content as Model3DGroup;
            if (m != null && m.Children.Count > 0)
            {
                foreach (Object o in m.Children)
                {
                    obj = o as GeometryModel3D;
                    if (obj != null)
                    {
                        if (locs.Contains(obj.GetName()))
                        {
                            setModelColor(obj, Brushes.Red);
                        }
                        else
                        {
                            setModelColor(obj, Brushes.CornflowerBlue);
                        }

                    }
                }
            }
            //foreach (string loc in locs)
            //{
            //    if (string.IsNullOrEmpty(loc))
            //    {
            //        continue;
            //    }
            //    obj = this.viewport.FindName(loc) as GeometryModel3D;
            //    if (obj != null)
            //    {
            //        setModelColor(obj, Brushes.Red);
            //    }
            //}
            //Close();
        }

        private void View_Click(object sender, RoutedEventArgs e)
        {
            this.setM.IsChecked = false;
            this.viewM.IsChecked = true;
            mode = ControlMode.View;
            //Close();
        }

        private void CameraRotationMode_Click(object sender, RoutedEventArgs e)
        {
            //Close();
            MenuItem m = sender as MenuItem;
            if (m == null)
                return;
            if (m.Header.ToString() == "Turntable")
            {
                this.viewport.CameraRotationMode = CameraRotationMode.Turntable;
                //this
                m.IsChecked = true;
                this.TurnballM.IsChecked = false;
                this.TrackballM.IsChecked = false;
            }
            if (m.Header.ToString() == "Turnball")
            {
                this.viewport.CameraRotationMode = CameraRotationMode.Turnball;
                //this
                m.IsChecked = true;
                this.TurntableM.IsChecked = false;
                this.TrackballM.IsChecked = false;
            }
            if (m.Header.ToString() == "Trackball")
            {
                this.viewport.CameraRotationMode = CameraRotationMode.Trackball;
                //this
                m.IsChecked = true;
                this.TurnballM.IsChecked = false;
                this.TurntableM.IsChecked = false;
            }
            //this.viewport.CameraRotationMode = CameraRotationMode.;

        }

        private void CameraMode_Click(object sender, RoutedEventArgs e)
        {
            MenuItem m = sender as MenuItem;
            if (m == null)
                return;
            if (m.Header.ToString() == "Inspect")
            {
                this.viewport.CameraMode = CameraMode.Inspect;
                //this
                m.IsChecked = true;
                this.FixedPositionM.IsChecked = false;
                this.WalkAroundM.IsChecked = false;
            }
            if (m.Header.ToString() == "WalkAround")
            {
                this.viewport.CameraMode = CameraMode.WalkAround;
                //this
                m.IsChecked = true;
                this.FixedPositionM.IsChecked = false;
                this.InspectM.IsChecked = false;
            }
            if (m.Header.ToString() == "FixedPosition")
            {
                this.viewport.CameraMode = CameraMode.FixedPosition;
                //this
                m.IsChecked = true;
                this.InspectM.IsChecked = false;
                this.WalkAroundM.IsChecked = false;
            }
        }

        private void TimerC_Click(object sender, RoutedEventArgs e)
        {
            MenuItem m = sender as MenuItem;
            if (m == null)
                return;
            if (m.Header.ToString() == "Start")
            {
                timer.Start();
                m.IsChecked = true;
                this.stopM.IsChecked = false;
            }
            if (m.Header.ToString() == "Stop")
            {
                timer.Stop();
                m.IsChecked = true;
                this.startM.IsChecked = false;
            }
        }
        //private void viewport_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    defaultPosition2D = e.GetPosition(this.viewport);
        //}

        //private void viewport_MouseUp(object sender, MouseButtonEventArgs e)
        //{

        //}

        //private void test_MouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        //    {
        //        Point currentPosition = e.GetPosition(this.viewport);

        //        // avoid any zero axis conditions
        //        if (currentPosition == defaultPosition2D) return;

        //        // Prefer tracking to zooming if both buttons are pressed.
        //            MoveControl(defaultPosition2D, currentPosition);

        //        defaultPosition2D = currentPosition;
        //    }
        //}
    }

    public enum ControlMode
    {
        Set,
        View
    }

    /// <summary>
    /// 3D事件类
    /// </summary>
    public class Visual3DEventArgs : EventArgs
    {
        public ModelVisual3D Visual3D { get; set; }
        public Visual3DEventArgs(ModelVisual3D modelVisual3D)
        {
            this.Visual3D = modelVisual3D;
        }
    }

    /// <summary>
    /// 3D控制类
    /// </summary>
    public static class ViewControl
    {
        /// <summary>
        /// 设置模型brush
        /// </summary>
        /// <param name="visual3D"></param>
        /// <param name="color"></param>
        public static void SetColor(this GeometryModel3D geometrymodel, Brush color)
        {
            //GeometryModel3D geometrymodel = visual3D.Content as GeometryModel3D;

            if (geometrymodel.Material is MaterialGroup)
            {
                var materialGroup = geometrymodel.Material as MaterialGroup;
                foreach (var groupItem in materialGroup.Children)
                {
                    if (groupItem is DiffuseMaterial)
                    {
                        DiffuseMaterial tmpItem = groupItem as DiffuseMaterial;
                        tmpItem.Brush = color;
                    }
                }
            }
            else
            {
                DiffuseMaterial material = geometrymodel.Material as DiffuseMaterial;
                DiffuseMaterial materialBack = geometrymodel.BackMaterial as DiffuseMaterial;
                if (material != null)
                {
                    material.Brush = color;
                }
                if (materialBack != null)
                {
                    materialBack.Brush = color;
                }
            }
        }


    }
}
