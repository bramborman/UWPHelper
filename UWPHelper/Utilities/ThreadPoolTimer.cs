using System;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace UWPHelper.Utilities
{
    public sealed class ThreadPoolTimer : IDisposable
    {
        private readonly TimerCallback timerCallback;

        private Timer timer;
        private TimeSpan _interval;

        public bool IsInvokedOnDispatcher { get; }
        public bool IsDisposedOnStop { get; }
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
                        SetTimerInterval();
                    }
                }
            }
        }

        public event EventHandler Tick;
        
        public ThreadPoolTimer(TimeSpan interval) : this(interval, true, true)
        {

        }
        
        public ThreadPoolTimer(TimeSpan interval, bool invokeOnDispatcher, bool disposeOnStop)
        {
            Interval                = interval;
            IsInvokedOnDispatcher   = invokeOnDispatcher;
            IsDisposedOnStop        = disposeOnStop;

            if (invokeOnDispatcher)
            {
                timerCallback = async state =>
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Tick?.Invoke(this, new EventArgs());
                    });
                };
            }
            else
            {
                timerCallback = state =>
                {
                    Tick?.Invoke(this, new EventArgs());
                };
            }
        }

        private void SetTimerInterval()
        {
            timer.Change(TimeSpan.Zero, Interval);
        }

        public void Start()
        {
            if (IsEnabled)
            {
                return;
            }

            if (timer == null)
            {
                timer = new Timer(timerCallback, null, TimeSpan.Zero, Interval);
            }
            else
            {
                SetTimerInterval();
            }

            IsEnabled = true;
        }

        public void Stop()
        {
            Stop(false);
        }

        public void Dispose()
        {
            Stop(true);
        }

        private void Stop(bool dispose)
        {
            IsEnabled = false;

            if (IsDisposedOnStop || dispose)
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
