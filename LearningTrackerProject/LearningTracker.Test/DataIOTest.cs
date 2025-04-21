namespace LearningTracker.Test;
using Microsoft.Data.Sqlite; 
using LearningTracker;

public class DataIOTest
{
    DataIO testDataIO = new DataIO();

    string testNoteType = "Note";
    string testNoteDatabaseName = "notes.db";
    string testNoteTableName = "notes";
    Dictionary<string, string> testData = new Dictionary<string, string>{
        {"Name", "testNoteName"},
        {"Description", "testNoteDescription"},
        {"Body", "testNoteBody"},
        {"ConnectedLearningCode", "1-1-xx"}
    };

    public DataIOTest(){
        // clear testing databases
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
    public void TestGetDataBaseNameFromReference()
    {
        string fakeDbName1 = "fakeDbName.db";
        string fakeDbName2 = "Learning";
        string fakeDbName3 = "fakeDbReference";

        string res1 = testDataIO.GetDataBaseNameFromReference(fakeDbName1);
        string res2 = testDataIO.GetDataBaseNameFromReference(fakeDbName2);
        Assert.Equal("fakeDbName.db", res1);
        Assert.Equal("learnings.db", res2);
        Assert.Throws<ArgumentException>(() => testDataIO.GetDataBaseNameFromReference(fakeDbName3));
    }

    [Fact]
    public void TestGetAllResultsFromTable()
    {
        testDataIO.AddToDB(testNoteType, testData);
        
        Dictionary<string, Dictionary<string, string>> res2 = testDataIO.GetAllResultsFromTable("notes");
        Assert.Single(res2);

    }

    [Fact]
    public void TestGetTableName()
    {
        string testName1 = "milestones";
        string testName2 = "Skill";
        string testName3 = "Goal";
        string testName4 = "Note";

        string res1 = testDataIO.GetTableName(testName1);
        string res2 = testDataIO.GetTableName(testName2);
        string res3 = testDataIO.GetTableName(testName3);
        string res4 = testDataIO.GetTableName(testName4);

        Assert.Equal("milestones", res1);
        Assert.Equal("skills", res2);
        Assert.Equal("goals", res3);
        Assert.Equal("notes", res4);
    }
}