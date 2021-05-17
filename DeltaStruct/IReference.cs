namespace DeltaStruct
{
    public interface IReference<TInst>
    {
        TInst Instance { get; set; }
    }
}
