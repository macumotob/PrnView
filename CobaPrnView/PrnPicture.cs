using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CobaPrnView
{

    public class PrnPicture
    {
        private Bitmap _bitmap;
        private Graphics _g;
        private int _width;
        private int _height;
        private int _x = 0;
        private int _y = 0;
        private int _step = 1;

        public PrnPicture(int width, int height)
        {
            _width = width;
            _height = height;
            _bitmap = new Bitmap(_width * _step, _height *_step, System.Drawing.Imaging.PixelFormat.Format24bppRgb); // or some other format
            _g = Graphics.FromImage(_bitmap);
            _y = -1 * _step;
            _Clear();

        }
        public Image Image
        {
            get
            {
                return _bitmap;
            }
        }
        System.Drawing.SolidBrush _blackBush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        System.Drawing.SolidBrush _whiteBush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
        System.Drawing.SolidBrush _blueBush = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);
        private void _Clear()
        {
            using (System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
            {
                _g.FillRectangle(myBrush, new Rectangle(0, 0, _width, _height)); // whatever
            }
        }
        public void ResetX()
        {
            _x = 0;
            _y += _step;
        }
        private byte[] _bytes = new byte[1];
        public void FeedLines(int count)
        {
            _y += count * _step;
        }
        public void Draw(byte data)
        {
            _bytes[0] = data;

            var bits = new BitArray(_bytes);
            foreach (bool bit in bits)
            {
                if (bit)
                {
                    _g.FillRectangle(_blackBush, new Rectangle(_x, _y, _step, _step)); // whatever
                }
                else
                {
                    _g.FillRectangle(_whiteBush, new Rectangle(_x, _y, _step, _step)); // whatever
                }
                _x += _step;
            }
        }
        public void DrawCompressed(byte data)
        {
            _bytes[0] = data;
            byte count = (byte)(data - 0x7F - 1);

            if (data < 0x7F) return;

            for (var i = 0; i < count; i++)
            {
                _g.FillRectangle(_blueBush, new Rectangle(_x, _y, _step, _step)); // whatever
                _x += _step;
            }
        }

    }
}
