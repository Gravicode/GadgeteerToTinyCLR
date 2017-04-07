namespace Gadgeteer.SocketInterfaces
{
    using System;

    public abstract class DigitalOutput : IDisposable
    {
        protected DigitalOutput()
        {
        }

        public virtual void Dispose()
        {
        }

        public abstract bool Read();
        public abstract void Write(bool state);
    }
}

