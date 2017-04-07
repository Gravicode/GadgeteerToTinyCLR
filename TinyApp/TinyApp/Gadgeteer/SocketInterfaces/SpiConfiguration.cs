namespace Gadgeteer.SocketInterfaces
{
    using System;
    using System.Runtime.InteropServices;

    public class SpiConfiguration
    {
        public readonly uint ChipSelectHoldTime;
        public readonly uint ChipSelectSetupTime;
        public readonly uint ClockRateKHz;
        public readonly bool IsBusyActiveHigh;
        public readonly bool IsChipSelectActiveHigh;
        public readonly bool IsClockIdleHigh;
        public readonly bool IsClockSamplingEdgeRising;

        public SpiConfiguration(bool chipSelectActiveHigh, uint chipSelectSetupTime, uint chipSelectHoldTime, bool clockIdleHigh, bool clockSamplingEdgeRising, uint clockRateKHz, [Optional, DefaultParameterValue(false)] bool busyActiveHigh)
        {
            this.IsChipSelectActiveHigh = chipSelectActiveHigh;
            this.ChipSelectSetupTime = chipSelectSetupTime;
            this.ChipSelectHoldTime = chipSelectHoldTime;
            this.IsClockIdleHigh = clockIdleHigh;
            this.IsClockSamplingEdgeRising = clockSamplingEdgeRising;
            this.ClockRateKHz = clockRateKHz;
            this.IsBusyActiveHigh = busyActiveHigh;
        }
    }
}

