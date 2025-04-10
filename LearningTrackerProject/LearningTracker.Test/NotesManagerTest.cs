namespace LearningTracker.Test;

public class NotesManagerTest
{
    NotesManager testNotesManager = new NotesManager();
    
    [Fact]
    public void TestGetCommonMetadataFieldsTextEntry()
    {
        var testTextEntryDict = testNotesManager.GetCommonMetadataFieldsTextEntry();
        Assert.Contains("Name", testTextEntryDict.Keys);
        Assert.Contains("Description", testTextEntryDict.Keys);
        Assert.Contains("Body", testTextEntryDict.Keys);
    }

}