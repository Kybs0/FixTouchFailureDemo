using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MouseFakeTouchSlidingDemo
{
    public class TouchSupportHelper
    {
        /// <summary>
        /// 触摸失效时，是否开启触摸修复
        /// </summary>
        public static readonly DependencyProperty IsOpenTouchFixWhenFailedProperty = DependencyProperty.RegisterAttached(
            "IsOpenTouchFixWhenFailed", typeof(bool), typeof(TouchSupportHelper),
            new PropertyMetadata(default(bool), OnIsOpenTouchFixWhenFailedPropertyChanged));

        private static async void OnIsOpenTouchFixWhenFailedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //暂时只支持窗口触摸失效修复 ContentDialog后续需要时再添加
            if (d is Window window)
            {
                //只有开启修复，关闭相关逻辑没有相关场景暂不添加。
                if (e.NewValue is bool isOpenTouchFixWhenFailed && isOpenTouchFixWhenFailed)
                {
                    // 延迟进行,防止影响窗口性能,暂时设置为5秒
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    //var canTouch = WindowsTouchHelper.CheckTouchOnlyOnce(out _);
                    //if (!canTouch)
                    //{
                    TouchSupportOperationHelper.RegisterMessageTouch(window);
                    //Log.Info($"{nameof(window)}触摸失效,已自动修复by{nameof(TouchSupportOperationHelper)}");
                    //}
                }
            }
        }

        public static void SetIsOpenTouchFixWhenFailed(DependencyObject element, bool value)
        {
            element.SetValue(IsOpenTouchFixWhenFailedProperty, value);
        }

        public static bool GetIsOpenTouchFixWhenFailed(DependencyObject element)
        {
            return (bool)element.GetValue(IsOpenTouchFixWhenFailedProperty);
        }
    }
}
