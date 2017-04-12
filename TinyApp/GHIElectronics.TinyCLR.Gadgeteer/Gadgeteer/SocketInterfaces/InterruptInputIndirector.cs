namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using System;
    using System.Runtime.CompilerServices;

    public delegate InterruptInput InterruptInputIndirector(Socket socket, Socket.Pin pin, GlitchFilterMode glitchFilterMode, ResistorMode resistorMode, InterruptMode interruptMode, Module module);
}

