namespace DeltaStruct.Types.Pointers
{
    [StructType]
    public partial class UInt32Pointer<TInst> : IStructReference<TInst>
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
                PointerValue = (uint)ptr;
                IsResolved = true;
            }
            else
            {
                PointerValue = (uint)Owner.NullOffsetValue;
                IsResolved = Instance == null;
            }
        }

        [StructMember]
        public uint PointerValue { get; set; }
    }
}
