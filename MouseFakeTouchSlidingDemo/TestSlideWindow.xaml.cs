using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// TestSlideWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestSlideWindow : Window
    {
        public TestSlideWindow()
        {
            InitializeComponent();
            Loaded += TestSlideWindow_Loaded;
            Loaded += InitTouchSupport;
        }

        private void InitTouchSupport(object sender, RoutedEventArgs e)
        {
            if (TouchSupportOperationHelper.HasDisabledTouch & TouchSupportOperationHelper.HasOpenedTouchFix)
            {
                TouchSupportOperationHelper.RegisterMessageTouch(this);
            }
        }
        public event EventHandler Test;
        #region 数据加载测试

        private void TestSlideWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var list = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                list.Add($"sdadsfa{i}");
            }
            ItemsSource = list;
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource", typeof(List<string>), typeof(TestSlideWindow), new PropertyMetadata(default(List<string>)));

        public List<string> ItemsSource
        {
            get { return (List<string>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        #endregion
        private void TestTouchWindow_OnTouchDown(object sender, TouchEventArgs e)
        {
            Debug.WriteLine("TestTouchWindow_OnTouchDown");
        }
        private void TestTouchWindow_OnStylusDown(object sender, StylusDownEventArgs e)
        {
            Debug.WriteLine("TestTouchWindow_OnStylusDown");
        }

        private void TestTouchWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("TestTouchWindow_OnMouseDown");
        }

        private void TestTouchWindow_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("TestTouchWindow_OnMouseUp");
        }

        private void TestTouchWindow_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("TestTouchWindow_OnPreviewMouseDown");
        }

        private void TestTouchWindow_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("TestTouchWindow_OnPreviewMouseUp");
        }

        private void TestTouchWindow_OnStylusUp(object sender, StylusEventArgs e)
        {
            Debug.WriteLine("TestTouchWindow_OnStylusUp");
        }

        private void TestTouchWindow_OnTouchUp(object sender, TouchEventArgs e)
        {
            Debug.WriteLine("TestTouchWindow_OnTouchUp");
        }

        private void TestTouchWindow_OnPreviewTouchDown(object sender, TouchEventArgs e)
        {
            Debug.WriteLine("TestTouchWindow_OnPreviewTouchDown");
        }

        private void TestTouchWindow_OnPreviewTouchUp(object sender, TouchEventArgs e)
        {
            Debug.WriteLine("TestTouchWindow_OnPreviewTouchUp");
        }

        private void TestTouchWindow_OnPreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            Debug.WriteLine("TestTouchWindow_OnPreviewStylusDown");
        }

        private void TestTouchWindow_OnPreviewStylusUp(object sender, StylusEventArgs e)
        {
            Debug.WriteLine("TestTouchWindow_OnPreviewStylusUp");
        }

        private void TestTouchWindow_OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Debug.WriteLine("TestTouchWindow_OnManipulationStarted");
        }

        private void TestTouchWindow_OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Debug.WriteLine("TestTouchWindow_OnManipulationCompleted");
        }

        private void TestTouchWindow_OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Debug.WriteLine("TestTouchWindow_OnManipulationDelta");
        }
    }
}
