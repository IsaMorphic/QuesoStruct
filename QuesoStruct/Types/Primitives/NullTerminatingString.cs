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
                    stream.SetLength(stream.Position);
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

        public NullTerminatingString() { References = new HashSet<IStructReference>(); }

        public long? Offset { get; set; }
        public IStructInstance Parent { get; set; }

        public IStructInstance RelativeOffsetBase { get; }
        public HashSet<IStructReference> References { get; }

        public string Value { get; set; }
    }
}
