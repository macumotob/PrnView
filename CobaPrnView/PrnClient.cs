using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrnView
{
    class PrnClient
    {
        private TcpClient _client;
        private RequestParser _parser = new RequestParser();
        public PrnClient(TcpClient Client)
        {
            _client = Client;
            _parser.OnGet += (method, url) => _SendResponse(method, url);
            _ReadRequest();

            // Закроем соединение
            //  _client.Close();
        }
        private string _GetContentType(string file)
        {
            string contentType = "";
            string extension = System.IO.Path.GetExtension(file);
            // Пытаемся определить тип содержимого по расширению файла
            switch (extension)
            {
                case ".htm":
                case ".html":
                    return "text/html";

                case ".css":
                    return "text/stylesheet";
                case ".js":
                    return "text/javascript";

                case ".jpg":
                    return "image/jpeg";

                case ".jpeg":
                case ".png":
                case ".gif":
                    return "image/" + extension.Substring(1);
                default:
                    if (extension.Length > 1)
                    {
                        return "application/" + extension.Substring(1);
                    }
                    else
                    {
                        return "application/unknown";
                    }
            }
        }
        private void _SendError(TcpClient Client, int Code)
        {
            // Получаем строку вида "200 OK"
            // HttpStatusCode хранит в себе все статус-коды HTTP/1.1
            string CodeStr = Code.ToString() + " " + ((HttpStatusCode)Code).ToString();
            // Код простой HTML-странички
            string Html = "<html><body><h1>" + CodeStr + "</h1></body></html>";
            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            string Str = "HTTP/1.1 " + CodeStr + "\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" + Html;
            // Приведем строку к виду массива байт
            byte[] Buffer = Encoding.ASCII.GetBytes(Str);
            // Отправим его клиенту
            Client.GetStream().Write(Buffer, 0, Buffer.Length);
            // Закроем соединение
            Client.Close();
        }
        private void _SendWebSocket(string data)
        {
            const string eol = "\r\n"; // HTTP/1.1 defines the sequence CR LF as the end-of-line marker

            Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + eol
                + "Connection: Upgrade" + eol
                + "Upgrade: websocket" + eol
                + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                    System.Security.Cryptography.SHA1.Create().ComputeHash(
                        Encoding.UTF8.GetBytes(
                            new System.Text.RegularExpressions.Regex("Sec-WebSocket-Key: (.*)")
                            .Match(data).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                        )
                    )
                ) + eol
                + eol);

            _client.GetStream().Write(response, 0, response.Length);
          //  _client.Close();
        }

        private void _SendFile(string path)
        {
            string ContentType = _GetContentType(path);
            FileStream FS;
            try
            {
                FS = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception)
            {
                // Если случилась ошибка, посылаем клиенту ошибку 500
                _SendError(_client, 500);
                return;
            }

            // Посылаем заголовки
            string Headers = "HTTP/1.1 200 OK\nContent-Type: " + ContentType + "\nContent-Length: " + FS.Length + "\n\n";
            byte[] HeadersBuffer = Encoding.ASCII.GetBytes(Headers);
            _client.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);

            byte[] Buffer = new byte[1024 * 10];
            // Пока не достигнут конец файла
            while (FS.Position < FS.Length)
            {
                int count = FS.Read(Buffer, 0, Buffer.Length);
                _client.GetStream().Write(Buffer, 0, count);
            }

            // Закроем файл и соединение
            FS.Close();
            _client.Close();
            _keepAlive = false;
        }
        private void _SendResponse(PrinGetPost method, string url)
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory + "\\data";
            if (method == PrinGetPost.Get)
            {
                switch (url)
                {
                    case "/":
                        _SendFile(folder + "/index.html");
                        return;
                    case "/print":
                        _SendOk();
                        return;
                    default:
                        _SendFile(AppDomain.CurrentDomain.BaseDirectory + "\\data" + url);
                        return;
                }
            }
            if (method == PrinGetPost.Post)
            {

            }
            //_client.Close();
        }
        //private void _SendResponse()
        //{
        //    string Html = "<html><body><h1>PRINTER EMULATOR!</h1><br/><img src='/pic.png'/></body></html>";
        //    // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
        //    string Str = "HTTP/1.1 200 OK\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" + Html;
        //    // Приведем строку к виду массива байт
        //    byte[] Buffer = Encoding.ASCII.GetBytes(Str);
        //    // Отправим его клиенту
        //    _client.GetStream().Write(Buffer, 0, Buffer.Length);
        //}
        private void _SendOk(bool closeSocket = true)
        {
            string ContentType = "application/unknown";
            string Headers = "HTTP/1.1 200 OK\nContent-Type: " + ContentType + "\nContent-Length: " + 0 + "\n\n";
            byte[] HeadersBuffer = Encoding.ASCII.GetBytes(Headers);
            _client.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);
            if (closeSocket)
            {
                _client.Close();
            }

        }
        private BinaryWriter _bw = null;
        private string _prnFileName;
        private void _SavePrnToFile(byte[] data, int offset, int count)
        {
            if (_bw == null)
            {
                string folder = AppDomain.CurrentDomain.BaseDirectory + "\\data\\";
                _prnFileName = folder + Guid.NewGuid().ToString() + ".prn";
                _bw = new BinaryWriter(File.Open(_prnFileName, FileMode.OpenOrCreate));
            }
            _bw.Write(data, offset, count);

            while (_client.Available > 0 && (count = _client.GetStream().Read(data, 0, data.Length)) > 0)
            {
                _bw.Write(data, 0, count);
            }
            _bw.Close();
            _bw.Dispose();
            _bw = null;

            PrnPicture _picture = new PrnPicture();
            string file = _picture.MakePng(_prnFileName);
            string msg = "{ status : 0, msg: '" + file + "'}";
            _SendOk(false);
            _SendMessage(msg);
            //_SendFile(file);
            //_client.Close();
        }
        private void _SendMessage(string str)
        {
            //ns is a NetworkStream class parameter
            NetworkStream ns = _client.GetStream();
            //logger.Output(">> sendind data to client ...", LogLevel.LL_INFO);
            try
            {

                var buf = Encoding.UTF8.GetBytes(str);
                int frameSize = 64;

                var parts = buf.Select((b, i) => new { b, i })
                                .GroupBy(x => x.i / (frameSize - 1))
                                .Select(x => x.Select(y => y.b).ToArray())
                                .ToList();

                for (int i = 0; i < parts.Count; i++)
                {
                    byte cmd = 0;
                    if (i == 0) cmd |= 1;
                    if (i == parts.Count - 1) cmd |= 0x80;

                    ns.WriteByte(cmd);
                    ns.WriteByte((byte)parts[i].Length);
                    ns.Write(parts[i], 0, parts[i].Length);
                }

                ns.Flush();

            }
            catch (Exception ex)
            {

            }
         //   _client.Close();
        }
        private bool _keepAlive = true;
        private void _ReadRequest()
        {
            string request = "";
            byte[] buffer = new byte[1024 * 4];
            int count;

            //if (_client.Connected && _client.Client.Available > 0)
            while(_keepAlive)
            {
                while ((count = _client.GetStream().Read(buffer, 0, buffer.Length)) > 0)
                {
                    var method = _parser.GetMethod(buffer);
                    if (method == PrinGetPost.Get)
                    {
                        int index = _parser.FindEndOfHeader(buffer, 0);
                        request += Encoding.ASCII.GetString(buffer, 0, index);
                        if (request.IndexOf("Upgrade: websocket") != -1)
                        {
                            _SendWebSocket(request);
                            break;
                        }
                        _parser.Parse(request);
                        break;
                    }
                    else if (method == PrinGetPost.WebSocket)
                    {
                        string message = Utils.DecodeMessage(buffer);
                    }
                    else
                    {
                        int index = _parser.FindEndOfHeader(buffer, 0);
                        if (index != -1)
                        {
                            index = _parser.FindEndOfHeader(buffer, index);
                        }
                        request += Encoding.ASCII.GetString(buffer, 0, index);
                        _parser.Parse(request);
                        _SavePrnToFile(buffer, index, count - index);
                    }
                    break;
                    //if (_parser.IsPost)
                    //{
                    //    _SavePrnToFile(buffer, 0, count);
                    //}
                    //request += Encoding.ASCII.GetString(buffer, 0, count);
                    //int index = request.IndexOf("\r\n\r\n");
                    //if (index >= 0)
                    //{
                    //    _parser.Parse(request);
                    //    if (_parser.IsPost)
                    //    {
                    //        _SavePrnToFile(buffer, index + 4, count - index - 4);
                    //        break;
                    //    }
                    //    else
                    //    {
                    //        break;
                    //    }
                    //    //_SendOk();
                    //  //  break;
                    //}
                }
                Thread.Sleep(100);
            }

        }
    }

}
