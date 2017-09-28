using System;
using System.Diagnostics;
using System.Threading;

namespace UWPHelper.Utilities
{
    [DebuggerDisplay("IsEnabled = {IsEnabled}")]
    public sealed class ThreadPoolTimer : IDisposable
    {
        private Timer timer;
        private TimeSpan _interval;
        
        public bool IsEnabled { get; private set; }
        public TimeSpan Interval
        {
            get { return _interval; }
            set
            {
                if (_interval != value)
                {
                    _interval = value;

                    if (IsEnabled)
                    {
                        UpdateTimerInterval();
                    }
                }
            }
        }

        public event EventHandler Tick;

        public ThreadPoolTimer() : this(TimeSpan.Zero)
        {

        }
        
        public ThreadPoolTimer(TimeSpan interval)
        {
            Interval = interval;
        }

        private void UpdateTimerInterval()
        {
            timer.Change(TimeSpan.Zero, Interval);
        }

        private void TimerCallback(object state)
        {
            Tick?.Invoke(this, EventArgs.Empty);
        }

        public void Start()
        {
            if (IsEnabled)
            {
                return;
            }

            if (timer == null)
            {
                timer = new Timer(TimerCallback, null, TimeSpan.Zero, Interval);
            }
            else
            {
                UpdateTimerInterval();
            }

            IsEnabled = true;
        }

        public void Restart()
        {
            Stop(false);
            Start();
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            Stop(true);
        }

        private void Stop(bool dispose)
        {
            IsEnabled = false;

            if (dispose)
            {
                timer?.Dispose();
                timer = null;
            }
            else
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }
    }
}
