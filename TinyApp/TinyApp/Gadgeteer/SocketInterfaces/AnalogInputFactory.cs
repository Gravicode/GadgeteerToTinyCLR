namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT.Hardware;
    using System;

    public static class AnalogInputFactory
    {
        public static Gadgeteer.SocketInterfaces.AnalogInput Create(Socket socket, Socket.Pin pin, Module module)
        {
            socket.EnsureTypeIsSupported('A', module);
            socket.ReservePin(pin, module);
            Cpu.AnalogChannel channel = Cpu.AnalogChannel.ANALOG_NONE;
            switch (pin)
            {
                case Socket.Pin.Three:
                    channel = socket.AnalogInput3;
                    break;

                case Socket.Pin.Four:
                    channel = socket.AnalogInput4;
                    break;

                case Socket.Pin.Five:
                    channel = socket.AnalogInput5;
                    break;
            }
            if ((channel == Cpu.AnalogChannel.ANALOG_NONE) && (socket.AnalogInputIndirector != null))
            {
                return socket.AnalogInputIndirector(socket, pin, module);
            }
            return new NativeAnalogInput(socket, pin, module, channel);
        }
    }
}

