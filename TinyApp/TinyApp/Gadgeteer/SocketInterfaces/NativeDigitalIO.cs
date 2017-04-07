namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT.Hardware;
    using System;

    internal class NativeDigitalIO : DigitalIO
    {
        private IOMode _mode;
        private TristatePort _port;

        public NativeDigitalIO(Socket socket, Socket.Pin pin, bool initialState, GlitchFilterMode glitchFilterMode, Gadgeteer.SocketInterfaces.ResistorMode resistorMode, Module module, Cpu.Pin cpuPin)
        {
            if (cpuPin == Cpu.Pin.GPIO_NONE)
            {
                throw Socket.InvalidSocketException.FunctionalityException(socket, "DigitalIO");
            }
            this._port = new TristatePort(cpuPin, initialState, glitchFilterMode == GlitchFilterMode.On, (Port.ResistorMode) resistorMode);
        }

        public override void Dispose()
        {
            this._port.Dispose();
        }

        public override bool Read()
        {
            this.Mode = IOMode.Input;
            return this._port.Read();
        }

        public override void Write(bool state)
        {
            this.Mode = IOMode.Output;
            this._port.Write(state);
        }

        public override IOMode Mode
        {
            get
            {
                return this._mode;
            }
            set
            {
                if (value != IOMode.Input)
                {
                    if ((value == IOMode.Output) && !this._port.Active)
                    {
                        this._port.Active = true;
                    }
                }
                else if (this._port.Active)
                {
                    this._port.Active = false;
                }
                this._mode = value;
            }
        }
    }
}

