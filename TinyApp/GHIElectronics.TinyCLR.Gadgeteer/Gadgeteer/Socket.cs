namespace Gadgeteer
{
    using Gadgeteer.Modules;
    using Gadgeteer.SocketInterfaces;
    //using Microsoft.SPOT;
    //using Microsoft.SPOT.Hardware;
    using GHIElectronics.TinyCLR.Devices.Gpio;
    using GHIElectronics.TinyCLR.Devices.I2c;
    using GHIElectronics.TinyCLR.Devices.Enumeration;
    using System;
    using System.Collections;
    using System.ComponentModel;

    public class Socket
    {
        private Cpu.AnalogChannel _AnalogInput3 = Cpu.AnalogChannel.ANALOG_NONE;
        private Cpu.AnalogChannel _AnalogInput4 = Cpu.AnalogChannel.ANALOG_NONE;
        private Cpu.AnalogChannel _AnalogInput5 = Cpu.AnalogChannel.ANALOG_NONE;
        private Cpu.AnalogOutputChannel _AnalogOutput5 = Cpu.AnalogOutputChannel.ANALOG_OUTPUT_NONE;
        private Cpu.PWMChannel _PWM7 = Cpu.PWMChannel.PWM_NONE;
        private Cpu.PWMChannel _PWM8 = Cpu.PWMChannel.PWM_NONE;
        private Cpu.PWMChannel _PWM9 = Cpu.PWMChannel.PWM_NONE;
        internal bool _registered;
        private string _serialPortName;
        internal static ArrayList _sockets = new ArrayList();
        private SPI.SPI_module _SPIModule = SocketInterfaces.SPIMissing;
        private char[] _SupportedTypes = new char[0];
        private Cpu.Pin[] <CpuPins>k__BackingField;
        private string <Name>k__BackingField;
        private int <SocketNumber>k__BackingField;
        public Gadgeteer.SocketInterfaces.AnalogInputIndirector AnalogInputIndirector;
        internal double AnalogInputOffset = double.MinValue;
        internal int AnalogInputPrecisionInBits = -2147483648;
        internal double AnalogInputScale = double.MinValue;
        public Gadgeteer.SocketInterfaces.AnalogOutputIndirector AnalogOutputIndirector;
        internal double AnalogOutputOffset = double.MinValue;
        internal int AnalogOutputPrecisionInBits = -2147483648;
        internal double AnalogOutputScale = double.MinValue;
        public Gadgeteer.SocketInterfaces.DigitalInputIndirector DigitalInputIndirector;
        public DigitalOIndirector DigitalIOIndirector;
        public Gadgeteer.SocketInterfaces.DigitalOutputIndirector DigitalOutputIndirector;
        public Gadgeteer.SocketInterfaces.I2CBusIndirector I2CBusIndirector;
        public InterruptInputIndirector InterruptIndirector;
        public Gadgeteer.SocketInterfaces.PwmOutputIndirector PwmOutputIndirector;
        public Gadgeteer.SocketInterfaces.SerialIndirector SerialIndirector;
        public Gadgeteer.SocketInterfaces.SpiIndirector SpiIndirector;
        public static readonly Cpu.Pin UnnumberedPin = ((Cpu.Pin) (-2147483648));
        public static readonly Cpu.Pin UnspecifiedPin = Cpu.Pin.GPIO_NONE;

        internal Socket(int socketNumber, string name)
        {
            this.Name = name;
            this.SocketNumber = socketNumber;
            this.CpuPins = new Cpu.Pin[] { UnspecifiedPin, UnspecifiedPin, UnspecifiedPin, UnspecifiedPin, UnspecifiedPin, UnspecifiedPin, UnspecifiedPin, UnspecifiedPin, UnspecifiedPin, UnspecifiedPin, UnspecifiedPin };
        }

        public void EnsureTypeIsSupported(char type, Module module)
        {
            if (!this.SupportsType(type))
            {
                string[] textArray1 = new string[] { "Socket ", this.Name, " does not support type '", type.ToString(), "'", (module != null) ? ("  required by " + module + " module.") : "." };
                throw new InvalidSocketException(string.Concat(textArray1));
            }
        }

        public void EnsureTypeIsSupported(char[] types, Module module)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (this.SupportsType(types[i]))
                {
                    return;
                }
            }
            string[] textArray1 = new string[] { "Socket ", this.Name, " does not support one of the types '", new string(types), "'", (module != null) ? ("  required by " + module + " module.") : "." };
            throw new InvalidSocketException(string.Concat(textArray1));
        }

        public static Socket GetSocket(int socketNumber, bool throwExceptionIfSocketNumberInvalid, Module module, string socketLabel)
        {
            if (socketLabel == "")
            {
                socketLabel = null;
            }
            if (socketNumber == Unused)
            {
                if (!throwExceptionIfSocketNumberInvalid)
                {
                    return null;
                }
                if (module == null)
                {
                    throw new InvalidSocketException("Cannot get Socket for socket number Socket.NotConnected");
                }
                string str = "Module " + module;
                if (socketLabel != null)
                {
                    str = str + " socket " + socketLabel;
                }
                throw new InvalidSocketException(str + " must have a valid socket number specified (it does not support Socket.Unused)");
            }
            ArrayList list = _sockets;
            lock (list)
            {
                for (int i = 0; i < _sockets.Count; i++)
                {
                    Socket socket = (Socket) _sockets[i];
                    if (socket.SocketNumber == socketNumber)
                    {
                        return socket;
                    }
                }
            }
            if (!throwExceptionIfSocketNumberInvalid)
            {
                return null;
            }
            if (module == null)
            {
                throw new InvalidSocketException("Invalid socket number " + socketNumber + " specified.");
            }
            string str2 = "Module " + module;
            if (socketLabel != null)
            {
                str2 = str2 + " socket " + socketLabel;
            }
            throw new InvalidSocketException(str2 + " cannot be used with invalid socket number " + socketNumber);
        }

        public Cpu.Pin ReservePin(Pin pin, Module module)
        {
            if (pin == Pin.None)
            {
                return UnspecifiedPin;
            }
            Cpu.Pin pin2 = this.CpuPins[(int) pin];
            if (pin2 == UnspecifiedPin)
            {
                throw new PinMissingException(this, pin);
            }
            if (pin2 == UnnumberedPin)
            {
                return Cpu.Pin.GPIO_NONE;
            }
            if (!(module is Module.DisplayModule) && ((this.SupportsType('R') || this.SupportsType('G')) || this.SupportsType('B')))
            {
                Program.Mainboard.EnsureRgbSocketPinsAvailable();
            }
            return pin2;
        }

        public bool SupportsType(char type)
        {
            return (Array.IndexOf(this._SupportedTypes, type) >= 0);
        }

        public override string ToString()
        {
            return this.Name;
        }

        public Cpu.AnalogChannel AnalogInput3
        {
            get
            {
                return this._AnalogInput3;
            }
            set
            {
                if (this._registered)
                {
                    throw new SocketImmutableAfterRegistrationException();
                }
                this._AnalogInput3 = value;
            }
        }

        public Cpu.AnalogChannel AnalogInput4
        {
            get
            {
                return this._AnalogInput4;
            }
            set
            {
                if (this._registered)
                {
                    throw new SocketImmutableAfterRegistrationException();
                }
                this._AnalogInput4 = value;
            }
        }

        public Cpu.AnalogChannel AnalogInput5
        {
            get
            {
                return this._AnalogInput5;
            }
            set
            {
                if (this._registered)
                {
                    throw new SocketImmutableAfterRegistrationException();
                }
                this._AnalogInput5 = value;
            }
        }

        public Cpu.AnalogOutputChannel AnalogOutput5
        {
            get
            {
                return this._AnalogOutput5;
            }
            set
            {
                if (this._registered)
                {
                    throw new SocketImmutableAfterRegistrationException();
                }
                this._AnalogOutput5 = value;
            }
        }

        public Cpu.Pin[] CpuPins
        {
            get
            {
                return this.<CpuPins>k__BackingField;
            }
            private set
            {
                this.<CpuPins>k__BackingField = value;
            }
        }

        public string Name
        {
            get
            {
                return this.<Name>k__BackingField;
            }
            private set
            {
                this.<Name>k__BackingField = value;
            }
        }

        public Cpu.PWMChannel PWM7
        {
            get
            {
                return this._PWM7;
            }
            set
            {
                if (this._registered)
                {
                    throw new SocketImmutableAfterRegistrationException();
                }
                this._PWM7 = value;
            }
        }

        public Cpu.PWMChannel PWM8
        {
            get
            {
                return this._PWM8;
            }
            set
            {
                if (this._registered)
                {
                    throw new SocketImmutableAfterRegistrationException();
                }
                this._PWM8 = value;
            }
        }

        public Cpu.PWMChannel PWM9
        {
            get
            {
                return this._PWM9;
            }
            set
            {
                if (this._registered)
                {
                    throw new SocketImmutableAfterRegistrationException();
                }
                this._PWM9 = value;
            }
        }

        public string SerialPortName
        {
            get
            {
                return this._serialPortName;
            }
            set
            {
                if (this._registered)
                {
                    throw new SocketImmutableAfterRegistrationException();
                }
                this._serialPortName = value;
            }
        }

        public int SocketNumber
        {
            get
            {
                return this.<SocketNumber>k__BackingField;
            }
            private set
            {
                this.<SocketNumber>k__BackingField = value;
            }
        }

        public SPI.SPI_module SPIModule
        {
            get
            {
                return this._SPIModule;
            }
            set
            {
                if (this._registered)
                {
                    throw new SocketImmutableAfterRegistrationException();
                }
                this._SPIModule = value;
            }
        }

        public char[] SupportedTypes
        {
            get
            {
                return this._SupportedTypes;
            }
            set
            {
                if (this._registered)
                {
                    throw new SocketImmutableAfterRegistrationException();
                }
                this._SupportedTypes = value;
            }
        }

        public static int Unused
        {
            get
            {
                return -2147483648;
            }
        }

        public class InvalidSocketException : ArgumentException
        {
            public InvalidSocketException(string message) : base(message)
            {
            }

            public InvalidSocketException(string message, Exception e) : base(message, e)
            {
            }

            [EditorBrowsable((EditorBrowsableState) EditorBrowsableState.Never)]
            public static Socket.InvalidSocketException FunctionalityException(Socket socket, string iface)
            {
                object[] objArray1 = new object[] { "Socket ", socket, " has an error with its ", iface, " functionality. Please try a different socket." };
                return new Socket.InvalidSocketException(string.Concat(objArray1));
            }

            [EditorBrowsable((EditorBrowsableState) EditorBrowsableState.Never)]
            public static void ThrowIfOutOfRange(Socket.Pin pin, Socket.Pin from, Socket.Pin to, string iface, Module module)
            {
                if ((pin < from) || (pin > to))
                {
                    object[] objArray1 = new object[] { "Cannot use ", iface, " interface on pin ", pin, " - pin must be in range ", from, " to ", to, "." };
                    string message = string.Concat(objArray1);
                    if (module != null)
                    {
                        message = "Module " + module + ": ";
                    }
                    throw new Socket.InvalidSocketException(message);
                }
            }
        }

        public enum Pin
        {
            None,
            One,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten
        }

        public class PinMissingException : ApplicationException
        {
            internal PinMissingException(Socket socket, Socket.Pin pin) : base(string.Concat(new object[] { "\nPin ", (int) pin, " on socket ", socket, " is not connected to a valid CPU pin." }))
            {
            }
        }

        public class SocketImmutableAfterRegistrationException : InvalidOperationException
        {
            internal SocketImmutableAfterRegistrationException() : base("Socket data is immutable after socket is registered.")
            {
            }
        }

        public static class SocketInterfaces
        {
            private static int autoSocketNumber = -10;
            private static bool DoRegistrationChecks = true;
            public static readonly SPI.SPI_module SPIMissing = ~SPI.SPI_module.SPI1;

            public static Socket CreateNumberedSocket(int socketNumber)
            {
                return new Socket(socketNumber, socketNumber.ToString());
            }

            public static Socket CreateUnnumberedSocket(string name)
            {
                int autoSocketNumber;
                ArrayList list = Socket._sockets;
                lock (list)
                {
                    while (Socket.GetSocket(Socket.SocketInterfaces.autoSocketNumber, false, null, null) != null)
                    {
                        Socket.SocketInterfaces.autoSocketNumber--;
                    }
                    autoSocketNumber = Socket.SocketInterfaces.autoSocketNumber;
                    Socket.SocketInterfaces.autoSocketNumber--;
                }
                return new Socket(autoSocketNumber, name);
            }

            public static void RegisterSocket(Socket socket)
            {
                if (DoRegistrationChecks)
                {
                    if ((socket.CpuPins == null) || (socket.CpuPins.Length != 11))
                    {
                        SocketRegistrationError(socket, "CpuPins array must be of length 11");
                    }
                    if ((socket.SupportedTypes == null) || (socket.SupportedTypes.Length == 0))
                    {
                        SocketRegistrationError(socket, "SupportedTypes list is null/empty");
                    }
                    foreach (char ch in socket.SupportedTypes)
                    {
                        switch (ch)
                        {
                            case 'A':
                            {
                                TestPinsPresent(socket, new int[] { 3, 4, 5, 6 }, ch);
                                if (socket.AnalogInputIndirector == null)
                                {
                                    if (((socket.AnalogInput3 == Cpu.AnalogChannel.ANALOG_NONE) || (socket.AnalogInput4 == Cpu.AnalogChannel.ANALOG_NONE)) || (socket.AnalogInput5 == Cpu.AnalogChannel.ANALOG_NONE))
                                    {
                                        SocketRegistrationError(socket, "Socket of type A must support analog input functionality on pins 3, 4 and 5");
                                    }
                                    if (((socket.AnalogInputScale == double.MinValue) || (socket.AnalogInputOffset == double.MinValue)) || (socket.AnalogInputPrecisionInBits == -2147483648))
                                    {
                                        SocketRegistrationError(socket, "Socket of type A must provide analog input scale/offset through calling SocketInterfaces.SetAnalogInputFactors");
                                    }
                                }
                                continue;
                            }
                            case 'B':
                            {
                                TestPinsPresent(socket, new int[] { 3, 4, 5, 6, 7, 8, 9 }, ch);
                                continue;
                            }
                            case 'C':
                            {
                                TestPinsPresent(socket, new int[] { 3, 4, 5, 6 }, ch);
                                continue;
                            }
                            case 'D':
                            {
                                TestPinsPresent(socket, new int[] { 3, 6, 7 }, ch);
                                continue;
                            }
                            case 'E':
                            {
                                TestPinsPresent(socket, new int[] { 6, 7, 8, 9 }, ch);
                                continue;
                            }
                            case 'F':
                            {
                                TestPinsPresent(socket, new int[] { 3, 4, 5, 6, 7, 8, 9 }, ch);
                                continue;
                            }
                            case 'G':
                            {
                                TestPinsPresent(socket, new int[] { 3, 4, 5, 6, 7, 8, 9 }, ch);
                                continue;
                            }
                            case 'H':
                            {
                                int[] pins = new int[] { 3 };
                                TestPinsPresent(socket, pins, ch);
                                continue;
                            }
                            case 'I':
                            {
                                TestPinsPresent(socket, new int[] { 3, 6, 8, 9 }, ch);
                                continue;
                            }
                            case 'K':
                            {
                                TestPinsPresent(socket, new int[] { 3, 4, 5, 6, 7 }, ch);
                                if ((socket.SerialIndirector == null) && (socket.SerialPortName == null))
                                {
                                    SocketRegistrationError(socket, "Socket of type K must specify serial port name");
                                }
                                continue;
                            }
                            case 'O':
                            {
                                TestPinsPresent(socket, new int[] { 3, 4, 5 }, ch);
                                if (socket.AnalogOutputIndirector == null)
                                {
                                    if (socket.AnalogOutput5 == Cpu.AnalogOutputChannel.ANALOG_OUTPUT_NONE)
                                    {
                                        SocketRegistrationError(socket, "Socket of type O must support analog output functionality");
                                    }
                                    if (((socket.AnalogOutputScale == double.MinValue) || (socket.AnalogOutputOffset == double.MinValue)) || (socket.AnalogOutputPrecisionInBits == -2147483648))
                                    {
                                        SocketRegistrationError(socket, "Socket of type O must provide analog output scale/offset through calling SocketInterfaces.SetAnalogOutputFactors");
                                    }
                                }
                                continue;
                            }
                            case 'P':
                            {
                                TestPinsPresent(socket, new int[] { 3, 6, 7, 8, 9 }, ch);
                                if ((socket.PwmOutputIndirector == null) && (((socket.PWM7 == Cpu.PWMChannel.PWM_NONE) || (socket.PWM8 == Cpu.PWMChannel.PWM_NONE)) || (socket.PWM9 == Cpu.PWMChannel.PWM_NONE)))
                                {
                                    SocketRegistrationError(socket, "Socket of type P must support PWM functionality");
                                }
                                continue;
                            }
                            case 'R':
                            {
                                TestPinsPresent(socket, new int[] { 3, 4, 5, 6, 7, 8, 9 }, ch);
                                continue;
                            }
                            case 'S':
                            {
                                TestPinsPresent(socket, new int[] { 3, 4, 5, 6, 7, 8, 9 }, ch);
                                if ((socket.SpiIndirector == null) && (socket.SPIModule == SPIMissing))
                                {
                                    SocketRegistrationError(socket, "Socket of type S must specify SPI module number");
                                }
                                continue;
                            }
                            case 'T':
                            {
                                TestPinsPresent(socket, new int[] { 4, 5, 6, 7 }, ch);
                                continue;
                            }
                            case 'U':
                            {
                                TestPinsPresent(socket, new int[] { 3, 4, 5, 6 }, ch);
                                if ((socket.SerialIndirector == null) && (socket.SerialPortName == null))
                                {
                                    SocketRegistrationError(socket, "Socket of type U must specify serial port name");
                                }
                                continue;
                            }
                            case 'X':
                            {
                                TestPinsPresent(socket, new int[] { 3, 4, 5 }, ch);
                                continue;
                            }
                            case 'Y':
                            {
                                TestPinsPresent(socket, new int[] { 3, 4, 5, 6, 7, 8, 9 }, ch);
                                continue;
                            }
                            case 'Z':
                            {
                                continue;
                            }
                            case '*':
                            {
                                TestPinsPresent(socket, new int[] { 3, 4, 5 }, ch);
                                continue;
                            }
                        }
                        SocketRegistrationError(socket, "Socket type '" + ch.ToString() + "' is not supported by Gadgeteer");
                    }
                }
                ArrayList list = Socket._sockets;
                lock (list)
                {
                    if (Socket.GetSocket(socket.SocketNumber, false, null, null) != null)
                    {
                        throw new Socket.InvalidSocketException("Cannot register socket - socket number " + socket.SocketNumber + " already used");
                    }
                    Socket._sockets.Add(socket);
                    socket._registered = true;
                }
            }

            public static void SetAnalogInputFactors(Socket socket, double scale, double offset, int precisionInBits)
            {
                socket.AnalogInputScale = scale;
                socket.AnalogInputOffset = offset;
                socket.AnalogInputPrecisionInBits = precisionInBits;
            }

            public static void SetAnalogOutputFactors(Socket socket, double scale, double offset, int precisionInBits)
            {
                socket.AnalogOutputScale = scale;
                socket.AnalogOutputOffset = offset;
                socket.AnalogOutputPrecisionInBits = precisionInBits;
            }

            private static void SocketRegistrationError(Socket socket, string message)
            {
                Debug.WriteLine(string.Concat(new object[] { "Warning: socket ", socket, " is not compliant with Gadgeteer : ", message }));
            }

            private static void TestPinsPresent(Socket socket, int[] pins, char type)
            {
                for (int i = 0; i < pins.Length; i++)
                {
                    if (socket.CpuPins[pins[i]] == Socket.UnspecifiedPin)
                    {
                        SocketRegistrationError(socket, string.Concat(new object[] { "Cpu pin ", pins[i], " must be specified for socket of type ", type.ToString() }));
                    }
                }
            }
        }
    }
}

