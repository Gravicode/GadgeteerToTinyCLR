namespace Gadgeteer.SocketInterfaces
{
    using System;
    using System.Text;
    using System.Threading;

    public abstract class Serial : IDisposable
    {
        private SerialDataReceivedEventHandler dataReceived;
        private System.Text.Decoder decoder;
        private System.Text.Encoding encoding = System.Text.Encoding.UTF8;
        private SerialLineReceivedEventHandler lineReceived;
        private string newLine = "\r\n";
        private ManualResetEvent readLineContinueEvent;
        private Thread readLineThread;

        public event SerialDataReceivedEventHandler DataReceived
        {
            add
            {
                if (this.dataReceived == null)
                {
                    this.OnDataReceivedFirstSubscribed();
                }
                this.dataReceived = (SerialDataReceivedEventHandler) Delegate.Combine(this.dataReceived, value);
            }
            remove
            {
                this.dataReceived = (SerialDataReceivedEventHandler) Delegate.Remove(this.dataReceived, value);
                if (this.dataReceived == null)
                {
                    this.OnDataReceivedLastUnsubscribed();
                }
            }
        }

        public event SerialLineReceivedEventHandler LineReceived
        {
            add
            {
                if (this.lineReceived == null)
                {
                    this.OnLineReceivedFirstSubscribed();
                }
                this.lineReceived = (SerialLineReceivedEventHandler) Delegate.Combine(this.lineReceived, value);
            }
            remove
            {
                this.lineReceived = (SerialLineReceivedEventHandler) Delegate.Remove(this.lineReceived, value);
                if (this.lineReceived == null)
                {
                    this.OnLineReceivedLastUnsubscribed();
                }
            }
        }

        protected Serial()
        {
        }

        public abstract void Close();
        public abstract void DiscardInBuffer();
        public abstract void DiscardOutBuffer();
        public virtual void Dispose()
        {
        }

        public abstract void Flush();
        protected abstract void OnDataReceivedFirstSubscribed();
        protected abstract void OnDataReceivedLastUnsubscribed();
        protected virtual void OnLineReceivedFirstSubscribed()
        {
            if (this.readLineThread == null)
            {
                if (this.readLineContinueEvent == null)
                {
                    this.readLineContinueEvent = new ManualResetEvent(true);
                }
                this.readLineThread = new Thread(new ThreadStart(this.ReadLineThread));
                this.readLineThread.Start();
            }
            this.readLineContinueEvent.Set();
        }

        protected virtual void OnLineReceivedLastUnsubscribed()
        {
            if (this.readLineThread != null)
            {
                this.readLineContinueEvent.Reset();
                if ((this.readLineThread.ThreadState & ThreadState.WaitSleepJoin) == ThreadState.WaitSleepJoin)
                {
                    this.readLineThread = null;
                    this.readLineThread.Abort();
                }
            }
        }

        public abstract void Open();
        protected void RaiseDataReceived()
        {
            SerialDataReceivedEventHandler dataReceived = this.dataReceived;
            if (dataReceived != null)
            {
                dataReceived(this);
            }
        }

        protected void RaiseLineReceived(string line)
        {
            SerialLineReceivedEventHandler lineReceived = this.lineReceived;
            if (lineReceived != null)
            {
                lineReceived(this, line);
            }
        }

        public abstract int Read(byte[] buffer, int offset, int count);
        public virtual int ReadByte()
        {
            byte[] buffer = new byte[1];
            if (this.Read(buffer, 0, 1) > 0)
            {
                return buffer[0];
            }
            return -1;
        }

        private void ReadLineThread()
        {
            this.decoder = this.encoding.GetDecoder();
            byte[] buffer = new byte[4];
            char[] chars = new char[2];
            int offset = 0;
            bool completed = false;
            StringBuilder builder = new StringBuilder();
            int num4 = 0;
            while (true)
            {
                if (!this.readLineContinueEvent.WaitOne(0, false))
                {
                    num4 = 0;
                    offset = 0;
                    completed = false;
                }
                this.readLineContinueEvent.WaitOne();
                if (!this.IsOpen)
                {
                    Thread.Sleep(100);
                }
                else
                {
                    int num5 = this.Read(buffer, offset, 1);
                    if (num5 > 0)
                    {
                        num4 += num5;
                        int byteIndex = 0;
                        while (!completed)
                        {
                            int num2;
                            int num3;
                            this.decoder.Convert(buffer, byteIndex, num4 - byteIndex, chars, 0, chars.Length, false, out num2, out num3, out completed);
                            if (num3 > 0)
                            {
                                builder.Append(chars, 0, num3);
                                if (builder.Length >= this.newLine.Length)
                                {
                                    int startIndex = 0;
                                    for (int i = Math.Max(0, builder.Length - Math.Max(this.newLine.Length, num3)); i <= (builder.Length - this.newLine.Length); i++)
                                    {
                                        if (builder.get_Item(i) != this.newLine[0])
                                        {
                                            continue;
                                        }
                                        bool flag2 = true;
                                        for (int j = 1; j < this.newLine.Length; j++)
                                        {
                                            if (builder.get_Item(i + j) != this.newLine[j])
                                            {
                                                flag2 = false;
                                                break;
                                            }
                                        }
                                        if (flag2)
                                        {
                                            string line = builder.ToString().Substring(startIndex, i - startIndex);
                                            this.RaiseLineReceived(line);
                                            startIndex = i + this.newLine.Length;
                                            i = startIndex - 1;
                                        }
                                    }
                                    if (startIndex > 0)
                                    {
                                        builder.Remove(0, startIndex);
                                    }
                                }
                            }
                            if (num2 > 0)
                            {
                                byteIndex += num2;
                            }
                            else
                            {
                                if (byteIndex > 0)
                                {
                                    Array.Copy(buffer, byteIndex, buffer, 0, num4 - byteIndex);
                                    offset = 0;
                                }
                                if (++offset >= buffer.Length)
                                {
                                    byte[] array = new byte[(buffer.Length * 3) / 2];
                                    buffer.CopyTo(array, 0);
                                    buffer = array;
                                }
                                break;
                            }
                        }
                        if (completed)
                        {
                            num4 = 0;
                            offset = 0;
                            completed = false;
                        }
                    }
                }
            }
        }

        public virtual void Write(params byte[] data)
        {
            if (data != null)
            {
                this.Write(data, 0, data.Length);
            }
        }

        public virtual void Write(string text)
        {
            if (text != null)
            {
                byte[] bytes = this.Encoding.GetBytes(text);
                this.Write(bytes, 0, bytes.Length);
            }
        }

        public abstract void Write(byte[] buffer, int offset, int count);
        public virtual void WriteLine(string text)
        {
            this.Write(text + this.NewLine);
        }

        public abstract int BaudRate { get; set; }

        public abstract int BytesToRead { get; }

        public abstract int BytesToWrite { get; }

        public abstract int DataBits { get; set; }

        public System.Text.Encoding Encoding
        {
            get
            {
                return this.encoding;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.encoding = value;
                if (this.decoder != null)
                {
                    this.decoder = value.GetDecoder();
                }
            }
        }

        public abstract bool IsOpen { get; }

        public abstract bool IsUsingHardwareFlowControl { get; }

        public string NewLine
        {
            get
            {
                return this.newLine;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                if (value.Length < 1)
                {
                    throw new ArgumentException();
                }
                this.newLine = value;
            }
        }

        public abstract SerialParity Parity { get; set; }

        public abstract string PortName { get; }

        public abstract int ReadTimeout { get; set; }

        public abstract SerialStopBits StopBits { get; set; }

        public abstract int WriteTimeout { get; set; }
    }
}

