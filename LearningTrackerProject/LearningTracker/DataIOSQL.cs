namespace LearningTracker;
using System.IO;
using Microsoft.Data.Sqlite;

public class DataIOSQL{

    public void CreateSqlDatabases(string databaseName){
        string skillsCommand = @"
            CREATE TABLE IF NOT EXISTS skills(
            Name TEXT NOT NULL,
            LearningType TEXT NOT NULL,
            Description TEXT NOT NULL,
            Status TEXT NOT NULL,
            PRIMARY KEY (LearningType, Name)
        )";

        string goalsCommand  = @"
            CREATE TABLE IF NOT EXISTS goals(
            Name TEXT NOT NULL,
            LearningType TEXT NOT NULL,
            Description TEXT NOT NULL,
            Status TEXT NOT NULL,
            ParentID TEXT NOT NULL,
            PRIMARY KEY (LearningType, Name)
        )";

        string milestonesCommand  = @"
            CREATE TABLE IF NOT EXISTS milestones(
            Name TEXT NOT NULL,
            LearningType TEXT NOT NULL,
            Description TEXT NOT NULL,
            Status TEXT NOT NULL,
            ParentID TEXT NOT NULL,
            PRIMARY KEY (LearningType, Name)
        )";

        string notesCommand = @"
            CREATE TABLE IF NOT EXISTS notes(
            Name TEXT NOT NULL,
            Description TEXT NOT NULL,
            Body TEXT NOT NULL,
            ConnectedLearningCode TEXT NOT NULL,
            PRIMARY KEY (ConnectedLearningCode, Name)
        )";

        List<string> commandsList = new List<string>();
        if(databaseName == "learnings.db"){
            commandsList = new List<string>{skillsCommand, goalsCommand, milestonesCommand};
        }
        else if(databaseName == "notes.db"){
            commandsList = new List<string>{notesCommand};
        }

        try
        {
            using var connection = new SqliteConnection($@"Data Source={databaseName}");
            connection.Open();

            foreach(string commandString in commandsList){
                using var command = new SqliteCommand(commandString, connection);
                command.ExecuteNonQuery();
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine(ex.Message);
        }

    }
    public void CreateOrOpenSQLDB(){
        if (!File.Exists("learnings.db"))
        {
            CreateSqlDatabases("learnings.db");
        }
        if (!File.Exists("notes.db"))
        {
            CreateSqlDatabases("notes.db");
        }
    }

    public void AddToDB(string addType, Dictionary<string, string> data){
        string databaseName = "";
        string tableName = "";
        string valuesCommand = "";
        List<List<string>> values = new List<List<string>>();
        if(addType == "Note"){
            databaseName = "notes.db";
            tableName = "notes";
            valuesCommand+=$"@Name, @Description, @Body, @ConnectedLearningCode";
            values = new List<List<string>>{
                new List<string>{"@Name", data["Name"]},
                new List<string>{"@Description", data["Description"]},
                new List<string>{"@Body", data["Body"]},
                new List<string>{"@ConnectedLearningCode", data["ConnectedLearningCode"]}
            };
        }
        else{
            databaseName = "learnings.db";
            valuesCommand+=$"@Name, @LearningType, @Description, @Status";
            values = new List<List<string>>{
                new List<string>{"@Name", data["Name"]},
                new List<string>{"@LearningType", data["LearningType"]},
                new List<string>{"@Description", data["Description"]},
                new List<string>{"@Status", data["Status"]}
            };
            if(addType == "Skill"){
                tableName = "skills";
                valuesCommand+=")";
            }
            else if(addType == "Goal"){
                tableName = "goals";
                valuesCommand+=", @ParentID)";
                values.Add(new List<string>{"@ParentID", data["ParentID"]});
            }
            else{
                tableName = "milestones";
                valuesCommand+=", @ParentID)";
                values.Add(new List<string>{"@ParentID", data["ParentID"]});
            }
        }

        string commandString = $"INSERT INTO {tableName} ({valuesCommand.Replace("@", "")}) VALUES ({valuesCommand})";
        // Console.WriteLine($"Command string: {commandString}");

        try
        {
            // Open a new database connection
            using var connection = new SqliteConnection($@"Data Source={databaseName}");
            connection.Open();

            // Bind parameters values
            using var command = new SqliteCommand(commandString, connection);

            foreach(List<string> entry in values){
                command.Parameters.AddWithValue(entry[0], entry[1]);
            }

            // Execute the INSERT statement
            var rowInserted = command.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    

    public Dictionary<string, Dictionary<string, string>> GetDataFromDB(string databaseName, string query, List<string> desiredValues){
        Dictionary<string, Dictionary<string, string>> notesResultsDict = new Dictionary<string, Dictionary<string, string>>();
        try{
            // Open a new database connection
            using var connection = new SqliteConnection($@"Data Source={databaseName}");
            connection.Open();

            using var command = new SqliteCommand(query, connection);

            // Bind parameters values
            SqliteDataReader reader = command.ExecuteReader();
            int count = 0;
            if (reader.HasRows){
                while (reader.Read()) {
                    notesResultsDict[count.ToString()] = new Dictionary<string, string>();
                    for (int i=0;i<reader.FieldCount;i++){
                        if(desiredValues.Contains(reader.GetName(i))){
                            notesResultsDict[count.ToString()][reader.GetName(i)] = reader[i].ToString()!; // null tolerant operator
                        }
                    }
                    count++;
                }
            }
        }
        catch (SqliteException ex){
            Console.WriteLine(ex.Message);
        }
        return notesResultsDict;
    }

    public void GetDataFromDB(string databaseName, string query){
        List<object> values = new List<object>();
        try{
            // Open a new database connection
            using var connection = new SqliteConnection($@"Data Source={databaseName}");
            connection.Open();

            using var command = new SqliteCommand(query, connection);

            // Bind parameters values
            SqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows){
                int current = 0;
                while (reader.Read()) {
                    values.Add(reader.GetValue(current++));
                }
            }

            foreach(object entry in values){
                Console.WriteLine(entry);
            }
        }
        catch (SqliteException ex){
            Console.WriteLine(ex.Message);
        }
    }
    
}