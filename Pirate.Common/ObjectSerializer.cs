using System.Runtime.Serialization;
using Pirate.Common.Exception.Exceptions;
using Newtonsoft.Json;
using Pirate.Common.Interfaces;
using Pirate.Common.FileHandler.Model;
using Pirate.Common.FileHandler.Enum;
using Pirate.Common.FileHandler.Interfaces;
using Pirate.Common.Logger.Enum;
using Pirate.Common.Logger.Interfaces;

namespace Pirate.Common;

/// <summary>
/// This class is used to serialize and deserialize objects to and from JSON.
/// </summary>
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
        bool exists = Directory.Exists(Location);
        if (!exists)
            Directory.CreateDirectory(Location);
        Logger = logger;
    }

    public void SerializeObject(object ObjectToSerialize, string FileName)
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
        catch (System.Exception ex)
        {
            Logger.Log($"Failed to Serialize {FileName}.json. \"{ex.ToString()}\"", LogType.ERROR);
            throw;
        }
    }

    public T Deserialize<T>(string FileName) where T : class
    {
        try
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                // TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, 
                Formatting = Formatting.Indented

            };
            if (!_fileReadHandler.FileExists(FileName, FileExtension.JSON, Location))
                throw new ModuleNotBuiltException(FileName);
            
            string json = _fileReadHandler.ReadAllTextFromFile(FileName, FileExtension.JSON, Location).GetAwaiter().GetResult();
            T deserializedObject = JsonConvert.DeserializeObject<T>(json, settings);

            if (deserializedObject == null) throw new SerializationException("Deserialized object is null");
            if (typeof(T).GetProperty("Nodes")?.GetValue(deserializedObject) == null) throw new SerializationException("Deserialized object contains no Nodes");
            Logger.Log($"Deserialized and converted {FileName} to {FileName}.json", LogType.INFO);

            return deserializedObject;
        }
        catch (ModuleNotBuiltException ex)
        {
            Logger.Error(ex);
            throw;
        }
        catch (JsonException ex)
        {
            var serializationException = new SerializationException($"Failed to Deserialize {FileName}.json", ex);
            Logger.Error(serializationException);
            throw serializationException;
        }
        catch (IOException ex)
        {
            var serializationException = new SerializationException($"Failed to read {FileName}.json", ex);
            Logger.Error(serializationException);
            throw serializationException;
        }
        catch (SerializationException ex)
        {
            Logger.Error(ex);
            throw;
        }
    }
}