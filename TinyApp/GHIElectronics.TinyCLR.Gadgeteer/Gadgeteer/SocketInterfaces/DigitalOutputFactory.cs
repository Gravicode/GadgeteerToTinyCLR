namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT.Hardware;
    using System;

    public static class DigitalOutputFactory
    {
        public static DigitalOutput Create(Socket socket, Socket.Pin pin, bool initialState, Module module)
        {
            Cpu.Pin cpuPin = socket.ReservePin(pin, module);
            if ((cpuPin == Cpu.Pin.GPIO_NONE) && (socket.DigitalOutputIndirector != null))
            {
                return socket.DigitalOutputIndirector(socket, pin, initialState, module);
            }
            return new NativeDigitalOutput(socket, pin, initialState, module, cpuPin);
        }
    }
}

