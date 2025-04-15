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
    public void TestFormatForScript()
    {
        // args: List<string> learnings, List<string> orderedHeaders
        List<string> testLearnings = new List<string>{"testId1|||testvalue1", "testId2|||testvalue2"};
        List<string> testOrderedHeaders = new List<string>{"testHeader1", "testHeader2"};
        var res = testDataManager.FormatForScript(testLearnings, testOrderedHeaders); 
        Assert.Equal(2, res.Keys.Count);
        Assert.Equal("testvalue1", res["testId1"]["testHeader2"]);
    }

    [Fact]
    public void TestGenerateLearningID()
    {
        // args: string learningType
        testDataManager.Skills = new List<string>{"testSkill1", "testSkill2", "testSkill3"};
        var res = testDataManager.GenerateLearningID("Skill"); 
        Assert.Equal("4", res);
    }

    [Fact]
    public void TestCreateNoteID()
    {
        // args: List<string>idComponents, int componentLength=3
        // test component length longer than default
        List<string> testIdComponents1 = new List<string>{"1", "2", "3", "4"};
        int testComponentLength1 = 4;
        string testNoteId1 = testDataManager.CreateNoteID(testIdComponents1, testComponentLength1);
        // test use of 'blank' segments
        List<string> testIdComponents2 = new List<string>{"1"};
        string testNoteId2 = testDataManager.CreateNoteID(testIdComponents2);
        Assert.Equal("1-2-3-4-0", testNoteId1);
        Assert.Equal("1-xx-xx-0", testNoteId2);
    }

    [Fact]
    public void TestSaveLearning()
    {
        // args: string learningType, Dictionary<string, string> learningDict
        testDataManager.Milestones = new List<string>{"existingMilestone1", "existingMilestone2"};
        Dictionary<string, string> testLearningDict = new Dictionary<string, string>{
            {"Name", "Test name"},
            {"Description", "Test description"},
            {"Status", "Test status"},
            {"parentID", "Test parent id"}
        };
        testDataManager.SaveLearning("Milestone", testLearningDict);
        Assert.Equal(3, testDataManager.Milestones.Count);
    }

    [Fact]
    public void TestSaveNote()
    {
        // args: Dictionary<string, string> noteMetadata, List<string> idComponents
        testDataManager.Notes = new List<string>{"existingNote1"};
        List<string> testIdComponents = new List<string>{"12", "16", "2"};
        Dictionary<string, string> testNoteMetadata = new Dictionary<string, string>{
            {"Name", "Test name"},
            {"Description", "Test description"},
            {"Body", "Test body"}
        };
        testDataManager.SaveNote(testNoteMetadata, testIdComponents);
        Assert.Equal(2, testDataManager.Notes.Count);
        Assert.Equal("12-16-2-0|||Test name|||Test description|||Test body", testDataManager.Notes[1]);
    }

    [Fact]
    public void TestFormatForDataStore()
    {
        // args: string id, Dictionary<string, string> recordDict, List<string> headers
        string testId = "5"; 
        List<string> testHeaders = new List<string>{"ID", "h1", "h2", "h3"};
        Dictionary<string, string> testRecordDict = new Dictionary<string, string>{
            {"h1", "Test value 1"},
            {"h2", "Test value 2"},
            {"h3", "Test value 3"}
        };
        string res = testDataManager.FormatForDataStore(testId, testRecordDict, testHeaders);
        Assert.Equal("5|||Test value 1|||Test value 2|||Test value 3", res);
    }
}