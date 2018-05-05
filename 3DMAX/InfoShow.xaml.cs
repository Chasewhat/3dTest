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
using System.Windows.Shapes;
using Visifire.Charts;
using BusinessSer;

namespace _3DMAX
{
    /// <summary>
    /// InfoShow.xaml 的交互逻辑  继承自定义window  详情参见样式引用
    /// </summary>
    public partial class InfoShow : CustomWindow
    {
        public InfoShow()
        {
            InitializeComponent();
        }
        //绘制饼状图基础数据
        //后面可以修改为从数据库获取此数据
        private List<string> strListx = new List<string>() { "HasIn", "CanIn"};
        private List<string> strListy = new List<string>();

        /// <summary>
        /// 绘制饼状图
        /// </summary>
        /// <param name="name"></param>
        /// <param name="valuex"></param>
        /// <param name="valuey"></param>
        public void CreateChartPie(string name, List<string> valuex, List<string> valuey)
        {
            //创建一个图标
            Chart chart = new Chart();

            //设置图标的宽度和高度
            //chart.Width = 580;
            //chart.Height = 380;
            chart.Margin = new Thickness(10, 5, 10, 5);
            //是否启用打印和保持图片
            chart.ToolBarEnabled = false;

            //设置图标的属性
            chart.ScrollingEnabled = false;//是否启用或禁用滚动
            chart.View3D = true;//3D效果显示

            //创建一个标题的对象
            Title title = new Title();

            //设置标题的名称
            title.Text = name;
            title.Padding = new Thickness(0, 10, 5, 0);
            chart.Background = new SolidColorBrush(Colors.Black);
            chart.Opacity = 1;
            chart.FontSize = 20;
            //向图标添加标题
            chart.Titles.Add(title);

            //Axis yAxis = new Axis();
            ////设置图标中Y轴的最小值永远为0           
            //yAxis.AxisMinimum = 0;
            ////设置图表中Y轴的后缀          
            //yAxis.Suffix = "个";
            //chart.AxesY.Add(yAxis);

            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();

            // 设置数据线的格式
            dataSeries.RenderAs = RenderAs.Pie;//柱状Stacked


            // 设置数据点              
            DataPoint dataPoint;
            for (int i = 0; i < valuex.Count; i++)
            {
                // 创建一个数据点的实例。                   
                dataPoint = new DataPoint();
                // 设置X轴点                    
                dataPoint.AxisXLabel = valuex[i];

                dataPoint.LegendText = "##" + valuex[i];
                //设置Y轴点                   
                dataPoint.YValue = double.Parse(valuey[i]);
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
            pieShow.Children.Add(gr);
        }
        /// <summary>
        /// 饼状图保留事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dataPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataPoint dp = sender as DataPoint;
            MessageBox.Show("保留方法，待以后扩展！\r\n" + dp.YValue.ToString());
        }
        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.showText.Text = "3220001210[175/R55 SXAT XFT]:100" + "\r\n" + "1.P01234【50】" + "\r\n" + "1.P07214【50】";
        }

        /// <summary>
        /// 根据当前点击模型  初始化库位展示信息
        /// </summary>
        /// <param name="bindStr"></param>
        public bool InitShow(string bindStr)
        {
            bool initSuccess = true;
            try
            {
                Location loc = BusinessControler.GetLocationInfo(bindStr);
                if (loc == null)
                {
                    throw new Exception("Failed to get location info");
                }
                strListy.Add(loc.HasIn);
                strListy.Add(loc.CanIn);
                pieShow.Children.Clear();
                CreateChartPie("Location:"+loc.LocationNum, strListx, strListy);
                int i = 1;
                string showStr = "";
                foreach (Pallet p in loc.LocationMater)
                {
                    showStr += "【" + i.ToString() + "】" + p.PalletNum + ":" + p.PalletMater + "(" + p.PalletQuantiy + ")"
                        + "\r\n" + p.PalletSpec;
                    i++;
                }
                this.showText.Text = showStr;
            }
            catch(Exception)
            {
                initSuccess = false;
            }
            return initSuccess;
        }
    }
}
