namespace DeltaStruct
{
    public interface IStructReference<TInst> : IStructInstance 
        where TInst : IStructInstance
    {
        TInst Instance { get; set; }
    }

    // TODO: Offset(long amount)
}
