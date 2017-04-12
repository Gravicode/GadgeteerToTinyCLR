namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT.Hardware;
    using System;

    internal class NativeAnalogInput : Gadgeteer.SocketInterfaces.AnalogInput
    {
        private Cpu.AnalogChannel _channel;
        private Microsoft.SPOT.Hardware.AnalogInput _port;
        private Socket _socket;

        public NativeAnalogInput(Socket socket, Socket.Pin pin, Module module, Cpu.AnalogChannel channel)
        {
            if (channel == Cpu.AnalogChannel.ANALOG_NONE)
            {
                Socket.InvalidSocketException.ThrowIfOutOfRange(pin, Socket.Pin.Three, Socket.Pin.Five, "AnalogInput", module);
                throw Socket.InvalidSocketException.FunctionalityException(socket, "AnalogInput");
            }
            this._channel = channel;
            this._socket = socket;
        }

        public override void Dispose()
        {
            this._port.Dispose();
            this._port = null;
        }

        public override double ReadVoltage()
        {
            this.IsActive = true;
            return this._port.Read();
        }

        public override bool IsActive
        {
            get
            {
                return (this._port > null);
            }
            set
            {
                if ((this._port > null) != value)
                {
                    if (value)
                    {
                        this._port = new Microsoft.SPOT.Hardware.AnalogInput(this._channel, this._socket.AnalogInputScale, this._socket.AnalogInputOffset, this._socket.AnalogInputPrecisionInBits);
                    }
                    else
                    {
                        this._port.Dispose();
                        this._port = null;
                    }
                }
            }
        }
    }
}

