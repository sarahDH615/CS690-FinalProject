namespace LearningTracker.Test;
using Microsoft.Data.Sqlite; 
using LearningTracker;

public class DataIOTest
{
    DataIO testDataIO = new DataIO();
    DataIOHelper testHelper = new DataIOHelper();

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
        testHelper.ClearTestDB("notes.db", new List<string>{"notes"});
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

public class DataIOHelper{

    public void ClearTestDB(string dbName, List<string>tableNames){
        using var connection = new SqliteConnection($@"Data Source={dbName}");
        connection.Open();

        foreach(string tableName in tableNames){
            using var command = new SqliteCommand($"DELETE FROM {tableName};", connection);
            command.ExecuteNonQuery();
        }
    }
}