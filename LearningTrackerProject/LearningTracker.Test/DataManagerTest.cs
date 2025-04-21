namespace LearningTracker.Test;

using System.Data.Common;
using LearningTracker;
using Microsoft.Data.Sqlite; 

public class DataManagerTest
{
    DataManager testDataManager = new DataManager();

    // SaveLearning, UniquenessCheck, GetLearningTypeList, SaveOrUpdateLearningsList, 
    // SaveOrUpdateNotesList, GetIntegerIndexFromIdentifier, SaveNote, FormatAsList, 
    // GetFilteredLearnings, UpdateLearning, UpdateNote, DeleteLearning, UpdateLearningList, 
    // DeleteNote, FilterByDictValue, FilterByDictKey, FormatAllAsList, MakeFilterDict, FormatStringForDBFilter
    public DataManagerTest(){
        // clear any results from the databases to make sure they do not affect tests
        using var connection = new SqliteConnection($@"Data Source=learnings.db");
        connection.Open();
        using var command = new SqliteCommand("DELETE FROM skills;", connection);
        command.ExecuteNonQuery();
        using var command2 = new SqliteCommand("DELETE FROM goals;", connection);
        command.ExecuteNonQuery();
        using var command3 = new SqliteCommand("DELETE FROM milestones;", connection);
        command.ExecuteNonQuery();

        using var connection2 = new SqliteConnection($@"Data Source=notes.db");
        connection2.Open();
        using var command4 = new SqliteCommand("DELETE FROM notes;", connection2);
        command4.ExecuteNonQuery();
    }

    [Fact]
    public void TestSaveLearning()
    {
        string testDataType = "Skill";
        Dictionary<string, string> testDataDict = new Dictionary<string, string>{
            {"Name", "testSkillName"},
            {"Description", "testSkillDescription"},
            {"Status", "To-Do"}
        };
        int beforeActionCount = testDataManager.learningsDict["Skill"].Keys.Count;
        testDataManager.SaveLearning(testDataType, testDataDict);
        int afterActionCount = testDataManager.learningsDict["Skill"].Keys.Count;
        Assert.Equal(beforeActionCount+1, afterActionCount);
    }
}