using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrnView
{
    public static class Utils
    {
        public static string OpenFile()
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog1.Filter = "prn files (*.prn)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    return openFileDialog1.FileName;
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            // Insert code to read the stream here.
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            return null;
        }


        private static bool is_bit_set(byte value, int bitindex)
        {
            return (value & (1 << bitindex)) != 0;
        }

        public static void TestRenderLine()
        {
            byte[] buf = { 0xFF, 0x01, 0xFF,0x00, 0xFF, 0x23, 0xFF, 0xF5, 0xFF, 0x00, 0x80};
            renderImage(buf, buf.Length, 0);
        }
        private static void renderImage(
            //LeitzDevOEM leitzDevOEM, 
            byte[] pBuf, int
            dwLen,
            int leftMargin)
        {

            int dataCount = dwLen - leftMargin;
            //size_t res;
            //if (leitzDevOEM->PreviousBytesCount != dataCount)
            //{
            //    res = (size_t)writeBytesPerLine((byte)(dwLen - leftMargin));

            //    if (res == E_FAIL)
            //        return E_FAIL;
            //    leitzDevOEM->PreviousBytesCount = (WORD)dataCount;
            //}

            //byte cmdB[3] = { 0x1B, 'B', (byte)leftMargin };
            //res = writeCommand(cmdB, 3);
            //if (res == E_FAIL)
            //    return E_FAIL;

            //std::vector<byte> pOptBuf;
            List<byte> pOptBuf = new List<byte>();

            int counter = leftMargin;
            byte byteToSend = 0xFF;
            bool lastValueIsBlack = false;

            while (counter < dwLen)
            {
                if (pOptBuf.Count >= dwLen - leftMargin)
                    break;

                if ((byteToSend + 8 <= 127)
                  && ((lastValueIsBlack && pBuf[counter] == 0xFF)
                    || (!lastValueIsBlack && pBuf[counter] == 0x00)))
                {
                    byteToSend += 8;
                    counter++;
                    if (counter == dwLen)
                    {
                        if (lastValueIsBlack)
                            byteToSend |= 1 << 7;
                        pOptBuf.Add(byteToSend);
                    }
                    continue;
                }

                for (int i = 7; i >= 0; i--)
                {

                    bool currentValueIsBlack = is_bit_set(pBuf[counter], i);
                    if (currentValueIsBlack != lastValueIsBlack || byteToSend == 127)
                    {
                        if (byteToSend != 0xFF)
                        {
                            if (lastValueIsBlack)
                            {
                                byteToSend |= 1 << 7;
                            }
                            pOptBuf.Add(byteToSend);
                        }
                        byteToSend = 0;
                    }
                    else
                    {
                        byteToSend++;
                    }
                    lastValueIsBlack = currentValueIsBlack;
                }

                counter++;
                if (counter == dwLen)
                {
                    if (lastValueIsBlack)
                    {
                        byteToSend |= 1 << 7;
                    }
                    pOptBuf.Add(byteToSend);
                }
            }

            if (pOptBuf.Count < dwLen - leftMargin)
            {
                //byte [] ETB = { 0x17 };
                //res = writeCommand(ETB, 1);
                //if (res == E_FAIL)
                //    return E_FAIL;
                ////byte * bt1 = pOptBuf.begin();
                ////int sz = pOptBuf.size();
                //res = writeCommand((byte*)pOptBuf.begin(), pOptBuf.size());
                ////clean-up
                //std::vector<byte>().swap(pOptBuf);
            }
            else
            {

                //byte SYN[1] = { 0x16 };
                //res = writeCommand(SYN, 1);

                //if (res == E_FAIL)
                //    return E_FAIL;

                //size_t sOutBufLen = dwLen - leftMargin;

                //byte* pOutBuf = new byte[sOutBufLen];

                //memset(pOutBuf, 0, sOutBufLen);
                //memcpy(pOutBuf, pBuf + leftMargin, dwLen - leftMargin);

                //res = writeCommand(pOutBuf, (int)sOutBufLen);
                //delete[] pOutBuf;
            }

            //return (int)res;
        }

        public static string DecodeMessage(Byte[] bytes)
        {
            String incomingData = String.Empty;
            Byte secondByte = bytes[1];
            Int32 dataLength = secondByte & 127;
            Int32 indexFirstMask = 2;
            if (dataLength == 126)
                indexFirstMask = 4;
            else if (dataLength == 127)
                indexFirstMask = 10;

            IEnumerable<Byte> keys = bytes.Skip(indexFirstMask).Take(4);
            Int32 indexFirstDataByte = indexFirstMask + 4;

            Byte[] decoded = new Byte[bytes.Length - indexFirstDataByte];
            for (Int32 i = indexFirstDataByte, j = 0; i < bytes.Length; i++, j++)
            {
                decoded[j] = (Byte)(bytes[i] ^ keys.ElementAt(j % 4));
            }
            return incomingData = Encoding.UTF8.GetString(decoded, 0, dataLength);
            //return incomingData = Encoding.UTF8.GetString(decoded, 0, decoded.Length);
        }

        public  static Byte[] EncodeMessageToSend(String message)
        {
            Byte[] response;
            Byte[] bytesRaw = Encoding.UTF8.GetBytes(message);
            Byte[] frame = new Byte[10];

            Int32 indexStartRawData = -1;
            Int32 length = bytesRaw.Length;

            frame[0] = (Byte)129;
            if (length <= 125)
            {
                frame[1] = (Byte)length;
                indexStartRawData = 2;
            }
            else if (length >= 126 && length <= 65535)
            {
                frame[1] = (Byte)126;
                frame[2] = (Byte)((length >> 8) & 255);
                frame[3] = (Byte)(length & 255);
                indexStartRawData = 4;
            }
            else
            {
                frame[1] = (Byte)127;
                frame[2] = (Byte)((length >> 56) & 255);
                frame[3] = (Byte)((length >> 48) & 255);
                frame[4] = (Byte)((length >> 40) & 255);
                frame[5] = (Byte)((length >> 32) & 255);
                frame[6] = (Byte)((length >> 24) & 255);
                frame[7] = (Byte)((length >> 16) & 255);
                frame[8] = (Byte)((length >> 8) & 255);
                frame[9] = (Byte)(length & 255);

                indexStartRawData = 10;
            }

            response = new Byte[indexStartRawData + length];

            Int32 i, reponseIdx = 0;

            //Add the frame bytes to the reponse
            for (i = 0; i < indexStartRawData; i++)
            {
                response[reponseIdx] = frame[i];
                reponseIdx++;
            }

            //Add the data bytes to the response
            for (i = 0; i < length; i++)
            {
                response[reponseIdx] = bytesRaw[i];
                reponseIdx++;
            }

            return response;
        }
    }
}
