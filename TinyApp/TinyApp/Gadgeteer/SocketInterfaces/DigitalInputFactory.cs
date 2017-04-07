namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT.Hardware;
    using System;

    public static class DigitalInputFactory
    {
        public static DigitalInput Create(Socket socket, Socket.Pin pin, GlitchFilterMode glitchFilterMode, Gadgeteer.SocketInterfaces.ResistorMode resistorMode, Module module)
        {
            Cpu.Pin cpuPin = socket.ReservePin(pin, module);
            if ((cpuPin == Cpu.Pin.GPIO_NONE) && (socket.DigitalInputIndirector != null))
            {
                return socket.DigitalInputIndirector(socket, pin, glitchFilterMode, resistorMode, module);
            }
            return new NativeDigitalInput(socket, pin, glitchFilterMode, resistorMode, module, cpuPin);
        }
    }
}

