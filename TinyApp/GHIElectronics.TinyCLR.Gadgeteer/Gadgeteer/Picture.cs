namespace Gadgeteer
{
    //using Microsoft.SPOT;
    using System;
    using System.Drawing;
    using System.IO;

    public class Picture
    {
        private PictureEncoding encoding;
        private byte[] pictureData;

        public Picture(byte[] pictureData, PictureEncoding encoding)
        {
            this.PictureData = pictureData;
            
            this.Encoding = encoding;
        }

        public Bitmap MakeBitmap()
        {
            return (Bitmap) this;
        }

        public static explicit operator Bitmap(Picture picture)
        {
            Stream stream = new MemoryStream(picture.pictureData);
            return new Bitmap(stream);
        }

        public PictureEncoding Encoding
        {
            get
            {
                return this.encoding;
            }
            private set
            {
                this.encoding = value;
            }
        }

        public byte[] PictureData
        {
            get
            {
                return this.pictureData;
            }
            private set
            {
                this.pictureData = value;
            }
        }

        public enum PictureEncoding
        {
            BMP = 3,
            GIF = 1,
            JPEG = 2
        }
    }
}

