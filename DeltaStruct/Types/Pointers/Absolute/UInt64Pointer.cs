namespace DeltaStruct.Types.Pointers
{
    public partial class Absolute
    {
        [StructType]
        public partial class UInt64Pointer<TInst> : IStructReference<TInst>
        where TInst : IStructInstance
        {
            public long? OffsetValue => (long)PointerValue;

            public void Update()
            {
                PointerValue = (ulong)Instance.Offset;
            }

            [StructMember]
            public ulong PointerValue { get; set; }

            public void OnAfterRead() { }
            public void OnBeforeWrite(Context context) { }
        }
    }
}
