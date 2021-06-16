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

namespace QuesoStruct.Types.Primitives
{
    public class NullTerminatingString : IStructInstance
    {
        public class Serializer : ISerializer<NullTerminatingString>
        {
            public NullTerminatingString Read(Context context)
            {
                var inst = new NullTerminatingString(context);
                context.TryAddInstance(inst);

                var stream = context.Stream;
                var encoding = context.Encoding;

                byte[] buffer = new byte[16];
                List<char> str = new List<char>();

                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                char[] chars = encoding.GetChars(buffer);

                while (bytesRead > 0 && !chars.Contains('\u0000'))
                {
                    str.AddRange(chars);

                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    chars = encoding.GetChars(buffer);
                }

                char[] leftover = chars
                    .TakeWhile(c => c != '\u0000')
                    .ToArray();

                if (bytesRead > leftover.Length + 1)
                {
                    stream.Seek((leftover.Length + 1) - buffer.Length, SeekOrigin.Current);
                }

                str.AddRange(leftover);

                inst.Value = new string(str.ToArray());
                return inst;
            }

            public void Write(NullTerminatingString inst, Context context)
            {
                var stream = context.Stream;
                var encoding = context.Encoding;

                List<byte> buffer = new List<byte>();

                buffer.AddRange(encoding.GetBytes(inst.Value));
                buffer.AddRange(encoding.GetBytes(new[] { '\u0000' }));

                stream.Write(buffer.ToArray(), 0, buffer.Count);
            }
        }

        public static void Init()
        {
            Serializers.Register<NullTerminatingString, Serializer>();
        }

        public NullTerminatingString(Context context) : this()
        {
            Offset = context.Stream.Position;
            Parent = context.Current;
        }

        public NullTerminatingString(IStructInstance parent = null) { Parent = parent; References = new HashSet<IStructReference>(); }

        public long? Offset { get; set; }
        public IStructInstance Parent { get; set; }

        public IStructInstance RelativeOffsetBase { get; }
        public HashSet<IStructReference> References { get; }

        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
