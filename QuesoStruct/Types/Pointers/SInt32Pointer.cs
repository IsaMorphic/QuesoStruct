﻿namespace QuesoStruct.Types.Pointers
{
    [StructType]
    public partial class SInt32Pointer<TInst> : IStructReference<TInst>
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
                PointerValue = (int)ptr;
                IsResolved = true;
            }
            else
            {
                Owner.SetNullPointer(this);
                IsResolved = Instance == null;
            }
        }

        [StructMember]
        public int PointerValue { get; set; }
    }
}