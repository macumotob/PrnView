using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrnView
{
    public enum PrinGetPost
    {
        Get,
        Post
    }
    class RequestParser
    {
        public delegate void EventGet(PrinGetPost method,string url);
        public event EventGet OnGet;

        const string _GET_ = "GET ";
        const string _POST_ = "POST ";
        const string _HTTP_1_1_ = " HTTP/1.1";
        const string _CONTENT_LENGTH_ = "Content-Length:";
        public string Get;
        public bool IsPost;
        public int ConentLength = 0;
        private void _Clear()
        {
            Get = null;
            IsPost = false;
            ConentLength = 0;
        }
        private void _Raise(PrinGetPost method, string url)
        {
            if(OnGet != null)
            {
                OnGet(method,url);
            }
        }
        public void Parse(string request)
        {
            _Clear();
            string[] data = request.Split('\n');
            string s;
            foreach(var item in data)
            {
                s = item.Replace("\r", "").Trim();
                if(s.IndexOf(_GET_) == 0)
                {
                    s = s.Replace(_GET_, "").Trim();
                    s = s.Replace(_HTTP_1_1_, "").Trim();
                    _Raise( PrinGetPost.Get,  s);
                    return;
                }
                if(s.IndexOf(_POST_) == 0)
                {
                    s = s.Replace(_POST_, "").Trim();
                    s = s.Replace(_HTTP_1_1_, "").Trim();
                    Get = s;
                    IsPost = true;
                    //_Raise( PrinGetPost.Post, s);
                }
                else if(s.IndexOf(_CONTENT_LENGTH_) == 0)
                {
                    s = s.Replace(_CONTENT_LENGTH_, "").Trim();
                    ConentLength = int.Parse(s);
                    return;
                }
            }
        }
        public int FindEndOfHeader(byte [] data, int offset)
        {
            while(offset < data.Length - 4)
            {
                if(data[offset] == 0x0D && data[offset+1] == 0x0A && data[offset+2] == 0x0D && data[offset+3] == 0x0A)
                {
                    offset += 4;
                    return offset;
                }
                offset++;
            }
            return -1;
        }
        public PrinGetPost GetMethod(byte [] data)
        {
            string method = Encoding.ASCII.GetString(data, 0, 4);
            if (method == _GET_)
            {
                return PrinGetPost.Get;
            }
            method = Encoding.ASCII.GetString(data, 0, 5);
            if (method == _POST_)
            {
                return PrinGetPost.Post;
            }
            throw new Exception("unsupported method");
        }
    }
}
