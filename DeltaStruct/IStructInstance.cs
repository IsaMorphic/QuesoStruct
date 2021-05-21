namespace DeltaStruct
{
    public interface IStructInstance
    {
        long? Offset { get; set; }
        IStructInstance Parent { get; set; }
    }

    // TODO: Offset(long amount)
}
