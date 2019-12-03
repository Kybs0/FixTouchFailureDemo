using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace MouseFakeTouchSlidingDemo
{
    public class MessageTouchDevice : TouchDevice
    {
        /// <inheritdoc />
        public MessageTouchDevice(int deviceId, Window window) : base(deviceId)
        {
            Window = window;
            window.Closed += Window_Closed;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MessageTouchDeviceHelper.Remove(this);
        }

        /// <inheritdoc />
        public override TouchPoint GetTouchPoint(IInputElement relativeTo)
        {
            Point pt = Position;
            if (relativeTo != null)
            {
                pt = ActiveSource.RootVisual.TransformToDescendant((Visual)relativeTo).Transform(Position);
            }
            return new TouchPoint(this, pt, new Rect(pt, Size), TouchAction);
        }
        protected override void OnCapture(IInputElement element, CaptureMode captureMode)
        {
            Mouse.PrimaryDevice.Capture(element, captureMode);
        }
        /// <inheritdoc />
        public override TouchPointCollection GetIntermediateTouchPoints(IInputElement relativeTo)
        {
            return new TouchPointCollection()
            {
                GetTouchPoint(relativeTo)
            };
        }

        /// <summary>
        /// 触摸点
        /// </summary>
        public Point Position { set; get; }

        /// <summary>
        /// 触摸大小
        /// </summary>
        public Size Size { set; get; }

        public Window Window { get; }

        public TouchAction TouchAction { set; get; }

        public void Down()
        {
            TouchAction = TouchAction.Down;

            if (!IsActive)
            {
                SetActiveSource(PresentationSource.FromVisual(Window));

                Activate();
                ReportDown();
            }
            else
            {
                ReportDown();
            }
        }

        public void Move()
        {
            TouchAction = TouchAction.Move;

            ReportMove();
        }

        public void Up()
        {
            TouchAction = TouchAction.Up;

            ReportUp();
            Deactivate();
        }
    }

    public static class MessageTouchDeviceHelper
    {
        private static readonly Dictionary<int, MessageTouchDevice> Devices = new Dictionary<int, MessageTouchDevice>();

        public static MessageTouchDevice GetDevice(Window window, int dwId)
        {
            if (!Devices.TryGetValue(dwId, out var device))
            {
                device = new MessageTouchDevice(dwId, window);
                Devices.Add(dwId, device);
            }
            return device;
        }

        public static void Remove(MessageTouchDevice device)
        {
            Devices.Remove(device.Id);
        }
    }
}
