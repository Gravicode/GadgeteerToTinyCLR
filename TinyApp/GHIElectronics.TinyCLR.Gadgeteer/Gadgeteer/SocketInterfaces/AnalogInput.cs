namespace Gadgeteer.SocketInterfaces
{
    using System;

    public abstract class AnalogInput : IDisposable
    {
        protected AnalogInput()
        {
        }

        public virtual void Dispose()
        {
        }

        public virtual double ReadProportion()
        {
            return (this.ReadVoltage() / 3.3);
        }

        public abstract double ReadVoltage();

        public abstract bool IsActive { get; set; }
    }
}

