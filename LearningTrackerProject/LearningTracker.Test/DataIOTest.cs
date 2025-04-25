namespace LearningTracker.Test;
using Microsoft.Data.Sqlite; 
using LearningTracker;
using Moq;
using System.Transactions;

public class DataIOTest
{
    DataIO testDataIO = new DataIO();
    // DataIOHelper testHelper = new DataIOHelper();

    // string testNoteType = "Note";
    // string testNoteDatabaseName = "notes.db";
    // string testNoteTableName = "notes";
    // Dictionary<string, string> testData = new Dictionary<string, string>{
    //     {"Name", "testNoteName"},
    //     {"Description", "testNoteDescription"},
    //     {"Body", "testNoteBody"},
    //     {"ConnectedLearningCode", "1-1-xx"}
    // };

    // public DataIOTest(){
        
    // }

    // void AddToDB(string addType, Dictionary<string, string> data);

    // Dictionary<string, Dictionary<string, string>> GetDataFromDB(string databaseName, string query, List<string> desiredValues);

    // void UpdateRecord(string recordId, Dictionary<string, string> updatedFileComponents, string recordType, string databaseReference="");

    // void DeleteRecord(string recordType, string recordId, string databaseReference = "");

    // Dictionary<string, Dictionary<string, string>> GetAllResultsFromTable(string tableName);

    // Dictionary<string, Dictionary<string, string>> GetOneResultFromTableByID(string tableName, string recordId);

    // string GetMostRecentValueFromTable(string tableName);

    // Dictionary<string, Dictionary<string, string>> GetFilteredDBResults(string recordType, Dictionary<string, string> filters, string comparative="=", bool idOnly=false);

    
    [Fact]
    public void TestAddToDB()
    {
        DataIOHelper helper = new DataIOHelper();
        helper.ClearTestDB("notes.db", new List<string>{"notes"});
        testDataIO.AddToDB("Note", new Dictionary<string, string>{
                {"Name", "test name"},
                {"Description", "test description"},
                {"Body", "test body"},
                {"ConnectedLearningCode", "1-xx-xx"}
            });
        List<Dictionary<string, string>> results = helper.ReturnNoteDBResults();
        Assert.Single(results.Count);
    }

    // [Fact]
    // public void TestGetAllResultsFromTable()
    // {
    //     testHelper.ClearTestDB("notes.db", new List<string>{"notes"});
    //     testDataIO.AddToDB(testNoteType, testData);
        
    //     Dictionary<string, Dictionary<string, string>> res2 = testDataIO.GetAllResultsFromTable("notes");
    //     Assert.Single(res2);

    // }

    // [Fact]
    // public void TestGetTableName()
    // {
    //     string testName1 = "milestones";
    //     string testName2 = "Skill";
    //     string testName3 = "Goal";
    //     string testName4 = "Note";

    //     string res1 = testDataIO.GetTableName(testName1);
    //     string res2 = testDataIO.GetTableName(testName2);
    //     string res3 = testDataIO.GetTableName(testName3);
    //     string res4 = testDataIO.GetTableName(testName4);

    //     Assert.Equal("milestones", res1);
    //     Assert.Equal("skills", res2);
    //     Assert.Equal("goals", res3);
    //     Assert.Equal("notes", res4);
    // }

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

    public List<Dictionary<string, string>> ReturnNoteDBResults(){
        List<Dictionary<string, string>> results = new List<Dictionary<string, string>>{};
        using var connection = new SqliteConnection($@"Data Source=notes.db");
        connection.Open();

        using var command = new SqliteCommand($"SELECT * FROM notes;", connection);
        using var reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var firstName = reader.GetString(1);
                var lastName = reader.GetString(2);
                results.Add(new Dictionary<string, string>{
                    {"ID", reader.GetString(0)},
                    {"Name", reader.GetString(1)},
                    {"Description", reader.GetString(2)},
                    {"Body", reader.GetString(3)},
                    {"ConnectedLearningCode", reader.GetString(4)},
                });
            }
        }
        return results;
    }
}
// // https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database
// public class SqliteInMemoryBloggingControllerTest : IClassFixture<TestDatabaseFixture>
//     {
//         // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
//         // at the end of the test (see Dispose below).
//         _connection = new SqliteConnection("Filename=:memory:");
//         _connection.Open();

//         // These options will be used by the context instances in this test suite, including the connection opened above.
//         _contextOptions = new DbContextOptionsBuilder<DataIO>()
//             .UseSqlite(_connection)
//             .Options;

//         // Create the schema and seed some data
//         using var context = new DataIO(_contextOptions);

//         if (context.Database.EnsureCreated())
//         {
//             using var viewCommand = context.Database.GetDbConnection().CreateCommand();
//             viewCommand.CommandText = @"
//                 CREATE VIEW AllResources AS
//                 SELECT Url
//                 FROM Blogs;";
//             viewCommand.ExecuteNonQuery();
//         }

//         context.AddRange(
//             new Dictionary<string, string>{
//                 {"Name", "test name"},
//                 {"Description", "test description"},
//                 {"Body", "test body"},
//                 {"ConnectedLearningCode", "1-xx-xx"}
//             }
//             );
//         context.SaveChanges();
//     }

//     DataIO CreateContext() => new DataIO(_contextOptions);

//     public void Dispose() => _connection.Dispose();