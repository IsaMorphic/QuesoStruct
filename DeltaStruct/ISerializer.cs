using System.IO;

namespace DeltaStruct
{
    public interface ISerializer<T>
    {
        T ReadFromStream(Stream stream);
        void WriteToStream(T inst, Stream stream);
    }
}
