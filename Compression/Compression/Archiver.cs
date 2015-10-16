using Compression.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Compression
{
    public class Archiver
    {
        private Archive _archive;

        private ByteStats _stats = new ByteStats();
        public ByteStats Stats
        {
            get { return _stats; }
        }

        private byte[] _rawData;

        public void LoadText(string text)
        {
            _rawData = Encoding.UTF8.GetBytes(text);
            Stats.Clear();
            foreach (var b in _rawData)
                Stats.Add(b);
            Stats.UpdateFrequency();
            Stats.Span();
        }

        public void Compress()
        {
            Dictionary<byte, string> codes = new Dictionary<byte, string>();

            foreach (var s in Stats.Bytes)
                codes.Add(s.Byte, s.Code);

            List<byte> Compressed = new List<byte>();
            using (MemoryStream mStream = new MemoryStream())
            {
                StringBuilder buffer = new StringBuilder();
                foreach (var b in _rawData)
                {
                    var code = codes[b];
                    buffer.Append(code);
                }
                string sBuffer = buffer.ToString();
                int bytesLength = (sBuffer.Length % 8 == 0) ? sBuffer.Length / 8 : sBuffer.Length / 8 + 1;

                for (int i = 0; i < bytesLength * 8; i += 8)
                {
                    string cache = (sBuffer.Length >= i + 8) ? sBuffer.Substring(i, 8) : sBuffer.Substring(i);
                    if (cache.Length < 8)
                        cache = cache.PadRight(8, '0');
                    mStream.WriteByte(Convert.ToByte(cache, 2));
                }
                _archive = new Archive(codes, (uint)_rawData.Count(), mStream.ToArray());
            }
        }

        public string Decompress()
        {
            List<byte> raw = new List<byte>();
            TreeNode root = new TreeNode();
            foreach (var code in _archive.Codes)
            {
                string sCode = Convert.ToString(code.Code, 2);
                while (sCode.Length < code.CodeLength)
                    sCode = "0" + sCode;
                TreeNode n = root;
                for (int i = 0; i < sCode.Length; i++)
                {
                    if (sCode[i] == '0')
                    {
                        if (n.Get0 == null)
                            n.Get0 = new TreeNode();
                        n = n.Get0;
                    }
                    else
                    {
                        if (n.Get1 == null)
                            n.Get1 = new TreeNode();
                        n = n.Get1;
                    }
                }
                n.Byte = code.Byte;
            }

            using (MemoryStream stream = new MemoryStream(_archive.Data))
            {

                TreeNode n = root;
                string buffer = string.Empty;
                for (int i = 0; i < stream.Length; i++)
                {
                    buffer = Convert.ToString(stream.ReadByte(), 2);
                    if (buffer.Length < 8)
                    {
                        buffer = buffer.PadLeft(8, '0');
                    }
                    foreach (var b in buffer)
                    {
                        if (raw.Count() < _archive.BytesCount)
                        {

                            if (b == '0')
                            {
                                if (n.Get0 != null)
                                    n = n.Get0;
                                else
                                    throw new Exception("Символ не найден!");
                            }
                            else
                            {
                                if (n.Get1 != null)
                                    n = n.Get1;
                                else
                                    throw new Exception("Символ не найден!");
                            }

                            //if (n.Byte != 0)
                            if(n.Get0==null && n.Get1==null)
                            {
                                raw.Add(n.Byte);
                                n = root;
                            }
                        }
                        else
                            break;
                    }
                }
            }

            return Encoding.UTF8.GetString(raw.ToArray());
        }

        public void WriteToFile(string fileName)
        {
            using (FileStream file = File.Create(fileName))
            {
                byte[] bytes = _archive.GetBytes();
                file.Write(bytes, 0, bytes.Count());
            }
        }

        public void ReadFromFile(string fileName)
        {
            byte[] compressed = File.ReadAllBytes(fileName);
            using (MemoryStream stream = new MemoryStream(compressed))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    _archive = new Archive(reader);
                }
            }
        }

    }
}
