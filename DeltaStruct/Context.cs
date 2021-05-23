using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public void OffsetAllAfter(IStructInstance inst, long amount)
        {
            foreach (var i in Instances
                .Where(i => i.Offset.HasValue)
                .OrderBy(i => i.Offset)
                .SkipWhile(i => !ReferenceEquals(i, inst))
                .Skip(1))
            {
                i.Offset += amount;
                foreach (var r in i.References)
                {
                    r.Update();
                }
            }
        }
    }
}
