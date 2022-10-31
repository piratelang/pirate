using Common.Interfaces;

namespace Common;

[Serializable]
public class FileReadHandler
{

    // Name: Without extension
    // Extension: With! dot
    // Location: Without root folder, i.e. "./"
    public bool WriteToFile(string name, string extension, string text, string location)
    {
        if (name == string.Empty || text == string.Empty || extension == string.Empty) { throw new ArgumentNullException("Name, Text or Extension provided is empty"); }


        // Write(name, extension, text, location);

        return true;
    }
}