namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using System;
    using System.Runtime.CompilerServices;

    public delegate Spi SpiIndirector(Socket socket, SpiConfiguration spiConfiguration, SpiSharing sharingMode, Socket chipSelectSocket, Socket.Pin chipSelectSocketPin, Socket busySocket, Socket.Pin busySocketPin, Module module);
}

