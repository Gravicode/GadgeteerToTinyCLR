namespace Gadgeteer.SocketInterfaces
{
    using System;

    public abstract class DigitalInput : IDisposable
    {
        protected DigitalInput()
        {
        }

        public virtual void Dispose()
        {
        }

        public abstract bool Read();
    }
}

