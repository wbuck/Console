using System;
using System.Threading.Tasks;
using System.Windows;

namespace Console.UI.Behaviors
{
    public static class Utilities
    {
        public static bool IsElementVisible(FrameworkElement element, FrameworkElement container)
        {
            if (!element.IsVisible)
                return false;

            Rect bounds = element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            Rect rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);

            if (rect.Left > bounds.Right || rect.Right < bounds.Left || rect.Bottom < bounds.Top || rect.Top > bounds.Bottom)
            {
                return false;
            }

            return true;
        }

        public static bool IsElementVisibleWithinParent(FrameworkElement element)
        {
            var parent = element.Parent as FrameworkElement;

            if (parent != null)
            {
                return IsElementVisible(element, parent);
            }
            else
            {
                throw new ArgumentNullException("element", "The element must have a FrameworkElement as a parent");
            }
        }

        public static Task Delay(double milliseconds)
        {
            var tcs = new TaskCompletionSource<bool>();
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += (obj, args) =>
            {
                tcs.TrySetResult(true);
            };
            timer.Interval = milliseconds;
            timer.AutoReset = false;
            timer.Start();
            return tcs.Task;
        }
    }
}