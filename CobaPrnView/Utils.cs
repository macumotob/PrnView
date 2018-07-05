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
    }
}
