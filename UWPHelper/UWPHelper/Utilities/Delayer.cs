using System;
using Windows.UI.Xaml;

namespace UWPHelper.Utilities
{
    // Inspired by Rudy Huyn - http://www.rudyhuyn.com/blog/2016/03/01/delay-an-action-debounce-and-throttle/
    public sealed class Delayer
    {
        DispatcherTimer timer;

        public bool IsRunning
        {
            get { return timer.IsEnabled; }
        }

        public event EventHandler Tick;

        public Delayer(double secondsInterval) : this(TimeSpan.FromSeconds(secondsInterval))
        {

        }

        public Delayer(TimeSpan interval)
        {
            timer = new DispatcherTimer();
            timer.Interval = interval;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, object e)
        {
            timer.Stop();
            Tick?.Invoke(this, new EventArgs());
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