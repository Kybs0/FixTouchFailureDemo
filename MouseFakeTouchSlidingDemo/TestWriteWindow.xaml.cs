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

namespace MouseFakeTouchSlidingDemo
{
    /// <summary>
    /// TestWriteWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWriteWindow : Window
    {
        public TestWriteWindow()
        {
            InitializeComponent();
            Loaded += InitTouchSupport;
        }

        private void InitTouchSupport(object sender, RoutedEventArgs e)
        {
            if (TouchSupportOperationHelper.HasDisabledTouch & TouchSupportOperationHelper.HasOpenedTouchFix)
            {
                TouchSupportOperationHelper.RegisterMessageTouch(this);
            }
        }
    }
}
