using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace MouseFakeTouchSlidingDemo
{
    /// <summary>
    /// 触摸支持辅助类
    /// </summary>
    public class TouchSupportOperationHelper
    {
        /// <summary>
        /// 当前是否已经禁用原有触摸
        /// </summary>
        public static bool HasDisabledTouch { get; set; }
        /// <summary>
        /// 是否开启触摸修复
        /// </summary>
        public static bool HasOpenedTouchFix { get; set; }

        /// <summary>
        /// 禁用原有WPF触摸
        /// 禁用后，原有的 WPF 触摸将会无法再次使用
        /// </summary>
        public static void DisableWpfTouch()
        {
            if (HasDisabledTouch)
            {
                return;
            }
            TouchTabletHelper.DisableWPFTabletSupport();
            HasDisabledTouch = true;
        }
        /// <summary>
        /// 使用消息机制模拟触摸
        /// <remarks>开启触摸模拟后，原有的 WPF 触摸将会无法再次使用</remarks>
        /// <remarks>理论上，不管触摸是否真的失效，禁用后再添加模拟触摸，触摸流程正常</remarks>
        /// </summary>
        public static void RegisterMessageTouch(Window window)
        {
            if (ReferenceEquals(window, null)) throw new ArgumentNullException(nameof(window));
            var hWnd = new WindowInteropHelper(window).Handle;

            if (hWnd == IntPtr.Zero)
            {
                throw new InvalidOperationException("请在SourceInitialized之后调用这个方法");
            }
            //禁用原有WPF触摸
            //如果真的触摸失效了，可以不执行禁用逻辑。但是触摸失效判断...emmm可能不准确，所以添加模拟触摸前禁用原有WPF触摸，保证整个触摸流程不重复不缺失。
            DisableWpfTouch();
            //注册触摸消息
            TouchNativeMethods.RegisterTouchWindow(hWnd, TouchNativeMethods.TWF_WANTPALM);
            //监听窗口触摸消息
            var source = HwndSource.FromHwnd(hWnd);
            Debug.Assert(source != null);
            source.AddHook((IntPtr hwnd, int msg, IntPtr param, IntPtr lParam, ref bool handled) =>
            {
                HandleWindowProc(window, msg, param, lParam, ref handled);
                return IntPtr.Zero;
            });
        }
        /// <summary>
        /// 处理窗口消息
        /// </summary>
        /// <param name="window"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        private static void HandleWindowProc(Window window, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == TouchNativeMethods.WM_TOUCH)
            {
                var inputCount = wParam.ToInt32() & 0xffff;
                var inputs = new TouchNativeMethods.TOUCHINPUT[inputCount];

                if (TouchNativeMethods.GetTouchInputInfo(lParam, inputCount, inputs, TouchNativeMethods.TouchInputSize))
                {
                    for (int i = 0; i < inputCount; i++)
                    {
                        var input = inputs[i];
                        //FakeTouchInputToPenContext0(window, input, inputCount);
                        //FakeTouchInputToPenContext1(window, input, inputCount);
                        FakeTouchInputToWindow(window, input);
                    }
                }

                TouchNativeMethods.CloseTouchInputHandle(lParam);
                handled = true;
            }
        }
        private static List<PresentationSource> registeredPresentationSources=new List<PresentationSource>();
        /// <summary>
        /// 待完成
        /// </summary>
        /// <param name="window"></param>
        /// <param name="input"></param>
        /// <param name="inputCount"></param>
        private static void FakeTouchInputToPenContext1(Window window, TouchNativeMethods.TOUCHINPUT input, int inputCount)
        {
            var stylusLogicType = typeof(TabletDevice).Assembly.GetType("System.Windows.Input.StylusLogic");
            var stylusLogic = stylusLogicType.GetProperty("CurrentStylusLogic", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, null);
            if (stylusLogic.GetType().FullName.Contains("WispLogic"))
            {
                var wispLogic = stylusLogic;

                //var wispTabletList = wispLogic.GetType().GetProperty("CurrentStylusDevice", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(wispLogic);

                if (!registeredPresentationSources.Contains(PresentationSource.FromVisual(window)))
                {
                    //先注销
                    wispLogic.GetType().GetMethod("UnRegisterHwndForInput", BindingFlags.NonPublic | BindingFlags.Instance)
                        .Invoke(wispLogic, new object[1] { (HwndSource)PresentationSource.FromVisual(window) });
                    //初始化PenContext等
                    //只能注册一次
                    wispLogic.GetType().GetMethod("RegisterHwndForInput", BindingFlags.NonPublic | BindingFlags.Instance)
                        .Invoke(wispLogic, new object[2] { InputManager.Current, PresentationSource.FromVisual(window) });
                    registeredPresentationSources.Add(PresentationSource.FromVisual(window));
                }

                var penContextsObject = wispLogic.GetType().GetMethod("GetPenContextsFromHwnd", BindingFlags.NonPublic | BindingFlags.Instance)
                    .Invoke(wispLogic, new[] { PresentationSource.FromVisual(window) });

                var penContextsValue = penContextsObject.GetType().GetField("_contexts", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(penContextsObject) as object[];
                if (penContextsValue == null || penContextsValue.Length == 0)
                {
                    penContextsObject.GetType().GetMethod("Enable", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(penContextsObject, null);
                    penContextsValue = penContextsObject.GetType().GetField("_contexts", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(penContextsObject) as object[];
                }
                if (penContextsValue == null || penContextsValue.Length == 0)
                {
                    //uint index = 1;
                    //penContextsInstance.GetType().GetMethod("AddContext", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(penContextsInstance, new object[1] { index });
                    //penContextsValue = penContextsInstance.GetType().GetField("_contexts", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(penContextsInstance) as object[];
                    //以上无法添加
                }

                object penContext = null;
                if (penContextsValue != null && penContextsValue.Length > 0)
                {
                    penContext = penContextsValue[0];
                    _lastPenContext = penContext;
                }
                else
                {
                    if (_lastPenContext == null)
                    {
                        return;
                    }
                    penContext = _lastPenContext;
                }
                var tabletDeviceId = (int)penContext.GetType().GetProperty("TabletDeviceId", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(penContext);
                //因触摸数据无法转换，此处调用但是WPF层好像无效
                penContextsValue.GetType().GetMethod("OnPenDown", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(penContextsValue, new object[5] { penContext, tabletDeviceId, input.DwID, new int[3], input.DwTime });//并且input输入无法转换
            }
        }

        /// <summary>
        /// 模拟触摸输入（PenContext）
        /// 因触摸数据无法转换、会多余一份Mouse事件。此方案废弃
        /// </summary>
        /// <param name="window"></param>
        /// <param name="input"></param>
        /// <param name="inputCount"></param>
        private static void FakeTouchInputToPenContext0(Window window, TouchNativeMethods.TOUCHINPUT input, int inputCount)
        {
            Assembly wpfAssembly = typeof(TabletDevice).Assembly;
            var wispLogicType = wpfAssembly.GetType("System.Windows.Input.StylusWisp.WispLogic");
            var wispLogicConstructorInfos = wispLogicType.GetConstructors(BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance);
            var wispLogicConstructorInfo = wispLogicConstructorInfos[0];
            var wispLogic = wispLogicConstructorInfo.Invoke(new object[] { InputManager.Current });

            var penContextsConstructorInfo = wpfAssembly.GetTypes().Where(i => i.FullName.Contains("PenContexts")).ToList()[0].
                GetConstructors(BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance).First();
            var penContextsInstance = penContextsConstructorInfo.Invoke(new object[2] { wispLogic, PresentationSource.FromVisual(window) });

            var penContextsValue = penContextsInstance.GetType().GetField("_contexts", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(penContextsInstance) as object[];
            if (penContextsValue == null || penContextsValue.Length == 0)
            {
                penContextsInstance.GetType().GetMethod("Enable", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(penContextsInstance, null);
                penContextsValue = penContextsInstance.GetType().GetField("_contexts", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(penContextsInstance) as object[];
            }
            if (penContextsValue == null || penContextsValue.Length == 0)
            {
                //uint index = 1;
                //penContextsInstance.GetType().GetMethod("AddContext", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(penContextsInstance, new object[1] { index });
                //penContextsValue = penContextsInstance.GetType().GetField("_contexts", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(penContextsInstance) as object[];
                //以上无法添加
            }

            object penContext = null;
            if (penContextsValue != null && penContextsValue.Length > 0)
            {
                penContext = penContextsValue[0];
                _lastPenContext = penContext;
            }
            else
            {
                if (_lastPenContext == null)
                {
                    return;
                }
                penContext = _lastPenContext;
            }
            var tabletDeviceId = (int)penContext.GetType().GetProperty("TabletDeviceId", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(penContext);
            //可能因触摸数据没有转换，此处调用但是WPF层好像无效
            penContextsValue.GetType().GetMethod("OnPenDown", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(penContextsValue, new object[5] { penContext, tabletDeviceId, input.DwID, new int[3], input.DwTime });//并且input输入无法转换
        }

        private static object _lastPenContext = null;
        private static void FakeTouchInpuToPenContext1(Window window, TouchNativeMethods.TOUCHINPUT input, int inputCount)
        {
            //InputManager.Current.PostProcessInput  InputManager.Current
            //var postProcessInputEvent = typeof(InputManager).GetEvent("_postProcessInput", BindingFlags.NonPublic | BindingFlags.Instance).GetRaiseMethod(true); ;
            //var privateInputEventArgsProperty = typeof(InputManager).GetField("_processInputEventArgs", BindingFlags.NonPublic | BindingFlags.Instance);
            //var inputEventArgs = privateInputEventArgsProperty.GetValue(InputManager.Current) as ProcessInputEventArgs;
            //if (inputEventArgs == null)
            //{
            //    inputEventArgs = Activator.CreateInstance(typeof(ProcessInputEventArgs)) as ProcessInputEventArgs;
            //    privateInputEventArgsProperty.SetValue(InputManager.Current, inputEventArgs);
            //}
            //postProcessInputEvent.Invoke(InputManager.Current, new object[1]{ inputEventArgs });
        }
        /// <summary>
        /// 触摸消息转WPF事件
        /// 此方案只支持Touch,没有Stylus事件。同时Touch与Mouse事件被独立开了。--此方案有缺陷，但能勉强使用。TODO 模拟Stylus事件触发
        /// </summary>
        /// <param name="window"></param>
        /// <param name="input"></param>
        private static void FakeTouchInputToWindow(Window window, TouchNativeMethods.TOUCHINPUT input)
        {
            // 相对的是没有处理 DPI 的屏幕坐标
            // 因为是 物理屏幕坐标的像素的百分之一表示，需要除 100 计算像素
            var screenLocation = new Point(input.X / 100.0, input.Y / 100.0);
            var wpfLocation = TransformToWpfPoint(screenLocation, window);
            var sizeInScreen = new Size(input.CxContact / 100.0, input.CyContact / 100.0);
            var wpfSize = TransformToWpfSize(sizeInScreen, window);
            //获取触摸设备信息
            var device = MessageTouchDeviceHelper.GetDevice(window, input.DwID);
            if (!device.IsActive && input.DwFlags.HasFlag(TouchNativeMethods.TOUCHEVENTF.TOUCHEVENTF_DOWN))
            {
                device.Position = wpfLocation;
                device.Size = wpfSize;
                device.Down();
            }
            else if (device.IsActive && input.DwFlags.HasFlag(TouchNativeMethods.TOUCHEVENTF.TOUCHEVENTF_MOVE))
            {
                device.Position = wpfLocation;
                device.Size = wpfSize;
                device.Move();
            }
            else if (device.IsActive && input.DwFlags.HasFlag(TouchNativeMethods.TOUCHEVENTF.TOUCHEVENTF_UP))
            {
                device.Position = wpfLocation;
                device.Size = wpfSize;
                device.Up();
                MessageTouchDeviceHelper.Remove(device);
            }
        }

        //临时
        private static Size TransformToWpfSize(Size sizeInScreen, Window window)
        {
            var intPtr = new WindowInteropHelper(window).Handle;//获取当前窗口的句柄
            using (Graphics currentGraphics = Graphics.FromHwnd(intPtr))
            {
                double dpiXRatio = currentGraphics.DpiX / DpiPercent;
                double dpiYRatio = currentGraphics.DpiY / DpiPercent;
                var width = sizeInScreen.Width / dpiXRatio;
                var height = sizeInScreen.Height / dpiYRatio;
                return new Size(width, height);
            }
        }

        private const int DpiPercent = 96;
        //临时
        private static Point TransformToWpfPoint(Point screenPoint, Window window)
        {
            var intPtr = new WindowInteropHelper(window).Handle;//获取当前窗口的句柄
            using (Graphics currentGraphics = Graphics.FromHwnd(intPtr))
            {
                double dpiXRatio = currentGraphics.DpiX / DpiPercent;
                double dpiYRatio = currentGraphics.DpiY / DpiPercent;
                var x = screenPoint.X / dpiXRatio;
                var y = screenPoint.Y / dpiYRatio;
                var wpfPointInScreen = new Point(x, y);
                var point = window.PointFromScreen(wpfPointInScreen);
                return point;
            }
        }
    }
}
