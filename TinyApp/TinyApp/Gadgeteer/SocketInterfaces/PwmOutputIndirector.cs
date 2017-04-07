namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using System;
    using System.Runtime.CompilerServices;

    public delegate PwmOutput PwmOutputIndirector(Socket socket, Socket.Pin pin, bool invert, Module module);
}

