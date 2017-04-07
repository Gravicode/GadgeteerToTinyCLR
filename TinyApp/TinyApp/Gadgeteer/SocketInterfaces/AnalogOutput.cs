namespace Gadgeteer.SocketInterfaces
{
    using System;

    public abstract class AnalogOutput : IDisposable
    {
        protected AnalogOutput()
        {
        }

        public virtual void Dispose()
        {
        }

        public virtual void WriteProportion(double proportion)
        {
            this.WriteVoltage(proportion * 3.3);
        }

        public abstract void WriteVoltage(double voltage);

        public abstract bool IsActive { get; set; }
    }
}

