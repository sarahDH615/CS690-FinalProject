namespace LearningTracker;
using System.IO;

public class DataIO{

    public List<string> GetOrCreateFile(string fileName, List<string> headers, string separator = "|||"){
        if (!File.Exists(fileName))
        {
            // Create a file to write to
            using (StreamWriter sw = File.CreateText(fileName))
            {
                // add headers
                sw.WriteLine(string.Join(separator, headers));
            }
        }
        // open the file and return any data existing in it
        // open
        List<string> fileContents = new List<string>();
        using (StreamReader sr = File.OpenText(fileName))
        {
            string s;
            while ((s = sr.ReadLine()!) != null) // null-forgiving operator
            {
                fileContents.Add(s);
            }
        }
        // remove header
        if(fileContents.Count > 0){
            fileContents.RemoveAt(0);
        }
        // // return data
        return fileContents;
    }

    public void SaveRecord(string fileName, string newEntry){
        File.AppendAllText(fileName, newEntry+"\n");
    }
}