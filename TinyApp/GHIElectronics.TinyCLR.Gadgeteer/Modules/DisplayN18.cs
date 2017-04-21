using GHIElectronics.TinyCLR.Devices.Gpio;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Spi;
using GT = Gadgeteer;
//using GTI = Gadgeteer.SocketInterfaces;
using GTM = Gadgeteer.Modules;

namespace Gadgeteer.Modules.GHIElectronics
{
    /// <summary>A CharacterDisplay module for Microsoft .NET Gadgeteer</summary>
    public class DisplayN18 : Module
    {
        private const byte St7735Madctl = 0x36;
        private const byte MadctlMy = 0x80;
        private const byte MadctlMx = 0x40;
        private const byte MadctlMv = 0x20;
        private const byte MadctlBgr = 0x08;

        //private GTI.Spi spi;
        //private GTI.SpiConfiguration spiConfig;
        //private SPI.Configuration netMFSpiConfig;

        private GpioPin _resetPin;
        private readonly GpioPin _backlightPin;
        private GpioPin _rsPin;

        private byte[] _byteArray;
        private ushort[] _shortArray;
        private bool _isBgr;

        /// <summary>Whether or not the backlight is enabled.</summary>
        public bool BacklightEnabled
        {
            get
            {
                return _backlightPin.Read()==GpioPinValue.High;
            }
            set
            {
                _backlightPin.Write(value?GpioPinValue.High : GpioPinValue.Low);
            }
        }

        /// <summary>Constructs a new instance.</summary>
        /// <param name="socketNumber">The socket that this module is plugged in to.</param>
        public DisplayN18(int digitalResetPin3, int digitalBackLightPin4,int digitalRsPin5)
        {
            _byteArray = new byte[1];
            _shortArray = new ushort[2];
            _isBgr = true;

            var controller = GpioController.GetDefault();

            _resetPin = controller.OpenPin(digitalResetPin3);
            _resetPin.SetDriveMode(GpioPinDriveMode.Output);

            _backlightPin = controller.OpenPin(digitalBackLightPin4);
            _backlightPin.SetDriveMode(GpioPinDriveMode.Output);

            _rsPin = controller.OpenPin(digitalRsPin5);
            _rsPin.SetDriveMode(GpioPinDriveMode.Output);

            //spiConfig = new GTI.SpiConfiguration(false, 0, 0, false, true, 12000);
            //netMFSpiConfig = new SPI.Configuration(this.socket.CpuPins[6], spiConfig.IsChipSelectActiveHigh, spiConfig.ChipSelectSetupTime, spiConfig.ChipSelectHoldTime, spiConfig.IsClockIdleHigh, spiConfig.IsClockSamplingEdgeRising, spiConfig.ClockRateKHz, this.socket.SPIModule);
            //spi = GTI.SpiFactory.Create(this.socket, spiConfig, GTI.SpiSharing.Shared, this.socket, Socket.Pin.Six, this);

            Reset();

            ConfigureDisplay();

            //base.OnDisplayConnected("Display N18", 128, 160, DisplayOrientation.Normal, null);

            Clear();
        }

        private void Clear()
        {
            // TODO
        }

        private void ConfigureDisplay()
        {
            // TODO
        }

        private void Reset()
        {
            // TODO
        }
    }
}
