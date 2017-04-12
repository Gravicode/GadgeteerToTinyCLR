namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT.Hardware;
    using System;

    internal class NativeDigitalInput : DigitalInput
    {
        private InputPort _port;

        public NativeDigitalInput(Socket socket, Socket.Pin pin, GlitchFilterMode glitchFilterMode, Gadgeteer.SocketInterfaces.ResistorMode resistorMode, Module module, Cpu.Pin cpuPin)
        {
            if (cpuPin == Cpu.Pin.GPIO_NONE)
            {
                throw Socket.InvalidSocketException.FunctionalityException(socket, "DigitalInput");
            }
            this._port = new InputPort(cpuPin, glitchFilterMode == GlitchFilterMode.On, (Port.ResistorMode) resistorMode);
        }

        public override void Dispose()
        {
            this._port.Dispose();
        }

        public override bool Read()
        {
            return this._port.Read();
        }
    }
}

