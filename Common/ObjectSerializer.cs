using System.Runtime.Serialization.Formatters.Binary;
namespace Common;

public class ObjectSerializer
{
    public void SerializeObject(object ObjectToSerialize, string location, string FileName)
    {
        Stream stream = File.Open($"{location}/{FileName}.dat", FileMode.OpenOrCreate);
        var binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(stream, ObjectToSerialize);
        stream.Close();
    }
}