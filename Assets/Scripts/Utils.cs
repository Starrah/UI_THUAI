using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Utils
{
    public static T Clone<T>(T obj)
        where T: class
    {
        MemoryStream stream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, obj);
        stream.Position = 0;
        var yyy = formatter.Deserialize(stream) as T;
        return yyy;
    }
}