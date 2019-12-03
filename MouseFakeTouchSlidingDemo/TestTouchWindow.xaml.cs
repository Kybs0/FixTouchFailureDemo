using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MouseFakeTouchSlidingDemo
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class TestTouchWindow : Window
    {
        public TestTouchWindow()
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
        private void TestTouchWindow_OnTouchDown(object sender, TouchEventArgs e)
        {
            var touchPoint = e.GetTouchPoint(this);
            var positionX = touchPoint.Position.X - 5;
            var positionY = touchPoint.Position.Y - 5;
            var ellipse = new Ellipse()
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Red
            };
            Canvas.SetLeft(ellipse, positionX);
            Canvas.SetTop(ellipse, positionY);
            RootCanvas.Children.Add(ellipse);
            Debug.WriteLine("TestTouchWindow_OnTouchDown");
        }
        private void TestTouchWindow_OnStylusDown(object sender, StylusDownEventArgs e)
        {
            Debug.WriteLine("TestTouchWindow_OnStylusDown");
            e.Handled = true;
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
