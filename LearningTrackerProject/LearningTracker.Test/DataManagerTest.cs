namespace LearningTracker.Test;
using LearningTracker;

public class DataManagerTest
{
    DataManager testDataManager = new DataManager();
    [Fact]
    public void TestGetLearning()
    {
        var res = testDataManager.GetLearning("Skill"); // dictionary will be empty because no skills will have been added to it
        Assert.Empty(res);
    }

    [Fact]
    public void TestFormatLearningsForScript()
    {
        // args: List<string> learnings, List<string> orderedHeaders
        List<string> testLearnings = new List<string>{"testId1|||testvalue1", "testId2|||testvalue2"};
        List<string> testOrderedHeaders = new List<string>{"testHeader1", "testHeader2"};
        var res = testDataManager.FormatLearningsForScript(testLearnings, testOrderedHeaders); 
        Assert.Equal(2, res.Keys.Count);
        Assert.Equal("testvalue1", res["testId1"]["testHeader2"]);
    }
}