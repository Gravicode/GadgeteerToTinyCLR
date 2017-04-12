namespace Gadgeteer.SocketInterfaces
{
    using Gadgeteer;
    using Gadgeteer.Modules;
    using Microsoft.SPOT.Hardware;
    using System;
    using System.Runtime.InteropServices;

    internal class NativeI2CBus : I2CBus
    {
        private I2CDevice.Configuration _configuration;
        private static I2CDevice _device;
        private int timeout = 0x3e8;

        public NativeI2CBus(Socket socket, ushort address, int clockRateKhz, Module module)
        {
            if (_device == null)
            {
                _device = new I2CDevice(new I2CDevice.Configuration(0, 50));
            }
            this._configuration = new I2CDevice.Configuration(address, clockRateKhz);
        }

        public override int Execute(params I2CDevice.I2CTransaction[] transactions)
        {
            int num;
            I2CDevice device = _device;
            lock (device)
            {
                _device.Config = this._configuration;
                num = _device.Execute(transactions, this.timeout);
            }
            if (base.LengthErrorBehavior == ErrorBehavior.ThrowException)
            {
                int num2 = 0;
                for (int i = 0; i < transactions.Length; i++)
                {
                    num2 += transactions[i].Buffer.Length;
                }
                if (num2 != num)
                {
                    throw base.NewLengthErrorException();
                }
            }
            return num;
        }

        public override void WriteRead(byte[] writeBuffer, int writeOffset, int writeLength, byte[] readBuffer, int readOffset, int readLength, out int numWritten, out int numRead)
        {
            numWritten = 0;
            numRead = 0;
            if ((readLength != 0) || (writeLength != 0))
            {
                if ((readLength == 0) && (writeOffset == 0))
                {
                    I2CDevice.I2CTransaction[] transactions = new I2CDevice.I2CTransaction[] { I2CDevice.CreateWriteTransaction(writeBuffer) };
                    numWritten = this.Execute(transactions);
                }
                else if ((writeLength == 0) && (readOffset == 0))
                {
                    I2CDevice.I2CTransaction[] transactionArray2 = new I2CDevice.I2CTransaction[] { I2CDevice.CreateReadTransaction(readBuffer) };
                    numRead = this.Execute(transactionArray2);
                }
                else if ((readOffset == 0) && (writeOffset == 0))
                {
                    I2CDevice.I2CTransaction[] transactionArray3 = new I2CDevice.I2CTransaction[] { I2CDevice.CreateWriteTransaction(writeBuffer), I2CDevice.CreateReadTransaction(readBuffer) };
                    int num = this.Execute(transactionArray3);
                    numWritten = Math.Min(num, writeLength);
                    numRead = num - numWritten;
                }
                else
                {
                    I2CDevice.I2CTransaction transaction = null;
                    I2CDevice.I2CTransaction transaction2 = null;
                    I2CDevice.I2CTransaction[] transactionArray = new I2CDevice.I2CTransaction[1];
                    int index = 0;
                    if (writeLength > 0)
                    {
                        transactionArray[0] = transaction = I2CDevice.CreateWriteTransaction(writeBuffer);
                        index++;
                    }
                    if (readLength > 0)
                    {
                        if (index > 0)
                        {
                            transactionArray = new I2CDevice.I2CTransaction[2];
                            transactionArray[0] = transaction;
                        }
                        transactionArray[index] = transaction2 = I2CDevice.CreateReadTransaction(readBuffer);
                    }
                    int num3 = this.Execute(transactionArray);
                    numWritten = Math.Min(num3, writeLength);
                    numRead = num3 - numWritten;
                }
            }
        }

        public override ushort Address
        {
            get
            {
                return this._configuration.Address;
            }
            set
            {
                this._configuration = new I2CDevice.Configuration(value, this._configuration.ClockRateKhz);
            }
        }

        public override int ClockRateKHz
        {
            get
            {
                return this._configuration.ClockRateKhz;
            }
            set
            {
                this._configuration = new I2CDevice.Configuration(this._configuration.Address, value);
            }
        }

        public override int Timeout
        {
            get
            {
                return this.timeout;
            }
            set
            {
                this.timeout = value;
            }
        }
    }
}

