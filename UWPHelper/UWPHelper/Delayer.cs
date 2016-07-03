using System;
using Windows.UI.Xaml;

namespace UWPHelper
{
    // Inspired by Rudy Huyn - http://www.rudyhuyn.com/blog/2016/03/01/delay-an-action-debounce-and-throttle/
    public class Delayer
    {
        DispatcherTimer timer;

        public event EventHandler<HandledEventArgs> Action;

        public Delayer(int miliseconds) : this(TimeSpan.FromMilliseconds(miliseconds))
        {

        }

        public Delayer(TimeSpan timeSpan)
        {
            timer = new DispatcherTimer();
            timer.Interval = timeSpan;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, object e)
        {
            timer.Stop();
            Action?.Invoke(this, new HandledEventArgs());
        }

        public void Reset()
        {
            timer.Stop();
            timer.Start();
        }
    }
}
