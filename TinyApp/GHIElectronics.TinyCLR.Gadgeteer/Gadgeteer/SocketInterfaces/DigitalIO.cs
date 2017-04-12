namespace Gadgeteer.SocketInterfaces
{
    using System;

    public abstract class DigitalIO : IDisposable
    {
        protected DigitalIO()
        {
        }

        public virtual void Dispose()
        {
        }

        public abstract bool Read();
        public abstract void Write(bool state);

        public abstract IOMode Mode { get; set; }
    }
}

