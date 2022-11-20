using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Common.Enum;
using Common.Interfaces;

namespace Common;

public class ObjectSerializer : IObjectSerializer
{
    public string Location { get; set; }
    public ILogger Logger { get; set; }
    public ObjectSerializer(ILogger logger, IEnvironmentVariables environmentVariables)
    {
        Location = environmentVariables.GetVariable("location") + "/cache";
        bool exists = System.IO.Directory.Exists(Location);
        if (!exists)
            System.IO.Directory.CreateDirectory(Location);
        Logger = logger;
    }

    [Obsolete]
    public void SerializeObject(object ObjectToSerialize, string FileName)
    {
        try
        {
            BinaryFormatter binaryFormatter = new();

            using (FileStream fileStream = new FileStream($"{Location}/{FileName}.bin", FileMode.OpenOrCreate))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        binaryFormatter.Serialize(memoryStream, ObjectToSerialize);
                        binaryWriter.Write(memoryStream.ToArray());
                    }
                }
            }
            Logger.Log($"Serialized and written \"{FileName}\" to \"{FileName}\".bin", this.GetType().Name, LogType.INFO);
        }
        catch (Exception ex)
        {
            Logger.Log($"Failed to Serialize {FileName}.bin. \"{ex.ToString()}\"", this.GetType().Name, LogType.ERROR);
            throw;
        }
    }

    [Obsolete]
    public T Deserialize<T>(string FileName) where T : class
    {
        try
        {
            using (StreamReader streamReader = new StreamReader($"{Location}/{FileName}.bin"))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                var @object = (T)binaryFormatter.Deserialize(streamReader.BaseStream);
                Logger.Log($"Deserialized and converted {FileName} to {FileName}.bin", this.GetType().Name, LogType.INFO);
                return @object;
            }
        }
        catch (SerializationException ex)
        {
            Logger.Log($"Failed to Deserialize {FileName}.bin. \"{((object)ex).ToString() + "\n" + ex.Source}\"", this.GetType().Name, LogType.ERROR);
            throw new SerializationException(((object)ex).ToString() + "\n" + ex.Source);
        }

    }
}