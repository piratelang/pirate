namespace Common;

public class FileHandler
{
    //Creates File and folder if not exists

    // Name: Without extension
    // Extension: Without dot
    // Location: Without root folder, i.e. "."
    public bool WriteToFile(string name, string extension, string text, string location, Boolean encryption = false)
    {
        if (name == string.Empty || text == string.Empty || extension == string.Empty) {throw new ArgumentNullException("Name, Text or Extension provided is empty");}

        if (encryption)
        {
            text = Encrypt(text);
        }

        CreateFolder(location);
        
        Write(name, extension, text, location);

        return true;
    }

    private string Encrypt(string text)
    {
        return text += " This should be encrypted.";
    }
    
    private void CreateFolder(string location)
    {
        var targetFolder = Path.Combine(Environment.CurrentDirectory, location);

        bool exists = Directory.Exists(targetFolder);
        if (!exists)
            Directory.CreateDirectory(targetFolder);
    }

    private void Write(string name, string extension, string text, string location)
    {
        name += extension;
        var targetFolder = Path.Combine(Environment.CurrentDirectory, location);

        string fileName = Path.Combine(targetFolder, name);

        File.WriteAllText(fileName, text);
    }
}
