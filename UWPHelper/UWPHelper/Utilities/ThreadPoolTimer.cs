using System;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace UWPHelper.Utilities
{
    public sealed class ThreadPoolTimer : IDisposable
    {
        private readonly object locker = new object();
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
                if (value < TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                if (_interval != value)
                {
                    _interval = value;

                    if (IsEnabled)
                    {
                        SetTimer();
                    }
                }
            }
        }

        public event EventHandler Tick;

        public ThreadPoolTimer() : this(true, true)
        {

        }
        
        public ThreadPoolTimer(bool invokeOnDispatcher, bool disposeOnStop)
        {
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

        private void SetTimer()
        {
            lock (locker)
            {
                timer.Change(new TimeSpan(0), Interval);
            }
        }

        public void Start()
        {
            if (IsEnabled || !IsDisposedOnStop)
            {
                SetTimer();
            }
            else if (IsDisposedOnStop)
            {
                timer = new Timer(timerCallback, null, new TimeSpan(0), Interval);
            }

            IsEnabled = true;
        }

        public void Stop()
        {
            IsEnabled = false;

            if (IsDisposedOnStop)
            {
                Dispose();
            }
            else
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        public void Dispose()
        {
            lock (locker)
            {
                timer.Dispose();
                timer = null;
            }
        }
    }
}
