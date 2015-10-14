using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace Compression.Models
{
    [Serializable]
    public class Archive
    {
        public Archive(Dictionary<byte, string> codes, byte[] data)
        {
            Codes = (from c in codes select new ByteCode(c.Key, c.Value)).ToArray();
            Data = data;
        }

        public Archive(BinaryReader reader)
        {
            int codesLength = (int)reader.ReadByte();
            List<ByteCode> codes = new List<ByteCode>(codesLength);
            for (int i = 0; i < codesLength; i++)
                codes.Add(new ByteCode(reader));
            Codes = codes.ToArray();

            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                Data = ms.ToArray();
            }

        }
        public ByteCode[] Codes { get; }
        public byte[] Data { get; }

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write((byte)Codes.Length);
                    foreach (var c in Codes)
                        writer.Write(c.GetBytes());
                    writer.Write(Data);
                }
                return stream.ToArray();
            }
        }
    }

    [Serializable]
    public class ByteCode
    {
        public ByteCode(Byte b, string code)
        {
            Byte = b;
            CodeLength = (byte)code.Length;
            Code = Convert.ToUInt32(code, 2);
        }

        public ByteCode(BinaryReader reader)
        {
            Byte = reader.ReadByte();
            CodeLength = reader.ReadByte();
            Code = reader.ReadUInt32();
        }

        public byte Byte { get; }
        public byte CodeLength { get; }
        public UInt32 Code { get; }

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(Byte);
                    writer.Write(CodeLength);
                    writer.Write(Code);
                }
                return stream.ToArray();
            }
        }
    }
}
