namespace DeltaStruct.Types.Pointers
{
    public partial class Absolute
    {
        [StructType]
        public partial class UInt16Pointer<TInst> : IStructReference<TInst>
            where TInst : IStructInstance
        {
            public long? OffsetValue => PointerValue;

            public void Update()
            {
                PointerValue = (ushort)Instance.Offset;
            }

            [StructMember]
            public ushort PointerValue { get; set; }

            public void OnAfterRead() { }
            public void OnBeforeWrite(Context context) { }
        }
    }
}
