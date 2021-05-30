using System;
using System.Collections.Generic;
using System.IO;

namespace DeltaStruct.Types.Primitives
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

        public Bytes() { References = new HashSet<IStructReference>(); }

        public long? Offset { get; set; }
        public IStructInstance Parent { get; set; }

        public HashSet<IStructReference> References { get; }

        public SubStream Stream { get; set; }
    }
}
