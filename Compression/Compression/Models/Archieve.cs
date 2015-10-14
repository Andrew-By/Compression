using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Compression.Models
{
    [Serializable]
    public class Archieve
    {
        public Archieve(List<ByteCode> codes, byte[] data)
        {
            Codes = codes.ToArray();
        }
        public ByteCode[] Codes { get; }
        public byte[] Data { get; }
    }

    [Serializable]
    public class ByteCode
    {
        public ByteCode(Byte b, string code)
        {
            Byte = b;
            CodeLength = code.Length;
            Code = Convert.ToUInt32(code, 2);
        }

        public byte Byte { get; }
        public int CodeLength { get; }
        public uint Code { get; }

        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("props", Byte, typeof(byte));
        //    info.AddValue("props", CodeLength, typeof(int));
        //    info.AddValue("props", Code, typeof(uint));
        //}
    }
}
