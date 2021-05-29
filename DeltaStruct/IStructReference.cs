using System;

namespace DeltaStruct
{
    public interface IStructReference : IStructInstance
    {
        Type InstanceType { get; }

        long OffsetValue { get; }
        bool IsResolved { get; }

        void Update();
    }

    public interface IStructReference<TInst> : IStructReference
        where TInst : IStructInstance
    {
        TInst Instance { get; set; }
    }
}
