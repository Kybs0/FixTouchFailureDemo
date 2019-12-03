using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MouseFakeTouchSlidingDemo
{
    /// <summary>
    /// 触摸设备辅助类
    /// Based on https://msdn.microsoft.com/en-us/library/vstudio/dd901337(v=vs.90).aspx
    /// https://jaytwo.github.io/2015/07/25/multi-touch-and-wpf.html
    /// </summary>
    public static class TouchTabletHelper
    {
        private static bool HasRemovedDevices { get; set; }
        private static List<TabletDevice> _lasTabletDevices = null;
        private static int _lastSeenDeviceCount;
        private static int _lasTabletDeviceId;

        public static void ReOpenWPFTabletSuttport()
        {
            if (HasRemovedDevices)
            {
                var inputManagerType = typeof(InputManager);
                var stylusLogic = inputManagerType.InvokeMember("StylusLogic",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, InputManager.Current, null);

                Type stylusLogicType;
                FieldInfo countField452;
                if (stylusLogic != null)
                {
                    stylusLogicType = stylusLogic.GetType();
                    countField452 = stylusLogicType.GetField("_lastSeenDeviceCount",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                }
                else
                {
                    return;
                }

                for (int i = 0; i < _lasTabletDevices.Count; i++)
                {
                    stylusLogicType.InvokeMember("OnTabletAdded",
                        BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                        null, stylusLogic, new object[] { (uint)i });
                }

                var tabletDeviceCollection = Tablet.TabletDevices;
            }
        }
        /// <summary>
        /// 禁用 WPF 触摸
        /// </summary>
        public static void DisableWPFTabletSupport()
        {
            // 先禁用 WPF 触摸
            if (!HasRemovedDevices)
            {
                var inputManagerType = typeof(InputManager);

                var stylusLogic = inputManagerType.InvokeMember("StylusLogic",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, InputManager.Current, null);
                //var wispLogic = stylusLogicType.GetProperty("CurrentStylusLogic", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, null);

                Type stylusLogicType;
                FieldInfo countField452;
                if (stylusLogic != null)
                {
                    stylusLogicType = stylusLogic.GetType();
                    countField452 = stylusLogicType.GetField("_lastSeenDeviceCount",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                }
                else
                {
                    return;
                }

                _lasTabletDeviceId = Tablet.CurrentTabletDevice.Id;

                while (Tablet.TabletDevices.Count > 0)
                {
                    // Only in .Net Framework 4.5.2 - see https://connect.microsoft.com/VisualStudio/Feedback/Details/1016534
                    if (countField452 != null)
                    {
                        countField452.SetValue(stylusLogic, 1 + (int)countField452.GetValue(stylusLogic));
                    }

                    int index = Tablet.TabletDevices.Count - 1;

                    stylusLogicType.InvokeMember("OnTabletRemoved",
                        BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                        null, stylusLogic, new object[] { (uint)index });

                    HasRemovedDevices = true;
                }
            }
        }

    }
}
