namespace Gadgeteer.SocketInterfaces
{
    using System;

    public enum PwmScaleFactor : uint
    {
        Microseconds = 0xf4240,
        Milliseconds = 0x3e8,
        Nanoseconds = 0x3b9aca00
    }
}

