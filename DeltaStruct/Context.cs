using System;
using System.Collections.Generic;
using System.IO;

namespace DeltaStruct
{
    public enum Endianess
    {
        Little,
        Big,
    }

    public class Context
    {
        public Stream Stream { get; }
        public Endianess Endianess { get; }

        public IStructInstance Current { get; set; }
        public List<IStructInstance> Instances { get; }

        public static readonly Endianess SystemEndianess = BitConverter.IsLittleEndian ? Endianess.Little : Endianess.Big;

        public Context(Stream stream, Endianess endianess)
        {
            Stream = stream;
            Endianess = endianess;

            Instances = new List<IStructInstance>();
        }
        
        // TODO: OffsetAllAfter(IStructInstance inst, long amount)
    }
}
