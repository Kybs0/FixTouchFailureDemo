using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MouseFakeTouchSlidingDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
           var isTouch= TouchNativeMethods.IsTouchWindow(new WindowInteropHelper(this).Handle, 0x00000002);
        }

        private void DisableTouchCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            var isChecked = DisableTouchCheckBox.IsChecked ?? false;
            if (isChecked)
            {
                TouchSupportOperationHelper.DisableWpfTouch();
                DisableTouchCheckBox.IsEnabled = false;
            }
            if (TouchSupportOperationHelper.HasDisabledTouch & TouchSupportOperationHelper.HasOpenedTouchFix)
            {
                TouchSupportOperationHelper.RegisterMessageTouch(this);
            }
        }

        private void OpenTouchCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            var isChecked = OpenTouchCheckBox.IsChecked ?? false;
            TouchSupportOperationHelper.HasOpenedTouchFix = isChecked;
            if (isChecked)
            {
                TouchSupportOperationHelper.RegisterMessageTouch(this);
            }
            var isTouch = TouchNativeMethods.IsTouchWindow(new WindowInteropHelper(this).Handle, 0x00000002);
            if (TouchSupportOperationHelper.HasDisabledTouch & TouchSupportOperationHelper.HasOpenedTouchFix)
            {
                TouchSupportOperationHelper.RegisterMessageTouch(this);
            }
        }

        private void MainWindow_OnTouchDown(object sender, TouchEventArgs e)
        {

        }
        private void MainWindow_OnStylusDown(object sender, StylusDownEventArgs e)
        {
           
        }
        private void TestTouchButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new TestTouchWindow();
            window.Owner = this;
            window.ShowDialog();
        }

        private void TestWriteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new TestWriteWindow();
            window.Owner = this;
            window.ShowDialog();
        }

        private void TestSlideButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new TestSlideWindow();
            window.Owner = this;
            window.ShowDialog();
        }
    }
}
