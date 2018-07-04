using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CobaPrnView
{
    public partial class Form1 : Form
    {
        private string _currentFileName;
        PrnViewer _viewer = new PrnViewer();
        PrnPicture _picture;
        public Form1()
        {
            InitializeComponent();
            _Init();
        }
        private string _CurrentFileName
        {
            get
            {
                return _currentFileName;
            }
            set
            {
                _currentFileName = value;
                Text = _currentFileName;
                _viewer.Load(_currentFileName);
                _picture = new PrnPicture(_viewer.LabelWidth, _viewer.LabelLength);
                _ShowPrnInfo();
            }
        }
        private void _ShowPrnInfo()
        {
            _txtInfo.Text = string.Format("Label Length :{0}\r\nLabel Width : {1}\r\nHigh Resolution:{2}\r\nDot Offset : {3}",
                _viewer.LabelLength, _viewer.LabelWidth,_viewer.UseHighResolution,_viewer.DotTabOffset);
            var x = _viewer.CurrentByte();
            _Refresh();
        }
        private void _Init()
        {
            _btnOpenPrn.Click += (s, e) =>
            {
                _picture._Clear();
                _pic.Image = _picture.Image;
                _CurrentFileName = Utils.OpenFile();
            };
            _btnRefresh.Click += (s, e) => { _viewer.ResetData(); _Refresh(); };
            _viewer.OnCommandReaded += _viewer_OnCommandReaded;
            string last = AppDomain.CurrentDomain.BaseDirectory + "\\data\\last_file.txt";
            if (System.IO.File.Exists(last))
            {
                string x = System.IO.File.ReadAllText(last).Trim();
                _CurrentFileName = x;
             //   _Refresh();
            }
        }

        private void _viewer_OnCommandReaded(PrnCommands command, byte[] data, int count)
        {
            switch (command)
            {
                case PrnCommands.FeedLines:
                    _picture.FeedLines(count);
                    return;
                case PrnCommands.LeftMargin:
                    if(count > 0)
                    {

                    }
                    return;
                case PrnCommands.EndOfFile:
                    return;
                case PrnCommands.EndJob:
                    _pic.Image = _picture.Image;
                    _pic.Image.Save(AppDomain.CurrentDomain.BaseDirectory + "\\data\\pic.png", ImageFormat.Png);
                    return;
                case PrnCommands.PrintComprLine:
                    _picture.ResetX();
                    for (int i = 0; i < count; i++)
                    {
                        _picture.DrawCompressed(data[i]);
                    }
                    return;

                case PrnCommands.PrintLine:
                    _picture.ResetX();
                    for (int i = 0; i < count; i++)
                    {
                        _picture.Draw(data[i]);
                    }
                    return;

            }
        }

        private void _Refresh()
        {
            while (!_viewer.EOF)
            {
                _viewer.Read();
            }
        }
    }
}
