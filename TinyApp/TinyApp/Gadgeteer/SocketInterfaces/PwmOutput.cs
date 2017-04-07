namespace Gadgeteer.SocketInterfaces
{
    using System;

    public abstract class PwmOutput : IDisposable
    {
        protected PwmOutput()
        {
        }

        public virtual void Dispose()
        {
        }

        public abstract void Set(double frequency, double dutyCycle);
        public abstract void Set(uint period, uint highTime, PwmScaleFactor factor);

        public abstract bool IsActive { get; set; }
    }
}

