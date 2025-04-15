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

    [Fact]
    public void TestGetLearningIdsAndNames()
    {
        // update the dataManager variables with testing variables
        testLearningsManager.dataManager.Skills = new List<string>{"k1|||v1|||v2|||v3", "k2|||v4|||v5|||v6"};
        testLearningsManager.dataManager.Goals = new List<string>{"g1|||v1|||v2|||v3|||k1", "g2|||v4|||v5|||v6|||k1"};
        testLearningsManager.dataManager.learningsDict["Skill"] = testLearningsManager.dataManager.FormatForScript(testLearningsManager.dataManager.Skills, testLearningsManager.dataManager.headersDict["Skill"]);
        testLearningsManager.dataManager.learningsDict["Goal"] = testLearningsManager.dataManager.FormatForScript(testLearningsManager.dataManager.Goals, testLearningsManager.dataManager.headersDict["Goal"]);


        var testSkillsDict = testLearningsManager.GetLearningIdsAndNames("Skill");
        var testGoalsDict1 = testLearningsManager.GetLearningIdsAndNames("Goal", "k1");
        var testGoalsDict2 = testLearningsManager.GetLearningIdsAndNames("Goal", "k2");
        Assert.Contains("k2", testSkillsDict["IDs"]);
        Assert.Contains("g1", testGoalsDict1["IDs"]);
        Assert.Contains("g2", testGoalsDict1["IDs"]);
        Assert.Contains("v1", testGoalsDict1["Names"]);
        Assert.Contains("v4", testGoalsDict1["Names"]);
        Assert.Empty(testGoalsDict2["IDs"]);
        Assert.Empty(testGoalsDict2["Names"]);
    }
}