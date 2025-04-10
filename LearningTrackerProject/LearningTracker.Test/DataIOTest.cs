namespace LearningTracker.Test;
using System.IO;
using LearningTracker;

public class DataIOTest
{
    DataIO testDataIO = new DataIO();
    
    [Fact]
    public void TestGetOrCreateFile()
    {
        // args: string fileName, List<string> headers, string separator = "|||"
        string testFileName = "Test.txt";
        List<string> testHeaders = new List<string>{"Test Header 1", "Test Header 2"};

        testDataIO.GetOrCreateFile(testFileName, testHeaders);
        string contents;
        using (StreamReader reader = new StreamReader(testFileName)){
            contents = reader.ReadToEnd();
        }
        Assert.True(File.Exists(testFileName));
        Assert.Equal("Test Header 1|||Test Header 2\n", contents);
    }

    [Fact]
    public void SaveRecordTest()
    {
        // args: string fileName, string newEntry

        // create a testing file
        string testFileName = "Test2.txt";
        using (StreamWriter sw = File.CreateText(testFileName))
        {
            sw.WriteLine("Test content");
        }
        testDataIO.SaveRecord(testFileName, "Additional Content");

        // read file contents
        string contents;
        using (StreamReader reader = new StreamReader(testFileName)){
            contents = reader.ReadToEnd();
        }
        Assert.Equal("Test content\nAdditional Content\n", contents);
    }
}