namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using System;
    using System.Runtime.CompilerServices;

    public delegate I2CBus I2CBusIndirector(Socket socket, Socket.Pin sdaPin, Socket.Pin sclPin, ushort address, int clockRateKHz, Module module);
}

