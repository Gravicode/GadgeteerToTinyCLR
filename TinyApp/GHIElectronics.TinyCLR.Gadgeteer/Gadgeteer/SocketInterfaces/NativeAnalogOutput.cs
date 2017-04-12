namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT.Hardware;
    using System;

    internal class NativeAnalogOutput : Gadgeteer.SocketInterfaces.AnalogOutput
    {
        private Cpu.AnalogOutputChannel _channel;
        private Microsoft.SPOT.Hardware.AnalogOutput _port;
        private Socket _socket;

        public NativeAnalogOutput(Socket socket, Socket.Pin pin, Module module, Cpu.AnalogOutputChannel channel)
        {
            if (channel == Cpu.AnalogOutputChannel.ANALOG_OUTPUT_NONE)
            {
                Socket.InvalidSocketException.ThrowIfOutOfRange(pin, Socket.Pin.Five, Socket.Pin.Five, "AnalogOutput", module);
                throw Socket.InvalidSocketException.FunctionalityException(socket, "AnalogOutput");
            }
            this._channel = channel;
            this._socket = socket;
        }

        public override void Dispose()
        {
            this._port.Dispose();
            this._port = null;
        }

        public override void WriteVoltage(double voltage)
        {
            this.IsActive = true;
            this._port.Write(voltage);
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
                        this._port = new Microsoft.SPOT.Hardware.AnalogOutput(this._channel, this._socket.AnalogOutputScale, this._socket.AnalogOutputOffset, this._socket.AnalogOutputPrecisionInBits);
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

