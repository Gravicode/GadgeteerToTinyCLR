namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT.Hardware;
    using System;

    public static class I2CBusFactory
    {
        public static I2CBus Create(Socket socket, ushort address, int clockRateKhz, Module module)
        {
            return Create(socket, address, clockRateKhz, Socket.Pin.Eight, Socket.Pin.Nine, module);
        }

        public static I2CBus Create(Socket socket, ushort address, int clockRateKhz, Socket.Pin sdaPin, Socket.Pin sclPin, Module module)
        {
            Cpu.Pin pin2;
            Cpu.Pin pin3;
            Cpu.Pin pin = socket.ReservePin(sclPin, module);
            HardwareProvider.HwProvider.GetI2CPins(out pin2, out pin3);
            if ((socket.ReservePin(sdaPin, module) == pin3) && (pin == pin2))
            {
                return new NativeI2CBus(socket, address, clockRateKhz, module);
            }
            if (socket.I2CBusIndirector != null)
            {
                return socket.I2CBusIndirector(socket, sdaPin, sclPin, address, clockRateKhz, module);
            }
            return new SoftwareI2CBus(socket, sdaPin, sclPin, address, clockRateKhz, module);
        }
    }
}

