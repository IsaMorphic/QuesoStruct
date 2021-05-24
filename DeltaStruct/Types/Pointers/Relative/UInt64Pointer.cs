namespace DeltaStruct.Types.Pointers
{
    public partial class Relative
    {
        [StructType]
        public partial class UInt64Pointer<TInst> : IStructReference<TInst>
        where TInst : IStructInstance
        {
            public long? OffsetValue => (long)PointerValue + RelativeTo.Offset;

            public void Update()
            {
                PointerValue = (ulong)(Instance.Offset - RelativeTo.Offset);
            }

            [StructMember]
            public ulong PointerValue { get; set; }

            public IStructInstance RelativeTo { get; set; }

            public void OnAfterRead() { }
            public void OnBeforeWrite(Context context) { }
        }
    }
}
