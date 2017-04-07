namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT.Hardware;
    using System;

    public static class AnalogOutputFactory
    {
        public static Gadgeteer.SocketInterfaces.AnalogOutput Create(Socket socket, Socket.Pin pin, Module module)
        {
            socket.EnsureTypeIsSupported('O', module);
            socket.ReservePin(pin, module);
            Cpu.AnalogOutputChannel channel = socket.AnalogOutput5;
            if ((channel == Cpu.AnalogOutputChannel.ANALOG_OUTPUT_NONE) && (socket.AnalogOutputIndirector != null))
            {
                return socket.AnalogOutputIndirector(socket, pin, module);
            }
            return new NativeAnalogOutput(socket, pin, module, channel);
        }
    }
}

