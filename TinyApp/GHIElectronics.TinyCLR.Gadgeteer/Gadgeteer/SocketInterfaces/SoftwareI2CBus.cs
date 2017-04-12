namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT;
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class SoftwareI2CBus : I2CBus
    {
        private byte address;
        private int clockRateKHz;
        public static bool ForceManagedPullUps;
        private static Hashtable ReservedSclPinPorts = new Hashtable();
        private static Hashtable ReservedSdaPinPorts = new Hashtable();
        private Socket.Pin sclPin;
        private DigitalIO sclPort;
        private Socket.Pin sdaPin;
        private DigitalIO sdaPort;
        private Socket socket;
        private static ArrayList SoftwareI2CTimeoutList = new ArrayList();
        private static bool threadExit;
        private bool timeout;
        private int timeoutCount;
        private const int timeoutLoopCount = 10;
        private const int timeoutLoopDelay = 100;
        private static Thread timeoutThread = null;

        public SoftwareI2CBus(Socket socket, Socket.Pin sdaPin, Socket.Pin sclPin, ushort address, int clockRateKHz, Module module)
        {
            this.address = (byte) address;
            this.clockRateKHz = clockRateKHz;
            string key = socket.ToString() + "___" + sdaPin;
            if (!ReservedSdaPinPorts.Contains(key))
            {
                this.sdaPort = DigitalIOFactory.Create(socket, sdaPin, false, GlitchFilterMode.Off, ForceManagedPullUps ? ResistorMode.PullUp : ResistorMode.Disabled, module);
                ReservedSdaPinPorts.Add(key, this.sdaPort);
            }
            else
            {
                this.sdaPort = (DigitalIO) ReservedSdaPinPorts[key];
            }
            string str2 = socket.ToString() + "___" + sclPin;
            if (!ReservedSclPinPorts.Contains(str2))
            {
                this.sclPort = DigitalIOFactory.Create(socket, sclPin, false, GlitchFilterMode.Off, ForceManagedPullUps ? ResistorMode.PullUp : ResistorMode.Disabled, module);
                ReservedSclPinPorts.Add(str2, this.sclPort);
            }
            else
            {
                this.sclPort = (DigitalIO) ReservedSclPinPorts[str2];
            }
            this.socket = socket;
            this.sdaPin = sdaPin;
            this.sclPin = sclPin;
            ArrayList list = SoftwareI2CTimeoutList;
            lock (list)
            {
                this.timeoutCount = -1;
                SoftwareI2CTimeoutList.Add(this);
                if (timeoutThread == null)
                {
                    threadExit = false;
                    timeoutThread = new Thread(new ThreadStart(SoftwareI2CBus.TimeoutHandler));
                    timeoutThread.Start();
                }
            }
        }

        private void DoManagedWriteRead(byte address, byte[] writeBuffer, int writeOffset, int writeLength, byte[] readBuffer, int readOffset, int readLength, out int numWritten, out int numRead)
        {
            DigitalIO sdaPort = this.sdaPort;
            lock (sdaPort)
            {
                ArrayList list = SoftwareI2CTimeoutList;
                lock (list)
                {
                    this.timeout = false;
                    this.timeoutCount = 0;
                }
                numWritten = 0;
                numRead = 0;
                if (writeLength != 0)
                {
                    if (!this.sclPort.Read())
                    {
                        throw new ApplicationException("Software I2C: clock signal on socket " + this.socket + " is being held low.");
                    }
                    this.sdaPort.Mode = IOMode.Output;
                    this.sclPort.Mode = IOMode.Output;
                    if (this.WriteByte((byte) (address << 1)))
                    {
                        for (int i = writeOffset; i < (writeOffset + writeLength); i++)
                        {
                            if (!this.WriteByte(writeBuffer[i]))
                            {
                                break;
                            }
                            numWritten++;
                        }
                    }
                    if ((readLength == 0) || (numWritten != writeLength))
                    {
                        this.sclPort.Mode = IOMode.Input;
                        while (!this.sclPort.Read() && !this.timeout)
                        {
                        }
                        this.sdaPort.Mode = IOMode.Input;
                    }
                    else
                    {
                        this.sdaPort.Mode = IOMode.Input;
                        while (!this.sdaPort.Read() && !this.timeout)
                        {
                        }
                        this.sclPort.Mode = IOMode.Input;
                    }
                }
                if (this.timeout)
                {
                    throw new ApplicationException("Software I2C: clock signal on socket " + this.socket + " is being held low.");
                }
                if ((numWritten == writeLength) && (readLength != 0))
                {
                    if (!this.sclPort.Read())
                    {
                        throw new ApplicationException("Software I2C: clock signal on socket " + this.socket + " is being held low.");
                    }
                    this.sdaPort.Mode = IOMode.Output;
                    this.sclPort.Mode = IOMode.Output;
                    if (this.WriteByte((byte) ((address << 1) | 1)))
                    {
                        int num2 = (readOffset + readLength) - 1;
                        for (int j = readOffset; j < (readOffset + readLength); j++)
                        {
                            if (!this.ReadByte(j == num2, out readBuffer[j]))
                            {
                                break;
                            }
                            numRead++;
                        }
                    }
                    this.sclPort.Mode = IOMode.Input;
                    while (!this.sclPort.Read() & !this.timeout)
                    {
                    }
                    this.sdaPort.Mode = IOMode.Input;
                }
                if (this.timeout)
                {
                    throw new ApplicationException("Software I2C: clock signal on socket " + this.socket + " is being held low.");
                }
                list = SoftwareI2CTimeoutList;
                lock (list)
                {
                    this.timeoutCount = -1;
                }
            }
        }

        private bool ReadByte(bool last, out byte data)
        {
            data = 0;
            this.sdaPort.Mode = IOMode.Input;
            for (int i = 0; i < 8; i++)
            {
                data = (byte) (data << 1);
                this.sclPort.Mode = IOMode.Input;
                while (!this.sclPort.Read() && !this.timeout)
                {
                }
                if (this.sdaPort.Read())
                {
                    data = (byte) (data | 1);
                }
                this.sclPort.Mode = IOMode.Output;
            }
            if (!last)
            {
                this.sdaPort.Mode = IOMode.Output;
            }
            this.sclPort.Mode = IOMode.Input;
            while (!this.sclPort.Read() && !this.timeout)
            {
            }
            this.sclPort.Mode = IOMode.Output;
            if (last)
            {
                this.sdaPort.Mode = IOMode.Output;
            }
            if (!this.timeout)
            {
                ArrayList list = SoftwareI2CTimeoutList;
                lock (list)
                {
                    this.timeoutCount = 0;
                    return true;
                }
            }
            return false;
        }

        private static void TimeoutHandler()
        {
            while (!threadExit)
            {
                try
                {
                    Thread.Sleep(100);
                    ArrayList list = SoftwareI2CTimeoutList;
                    lock (list)
                    {
                        foreach (SoftwareI2CBus bus in SoftwareI2CTimeoutList)
                        {
                            if (bus.timeoutCount >= 0)
                            {
                                bus.timeoutCount++;
                                if (bus.timeoutCount >= 10)
                                {
                                    bus.timeout = true;
                                    bus.timeoutCount = -1;
                                }
                            }
                        }
                    }
                    continue;
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("Exception in SoftwareI2C timeout handling: " + exception);
                    continue;
                }
            }
        }

        private bool WriteByte(byte data)
        {
            byte num = 0;
            for (int i = 0; i < 8; i++)
            {
                byte num2 = (byte) (data & 0x80);
                if (num2 != num)
                {
                    if (num2 == 0)
                    {
                        this.sdaPort.Mode = IOMode.Output;
                    }
                    else
                    {
                        this.sdaPort.Mode = IOMode.Input;
                    }
                    num = num2;
                }
                data = (byte) (data << 1);
                this.sclPort.Mode = IOMode.Input;
                while (!this.sclPort.Read() && !this.timeout)
                {
                }
                this.sclPort.Mode = IOMode.Output;
            }
            if (num == 0)
            {
                this.sdaPort.Mode = IOMode.Input;
            }
            this.sclPort.Mode = IOMode.Input;
            while (!this.sclPort.Read() && !this.timeout)
            {
            }
            this.sclPort.Mode = IOMode.Output;
            this.sdaPort.Mode = IOMode.Output;
            if (!this.sdaPort.Read() && !this.timeout)
            {
                ArrayList list = SoftwareI2CTimeoutList;
                lock (list)
                {
                    this.timeoutCount = 0;
                    return true;
                }
            }
            return false;
        }

        public override void WriteRead(byte[] writeBuffer, int writeOffset, int writeLength, byte[] readBuffer, int readOffset, int readLength, out int numWritten, out int numRead)
        {
            if (writeBuffer == null)
            {
                writeOffset = writeLength = 0;
            }
            else if (((writeBuffer.Length < (writeOffset + writeLength)) || (writeOffset < 0)) || (writeLength < 0))
            {
                object[] objArray1 = new object[] { "SoftwareI2C: WriteRead call to device at address ", this.address, " on socket ", this.socket, " has bad writeBuffer parameters (buffer too small or negative length or offset specified)" };
                throw new ArgumentException(string.Concat(objArray1));
            }
            if (readBuffer == null)
            {
                readOffset = readLength = 0;
            }
            else if (((readBuffer.Length < (readOffset + readLength)) || (readOffset < 0)) || (readLength < 0))
            {
                object[] objArray2 = new object[] { "SoftwareI2C: WriteRead call to device at address ", this.address, " on socket ", this.socket, " has bad readBuffer parameters (buffer too small or negative length or offset specified)" };
                throw new ArgumentException(string.Concat(objArray2));
            }
            this.DoManagedWriteRead(this.address, writeBuffer, writeOffset, writeLength, readBuffer, readOffset, readLength, out numWritten, out numRead);
            if (((base.LengthErrorBehavior == ErrorBehavior.ThrowException) && (writeLength != numWritten)) && (readLength != numRead))
            {
                throw base.NewLengthErrorException();
            }
        }

        public override ushort Address
        {
            get
            {
                return this.address;
            }
            set
            {
                this.address = (byte) value;
            }
        }

        public override int ClockRateKHz
        {
            get
            {
                return this.clockRateKHz;
            }
            set
            {
                this.clockRateKHz = value;
            }
        }

        public override int Timeout
        {
            get
            {
                return 0x3e8;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}

