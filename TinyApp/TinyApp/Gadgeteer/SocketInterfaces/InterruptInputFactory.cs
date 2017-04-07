namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT.Hardware;
    using System;

    public static class InterruptInputFactory
    {
        public static InterruptInput Create(Socket socket, Socket.Pin pin, GlitchFilterMode glitchFilterMode, Gadgeteer.SocketInterfaces.ResistorMode resistorMode, Gadgeteer.SocketInterfaces.InterruptMode interruptMode, Module module)
        {
            Cpu.Pin cpuPin = socket.ReservePin(pin, module);
            if ((cpuPin == Cpu.Pin.GPIO_NONE) && (socket.InterruptIndirector != null))
            {
                return socket.InterruptIndirector(socket, pin, glitchFilterMode, resistorMode, interruptMode, module);
            }
            return new NativeInterruptInput(socket, pin, glitchFilterMode, resistorMode, interruptMode, module, cpuPin);
        }
    }
}

