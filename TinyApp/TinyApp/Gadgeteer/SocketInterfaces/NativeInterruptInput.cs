namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT.Hardware;
    using System;

    internal class NativeInterruptInput : InterruptInput
    {
        private InterruptPort _port;

        public NativeInterruptInput(Socket socket, Socket.Pin pin, GlitchFilterMode glitchFilterMode, Gadgeteer.SocketInterfaces.ResistorMode resistorMode, Gadgeteer.SocketInterfaces.InterruptMode interruptMode, Module module, Cpu.Pin cpuPin)
        {
            if (cpuPin == Cpu.Pin.GPIO_NONE)
            {
                throw Socket.InvalidSocketException.FunctionalityException(socket, "InterruptInput");
            }
            this._port = new InterruptPort(cpuPin, glitchFilterMode == GlitchFilterMode.On, (Port.ResistorMode) resistorMode, (Port.InterruptMode) interruptMode);
        }

        public override void Dispose()
        {
            this._port.Dispose();
        }

        protected override void OnInterruptFirstSubscribed()
        {
            this._port.OnInterrupt += new NativeEventHandler(this.OnPortInterrupt);
        }

        protected override void OnInterruptLastUnsubscribed()
        {
            this._port.OnInterrupt -= new NativeEventHandler(this.OnPortInterrupt);
        }

        private void OnPortInterrupt(uint data1, uint data2, DateTime time)
        {
            base.RaiseInterrupt(data2 > 0);
        }

        public override bool Read()
        {
            return this._port.Read();
        }
    }
}

