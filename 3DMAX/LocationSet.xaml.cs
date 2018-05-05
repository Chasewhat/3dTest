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
using BusinessSer;

namespace _3DMAX
{
    /// <summary>
    /// WindowTest.xaml 的交互逻辑
    /// </summary>
    public partial class LocationSet :CustomWindow
    {
        public string parentStr = "";
        private string currLoc = "";
        private bool flag = false;
        public LocationSet()
        {
            InitializeComponent();
        }

        public LocationSet(string id)
        {
            InitializeComponent();
            InitShow(id);
            parentStr = id;
        }
        /// <summary>
        /// 初始化界面显示
        /// </summary>
        public void InitShow(string id)
        {
            this.textBoxSet.Text = BusinessControler.GetLoctionBind(id);
            if (!string.IsNullOrEmpty(this.textBoxSet.Text.Trim()))
            {
                currLoc = this.textBoxSet.Text;
                flag = true;
            }
        }

        /// <summary>
        /// enter时同样触发确认事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxSet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnConfirm_Click(null, null);
            }
        }

        /// <summary>
        /// 确认事件  为指定图形节点绑定实际库位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (flag)
            {
                MessageBoxResult result = MessageBox.Show("Sure to change?\r\n" +
                    currLoc + "->" + this.textBoxSet.Text.ToUpper(), "SystemPromt",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            string tempStr = BusinessControler.SetLoctionBind(this.textBoxSet.Text.ToUpper(),parentStr);
            if (tempStr != "S")
            {
                MessageBox.Show(tempStr);
                return;
            }
            this.Close();
        }
    }
}
