namespace Gadgeteer.SocketInterfaces
{
    using System;

    public abstract class Spi : IDisposable
    {
        protected Spi()
        {
        }

        public virtual void Dispose()
        {
        }

        public void Write(params byte[] writeBuffer)
        {
            this.WriteRead(writeBuffer, 0, (writeBuffer == null) ? 0 : writeBuffer.Length, null, 0, 0, 0);
        }

        public void Write(params ushort[] writeBuffer)
        {
            this.WriteRead(writeBuffer, 0, (writeBuffer == null) ? 0 : writeBuffer.Length, null, 0, 0, 0);
        }

        public void WriteRead(byte[] writeBuffer, byte[] readBuffer)
        {
            this.WriteRead(writeBuffer, 0, (writeBuffer == null) ? 0 : writeBuffer.Length, readBuffer, 0, (readBuffer == null) ? 0 : readBuffer.Length, 0);
        }

        public void WriteRead(ushort[] writeBuffer, ushort[] readBuffer)
        {
            this.WriteRead(writeBuffer, 0, (writeBuffer == null) ? 0 : writeBuffer.Length, readBuffer, 0, (readBuffer == null) ? 0 : readBuffer.Length, 0);
        }

        public void WriteRead(byte[] writeBuffer, byte[] readBuffer, int readOffset)
        {
            this.WriteRead(writeBuffer, 0, (writeBuffer == null) ? 0 : writeBuffer.Length, readBuffer, 0, (readBuffer == null) ? 0 : readBuffer.Length, readOffset);
        }

        public void WriteRead(ushort[] writeBuffer, ushort[] readBuffer, int readOffset)
        {
            this.WriteRead(writeBuffer, 0, (writeBuffer == null) ? 0 : writeBuffer.Length, readBuffer, 0, (readBuffer == null) ? 0 : readBuffer.Length, readOffset);
        }

        public abstract void WriteRead(byte[] writeBuffer, int writeOffset, int writeLength, byte[] readBuffer, int readOffset, int readLength, int startReadOffset);
        public abstract void WriteRead(ushort[] writeBuffer, int writeOffset, int writeLength, ushort[] readBuffer, int readOffset, int readLength, int startReadOffset);

        public abstract SpiConfiguration Configuration { get; set; }
    }
}

