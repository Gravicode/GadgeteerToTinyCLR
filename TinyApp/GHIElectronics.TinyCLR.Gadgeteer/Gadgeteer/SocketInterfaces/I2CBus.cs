namespace Gadgeteer.SocketInterfaces
{
    using Microsoft.SPOT.Hardware;
    using System;
    using System.Runtime.InteropServices;

    public abstract class I2CBus : IDisposable
    {
        public ErrorBehavior LengthErrorBehavior;

        protected I2CBus()
        {
        }

        public virtual void Dispose()
        {
        }

        public virtual int Execute(params I2CDevice.I2CTransaction[] transactions)
        {
            if (transactions == null)
            {
                return 0;
            }
            int num = 0;
            for (int i = 0; i < transactions.Length; i++)
            {
                int num4;
                int num5;
                int writeLength = (transactions[i].Buffer == null) ? 0 : transactions[i].Buffer.Length;
                if ((((i + 1) < transactions.Length) && (transactions[i] is I2CDevice.I2CWriteTransaction)) && (transactions[i + 1] is I2CDevice.I2CReadTransaction))
                {
                    this.WriteRead(transactions[i].Buffer, 0, transactions[i].Buffer.Length, transactions[i + 1].Buffer, 0, transactions[i + 1].Buffer.Length, out num4, out num5);
                    i++;
                    if ((this.LengthErrorBehavior == ErrorBehavior.ThrowException) && ((num4 != writeLength) || (num5 != transactions[i + 1].Buffer.Length)))
                    {
                        throw this.NewLengthErrorException();
                    }
                }
                else if (transactions[i] is I2CDevice.I2CWriteTransaction)
                {
                    this.WriteRead(transactions[i].Buffer, 0, writeLength, null, 0, 0, out num4, out num5);
                    if ((this.LengthErrorBehavior == ErrorBehavior.ThrowException) && (num4 != writeLength))
                    {
                        throw this.NewLengthErrorException();
                    }
                }
                else
                {
                    this.WriteRead(null, 0, 0, transactions[i].Buffer, 0, writeLength, out num4, out num5);
                    if ((this.LengthErrorBehavior == ErrorBehavior.ThrowException) && (num5 != writeLength))
                    {
                        throw this.NewLengthErrorException();
                    }
                }
                num += num4 + num5;
            }
            return num;
        }

        internal Exception NewLengthErrorException()
        {
            return new ApplicationException("I2C: Exception writing to device at address " + this.Address + " - perhaps device is not responding or not plugged in.");
        }

        public int Read(byte[] readBuffer)
        {
            return this.WriteRead(null, readBuffer);
        }

        public byte ReadRegister(byte register)
        {
            byte[] writeBuffer = new byte[] { register };
            if (this.WriteRead(writeBuffer, writeBuffer) > 0)
            {
                return writeBuffer[0];
            }
            return 0;
        }

        public int Write(params byte[] writeBuffer)
        {
            return this.WriteRead(writeBuffer, null);
        }

        public int WriteRead(byte[] writeBuffer, byte[] readBuffer)
        {
            int num3;
            int num4;
            int writeLength = (writeBuffer == null) ? 0 : writeBuffer.Length;
            int readLength = (readBuffer == null) ? 0 : readBuffer.Length;
            this.WriteRead(writeBuffer, 0, writeLength, readBuffer, 0, readLength, out num3, out num4);
            if (num3 < 0)
            {
                return num3;
            }
            if (num4 < 0)
            {
                return num4;
            }
            return (num3 + num4);
        }

        public abstract void WriteRead(byte[] writeBuffer, int writeOffset, int writeLength, byte[] readBuffer, int readOffset, int readLength, out int numWritten, out int numRead);

        public abstract ushort Address { get; set; }

        public abstract int ClockRateKHz { get; set; }

        public abstract int Timeout { get; set; }
    }
}

