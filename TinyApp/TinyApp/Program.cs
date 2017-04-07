using System;
using System.Collections;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
//using GHIElectronics.TinyCLR.Pins;
using Gadgeteer.Modules.GHIElectronics;
using GHI.Pins;
using System.Diagnostics;

namespace TinyApp
{
    public class Program
    {
        public static void Main()
        {
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
            
        }
    }
}
