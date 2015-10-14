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
                using (BitStream stream = new BitStream(mStream))
                {
                    foreach (var b in _rawData)
                    {
                        var code = codes[b];
                        stream.WriteBits(BitConverter.GetBytes(Convert.ToUInt32(code, 2)), 0, (ulong)code.Length);
                    }
                    _archive = new Archive(codes, mStream.ToArray());
                }
            }
        }

        public void Decompress()
        {
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

            //using (MemoryStream stream = new MemoryStream())
            //{
            //    using (BitStream bStream = new BitStream(stream))
            //    {
            //        List<bool> buffer = new List<bool>();
            //        for (int i = 1; i < bStream.Length; i++)
            //        {
            //            byte b;
            //            bStream.ReadBits(out b, i);
            //        }
            //    }
            //}
        }

        public void WriteToFile(string fileName)
        {
            using (FileStream file = File.Create(fileName))
            {
                //IFormatter formatter = new BinaryFormatter();
                //formatter.Serialize(file, _archive);
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
