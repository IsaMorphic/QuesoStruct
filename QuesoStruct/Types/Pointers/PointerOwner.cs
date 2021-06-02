namespace QuesoStruct.Types.Pointers
{
    public interface IPointerOwner : IStructInstance
    {
        IStructInstance RelativeOffsetBase { get; }
        long AddedOffsetFromBase { get; }

        bool IsNullPointer(IStructReference refr);
        void SetNullPointer(IStructReference refr);
    }
}
