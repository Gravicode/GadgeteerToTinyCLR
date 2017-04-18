using System.Threading;
using Gadgeteer.Modules.GHIElectronics;
using GHI.Pins;
using System.Diagnostics;
using TinyApp.Properties;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using GHIElectronics.TinyCLR.Storage.Streams;
using System;
using System.Text;

namespace TinyApp
{
    public class Program
    {

        public static void Main()
        {
            //Graphics
            /*
            var Lcd = new DisplayT35(FEZRaptor.Socket16.Pin9);
            var background = Resources.GetBitmap(Resources.BitmapResources.nature);
            var font = Resources.GetFont(Resources.FontResources.small);
            Lcd.Screen.DrawImage(background, 0, 0);

            Lcd.Screen.DrawString("Hello, world", font, new SolidBrush(Color.White), 10, 200);
            Lcd.Screen.Flush();

            ////Gets a byte array for the given bitmap. Only MemoryBmp is supported at this time.
            //using (var bmp = new Bitmap(2, 2))
            //{
            //    var graphics = Graphics.FromImage(bmp);

            //    graphics.DrawLine(new Pen(Color.White, 1), 0, 0, 1, 0);

            //    using (var stream = new MemoryStream())
            //    {
            //        bmp.Save(stream, ImageFormat.MemoryBmp);

            //        var byteArray = stream.ToArray();
            //    }
            //}
            //camera serial
            var Camera = new SerialCameraL1(FEZRaptor.Socket4.SerialPortName);
            while (true)
            {
                if (Camera.NewImageReady)
                {
                    var bitmap = Camera.GetImage();
                    Lcd.Screen.DrawImage(bitmap, 0, 0);
                    break;
                }
                Thread.Sleep(200);
            }
            */
            //TestLed7C();
            /*
            var Lcd = new DisplayNHVN(FEZRaptor.I2cBus.I2c1,FEZRaptor.Socket16.Pin9,FEZRaptor.Socket13.Pin3,DisplayNHVN.DisplayTypes.Display7inch);
            var background = Resources.GetBitmap(Resources.BitmapResources.car);
            var font = Resources.GetFont(Resources.FontResources.NinaB);
            Lcd.Screen.DrawImage(background, 0, 0);

            Lcd.Screen.DrawString("Hello, world", font, new SolidBrush(Color.White), 10, 400);
            Lcd.Screen.Flush();
            Lcd.CapacitiveScreenReleased += Lcd_CapacitiveScreenReleased; ;
            Thread.Sleep(Timeout.Infinite);
            */
            
            //TestOximeter();

            //Test max o (there is a bug when initialize the spi)
            var mxO = new MaxO(FEZRaptor.Socket3.SpiModule, FEZRaptor.Socket3.Pin3, FEZRaptor.Socket3.Pin4, FEZRaptor.Socket3.Pin5);
            mxO.Boards = 1;
            mxO.Write(new byte[] { 0xAA, 0xAA, 0xAA, 0xAA });
            //https://www.ghielectronics.com/docs/81/maxo-module
            Thread.Sleep(Timeout.Infinite);
            
            
        }
        static void TestXbee()
        {
            var xbee = new XBeeAdapter();
            xbee.Configure(FEZRaptor.Socket4.SerialPortName);
            while (true)
            {
                var count = xbee.Port.BytesReceived;
                var _Serialreadbuffer = new Buffer(count);
                //_Serialreadbuffer.Capacity
                uint read = xbee._inStream.Read(_Serialreadbuffer, count, InputStreamOptions.None);
                if (read > 0)
                {
                    var buffer = _Serialreadbuffer.Data;
                    string stng = new string(Encoding.UTF8.GetChars(buffer));
                    Debug.WriteLine(stng);
                    count = _Serialreadbuffer.Capacity - read;
                }
                else
                if (read <= 0)
                {
                    throw new InvalidOperationException("Failed to read all of the bytes from the port.");
                }

                Thread.Sleep(100);
            }
        }
        static void TestTB10()
        {

            var BO10 = new BreakoutTB10();
            var Led = BO10.CreateDigitalOutput(FEZRaptor.Socket18.Pin3, true);
            Led.Write(GHIElectronics.TinyCLR.Devices.Gpio.GpioPinValue.High);
            Thread.Sleep(Timeout.Infinite);
        }
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
        static void TestOximeter()
        {
            PulseOximeter ox = new PulseOximeter(FEZRaptor.Socket10.SerialPortName);
            ox.ProbeAttached += (PulseOximeter sender, EventArgs e) =>
            {
                Debug.WriteLine("probe attached.");
            };
            ox.ProbeDetached += (PulseOximeter sender, EventArgs e) =>
            {
                Debug.WriteLine("probe detached.");
            };
            ox.Heartbeat += (PulseOximeter sender, PulseOximeter.Reading e) => { Debug.WriteLine($"SPO: {e.SPO2} ,Pulse:{e.PulseRate}"); };

        }

        private static void Lcd_CapacitiveScreenReleased(DisplayNHVN sender, DisplayNHVN.TouchEventArgs e)
        {
            Debug.WriteLine("you touch the lcd");

        }

        private static void Lcd_CapacitiveScreenPressed(DisplayNHVN sender, DisplayNHVN.TouchEventArgs e)
        {
            Debug.WriteLine("you touch the lcd");
        }


        /*
        #region Testing

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
        static void TestMotor()
        {
            MotorDriverL298 motor = new MotorDriverL298(FEZRaptor.Socket18.PwmPin.Controller0.Id, FEZRaptor.Socket18.PwmPin.Controller1.Id,
               FEZRaptor.Socket18.PwmPin.Controller0.PC18, FEZRaptor.Socket18.PwmPin.Controller1.PC19, FEZRaptor.Socket18.Pin6, FEZRaptor.Socket18.Pin9);
            motor.SetSpeed(MotorDriverL298.Motor.Motor1, -0.8);
            motor.SetSpeed(MotorDriverL298.Motor.Motor2, -0.8);
            Thread.Sleep(Timeout.Infinite);
        }
        
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
        static void TestCompass()
        {
            Compass com = new Compass(FEZRaptor.I2cBus.I2c1, FEZRaptor.Socket14.Pin3);
            com.StartTakingMeasurements();
            com.MeasurementComplete += (Compass sender, Compass.MeasurementCompleteEventArgs e) => { Debug.WriteLine($"Info : {e.ToString()}"); };
            Thread.Sleep(Timeout.Infinite);
        }

        static void TestACS712()
        {
            var curSensor = new CurrentACS712(FEZRaptor.Socket13.AnalogInput5);
            while (true)
            {
                Debug.WriteLine($"current : {curSensor.ReadACCurrent()}");
                Thread.Sleep(500);
            }
        }
        static void TestGyro(){
             var gyro = new Gyro(FEZRaptor.I2cBus.I2c1);
            gyro.MeasurementComplete += (Gyro sender, Gyro.MeasurementCompleteEventArgs e)=>
            {
                Debug.WriteLine(e.ToString());
            };
            gyro.StartTakingMeasurements();
            Thread.Sleep(Timeout.Infinite);
        }
        static void TestAccelG248()
        {
            var accel = new AccelG248(FEZRaptor.I2cBus.I2c1);
            while (true)
            {
                Debug.WriteLine(accel.GetAcceleration().ToString());
                Thread.Sleep(200);
            }
        }
        static void TestRotary()
        {
            var rotary = new RotaryH1(FEZRaptor.Socket17.Pin5, FEZRaptor.Socket17.Pin6, FEZRaptor.Socket17.Pin7, FEZRaptor.Socket17.Pin8, FEZRaptor.Socket17.Pin9);
            while (true)
            {
                Debug.WriteLine($"dir : {rotary.GetDirection()}, count : {rotary.GetCount()}");
                Thread.Sleep(200);
            }

        }
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
        static void TestJoystick()
        {
            var joystik = new Joystick(FEZRaptor.Socket2.AnalogInput4, FEZRaptor.Socket2.AnalogInput5, FEZRaptor.Socket2.Pin3);
            while (true)
            {
                Debug.WriteLine($"pos:{joystik.GetPosition().X} - { joystik.GetPosition().Y}");
                Thread.Sleep(200);
            }
            
        }

        static void TestDisplayT35()
        {
            var joystik = new Joystick(FEZRaptor.Socket2.AnalogInput4, FEZRaptor.Socket2.AnalogInput5, FEZRaptor.Socket2.Pin3);
            var Lcd = new DisplayT35(FEZRaptor.Socket16.Pin9);
            var background = Resources.GetBitmap(Resources.BitmapResources.nature);
            var font = Resources.GetFont(Resources.FontResources.small);
            Lcd.Screen.DrawImage(background, 0, 0);

            //screen.FillEllipse(new SolidBrush(Color.FromArgb(100, 0xFF, 0, 0)), 0, 0, 100, 100);
            //screen.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 0xFF)), 0, 100, 100, 100);
            //screen.DrawEllipse(new Pen(Color.Blue), 100, 0, 100, 100);
            //screen.DrawRectangle(new Pen(Color.Red), 100, 100, 100, 100);
            //screen.DrawLine(new Pen(Color.Green, 5), 250, 0, 220, 240);
            Lcd.Screen.DrawString("Hello, world", font, new SolidBrush(Color.White), 10, 200);
            //var i = 0;
            //screen.DrawLine(new Pen(Color.Black, 4), 260 + i, 10, 260 + i, 50); i += 4;
            //screen.DrawLine(new Pen(Color.White, 4), 260 + i, 10, 260 + i, 50); i += 4;
            //screen.DrawLine(new Pen(Color.Gray, 4), 260 + i, 10, 260 + i, 50); i += 4;
            //screen.DrawLine(new Pen(Color.Red, 4), 260 + i, 10, 260 + i, 50); i += 4;
            //screen.DrawLine(new Pen(Color.Green, 4), 260 + i, 10, 260 + i, 50); i += 4;
            //screen.DrawLine(new Pen(Color.Blue, 4), 260 + i, 10, 260 + i, 50); i += 4;
            //screen.DrawLine(new Pen(Color.Yellow, 4), 260 + i, 10, 260 + i, 50); i += 4;
            //screen.DrawLine(new Pen(Color.Purple, 4), 260 + i, 10, 260 + i, 50); i += 4;
            //screen.DrawLine(new Pen(Color.Teal, 4), 260 + i, 10, 260 + i, 50); i += 4;
            Lcd.Screen.Flush();

            //Gets a byte array for the given bitmap. Only MemoryBmp is supported at this time.
            using (var bmp = new Bitmap(2, 2))
            {
                var graphics = Graphics.FromImage(bmp);

                graphics.DrawLine(new Pen(Color.White, 1), 0, 0, 1, 0);

                using (var stream = new MemoryStream())
                {
                    bmp.Save(stream, ImageFormat.MemoryBmp);

                    var byteArray = stream.ToArray();
                }
            }
            Thread.Sleep(Timeout.Infinite);
        }
       
        static void TestLed7R()
        {
            //test led strip
            var pins = new int[] { FEZRaptor.Socket18.Pin3, FEZRaptor.Socket18.Pin4, FEZRaptor.Socket18.Pin5,
            FEZRaptor.Socket18.Pin6,FEZRaptor.Socket18.Pin7,FEZRaptor.Socket18.Pin8,FEZRaptor.Socket18.Pin9};
            var LedStrip = new LEDStrip(pins);
            int counter = 0;
            while (true)
            {
                LedStrip.SetLeds(counter);
                if (counter >= LedStrip.LedCount) counter = 0;
                counter++;
                Thread.Sleep(200);
            }
        }
        static void TestButton()
        {
            
            //test button, light sense, gas sense, temp humid
            Button btn = new Button(FEZRaptor.Socket18.Pin3, FEZRaptor.Socket18.Pin4);
            LightSense light = new LightSense(FEZRaptor.Socket2.AnalogInput3);
            GasSense gas = new GasSense(FEZRaptor.Socket14.AnalogInput3, FEZRaptor.Socket14.Pin4);
            TempHumidSI70 TempSensor = new TempHumidSI70(GHIElectronics.TinyCLR.Pins.G400S.I2cBus.I2c1);
                         
            while (true)
            {

                if (btn.Pressed)
                {
                    Debug.WriteLine("button is pressed");
                }
                else
                {
                    Debug.WriteLine("button is not pressed");
                }
                Debug.WriteLine($"light : {light.GetIlluminance()}");
                Debug.WriteLine($"gas : {gas.ReadProportion()}");
                Debug.WriteLine($"temp n humid : {TempSensor.TakeMeasurement().Temperature} - {TempSensor.TakeMeasurement().RelativeHumidity}");
                Thread.Sleep(200);
            }
            
        }
        /// <summary>
        /// Led7C Test
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
        /// Tunes test
        /// </summary>
        static void TestTunes()
        {
            var melody = new Gadgeteer.Modules.GHIElectronics.Tunes.Melody();
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
        */
    }

}
