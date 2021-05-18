namespace DeltaStruct
{
    public enum Endianess
    {
        Little,
        Big,
    }

    public class Context
    {
        public object Parent { get; }

        public long FileOffset { get; }

        public Endianess Endianess { get; }

        public Context(object parent, long fileOffset, Endianess endianess)
        {
            Parent = parent;
            FileOffset = fileOffset;
            Endianess = endianess;
        }
    }
}
