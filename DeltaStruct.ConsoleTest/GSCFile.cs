using DeltaStruct.Types.Collections;
using DeltaStruct.Types.Primitives;
using System;
using System.Linq;
using System.Text;

namespace DeltaStruct.ConsoleTest
{
    [StructType]
    public partial class GSCSegment : IBytesOwner
    {
        public long BytesLength => Size - 8;
        public string TagName => Encoding.ASCII.GetString(BitConverter.GetBytes(Tag));

        [StructMember]
        public uint Tag { get; set; }

        [StructMember]
        public uint Size { get; set; }

        [StructMember]
        public Bytes Data { get; set; }
    }

    [StructType]
    public partial class GSCFile : ICollectionOwner<GSCSegment>
    {
        public long? ItemCount => null;
        public bool TerminateOnStreamEnd => true;

        public bool IsTerminator(IStructInstance inst) => false;

        [StructMember]
        public uint Magic
        {
            get => magic;
            set => magic = BitConverter.GetBytes(value)
                .SequenceEqual(Encoding.ASCII.GetBytes("NU20"))
                ? value : throw new ArgumentException("Invalid file! Wrong magic!");
        }
        private uint magic;

        [StructMember]
        public int NegativeSize { get; set; }
        public uint Size => (uint)-NegativeSize;

        [StructMember]
        public ulong Unk000 { get; set; }

        [StructMember]
        public Collection<GSCSegment> Segments { get; set; }
    }
}
