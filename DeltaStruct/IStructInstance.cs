using System.Collections.Generic;

namespace DeltaStruct
{
    public interface IStructInstance
    {
        long? Offset { get; set; }
        IStructInstance Parent { get; set; }

        HashSet<IStructReference> References { get; }
    }

    public static class StructInstanceExtensions
    {
        public static void SetOffsetWithRefUpdate(this IStructInstance inst, long offset)
        {
            inst.Offset = offset;
            foreach (var r in inst.References)
            {
                r.Update();
            }
        }
    }
}
