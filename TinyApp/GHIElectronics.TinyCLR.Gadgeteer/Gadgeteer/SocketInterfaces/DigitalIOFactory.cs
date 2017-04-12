namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT.Hardware;
    using System;

    public static class DigitalIOFactory
    {
        public static DigitalIO Create(Socket socket, Socket.Pin pin, bool initialState, GlitchFilterMode glitchFilterMode, Gadgeteer.SocketInterfaces.ResistorMode resistorMode, Module module)
        {
            Cpu.Pin cpuPin = socket.ReservePin(pin, module);
            if ((cpuPin == Cpu.Pin.GPIO_NONE) && (socket.DigitalIOIndirector != null))
            {
                return socket.DigitalIOIndirector(socket, pin, initialState, glitchFilterMode, resistorMode, module);
            }
            return new NativeDigitalIO(socket, pin, initialState, glitchFilterMode, resistorMode, module, cpuPin);
        }
    }
}

