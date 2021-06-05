/*
    QuesoStruct makes .NET based binary serialization code neat and easy
    Copyright (C) 2021 Chosen Few Software

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuesoStruct
{
    public enum Endianess
    {
        Little,
        Big,
    }

    public class Context
    {
        private class TupleComparer : Comparer<(long offset, Type type)>
        {
            public override int Compare((long offset, Type type) x, (long offset, Type type) y)
            {
                var result = x.offset.CompareTo(y.offset);
                return result == 0 ? x.type.FullName.CompareTo(y.type.FullName) : result;
            }
        }

        public Stream Stream { get; }
        public Endianess Endianess { get; }
        public Encoding Encoding { get; }

        public IStructInstance Current { get; set; }
        public SortedDictionary<(long offset, Type type), IStructInstance> Instances { get; }
        public HashSet<IStructReference> Unresolved { get; }

        public static readonly Endianess SystemEndianess = BitConverter.IsLittleEndian ? Endianess.Little : Endianess.Big;

        public Context(Stream stream, Endianess endianess, Encoding encoding)
        {
            Stream = stream;
            Endianess = endianess;
            Encoding = encoding;

            Instances = new SortedDictionary<(long, Type), IStructInstance>(new TupleComparer());
            Unresolved = new HashSet<IStructReference>();
        }

        public void OffsetAllAfter(IStructInstance inst, long amount)
        {
            foreach (var i in Instances.Values
                .Where(i => i.Offset.HasValue)
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

        public void RewriteUnresolvedReferences()
        {
            foreach (var refr in Unresolved)
            {
                Serializers.Get(refr.GetType()).Write(refr, this);
            }

            Unresolved.Clear();
        }

        public bool IsReferenceValid(IStructReference refr)
        {
            return Instances.ContainsKey((refr.OffsetValue, refr.InstanceType));
        }

        public IStructInstance GetReferencedInstance(IStructReference refr)
        {
            return Instances[(refr.OffsetValue, refr.InstanceType)];
        }

        public bool TryAddInstance(IStructInstance inst)
        {
            return Instances.TryAdd((inst.Offset.Value, inst.GetType()), inst);
        }
    }
}
