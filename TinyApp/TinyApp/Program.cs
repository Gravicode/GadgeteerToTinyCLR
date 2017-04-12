using System.Threading;
using Gadgeteer.Modules.GHIElectronics;
using GHI.Pins;
using System.Diagnostics;
using TinyApp.Properties;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

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
            var Lcd = new DisplayNHVN(FEZRaptor.I2cBus.I2c1,FEZRaptor.Socket16.Pin9,FEZRaptor.Socket13.Pin3,DisplayNHVN.DisplayTypes.Display7inch);
            var background = Resources.GetBitmap(Resources.BitmapResources.car);
            var font = Resources.GetFont(Resources.FontResources.NinaB);
            Lcd.Screen.DrawImage(background, 0, 0);

            Lcd.Screen.DrawString("Hello, world", font, new SolidBrush(Color.White), 10, 400);
            Lcd.Screen.Flush();
            Lcd.CapacitiveScreenReleased += Lcd_CapacitiveScreenReleased; ;
            Thread.Sleep(Timeout.Infinite);
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
