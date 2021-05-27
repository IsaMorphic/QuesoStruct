using System.Collections.Generic;

namespace DeltaStruct
{
    public interface IStructInstance
    {
        long? Offset { get; set; }
        IStructInstance Parent { get; set; }

        HashSet<IStructReference> References { get; }
    }
}
