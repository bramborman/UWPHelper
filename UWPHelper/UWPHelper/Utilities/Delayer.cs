using System;
using Windows.UI.Xaml;

namespace UWPHelper.Utilities
{
    // Inspired by Rudy Huyn - http://www.rudyhuyn.com/blog/2016/03/01/delay-an-action-debounce-and-throttle/
    public sealed class Delayer
    {
        private DispatcherTimer timer;

        public bool IsRunning
        {
            get { return timer.IsEnabled; }
        }
        public TimeSpan Delay
        {
            get { return timer.Interval; }
            set { timer.Interval = value; }
        }

        public event Action<Delayer> Tick;

        public Delayer(double secondsDelay) : this(TimeSpan.FromSeconds(secondsDelay))
        {

        }

        public Delayer(TimeSpan delay)
        {
            timer = new DispatcherTimer();
            timer.Interval = delay;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, object e)
        {
            timer.Stop();
            Tick?.Invoke(this);
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
