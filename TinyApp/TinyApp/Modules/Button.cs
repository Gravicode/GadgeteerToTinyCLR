﻿using GHIElectronics.TinyCLR.Devices.Gpio;
using GT = Gadgeteer;
//using GTI = Gadgeteer.SocketInterfaces;
using GTM = Gadgeteer.Modules;

namespace Gadgeteer.Modules.GHIElectronics
{
    /// <summary>A Button module for .NET Gadgeteer</summary>
    public class Button : GTM.Module
    {
        private GpioPin input;
        private GpioPin led;
        private LedMode currentMode;

        private ButtonEventHandler onButtonEvent;

        /// <summary>Represents the delegate that is used to handle the <see cref="ButtonReleased" /> and <see cref="ButtonPressed" /> events.</summary>
        /// <param name="sender">The <see cref="Button" /> object that raised the event.</param>
        /// <param name="state">The state of the Button</param>
        public delegate void ButtonEventHandler(Button sender, ButtonState state);

        /// <summary>Raised when the button is released.</summary>
        public event ButtonEventHandler ButtonReleased;

        /// <summary>Raised when the button is pressed.</summary>
        public event ButtonEventHandler ButtonPressed;

        /// <summary>Whether or not the button is pressed.</summary>
        public bool Pressed
        {
            get
            {
                return !(this.input.Read() == GpioPinValue.High);
            }
        }

        /// <summary>Whether or not the LED is currently on or off.</summary>
        public bool IsLedOn
        {
            get
            {
                return this.led.Read() == GpioPinValue.High;
            }
        }

        /// <summary>Gets or sets the LED's current mode of operation.</summary>
        public LedMode Mode
        {
            get
            {
                return this.currentMode;
            }

            set
            {
                this.currentMode = value;

                if (this.currentMode == LedMode.On || (this.currentMode == LedMode.OnWhilePressed && this.Pressed) || (this.currentMode == LedMode.OnWhileReleased && !this.Pressed))
                    this.TurnLedOn();
                else if (this.currentMode == LedMode.Off || (this.currentMode == LedMode.OnWhileReleased && this.Pressed) || (this.currentMode == LedMode.OnWhilePressed && !this.Pressed))
                    this.TurnLedOff();
            }
        }

        /// <summary>The state of the button.</summary>
        public enum ButtonState
        {

            /// <summary>The button is pressed.</summary>
            Pressed = 0,

            /// <summary>The button is released.</summary>
            Released = 1
        }

        /// <summary>The various modes a LED can be set to.</summary>
        public enum LedMode
        {

            /// <summary>The LED is on regardless of the button state.</summary>
            On,

            /// <summary>The LED is off regardless of the button state.</summary>
            Off,

            /// <summary>The LED changes state whenever the button is pressed.</summary>
            ToggleWhenPressed,

            /// <summary>The LED changes state whenever the button is released.</summary>
            ToggleWhenReleased,

            /// <summary>The LED is on while the button is pressed.</summary>
            OnWhilePressed,

            /// <summary>The LED is on except when the button is pressed.</summary>
            OnWhileReleased
        }

        /// <summary>Constructs a new instance.</summary>
        /// <param name="DigitalPin3">The mainboard pin that has digital pin.</param>
        public Button(int DigitalPin3, int DigitalPin4)
        {
            //Socket socket = Socket.GetSocket(socketNumber, true, this, null);

            //socket.EnsureTypeIsSupported(new char[] { 'X', 'Y' }, this);

            this.currentMode = LedMode.Off;
            var controller = GpioController.GetDefault();
            this.led = controller.OpenPin(DigitalPin4);
            this.led.SetDriveMode(GpioPinDriveMode.Output);

            this.input = controller.OpenPin(DigitalPin3);//GTI.InterruptInputFactory.Create(socket, GT.Socket.Pin.Three, GTI.GlitchFilterMode.On, GTI.ResistorMode.PullUp, GTI.InterruptMode.RisingAndFallingEdge, this);
            if (input.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                input.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                input.SetDriveMode(GpioPinDriveMode.Input);

            //this.input.Interrupt += this.OnInterrupt;
            input.ValueChanged += Input_ValueChanged; ;
        }

        private void Input_ValueChanged(object sender, GpioPinValueChangedEventArgs e)
        {

            var state = e.Edge == GpioPinEdge.FallingEdge ? ButtonState.Released : ButtonState.Pressed;

            switch (state)
            {
                case ButtonState.Released:
                    if (this.Mode == LedMode.OnWhilePressed)
                        this.TurnLedOff();
                    else if (this.Mode == LedMode.OnWhileReleased)
                        this.TurnLedOn();
                    else if (this.Mode == LedMode.ToggleWhenReleased)
                        this.ToggleLED();

                    break;

                case ButtonState.Pressed:
                    if (this.Mode == LedMode.OnWhilePressed)
                        this.TurnLedOn();
                    else if (this.Mode == LedMode.OnWhileReleased)
                        this.TurnLedOff();
                    else if (this.Mode == LedMode.ToggleWhenPressed)
                        this.ToggleLED();

                    break;
            }

            this.OnButtonEvent(this, state);
        }


        /// <summary>Turns on the LED.</summary>
        public void TurnLedOn()
        {
            this.led.Write(GpioPinValue.High);
        }

        /// <summary>Turns off the LED.</summary>
        public void TurnLedOff()
        {
            this.led.Write(GpioPinValue.Low);
        }

        /// <summary>Turns the LED off if it is on and on if it is off.</summary>
        public void ToggleLED()
        {
            if (this.IsLedOn)
                this.TurnLedOff();
            else
                this.TurnLedOn();
        }

        /*
         private void OnInterrupt(GTI.InterruptInput input, bool value)
        {
            var state = value ? ButtonState.Released : ButtonState.Pressed;

            switch (state)
            {
                case ButtonState.Released:
                    if (this.Mode == LedMode.OnWhilePressed)
                        this.TurnLedOff();
                    else if (this.Mode == LedMode.OnWhileReleased)
                        this.TurnLedOn();
                    else if (this.Mode == LedMode.ToggleWhenReleased)
                        this.ToggleLED();

                    break;

                case ButtonState.Pressed:
                    if (this.Mode == LedMode.OnWhilePressed)
                        this.TurnLedOn();
                    else if (this.Mode == LedMode.OnWhileReleased)
                        this.TurnLedOff();
                    else if (this.Mode == LedMode.ToggleWhenPressed)
                        this.ToggleLED();

                    break;
            }

            this.OnButtonEvent(this, state);
        }
        */
        private void OnButtonEvent(Button sender, ButtonState state)
        {
            if (this.onButtonEvent == null)
                this.onButtonEvent = this.OnButtonEvent;
            switch (state)
            {
                case ButtonState.Released:
                    if(ButtonReleased!=null)
                        this.ButtonReleased(sender, state); break;
                case ButtonState.Pressed:
                    if(ButtonPressed!=null)
                    this.ButtonPressed(sender, state); break;
            }
            /*
            if (Program.CheckAndInvoke(state == ButtonState.Released ? this.ButtonReleased : this.ButtonPressed, this.onButtonEvent, sender, state))
            {
                switch (state)
                {
                    case ButtonState.Released: this.ButtonReleased(sender, state); break;
                    case ButtonState.Pressed: this.ButtonPressed(sender, state); break;
                }
            }*/
        }
    }
}
