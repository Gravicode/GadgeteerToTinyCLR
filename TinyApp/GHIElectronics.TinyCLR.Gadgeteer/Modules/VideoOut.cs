﻿//using Microsoft.SPOT;
using GHIElectronics.TinyCLR.Devices.Display;
using GHIElectronics.TinyCLR.Devices.Enumeration;
using GHIElectronics.TinyCLR.Devices.I2c;
using System;
using System.Drawing;
using System.Threading;
using GT = Gadgeteer;
//using GTI = Gadgeteer.SocketInterfaces;
using GTM = Gadgeteer.Modules;

namespace Gadgeteer.Modules.GHIElectronics {
	/// <summary>A VideoOut module for Microsoft .NET Gadgeteer</summary>
	public class VideoOut : GTM.Module {
		private I2cDevice i2c;
        const int I2C_ADDRESS = 0x76;
        public Graphics Screen { set; get; }
        private uint currentWidth;
		private uint currentHeight;

		/// <summary>The input resolution of the module.</summary>
		public enum Resolution {

			/// <summary>Represents the values for an RCA display in the NTSC standard with a resolution of 320x240.</summary>
			Rca320x240 = 0,

			/// <summary>Represents the values for an RCA display in the NTSC standard with a resolution of 640x480.</summary>
			Rca640x480 = 1,

			/// <summary>Represents the values for an RCA display in the NTSC standard with a resolution of 800x600.</summary>
			Rca800x600 = 2,

			/// <summary>Represents the values for a VGA display with a resolution of 320x240.</summary>
			Vga320x240 = 10,

			/// <summary>Represents the values for a VGA display with a resolution of 640x480.</summary>
			Vga640x480 = 11,

			/// <summary>Represents the values for a VGA display with a resolution of 800x600.</summary>
			Vga800x600 = 12,

			/// <summary>Represents the values for an RCA display in the PAL standard with a resolution of 320x240.</summary>
			RcaPal320x240 = 20
		}

		/// <summary>Constructs a new instance.</summary>
		/// <param name="rSocketNumber">The mainboard socket that has the display's R socket connected to it.</param>
		/// <param name="gSocketNumber">The mainboard socket that has the display's G socket connected to it.</param>
		/// <param name="bSocketNumber">The mainboard socket that has the display's B socket connected to it.</param>
		/// <param name="i2cSocketNumber">The mainboard socket that has the display's I socket connected to it.</param>
		public VideoOut(string I2CBus)
        {
            var Devices = DeviceInformation.FindAll(I2CBus);

            // Device I2C1 Slave address
            I2cConnectionSettings Setting = new I2cConnectionSettings(I2C_ADDRESS);
            
            Setting.BusSpeed = I2cBusSpeed.StandardMode; // 100kHz
            i2c = I2cDevice.FromId(Devices[0].Id, Setting);
            this.currentHeight = 320;
			this.currentHeight = 240;

            /*
			var i2cSocket = Socket.GetSocket(i2cSocketNumber, true, this, null);
			i2cSocket.EnsureTypeIsSupported(new char[] { 'X', 'Y' }, this);

			this.i2c = new GTI.SoftwareI2CBus(i2cSocket, Socket.Pin.Five, Socket.Pin.Four, 0x76, 100, this);

			var rSocket = Socket.GetSocket(rSocketNumber, true, this, null);
			var gSocket = Socket.GetSocket(gSocketNumber, true, this, null);
			var bSocket = Socket.GetSocket(bSocketNumber, true, this, null);

			rSocket.EnsureTypeIsSupported('R', this);
			gSocket.EnsureTypeIsSupported('G', this);
			bSocket.EnsureTypeIsSupported('B', this);

			rSocket.ReservePin(Socket.Pin.Three, this);
			rSocket.ReservePin(Socket.Pin.Four, this);
			rSocket.ReservePin(Socket.Pin.Five, this);
			rSocket.ReservePin(Socket.Pin.Six, this);
			rSocket.ReservePin(Socket.Pin.Seven, this);
			rSocket.ReservePin(Socket.Pin.Eight, this);
			rSocket.ReservePin(Socket.Pin.Nine, this);

			gSocket.ReservePin(Socket.Pin.Three, this);
			gSocket.ReservePin(Socket.Pin.Four, this);
			gSocket.ReservePin(Socket.Pin.Five, this);
			gSocket.ReservePin(Socket.Pin.Six, this);
			gSocket.ReservePin(Socket.Pin.Seven, this);
			gSocket.ReservePin(Socket.Pin.Eight, this);
			gSocket.ReservePin(Socket.Pin.Nine, this);

			bSocket.ReservePin(Socket.Pin.Three, this);
			bSocket.ReservePin(Socket.Pin.Four, this);
			bSocket.ReservePin(Socket.Pin.Five, this);
			bSocket.ReservePin(Socket.Pin.Six, this);
			bSocket.ReservePin(Socket.Pin.Seven, this);
			bSocket.ReservePin(Socket.Pin.Eight, this);
			bSocket.ReservePin(Socket.Pin.Nine, this);
            */
		}

        /// <summary>Sets the output type and resolution and the mainboard's configuration.</summary>
        /// <param name="resolution">The desired output type and resolution.</param>
        /// <remarks>This method must be called to change the resolution. Setting the processor's display configuration is not enough.</remarks>
        public void SetDisplayConfiguration(Resolution resolution)
        {
            switch (resolution)
            {
                case Resolution.Rca320x240:
                    this.currentWidth = 320;
                    this.currentHeight = 240;
                    this.Write320x240RcaRegisters();
                    break;

                case Resolution.Rca640x480:
                    this.currentWidth = 640;
                    this.currentHeight = 480;
                    this.Write640x480RcaRegisters();
                    break;

                case Resolution.Rca800x600:
                    this.currentWidth = 800;
                    this.currentHeight = 600;
                    this.Write800x600RcaRegisters();
                    break;

                case Resolution.Vga320x240:
                    this.currentWidth = 320;
                    this.currentHeight = 240;
                    this.Write320x240VgaRegisters();
                    break;

                case Resolution.Vga640x480:
                    this.currentWidth = 640;
                    this.currentHeight = 480;
                    this.Write640x480VgaRegisters();
                    break;

                case Resolution.Vga800x600:
                    this.currentWidth = 800;
                    this.currentHeight = 600;
                    this.Write800x600VgaRegisters();
                    break;

                case Resolution.RcaPal320x240:
                    this.currentWidth = 320;
                    this.currentHeight = 240;
                    this.Write320x240RcaPalRegisters();
                    break;
            }
            var displayController = DisplayController.GetDefault(); //Currently returns the hardware LCD controller by default


            displayController.ApplySettings(new LcdControllerSettings
            {
                Width = this.currentWidth,
                Height = this.currentHeight,
                PixelClockRate = 10000,
                PixelPolarity = false,//
                OutputEnablePolarity = false,//
                OutputEnableIsFixed = false,//
                HorizontalFrontPorch = 10,
                HorizontalBackPorch = 10,
                HorizontalSyncPulseWidth = 10,
                HorizontalSyncPolarity = true,//
                VerticalFrontPorch = 10,
                VerticalBackPorch = 10,
                VerticalSyncPulseWidth = 10,
                VerticalSyncPolarity = true,//
            });
            Screen = Graphics.FromHdc(displayController.Hdc); //Calling flush on the object returned will flush to the display represented by Hdc. Only one active display is supported at this time.
                                                              /*
                                                              var config = new DisplayModule.TimingRequirements() {
                                                                  PixelDataIsActiveHigh = true, //not the proper property, but we needed it for PriorityEnable
                                                                  UsesCommonSyncPin = false, //not the proper property, but we needed it for OutputEnableIsFixed
                                                                  CommonSyncPinIsActiveHigh = false, //not the proper property, but we needed it for OutputEnablePolarity
                                                                  HorizontalSyncPulseIsActiveHigh = true,
                                                                  VerticalSyncPulseIsActiveHigh = true,
                                                                  PixelDataIsValidOnClockRisingEdge = false,
                                                                  HorizontalSyncPulseWidth = 10,
                                                                  HorizontalBackPorch = 10,
                                                                  HorizontalFrontPorch = 10,
                                                                  VerticalSyncPulseWidth = 10,
                                                                  VerticalBackPorch = 10,
                                                                  VerticalFrontPorch = 10,
                                                                  MaximumClockSpeed = 10000
                                                              };
                                                              */
                                                              //base.OnDisplayConnected("Video Out", this.currentWidth, this.currentHeight, DisplayOrientation.Normal, config);

            Thread.Sleep(1000);

            var ud = ReadRegister(0x00);
            if (ud != 0x55 && ud != 0x54)
                this.ErrorPrint("Setting the display configuration failed.");
        }

        /// <summary>Renders display data on the display device.</summary>
        /// <param name="bitmap">The bitmap object to render on the display.</param>
        /// <param name="x">The start x coordinate of the dirty area.</param>
        /// <param name="y">The start y coordinate of the dirty area.</param>
        /// <param name="width">The width of the dirty area.</param>
        /// <param name="height">The height of the dirty area.</param>
        public void Paint(Bitmap bitmap, int x, int y, int width, int height)
        {
            try
            {
                Screen.DrawImage(bitmap, x, y);
                Screen.Flush();
            }
            catch
            {
                this.ErrorPrint("Painting error");
            }
        }

        private void Write320x240RcaRegisters() {
			this.WriteRegister(0x02, 0x01);
			this.WriteRegister(0x02, 0x03);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x04, 0x39);
			this.WriteRegister(0x0A, 0x10);
			this.WriteRegister(0x0C, 0xD2);
			this.WriteRegister(0x0D, 0x80);
			this.WriteRegister(0x11, 0x5E);
			this.WriteRegister(0x12, 0x40);
			this.WriteRegister(0x13, 0x0A);
			this.WriteRegister(0x14, 0x0A);
			this.WriteRegister(0x17, 0x0E);
			this.WriteRegister(0x19, 0x0A);
			this.WriteRegister(0x1A, 0x0A);
			this.WriteRegister(0x41, 0x9A);
			this.WriteRegister(0x4D, 0x04);
			this.WriteRegister(0x4E, 0x80);
			this.WriteRegister(0x51, 0x4B);
			this.WriteRegister(0x52, 0x12);
			this.WriteRegister(0x53, 0x1B);
			this.WriteRegister(0x55, 0xE5);
			this.WriteRegister(0x5E, 0x80);
			this.WriteRegister(0x77, 0x03);
			this.WriteRegister(0x7D, 0x62);
			this.WriteRegister(0x04, 0x38);
			this.WriteRegister(0x06, 0x71);

			//The following repetitions are used here to wait for memory initialization to complete. See Appendix A of CH7025(26)B Programming Guide Rev 2.03 for more information.
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);

			this.WriteRegister(0x06, 0x70);
			this.WriteRegister(0x02, 0x02);
			this.WriteRegister(0x02, 0x03);
			this.WriteRegister(0x04, 0x30);
		}

		private void Write640x480RcaRegisters() {
			this.WriteRegister(0x02, 0x01);
			this.WriteRegister(0x02, 0x03);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x04, 0x39);
			this.WriteRegister(0x0A, 0x10);
			this.WriteRegister(0x0C, 0xD2);
			this.WriteRegister(0x0F, 0x12);
			this.WriteRegister(0x10, 0x80);
			this.WriteRegister(0x11, 0x9E);
			this.WriteRegister(0x12, 0x40);
			this.WriteRegister(0x13, 0x0A);
			this.WriteRegister(0x14, 0x0A);
			this.WriteRegister(0x15, 0x09);
			this.WriteRegister(0x16, 0xE0);
			this.WriteRegister(0x17, 0xFE);
			this.WriteRegister(0x19, 0x0A);
			this.WriteRegister(0x1A, 0x0A);
			this.WriteRegister(0x41, 0x9A);
			this.WriteRegister(0x4D, 0x04);
			this.WriteRegister(0x4E, 0x80);
			this.WriteRegister(0x51, 0x4B);
			this.WriteRegister(0x52, 0x12);
			this.WriteRegister(0x53, 0x1B);
			this.WriteRegister(0x55, 0xE5);
			this.WriteRegister(0x5E, 0x80);
			this.WriteRegister(0x77, 0x03);
			this.WriteRegister(0x7D, 0x62);
			this.WriteRegister(0x04, 0x38);
			this.WriteRegister(0x06, 0x71);

			//The following repetitions are used here to wait for memory initialization to complete. See Appendix A of CH7025(26)B Programming Guide Rev 2.03 for more information.
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);

			this.WriteRegister(0x06, 0x70);
			this.WriteRegister(0x02, 0x02);
			this.WriteRegister(0x02, 0x03);
			this.WriteRegister(0x04, 0x30);
		}

		private void Write800x600RcaRegisters() {
			this.WriteRegister(0x02, 0x01);
			this.WriteRegister(0x02, 0x03);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x04, 0x39);
			this.WriteRegister(0x06, 0x6B);
			this.WriteRegister(0x0A, 0x10);
			this.WriteRegister(0x0C, 0xD2);
			this.WriteRegister(0x0F, 0x1B);
			this.WriteRegister(0x10, 0x20);
			this.WriteRegister(0x11, 0x3E);
			this.WriteRegister(0x12, 0x40);
			this.WriteRegister(0x13, 0x0A);
			this.WriteRegister(0x14, 0x0A);
			this.WriteRegister(0x15, 0x12);
			this.WriteRegister(0x16, 0x58);
			this.WriteRegister(0x17, 0x76);
			this.WriteRegister(0x19, 0x0A);
			this.WriteRegister(0x1A, 0x0A);
			this.WriteRegister(0x41, 0x9A);
			this.WriteRegister(0x4D, 0x04);
			this.WriteRegister(0x4E, 0x80);
			this.WriteRegister(0x51, 0x4B);
			this.WriteRegister(0x52, 0x12);
			this.WriteRegister(0x53, 0x1B);
			this.WriteRegister(0x55, 0xE5);
			this.WriteRegister(0x5E, 0x80);
			this.WriteRegister(0x69, 0x64);
			this.WriteRegister(0x77, 0x03);
			this.WriteRegister(0x7D, 0x62);
			this.WriteRegister(0x04, 0x38);
			this.WriteRegister(0x06, 0x69);

			//The following repetitions are used here to wait for memory initialization to complete. See Appendix A of CH7025(26)B Programming Guide Rev 2.03 for more information.
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);

			this.WriteRegister(0x06, 0x68);
			this.WriteRegister(0x02, 0x02);
			this.WriteRegister(0x02, 0x03);
			this.WriteRegister(0x04, 0x30);
		}

		private void Write320x240VgaRegisters() {
			this.WriteRegister(0x02, 0x01);
			this.WriteRegister(0x02, 0x03);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x04, 0x39);
			this.WriteRegister(0x08, 0x08);
			this.WriteRegister(0x09, 0x80);
			this.WriteRegister(0x0C, 0xD2);
			this.WriteRegister(0x0D, 0x88);
			this.WriteRegister(0x11, 0x5E);
			this.WriteRegister(0x12, 0x40);
			this.WriteRegister(0x13, 0x0A);
			this.WriteRegister(0x17, 0x0E);
			this.WriteRegister(0x19, 0x0A);
			this.WriteRegister(0x1A, 0x0A);
			this.WriteRegister(0x1B, 0x23);
			this.WriteRegister(0x1C, 0x20);
			this.WriteRegister(0x1D, 0x20);
			this.WriteRegister(0x1F, 0x28);
			this.WriteRegister(0x20, 0x80);
			this.WriteRegister(0x21, 0x12);
			this.WriteRegister(0x22, 0x58);
			this.WriteRegister(0x23, 0x74);
			this.WriteRegister(0x25, 0x01);
			this.WriteRegister(0x26, 0x04);
			this.WriteRegister(0x37, 0x20);
			this.WriteRegister(0x39, 0x20);
			this.WriteRegister(0x3B, 0x20);
			this.WriteRegister(0x41, 0x9A);
			this.WriteRegister(0x4D, 0x03);
			this.WriteRegister(0x4E, 0x50);
			this.WriteRegister(0x4F, 0xDA);
			this.WriteRegister(0x50, 0x74);
			this.WriteRegister(0x51, 0x4B);
			this.WriteRegister(0x52, 0x12);
			this.WriteRegister(0x53, 0x13);
			this.WriteRegister(0x55, 0xE5);
			this.WriteRegister(0x5E, 0x80);
			this.WriteRegister(0x77, 0x03);
			this.WriteRegister(0x7D, 0x62);
			this.WriteRegister(0x04, 0x38);
			this.WriteRegister(0x06, 0x71);

			//The following repetitions are used here to wait for memory initialization to complete. See Appendix A of CH7025(26)B Programming Guide Rev 2.03 for more information.
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);

			this.WriteRegister(0x06, 0x70);
			this.WriteRegister(0x02, 0x02);
			this.WriteRegister(0x02, 0x03);
			this.WriteRegister(0x04, 0x00);
		}

		private void Write640x480VgaRegisters() {
			this.WriteRegister(0x02, 0x01);
			this.WriteRegister(0x02, 0x03);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x04, 0x39);
			this.WriteRegister(0x08, 0x08);
			this.WriteRegister(0x09, 0x80);
			this.WriteRegister(0x0C, 0xD2);
			this.WriteRegister(0x0D, 0x08);
			this.WriteRegister(0x0F, 0x12);
			this.WriteRegister(0x10, 0x80);
			this.WriteRegister(0x11, 0x9E);
			this.WriteRegister(0x12, 0x40);
			this.WriteRegister(0x13, 0x0A);
			this.WriteRegister(0x14, 0x0A);
			this.WriteRegister(0x15, 0x09);
			this.WriteRegister(0x16, 0xE0);
			this.WriteRegister(0x17, 0xFE);
			this.WriteRegister(0x19, 0x0A);
			this.WriteRegister(0x1A, 0x0A);
			this.WriteRegister(0x1B, 0x23);
			this.WriteRegister(0x1C, 0x20);
			this.WriteRegister(0x1D, 0x20);
			this.WriteRegister(0x1F, 0x28);
			this.WriteRegister(0x20, 0x80);
			this.WriteRegister(0x21, 0x12);
			this.WriteRegister(0x22, 0x58);
			this.WriteRegister(0x23, 0x74);
			this.WriteRegister(0x25, 0x01);
			this.WriteRegister(0x26, 0x04);
			this.WriteRegister(0x37, 0x20);
			this.WriteRegister(0x39, 0x20);
			this.WriteRegister(0x3B, 0x20);
			this.WriteRegister(0x41, 0x9A);
			this.WriteRegister(0x4D, 0x03);
			this.WriteRegister(0x4E, 0x50);
			this.WriteRegister(0x4F, 0xDA);
			this.WriteRegister(0x50, 0x74);
			this.WriteRegister(0x51, 0x4B);
			this.WriteRegister(0x52, 0x12);
			this.WriteRegister(0x53, 0x13);
			this.WriteRegister(0x55, 0xE5);
			this.WriteRegister(0x5E, 0x80);
			this.WriteRegister(0x77, 0x03);
			this.WriteRegister(0x7D, 0x62);
			this.WriteRegister(0x04, 0x38);
			this.WriteRegister(0x06, 0x71);

			//The following repetitions are used here to wait for memory initialization to complete. See Appendix A of CH7025(26)B Programming Guide Rev 2.03 for more information.
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);

			this.WriteRegister(0x06, 0x70);
			this.WriteRegister(0x02, 0x02);
			this.WriteRegister(0x02, 0x03);
			this.WriteRegister(0x04, 0x00);
		}

		private void Write800x600VgaRegisters() {
			this.WriteRegister(0x02, 0x01);
			this.WriteRegister(0x02, 0x03);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x04, 0x39);
			this.WriteRegister(0x06, 0x6B);
			this.WriteRegister(0x08, 0x08);
			this.WriteRegister(0x09, 0x80);
			this.WriteRegister(0x0C, 0xD2);
			this.WriteRegister(0x0D, 0x08);
			this.WriteRegister(0x0F, 0x1B);
			this.WriteRegister(0x10, 0x20);
			this.WriteRegister(0x11, 0x3E);
			this.WriteRegister(0x12, 0x40);
			this.WriteRegister(0x13, 0x0A);
			this.WriteRegister(0x14, 0x0A);
			this.WriteRegister(0x15, 0x12);
			this.WriteRegister(0x16, 0x58);
			this.WriteRegister(0x17, 0x76);
			this.WriteRegister(0x19, 0x0A);
			this.WriteRegister(0x1A, 0x0A);
			this.WriteRegister(0x1B, 0x23);
			this.WriteRegister(0x1C, 0x20);
			this.WriteRegister(0x1D, 0x20);
			this.WriteRegister(0x1F, 0x28);
			this.WriteRegister(0x20, 0x80);
			this.WriteRegister(0x21, 0x12);
			this.WriteRegister(0x22, 0x58);
			this.WriteRegister(0x23, 0x74);
			this.WriteRegister(0x25, 0x01);
			this.WriteRegister(0x26, 0x04);
			this.WriteRegister(0x37, 0x20);
			this.WriteRegister(0x39, 0x20);
			this.WriteRegister(0x3B, 0x20);
			this.WriteRegister(0x41, 0x9A);
			this.WriteRegister(0x4D, 0x03);
			this.WriteRegister(0x4E, 0x50);
			this.WriteRegister(0x4F, 0xDA);
			this.WriteRegister(0x50, 0x74);
			this.WriteRegister(0x51, 0x4B);
			this.WriteRegister(0x52, 0x12);
			this.WriteRegister(0x53, 0x13);
			this.WriteRegister(0x55, 0xE5);
			this.WriteRegister(0x5E, 0x80);
			this.WriteRegister(0x69, 0x64);
			this.WriteRegister(0x77, 0x03);
			this.WriteRegister(0x7D, 0x62);
			this.WriteRegister(0x04, 0x38);
			this.WriteRegister(0x06, 0x69);

			//The following repetitions are used here to wait for memory initialization to complete. See Appendix A of CH7025(26)B Programming Guide Rev 2.03 for more information.
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);

			this.WriteRegister(0x06, 0x68);
			this.WriteRegister(0x02, 0x02);
			this.WriteRegister(0x02, 0x03);
			this.WriteRegister(0x04, 0x00);
		}

		private void Write320x240RcaPalRegisters() {
			this.WriteRegister(0x02, 0x01);
			this.WriteRegister(0x02, 0x03);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x04, 0x39);
			this.WriteRegister(0x0A, 0x10);
			this.WriteRegister(0x0C, 0xD2);
			this.WriteRegister(0x0D, 0x84);
			this.WriteRegister(0x11, 0x5E);
			this.WriteRegister(0x12, 0x40);
			this.WriteRegister(0x13, 0x0A);
			this.WriteRegister(0x14, 0x0A);
			this.WriteRegister(0x17, 0x0E);
			this.WriteRegister(0x19, 0x0A);
			this.WriteRegister(0x1A, 0x0A);
			this.WriteRegister(0x41, 0x9A);
			this.WriteRegister(0x4D, 0x04);
			this.WriteRegister(0x4E, 0x80);
			this.WriteRegister(0x51, 0x4B);
			this.WriteRegister(0x52, 0x12);
			this.WriteRegister(0x53, 0x1B);
			this.WriteRegister(0x55, 0xE5);
			this.WriteRegister(0x5E, 0x80);
			this.WriteRegister(0x77, 0x03);
			this.WriteRegister(0x7D, 0x62);
			this.WriteRegister(0x04, 0x38);
			this.WriteRegister(0x06, 0x71);

			//The following repetitions are used here to wait for memory initialization to complete. See Appendix A of CH7025(26)B Programming Guide Rev 2.03 for more information.
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);
			this.WriteRegister(0x03, 0x00);

			this.WriteRegister(0x06, 0x70);
			this.WriteRegister(0x02, 0x02);
			this.WriteRegister(0x02, 0x03);
			this.WriteRegister(0x04, 0x30);
		}

		private void WriteRegister(byte address, byte data) {
			var writeBuffer = new byte[] { address, data };

			this.i2c.Write(writeBuffer);

			Thread.Sleep(1);
		}

		private byte ReadRegister(byte address) {
			var writeBuffer = new byte[] { address };
			var readBuffer = new byte[] { 0x00 };

			this.i2c.WriteRead(writeBuffer, readBuffer);

			return readBuffer[0];
		}
	}
}