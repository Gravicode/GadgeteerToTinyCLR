namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT.Hardware;
    using System;

    internal class NativePwmOutput : PwmOutput
    {
        private Cpu.PWMChannel _channel;
        private bool _invert;
        private PWM _port;
        private Socket _socket;
        private bool _started;

        public NativePwmOutput(Socket socket, Socket.Pin pin, bool invert, Module module, Cpu.PWMChannel channel)
        {
            if (channel == Cpu.PWMChannel.PWM_NONE)
            {
                Socket.InvalidSocketException.ThrowIfOutOfRange(pin, Socket.Pin.Seven, Socket.Pin.Nine, "PWM", module);
                throw Socket.InvalidSocketException.FunctionalityException(socket, "PWM");
            }
            this._channel = channel;
            this._socket = socket;
            this._invert = invert;
        }

        public override void Dispose()
        {
            this._port.Dispose();
        }

        public override void Set(double frequency, double dutyCycle)
        {
            if (frequency < 0.0)
            {
                throw new ArgumentException("frequency");
            }
            if ((dutyCycle < 0.0) || (dutyCycle > 1.0))
            {
                throw new ArgumentException("dutyCycle");
            }
            if (this._port == null)
            {
                this._port = new PWM(this._channel, frequency, dutyCycle, this._invert);
                this._port.Start();
                this._started = true;
            }
            else
            {
                if (this._started)
                {
                    this._port.Stop();
                }
                this._port.Frequency = frequency;
                this._port.DutyCycle = dutyCycle;
                this._port.Start();
                this._started = true;
            }
        }

        public override void Set(uint period, uint highTime, PwmScaleFactor factor)
        {
            if (this._port == null)
            {
                this._port = new PWM(this._channel, period, highTime, (PWM.ScaleFactor) factor, this._invert);
                this._port.Start();
                this._started = true;
            }
            else
            {
                if (this._started)
                {
                    this._port.Stop();
                }
                this._port.Scale = (PWM.ScaleFactor) factor;
                this._port.Period = period;
                this._port.Duration = highTime;
                this._port.Start();
                this._started = true;
            }
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
                        this._port = new PWM(this._channel, 1.0, 0.5, this._invert);
                        this._started = false;
                    }
                    else
                    {
                        if (this._started)
                        {
                            this._port.Stop();
                        }
                        this._port.Dispose();
                        this._port = null;
                    }
                }
            }
        }
    }
}

