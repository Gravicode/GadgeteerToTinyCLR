namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT.Hardware;
    using System;

    internal class NativeDigitalOutput : DigitalOutput
    {
        private OutputPort _port;

        public NativeDigitalOutput(Socket socket, Socket.Pin pin, bool initialState, Module module, Cpu.Pin cpuPin)
        {
            if (cpuPin == Cpu.Pin.GPIO_NONE)
            {
                throw Socket.InvalidSocketException.FunctionalityException(socket, "DigitalOutput");
            }
            this._port = new OutputPort(cpuPin, initialState);
        }

        public override void Dispose()
        {
            this._port.Dispose();
        }

        public override bool Read()
        {
            return this._port.Read();
        }

        public override void Write(bool state)
        {
            this._port.Write(state);
        }
    }
}

