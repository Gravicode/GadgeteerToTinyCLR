namespace Gadgeteer
{
    using Microsoft.SPOT;
    using Microsoft.SPOT.Hardware;
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;

    public class Timer
    {
        private BehaviorType <Behavior>k__BackingField;
        private static Hashtable activeTimers = new Hashtable();
        private DispatcherTimer dt;

        public event TickEventHandler Tick;

        public Timer(int intervalMilliseconds) : this(new TimeSpan(0, 0, 0, 0, intervalMilliseconds), BehaviorType.RunContinuously)
        {
        }

        public Timer(TimeSpan interval) : this(interval, BehaviorType.RunContinuously)
        {
        }

        public Timer(int intervalMilliseconds, BehaviorType behavior) : this(new TimeSpan(0, 0, 0, 0, intervalMilliseconds), behavior)
        {
        }

        public Timer(TimeSpan interval, BehaviorType behavior)
        {
            if (Program.Dispatcher == null)
            {
                Debug.WriteLine("WARN: null Program.Dispatcher in GT.Timer constructor");
            }
            this.dt = new DispatcherTimer(Program.Dispatcher ?? Dispatcher.CurrentDispatcher);
            this.dt.Interval = interval;
            this.dt.Tick += new Microsoft.SPOT.EventHandler(this.dt_Tick);
            this.Behavior = behavior;
        }

        private void dt_Tick(object sender, Microsoft.SPOT.EventArgs e)
        {
            try
            {
                if (this.Behavior == BehaviorType.RunOnce)
                {
                    this.Stop();
                }
                if (this.Tick != null)
                {
                    this.Tick(this);
                }
            }
            catch
            {
                Debug.WriteLine("Exception performing Timer operation");
            }
        }

        public override int GetHashCode()
        {
            return this.dt.GetHashCode();
        }

        public static TimeSpan GetMachineTime()
        {
            return Utility.GetMachineTime();
        }

        public void Restart()
        {
            this.dt.Stop();
            this.Start();
        }

        public void Start()
        {
            if (!activeTimers.Contains(this))
            {
                activeTimers.Add(this, null);
            }
            this.dt.Start();
        }

        public void Stop()
        {
            if (activeTimers.Contains(this))
            {
                activeTimers.Remove(this);
            }
            this.dt.Stop();
        }

        public BehaviorType Behavior
        {
            get
            {
                return this.<Behavior>k__BackingField;
            }
            set
            {
                this.<Behavior>k__BackingField = value;
            }
        }

        public TimeSpan Interval
        {
            get
            {
                return this.dt.Interval;
            }
            set
            {
                this.dt.Interval = value;
            }
        }

        public bool IsRunning
        {
            get
            {
                return this.dt.IsEnabled;
            }
        }

        public enum BehaviorType
        {
            RunOnce,
            RunContinuously
        }

        public delegate void TickEventHandler(Timer timer);
    }
}

