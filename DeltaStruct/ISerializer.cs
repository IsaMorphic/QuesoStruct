namespace DeltaStruct
{
    public interface ISerializer<TInst> 
        where TInst : IStructInstance
    {
        TInst Read(Context context);
        void Write(TInst inst, Context context);
    }
}
