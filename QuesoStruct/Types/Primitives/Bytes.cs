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

namespace QuesoStruct.Types.Primitives
{
    public interface IBytesOwner : IStructInstance
    {
        long BytesLength { get; }
    }

    public class Bytes : IStructInstance
    {
        public class Serializer : ISerializer<Bytes>, ISerializer
        {
            public Type InstanceType => typeof(Bytes);

            public Bytes Read(Context context)
            {
                var inst = new Bytes(context);
                context.TryAddInstance(inst);

                var owner = context.Current as IBytesOwner;

                var stream = context.Stream;

                var subStream = new SubStream(stream, stream.Position);
                subStream.SetLength(owner.BytesLength);
                subStream.Lock();

                stream.Seek(owner.BytesLength, SeekOrigin.Current);

                inst.Stream = subStream;
                return inst;
            }

            public void Write(Bytes inst, Context context)
            {
                var stream = context.Stream;
                stream.Seek(0, SeekOrigin.Begin);
                inst.Stream.CopyTo(stream);
            }

            IStructInstance ISerializer.Read(Context context) => Read(context);
            void ISerializer.Write(IStructInstance inst, Context context) => Write(inst as Bytes, context);
        }

        public static void Init()
        {
            Serializers.Register<Bytes, Serializer>();
        }

        public Bytes(Context context) : this()
        {
            Offset = context.Stream.Position;
            Parent = context.Current;
        }

        public Bytes(IStructInstance parent = null) { Parent = parent; References = new HashSet<IStructReference>(); }

        public long? Offset { get; set; }
        public IStructInstance Parent { get; set; }

        public HashSet<IStructReference> References { get; }

        public Stream Stream { get; set; }
    }
}
