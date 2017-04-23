using System.Threading;
using Gadgeteer.Modules.GHIElectronics;
using GHI.Pins;
using System.Diagnostics;
using TinyApp.Properties;
using System.Drawing;
using GHIElectronics.TinyCLR.Storage.Streams;
using System;
using System.Text;

// ReSharper disable UnusedMember.Local
// ReSharper disable FunctionNeverReturns

namespace TinyApp
{
    public static class Program
    {

        public static void Main()
        {
            // DisplayT35  
            // var Lcd = TestDisplayT35();

            // camera serial
            // TestSerialCameraL1(Lcd);

            // Led 7 colors
            // TestLed7C();

            // DisplayNHVN
            // TestDisplayNHVN();

            // Oximeter
            // TestOximeter();

             // Test max o (there is a bug when initialize the spi)       
             // TestMaxO();

            // Display N18
            // TestDisplayN18();

            // DisplayT43
            TestDisplayT43();
        }

        #region Testing

        /// <summary>
        /// Testing method for MaxO module
        /// </summary>
        private static void TestMaxO()
        {
            var mxO = new MaxO(FEZRaptor.SpiBus.Spi2, FEZRaptor.Socket3.Pin3, FEZRaptor.Socket3.Pin4,
                FEZRaptor.Socket3.Pin5) {Boards = 1};
            mxO.Write(new byte[] { 0xAA, 0xAA, 0xAA, 0xAA });
            //https://www.ghielectronics.com/docs/81/maxo-module

            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Testing method for DisplayNHVN module
        /// </summary>
        private static void TestDisplayNhvn()
        {
            var lcd = new DisplayNHVN(FEZRaptor.I2cBus.I2c1, FEZRaptor.Socket16.Pin9, FEZRaptor.Socket13.Pin3,
                DisplayNHVN.DisplayTypes.Display7inch);
            var background = Resources.GetBitmap(Resources.BitmapResources.car);
            var font = Resources.GetFont(Resources.FontResources.NinaB);
            lcd.Screen.DrawImage(background, 0, 0);
            lcd.Screen.DrawString("Hello, world", font, new SolidBrush(Color.White), 10, 400);
            lcd.Screen.Flush();
            lcd.CapacitiveScreenReleased += Lcd_CapacitiveScreenReleased;
            lcd.CapacitiveScreenPressed += Lcd_CapacitiveScreenPressed;

            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Testing method for SerialCameraL1 module
        /// </summary>
        /// <param name="lcd"></param>
        private static void TestSerialCameraL1(DisplayT35 lcd)
        {
            var camera = new SerialCameraL1(FEZRaptor.Socket4.SerialPortName);
            while (true)
            {
                if (camera.NewImageReady)
                {
                    var bitmap = camera.GetImage();
                    lcd.Screen.DrawImage(bitmap, 0, 0);
                    break;
                }
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Testing method for DisplayT35 module
        /// </summary>
        /// <returns>Return an object to lcd screen</returns>
        private static DisplayT35 TestDisplayT35()
        {
            var lcd = new DisplayT35(FEZRaptor.Socket16.Pin9);
            var background = Resources.GetBitmap(Resources.BitmapResources.nature);
            var font = Resources.GetFont(Resources.FontResources.small);
            lcd.Screen.DrawImage(background, 0, 0);
            lcd.Screen.DrawString("Hello, world", font, new SolidBrush(Color.White), 10, 200);
            lcd.Screen.Flush();
            return lcd;
        }

        /// <summary>
        /// Testing method for DisplayN18 module
        /// </summary>
        static void TestDisplayN18()
        {
            var displayN18 = new DisplayN18(FEZSpiderII.Socket6.SpiModule, FEZSpiderII.Socket6.Pin3,
                FEZSpiderII.Socket6.Pin4, FEZSpiderII.Socket6.Pin5, FEZSpiderII.Socket6.Pin6)
            { BacklightEnabled = true };

            displayN18.DrawPixel(10, 10, Color.Red);
            Thread.Sleep(1000);
            displayN18.DrawFillRect(0, 0, 100, 100, Color.Green);
            Thread.Sleep(1000);
            Bitmap bmp = Resources.GetBitmap(Resources.BitmapResources.logo);
            displayN18.DrawBitmap(bmp, 0, 0);
            Thread.Sleep(1000);
            Font font = Resources.GetFont(Resources.FontResources.NinaB);
            displayN18.DrawString("HMMMMMMMH", font, 10, 10, Color.Red);
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Testing method for DisplayT43 module
        /// </summary>
        private static void TestDisplayT43()
        {
            var lcd = new DisplayT43(FEZRaptor.Socket16.Pin9);
            
            var background = Resources.GetBitmap(Resources.BitmapResources.nature);
            var font = Resources.GetFont(Resources.FontResources.small);
            lcd.Screen.DrawImage(background, 0, 0);

            lcd.Screen.DrawString("Hello, world", font, new SolidBrush(Color.White), 10, 200);
            lcd.Screen.Flush();
        }

        /// <summary>
        /// Testing method for PulseOximeter module
        /// </summary>
        static void TestOximeter()
        {
            PulseOximeter ox = new PulseOximeter(FEZRaptor.Socket10.SerialPortName);
            ox.ProbeAttached += (sender, e) =>
            {
                Debug.WriteLine("probe attached.");
            };
            ox.ProbeDetached += (sender, e) =>
            {
                Debug.WriteLine("probe detached.");
            };
            ox.Heartbeat += (sender, e) => { Debug.WriteLine($"SPO: {e.SPO2} ,Pulse:{e.PulseRate}"); };
        }

        /// <summary>
        /// Testing method for RelayX1 module (function never returns)
        /// </summary>
        static void TestRelay()
        {
            var relay = new RelayX1(FEZRaptor.Socket18.Pin5);
            while (true)
            {
                relay.TurnOn();
                Thread.Sleep(2000);
                relay.TurnOff();
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// Testing method for BreakoutTB10 module
        /// </summary>
        static void TestTb10()
        {
            var tb10 = new BreakoutTB10();
            var led = tb10.CreateDigitalOutput(FEZRaptor.Socket18.Pin3, true);
            led.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.High);
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Testing method for TouchC8 module (function never returns)
        /// </summary>
        static void TestTouchC8()
        {
            var touch = new TouchC8(FEZRaptor.I2cBus.I2c1, FEZRaptor.Socket14.Pin3, FEZRaptor.Socket14.Pin6);
            while (true)
            {
                Debug.WriteLine($"button down : {touch.IsButtonPressed(TouchC8.Button.Down)}");
                Debug.WriteLine($"button middle : {touch.IsButtonPressed(TouchC8.Button.Middle)}");
                Debug.WriteLine($"button up : {touch.IsButtonPressed(TouchC8.Button.Up)}");
                Debug.WriteLine($"wheel pressed : {touch.IsWheelPressed()}");
                Debug.WriteLine($"proximity detected : {touch.IsProximityDetected()}");
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Testing method for VideoOut module (function never returns)
        /// </summary>
        static void TestVideoOut()
        {
            var lcd = new VideoOut(FEZRaptor.I2cBus.I2c1);
            lcd.SetDisplayConfiguration(VideoOut.Resolution.Vga800x600);
            var background = Resources.GetBitmap(Resources.BitmapResources.car);
            var font = Resources.GetFont(Resources.FontResources.NinaB);
            lcd.Screen.DrawImage(background, 0, 0);
            lcd.Screen.DrawString("Hello, world", font, new SolidBrush(Color.White), 10, 400);
            lcd.Screen.Flush();

            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Testing method for XBeeAdapter module (function never returns)
        /// </summary>
        static void TestXbee()
        {
            var xbee = new XBeeAdapter();
            xbee.Configure(FEZRaptor.Socket4.SerialPortName);
            while (true)
            {
                var count = xbee.Port.BytesReceived;
                var serialReadBuffer = new Buffer(count);
                //_Serialreadbuffer.Capacity
                uint read = xbee._inStream.Read(serialReadBuffer, count, InputStreamOptions.None);
                if (read > 0)
                {
                    var buffer = serialReadBuffer.Data;
                    string stng = new string(Encoding.UTF8.GetChars(buffer));
                    Debug.WriteLine(stng);
                }
                else
                if (read <= 0)
                {
                    throw new InvalidOperationException("Failed to read all of the bytes from the port.");
                }
                Thread.Sleep(100);
            }
        }

        #region Lcd Capacitive Touch Events
        /// <summary>
        /// Function called when released event raises
        /// </summary>
        /// <param name="sender">sender of event</param>
        /// <param name="e">EventArgs of event</param>
        private static void Lcd_CapacitiveScreenReleased(DisplayNHVN sender, DisplayNHVN.TouchEventArgs e)
        {
            Debug.WriteLine("you release the lcd at X:"+e.X+" ,Y:"+e.Y);
        }

        /// <summary>
        /// Function called when pressed event raises
        /// </summary>
        /// <param name="sender">sender of event</param>
        /// <param name="e">EventArgs of event</param>
        private static void Lcd_CapacitiveScreenPressed(DisplayNHVN sender, DisplayNHVN.TouchEventArgs e)
        {
            Debug.WriteLine("you press the lcd at X:" + e.X + " ,Y:" + e.Y);
        }
        #endregion

        /// <summary>
        /// Testing method for PulseCount module (function never returns)
        /// </summary>
        static void TestPulseCount()
        {
            PulseCount cnt = new PulseCount(FEZRaptor.Socket17.Pin5, FEZRaptor.Socket17.Pin6, FEZRaptor.Socket17.Pin7,
                FEZRaptor.Socket17.Pin8, FEZRaptor.Socket17.Pin9);
            var count = cnt.GetCount();
            while (true)
            {
                Debug.WriteLine($"count:{count}");
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Testing method for MotorDriverL298 module (function never returns)
        /// </summary>
        static void TestMotor()
        {
            MotorDriverL298 motor = new MotorDriverL298(FEZRaptor.Socket18.PwmPin.Controller0.Id, FEZRaptor.Socket18.PwmPin.Controller1.Id,
               FEZRaptor.Socket18.PwmPin.Controller0.PC18, FEZRaptor.Socket18.PwmPin.Controller1.PC19, FEZRaptor.Socket18.Pin6, FEZRaptor.Socket18.Pin9);
            motor.SetSpeed(MotorDriverL298.Motor.Motor1, -0.8);
            motor.SetSpeed(MotorDriverL298.Motor.Motor2, -0.8);
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Testing method for Moisture module (function never returns)
        /// </summary>
        static void TestMoisture()
        {
            Moisture mois = new Moisture(FEZRaptor.Socket2.Pin6, FEZRaptor.Socket2.AnalogInput3);
            while (true)
            {
                var nilai = mois.ReadMoisture();
                Debug.WriteLine($"value : {nilai}");
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Testing method for Compass module (function never returns)
        /// </summary>
        static void TestCompass()
        {
            Compass com = new Compass(FEZRaptor.I2cBus.I2c1, FEZRaptor.Socket14.Pin3);
            com.StartTakingMeasurements();
            com.MeasurementComplete += (sender, e) => { Debug.WriteLine($"Info : {e.ToString()}"); };
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Testing method for CurrentACS712 module (function never returns)
        /// </summary>
        static void TestAcs712()
        {
            var curSensor = new CurrentACS712(FEZRaptor.Socket13.AnalogInput5);
            while (true)
            {
                Debug.WriteLine($"current : {curSensor.ReadACCurrent()}");
                Thread.Sleep(500);
            }
        }
        /// <summary>
        /// Testing method for Gyro module (function never returns)
        /// </summary>
        static void TestGyro()
        {
            var gyro = new Gyro(FEZRaptor.I2cBus.I2c1);
            gyro.MeasurementComplete += (sender, e) =>
            {
                Debug.WriteLine(e.ToString());
            };
            gyro.StartTakingMeasurements();
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Testing method for AccelG248 module (function never returns)
        /// </summary>
        static void TestAccelG248()
        {
            var accel = new AccelG248(FEZRaptor.I2cBus.I2c1);
            while (true)
            {
                Debug.WriteLine(accel.GetAcceleration().ToString());
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Testing method for RotaryH1 module (function never returns)
        /// </summary>
        static void TestRotary()
        {
            var rotary = new RotaryH1(FEZRaptor.Socket17.Pin5, FEZRaptor.Socket17.Pin6, FEZRaptor.Socket17.Pin7, FEZRaptor.Socket17.Pin8, FEZRaptor.Socket17.Pin9);
            while (true)
            {
                Debug.WriteLine($"dir : {rotary.GetDirection()}, count : {rotary.GetCount()}");
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Testing method for CharacterDisplay module (function never returns)
        /// </summary>
        static void TestCharDisplay()
        {
            var characterDisp = new CharacterDisplay(FEZRaptor.Socket1.Pin3, FEZRaptor.Socket1.Pin4, FEZRaptor.Socket1.Pin5, FEZRaptor.Socket1.Pin6, FEZRaptor.Socket1.Pin7, FEZRaptor.Socket1.Pin8, FEZRaptor.Socket1.Pin9);
            characterDisp.Print("Hellow world...");
            characterDisp.BacklightEnabled = true;
            Thread.Sleep(5000);
            characterDisp.Clear();
            characterDisp.Print("Hurrayyy...");
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Testing method for Joystick module (function never returns)
        /// </summary>
        static void TestJoystick()
        {
            var joystik = new Joystick(FEZRaptor.Socket2.AnalogInput4, FEZRaptor.Socket2.AnalogInput5, FEZRaptor.Socket2.Pin3);
            while (true)
            {
                Debug.WriteLine($"pos:{joystik.GetPosition().X} - { joystik.GetPosition().Y}");
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Testing method for LedStrip module (function never returns)
        /// </summary>
        static void TestLedStrip()
        {
            //test led strip
            var pins = new[] { FEZRaptor.Socket18.Pin3, FEZRaptor.Socket18.Pin4, FEZRaptor.Socket18.Pin5,
            FEZRaptor.Socket18.Pin6,FEZRaptor.Socket18.Pin7,FEZRaptor.Socket18.Pin8,FEZRaptor.Socket18.Pin9};
            var ledStrip = new LedStrip(pins);
            int counter = 0;
            while (true)
            {
                ledStrip.SetLeds(counter);
                if (counter >= ledStrip.LedCount) counter = 0;
                counter++;
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Testing method for Button module (function never returns)
        /// </summary>
        static void TestButton()
        {
            //test button, light sense, gas sense, temp humid
            Button btn = new Button(FEZRaptor.Socket18.Pin3, FEZRaptor.Socket18.Pin4);

            while (true)
            {
                Debug.WriteLine(btn.Pressed ? "button is pressed" : "button is not pressed");
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Testing method for LightSense module (function never returns)
        /// </summary>
        static void TestLightSense()
        {
            LightSense light = new LightSense(FEZRaptor.Socket2.AnalogInput3);

            while (true)
            {
                Debug.WriteLine($"light : {light.GetIlluminance()}");
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Testing method for GasSense module (function never returns)
        /// </summary>
        static void TestGasSense()
        {
            //test button, light sense, gas sense, temp humid
            GasSense gas = new GasSense(FEZRaptor.Socket14.AnalogInput3, FEZRaptor.Socket14.Pin4);

            while (true)
            {
                Debug.WriteLine($"gas : {gas.ReadProportion()}");
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Testing method for TempHumidSI70 module (function never returns)
        /// </summary>
        static void TestTempHumidSi70()
        {
            //test button, light sense, gas sense, temp humid
            TempHumidSI70 tempSensor = new TempHumidSI70(GHIElectronics.TinyCLR.Pins.G400S.I2cBus.I2c1);

            while (true)
            {
                Debug.WriteLine($"temp & humid : {tempSensor.TakeMeasurement().Temperature} - {tempSensor.TakeMeasurement().RelativeHumidity}");
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Testing method for Led7C module
        /// </summary>
        static void TestLed7C()
        {
            int delay = 1000;
            var led7C = new Led7C(FEZRaptor.Socket12.Pin3, FEZRaptor.Socket12.Pin4, FEZRaptor.Socket12.Pin5);
            led7C.SetColor(Led7C.Color.Blue);
            Thread.Sleep(delay);
            led7C.SetColor(Led7C.Color.Red);
            Thread.Sleep(delay);
            led7C.SetColor(Led7C.Color.Green);
            Thread.Sleep(delay);
            led7C.SetColor(Led7C.Color.Magenta);
            Thread.Sleep(delay);
            led7C.SetColor(Led7C.Color.Cyan);
            Thread.Sleep(delay);
            led7C.SetColor(Led7C.Color.Yellow);
            Thread.Sleep(delay);
            led7C.SetColor(Led7C.Color.Black);
            Thread.Sleep(delay);
            led7C.SetColor(Led7C.Color.White);
            Thread.Sleep(delay);
        }

        /// <summary>
        /// Testing method for Tunes module
        /// </summary>
        static void TestTunes()
        {
            var melody = new Tunes.Melody();
            Tunes.MusicNote note = new Tunes.MusicNote(Tunes.Tone.C4, 400);

            melody.Add(note);

            // up
            melody.Add(PlayNote(Tunes.Tone.C4));
            melody.Add(PlayNote(Tunes.Tone.D4));
            melody.Add(PlayNote(Tunes.Tone.E4));
            melody.Add(PlayNote(Tunes.Tone.F4));
            melody.Add(PlayNote(Tunes.Tone.G4));
            melody.Add(PlayNote(Tunes.Tone.A4));
            melody.Add(PlayNote(Tunes.Tone.B4));
            melody.Add(PlayNote(Tunes.Tone.C5));

            //// back down
            melody.Add(PlayNote(Tunes.Tone.B4));
            melody.Add(PlayNote(Tunes.Tone.A4));
            melody.Add(PlayNote(Tunes.Tone.G4));
            melody.Add(PlayNote(Tunes.Tone.F4));
            melody.Add(PlayNote(Tunes.Tone.E4));
            melody.Add(PlayNote(Tunes.Tone.D4));
            melody.Add(PlayNote(Tunes.Tone.C4));

            //// arpeggio
            melody.Add(PlayNote(Tunes.Tone.E4));
            melody.Add(PlayNote(Tunes.Tone.G4));
            melody.Add(PlayNote(Tunes.Tone.C5));
            melody.Add(PlayNote(Tunes.Tone.G4));
            melody.Add(PlayNote(Tunes.Tone.E4));
            melody.Add(PlayNote(Tunes.Tone.C4));

            //tunes.Play();

            //Thread.Sleep(100);

            melody.Add(PlayNote(Tunes.Tone.E4));
            melody.Add(PlayNote(Tunes.Tone.G4));
            melody.Add(PlayNote(Tunes.Tone.C5));
            melody.Add(PlayNote(Tunes.Tone.G4));
            melody.Add(PlayNote(Tunes.Tone.E4));
            melody.Add(PlayNote(Tunes.Tone.C4));
            var tunes = new Tunes(FEZRaptor.Socket18.PwmPin.Controller2.Id, FEZRaptor.Socket18.PwmPin.Controller2.PC20);
            tunes.Play(melody);
        }

        static Tunes.MusicNote PlayNote(Tunes.Tone tone)
        {
            Tunes.MusicNote note = new Tunes.MusicNote(tone, 200);

            return note;
        }
        #endregion
    }
}
