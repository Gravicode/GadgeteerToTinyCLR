using System.Threading;
using Gadgeteer.Modules.GHIElectronics;
using GHI.Pins;

namespace TinyApp
{
    public class Program
    {

        public static void Main()
        {
            //Graphics
            /*
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
            TestLed7C();

            /*var joystik = new Joystick(FEZRaptor.Socket2.AnalogInput4, FEZRaptor.Socket2.AnalogInput5, FEZRaptor.Socket2.Pin3);
            var Lcd = new DisplayT35(FEZRaptor.Socket16.Pin9);
            var background = Resources.GetBitmap(Resources.BitmapResources.nature);
            var font = Resources.GetFont(Resources.FontResources.small);
            Lcd.Screen.DrawImage(background, 0, 0);
            
            screen.FillEllipse(new SolidBrush(Color.FromArgb(100, 0xFF, 0, 0)), 0, 0, 100, 100);
            screen.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 0xFF)), 0, 100, 100, 100);
            screen.DrawEllipse(new Pen(Color.Blue), 100, 0, 100, 100);
            screen.DrawRectangle(new Pen(Color.Red), 100, 100, 100, 100);
            screen.DrawLine(new Pen(Color.Green, 5), 250, 0, 220, 240);
            Lcd.Screen.DrawString("Hello, world", font, new SolidBrush(Color.White), 10, 200);
            var i = 0;
            screen.DrawLine(new Pen(Color.Black, 4), 260 + i, 10, 260 + i, 50); i += 4;
            screen.DrawLine(new Pen(Color.White, 4), 260 + i, 10, 260 + i, 50); i += 4;
            screen.DrawLine(new Pen(Color.Gray, 4), 260 + i, 10, 260 + i, 50); i += 4;
            screen.DrawLine(new Pen(Color.Red, 4), 260 + i, 10, 260 + i, 50); i += 4;
            screen.DrawLine(new Pen(Color.Green, 4), 260 + i, 10, 260 + i, 50); i += 4;
            screen.DrawLine(new Pen(Color.Blue, 4), 260 + i, 10, 260 + i, 50); i += 4;
            screen.DrawLine(new Pen(Color.Yellow, 4), 260 + i, 10, 260 + i, 50); i += 4;
            screen.DrawLine(new Pen(Color.Purple, 4), 260 + i, 10, 260 + i, 50); i += 4;
            screen.DrawLine(new Pen(Color.Teal, 4), 260 + i, 10, 260 + i, 50); i += 4;
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
            while (true)
            {
                Debug.WriteLine($"pos:{joystik.GetPosition().X} - { joystik.GetPosition().Y}");
                Thread.Sleep(200);
            }
            Thread.Sleep(Timeout.Infinite);
            //tunes test
            //PlayMusic();
            /*
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
            }*/
            /*
            //test button, light sense, gas sense, temp humid
            var controller = GpioController.GetDefault();
            var pin = controller.OpenPin(GHI.Pins.FEZRaptor.DebugLed);
            Button btn = new Button(FEZRaptor.Socket18.Pin3, FEZRaptor.Socket18.Pin4);
            LightSense light = new LightSense(FEZRaptor.Socket2.AnalogInput3);
            GasSense gas = new GasSense(FEZRaptor.Socket14.AnalogInput3, FEZRaptor.Socket14.Pin4);
            TempHumidSI70 TempSensor = new TempHumidSI70(GHIElectronics.TinyCLR.Pins.G400S.I2cBus.I2c1);
             
            pin.SetDriveMode(GpioPinDriveMode.Output);
            
            while (true)
            {

                if (btn.Pressed)
                {
                    Debug.WriteLine("button is pressed");
                    pin.Write(GpioPinValue.High);
                }
                else
                {
                    Debug.WriteLine("button is not pressed");
                    pin.Write(GpioPinValue.Low);
                }
                Debug.WriteLine($"light : {light.GetIlluminance()}");
                Debug.WriteLine($"gas : {gas.ReadProportion()}");
                Debug.WriteLine($"temp n humid : {TempSensor.TakeMeasurement().Temperature} - {TempSensor.TakeMeasurement().RelativeHumidity}");
                Thread.Sleep(200);
            }
            */
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
        static void PlayMusic()
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
    }
}
