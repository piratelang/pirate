using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using Common.Enum;
using Common.FileHandlers;
using Common.FileHandlers.Interfaces;
using Common.Interfaces;
using Newtonsoft.Json;

namespace Common;

public class ObjectSerializer : IObjectSerializer
{
    public string Location { get; set; }
    public ILogger Logger { get; set; }

    private IFileWriteHandler _fileWriteHandler;
    private IFileReadHandler _fileReadHandler;

    public ObjectSerializer(ILogger logger, IEnvironmentVariables environmentVariables, IFileWriteHandler fileWriteHandler, IFileReadHandler fileReadHandler)
    {
        _fileReadHandler = fileReadHandler;
        _fileWriteHandler = fileWriteHandler;

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
            Logger.Log($"Serialized and written \"{FileName}\" to \"{FileName}\".bin", LogType.INFO);
            SerializeObjectToJSON(ObjectToSerialize, FileName);
        }
        catch (Exception ex)
        {
            Logger.Log($"Failed to Serialize {FileName}.bin. \"{ex.ToString()}\"", LogType.ERROR);
            throw;
        }
    }

    public void SerializeObjectToJSON(object ObjectToSerialize, string FileName)
    {
        try
        {
            var settings = new JsonSerializerSettings
            { 
                TypeNameHandling = TypeNameHandling.Objects,
                // TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, 
                Formatting = Formatting.Indented 
            };
            string json = JsonConvert.SerializeObject(ObjectToSerialize, settings);
            _fileWriteHandler.WriteToFile(new FileWriteModel(FileName, FileExtension.JSON, Location, json));
            
            Logger.Log($"Serialized and written \"{FileName}\" to \"{FileName}\".json", LogType.INFO);
        }
        catch (Exception ex)
        {
            Logger.Log($"Failed to Serialize {FileName}.json. \"{ex.ToString()}\"", LogType.ERROR);
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
                
                Logger.Log($"Deserialized and converted {FileName} to {FileName}.bin", LogType.INFO);
                var test =  DeserializeFromJSON<T>(FileName);
                
                return @object;
            }
        }
        catch (SerializationException ex)
        {
            Logger.Log($"Failed to Deserialize {FileName}.bin. \"{((object)ex).ToString() + "\n" + ex.Source}\"", LogType.ERROR);
            throw new SerializationException(((object)ex).ToString() + "\n" + ex.Source);
        }

    }

    public T DeserializeFromJSON<T>(string FileName) where T : class
    {
        try
        {
            var settings = new JsonSerializerSettings
            { 
                TypeNameHandling = TypeNameHandling.Objects,
                // TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, 
                Formatting = Formatting.Indented 
            };
            string json = _fileReadHandler.ReadAllTextFromFile(FileName, FileExtension.JSON, Location).Result;
            T deserializedObject = JsonConvert.DeserializeObject<T>(json, settings);

            if (deserializedObject == null) throw new SerializationException("Deserialized object is null");
            Logger.Log($"Deserialized and converted {FileName} to {FileName}.json", LogType.INFO);
            
            return deserializedObject;
        }
        catch (SerializationException ex)
        {
            Logger.Log($"Failed to Deserialize {FileName}.json. \"{((object)ex).ToString() + "\n" + ex.Source}\"", LogType.ERROR);
            throw new SerializationException(((object)ex).ToString() + "\n" + ex.Source);
        }
    }
}