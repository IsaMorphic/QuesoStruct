using System;

namespace QuesoStruct
{
    public interface ISerializer
    {
        Type InstanceType { get; }

        IStructInstance Read(Context context);
        void Write(IStructInstance inst, Context context);
    }

    public interface ISerializer<TInst>
        where TInst : IStructInstance
    {
        TInst Read(Context context);
        void Write(TInst inst, Context context);
    }
}
