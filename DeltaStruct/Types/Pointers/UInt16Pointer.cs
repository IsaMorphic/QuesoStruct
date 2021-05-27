namespace DeltaStruct.Types.Pointers
{
    [StructType]
    public partial class UInt16Pointer<TInst> : IStructReference<TInst>
        where TInst : class, IStructInstance
    {
        public IPointerOwner Owner => Parent as IPointerOwner;

        public long OffsetValue => PointerValue + Owner.RelativeOffsetBase.Offset.Value + Owner.AddedOffsetFromBase;

        public bool IsResolved { get; private set; }

        public void Update()
        {
            var ptr = Instance?.Offset - Owner.RelativeOffsetBase?.Offset - Owner.AddedOffsetFromBase;
            if (ptr.HasValue)
            {
                PointerValue = (ushort)ptr;
                IsResolved = true;
            }
            else
            {
                PointerValue = (ushort)Owner.NullOffsetValue;
                IsResolved = Instance == null;
            }
        }

        [StructMember]
        public ushort PointerValue { get; set; }
    }
}
