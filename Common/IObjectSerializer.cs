namespace Common;

public interface IObjectSerializer
{
    string Location { get; set; }
    ILogger Logger { get; set; }

    T Deserialize<T>(string FileName) where T : class;
    void SerializeObject(object ObjectToSerialize, string FileName);
}
