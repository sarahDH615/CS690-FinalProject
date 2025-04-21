namespace LearningTracker.Test;
using LearningTracker;


public class LearningsManagerTest
{
    LearningsManager testLearningsManager = new LearningsManager();
    
    [Fact]
    public void TestGetCommonMetadataFieldsTextEntry()
    {
        var testTextEntryDict = testLearningsManager.GetCommonMetadataFieldsTextEntry();
        Assert.Contains("Name", testTextEntryDict.Keys);
        Assert.Contains("Description", testTextEntryDict.Keys);
    }

    [Fact]
    public void TestGetCommonMetadataFieldsSelection()
    {
        var testFieldsDict = testLearningsManager.GetCommonMetadataFieldsSelection();
        Assert.Contains("Status", testFieldsDict.Keys);
        Assert.Contains("To-Do", testFieldsDict["Status"]);
        Assert.Contains("Completed", testFieldsDict["Status"]);
    }
}