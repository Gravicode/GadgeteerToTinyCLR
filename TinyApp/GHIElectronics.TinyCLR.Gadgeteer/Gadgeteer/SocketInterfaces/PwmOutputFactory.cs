namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT.Hardware;
    using System;

    public static class PwmOutputFactory
    {
        public static PwmOutput Create(Socket socket, Socket.Pin pin, bool invert, Module module)
        {
            socket.EnsureTypeIsSupported('P', module);
            socket.ReservePin(pin, module);
            Cpu.PWMChannel channel = Cpu.PWMChannel.PWM_NONE;
            switch (pin)
            {
                case Socket.Pin.Seven:
                    channel = socket.PWM7;
                    break;

                case Socket.Pin.Eight:
                    channel = socket.PWM8;
                    break;

                case Socket.Pin.Nine:
                    channel = socket.PWM9;
                    break;
            }
            if ((channel == Cpu.PWMChannel.PWM_NONE) && (socket.PwmOutputIndirector != null))
            {
                return socket.PwmOutputIndirector(socket, pin, invert, module);
            }
            return new NativePwmOutput(socket, pin, invert, module, channel);
        }
    }
}

