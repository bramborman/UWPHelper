using System;
using System.Diagnostics;
using Windows.UI.Xaml;

namespace UWPHelper.Utilities
{
    // Inspired by Rudy Huyn - http://www.rudyhuyn.com/blog/2016/03/01/delay-an-action-debounce-and-throttle/
    [DebuggerDisplay("IsRunning = {IsRunning}")]
    public sealed class Debouncer
    {
        private DispatcherTimer timer;

        public bool IsRunning
        {
            get { return timer.IsEnabled; }
        }
        public TimeSpan Delay
        {
            get { return timer.Interval; }
            set
            {
                if (timer.Interval != value)
                {
                    ExceptionHelper.ValidateIsGreaterOrEqual(value, TimeSpan.Zero, nameof(Delay));
                    timer.Interval = value;
                }
            }
        }

        public event EventHandler Tick;

        public Debouncer() : this(TimeSpan.Zero)
        {

        }
        
        public Debouncer(TimeSpan delay)
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;

            Delay = delay;
        }

        private void Timer_Tick(object sender, object e)
        {
            timer.Stop();
            Tick?.Invoke(this, EventArgs.Empty);
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
