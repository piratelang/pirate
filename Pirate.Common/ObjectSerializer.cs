using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
    private sealed class AllowedAstSerializationBinder : ISerializationBinder
    {
        private static readonly string[] AllowedTypePrefixes =
        {
            "Pirate.Parser.Node.",
            "Pirate.Parser.Node.Interfaces."
        };

        private static readonly HashSet<string> AllowedTypes = new()
        {
            "Pirate.Parser.Scope",
            "Pirate.Lexer.Tokens.Token"
        };

        public Type BindToType(string? assemblyName, string typeName)
        {
            if (!IsAllowedType(typeName))
            {
                throw new JsonSerializationException($"Type '{typeName}' is not allowed for deserialization.");
            }

            var resolvedType = assemblyName is null
                ? Type.GetType(typeName, throwOnError: false)
                : Type.GetType($"{typeName}, {assemblyName}", throwOnError: false);
            if (resolvedType is null)
            {
                throw new JsonSerializationException($"Could not resolve type '{typeName}'.");
            }

            return resolvedType;
        }

        public void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
        {
            assemblyName = serializedType.Assembly.FullName;
            typeName = serializedType.FullName;
        }

        private static bool IsAllowedType(string typeName)
        {
            if (AllowedTypes.Contains(typeName))
            {
                return true;
            }

            return AllowedTypePrefixes.Any(prefix => typeName.StartsWith(prefix, StringComparison.Ordinal));
        }
    }

    private static readonly ISerializationBinder AstSerializationBinder = new AllowedAstSerializationBinder();

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
                SerializationBinder = AstSerializationBinder,
                // TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, 
                Formatting = Formatting.Indented

            };
            if (!_fileReadHandler.FileExists(FileName, FileExtension.JSON, Location)) _fileWriteHandler.WriteToFile(new FileWriteModel(FileName, FileExtension.JSON, Location, "{}"));
            string json = _fileReadHandler.ReadAllTextFromFile(FileName, FileExtension.JSON, Location).Result;
            T deserializedObject = JsonConvert.DeserializeObject<T>(json, settings);

            if (deserializedObject == null) throw new SerializationException("Deserialized object is null");
            Logger.Log($"Deserialized and converted {FileName} to {FileName}.json", LogType.INFO);

            return deserializedObject;
        }
        catch (SerializationException ex)
        {
            Logger.Log($"Failed to Deserialize {FileName}.json. \"{ex.ToString() + "\n" + ex.Source}\"", LogType.ERROR);
            throw new SerializationException(ex.ToString() + "\n" + ex.Source);
        }
    }
}