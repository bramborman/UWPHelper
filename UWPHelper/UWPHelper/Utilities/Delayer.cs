using System;
using Windows.UI.Xaml;

namespace UWPHelper.Utilities
{
    // Inspired by Rudy Huyn - http://www.rudyhuyn.com/blog/2016/03/01/delay-an-action-debounce-and-throttle/
    public class Delayer
    {
        DispatcherTimer timer;

        public event EventHandler<HandledEventArgs> Tick;

        public Delayer(double seconds) : this(TimeSpan.FromSeconds(seconds))
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
            Tick?.Invoke(this, new HandledEventArgs());
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void Reset()
        {
            timer.Stop();
            timer.Start();
        }
    }
}