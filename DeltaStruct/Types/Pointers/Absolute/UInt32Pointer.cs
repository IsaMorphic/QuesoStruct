namespace DeltaStruct.Types.Pointers
{
    public partial class Absolute
    {
        [StructType]
        public partial class UInt32Pointer<TInst> : IStructReference<TInst>
        where TInst : IStructInstance
        {
            public long? OffsetValue => PointerValue;

            public void Update()
            {
                PointerValue = (uint)Instance.Offset;
            }

            [StructMember]
            public uint PointerValue { get; set; }

            public void OnAfterRead() { }
            public void OnBeforeWrite(Context context) { }
        }
    }
}
