namespace DeltaStruct.Types.Pointers
{
    public interface IPointerOwner : IStructInstance
    {
        IStructInstance RelativeOffsetBase { get; }
        long AddedOffsetFromBase { get; }

        long NullOffsetValue { get; }
    }
}
