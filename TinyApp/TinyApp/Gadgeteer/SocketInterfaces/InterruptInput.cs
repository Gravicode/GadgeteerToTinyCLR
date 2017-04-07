namespace Gadgeteer.SocketInterfaces
{
    using System;

    public abstract class InterruptInput : IDisposable
    {
        private InterruptEventHandler interrupt;

        public event InterruptEventHandler Interrupt
        {
            add
            {
                if (this.interrupt == null)
                {
                    this.OnInterruptFirstSubscribed();
                }
                this.interrupt = (InterruptEventHandler) Delegate.Combine(this.interrupt, value);
            }
            remove
            {
                this.interrupt = (InterruptEventHandler) Delegate.Remove(this.interrupt, value);
                if (this.interrupt == null)
                {
                    this.OnInterruptLastUnsubscribed();
                }
            }
        }

        protected InterruptInput()
        {
        }

        public virtual void Dispose()
        {
        }

        protected abstract void OnInterruptFirstSubscribed();
        protected abstract void OnInterruptLastUnsubscribed();
        protected void RaiseInterrupt(bool value)
        {
            if (this.interrupt != null)
            {
                this.interrupt(this, value);
            }
        }

        public abstract bool Read();
    }
}

