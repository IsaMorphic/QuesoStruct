namespace DeltaStruct
{
    public interface IStructReference : IStructInstance
    {
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
