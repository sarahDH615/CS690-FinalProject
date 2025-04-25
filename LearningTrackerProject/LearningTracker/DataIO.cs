namespace LearningTracker;
using System.IO;
using Microsoft.Data.Sqlite;

interface IDataIO<T> { 
      
    void AddToDB(string addType, Dictionary<string, string> data);

    Dictionary<string, Dictionary<string, string>> GetDataFromDB(string databaseName, string query, List<string> desiredValues);

    void UpdateRecord(string recordId, Dictionary<string, string> updatedFileComponents, string recordType, string databaseReference="");

    void DeleteRecord(string recordType, string recordId, string databaseReference = "");

    Dictionary<string, Dictionary<string, string>> GetAllResultsFromTable(string tableName);

    Dictionary<string, Dictionary<string, string>> GetOneResultFromTableByID(string tableName, string recordId);

    string GetMostRecentValueFromTable(string tableName);

    Dictionary<string, Dictionary<string, string>> GetFilteredDBResults(string recordType, Dictionary<string, string> filters, string comparative="=", bool idOnly=false);

} 

public class DataIO : IDataIO<DataIO> {

    public Dictionary<string, string> databaseNameDict = new Dictionary<string, string>{};
    public Dictionary<string, string> createTablesCommandsDicts = new Dictionary<string, string>{};

    public DataIO(){
        databaseNameDict = new Dictionary<string, string>{
            {"Note", "notes.db"},
            {"Learning", "learnings.db"}
        };
        createTablesCommandsDicts = new Dictionary<string, string>{
            {
                "Skill", @"
                    CREATE TABLE IF NOT EXISTS skills(
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    UNIQUE(Name)
                )"
            },
            {
                "Goal", @"
                    CREATE TABLE IF NOT EXISTS goals(
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    ParentID TEXT NOT NULL,
                    UNIQUE(Name, ParentID)
                )"
            },
            {
                "Milestone", @"
                    CREATE TABLE IF NOT EXISTS milestones(
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    ParentID TEXT NOT NULL,
                    UNIQUE(Name, ParentID)
                )"
            },
            {
                "Note", @"
                    CREATE TABLE IF NOT EXISTS notes(
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    Body TEXT NOT NULL,
                    ConnectedLearningCode TEXT NOT NULL,
                    UNIQUE(Name, ConnectedLearningCode)
                )"
            }
        };
        CreateOrOpenSQLDB(); // create databases if they don't exist
    }
    // function to run upon initialisation
    protected internal void CreateOrOpenSQLDB(){
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
        var databaseInfo = GetDatabaseInfo(addType);
        string databaseName = databaseInfo.databaseName;
        string tableName = databaseInfo.tableName;
        var valuesInfo = GetAddCommandValues(tableName, data);
        string valuesCommand = valuesInfo.valuesCommand;
        List<List<string>> values = valuesInfo.values;

        string commandString = $"INSERT INTO {tableName} ({valuesCommand.Replace("@", "")}) VALUES ({valuesCommand})";
        
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
        Dictionary<string, Dictionary<string, string>> resultsDict = new Dictionary<string, Dictionary<string, string>>();
        if(databaseName.EndsWith(".db") is false && databaseNameDict.Keys.Contains(databaseName)){
            databaseName = databaseNameDict[databaseName];
        }
        try{
            // Open a new database connection
            using var connection = new SqliteConnection($@"Data Source={databaseName}");
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            SqliteDataReader reader = command.ExecuteReader();
            resultsDict = GetResultsFromReader(reader, resultsDict, desiredValues);
        }
        catch (SqliteException ex){
            Console.WriteLine(ex.Message);
        }
        return resultsDict;
    }

    public void UpdateRecord(string recordId, Dictionary<string, string> updatedFileComponents, string recordType, string databaseReference=""){
        var databaseInfo = GetDatabaseInfo(recordType);
        string tableName = databaseInfo.tableName;
        string databaseName = databaseInfo.databaseName;
        
        List<string> setStringList = new List<string>();
        foreach(KeyValuePair<string, string> component in updatedFileComponents){
            setStringList.Add($"{component.Key} = {component.Value}");
        }
        string setString = string.Join(", ", setStringList);

        string updateCommandString = $@"
            UPDATE {tableName}
            SET {setString}
            WHERE ID = {recordId};";
        
        ExecuteSQLCommand(databaseName, updateCommandString);
    }

    public void DeleteRecord(string recordType, string recordId, string databaseReference = ""){
        var databaseInfo = GetDatabaseInfo(recordType);
        string tableName = databaseInfo.tableName;
        string databaseName = databaseInfo.databaseName;
        string deleteCommandString = $@"DELETE FROM {tableName} WHERE ID = {recordId};";
        ExecuteSQLCommand(databaseName, deleteCommandString);
    }

    public Dictionary<string, Dictionary<string, string>> GetAllResultsFromTable(string tableName){
        var databaseInfo = GetDatabaseInfo(tableName);
        string databaseName = databaseInfo.databaseName;
        List<string> desiredColumns = databaseInfo.columns;
        string selectAllString = $"SELECT * FROM {tableName};";
        
        return GetDataFromDB(databaseName, selectAllString, desiredColumns);
    }

    public Dictionary<string, Dictionary<string, string>> GetOneResultFromTableByID(string tableName, string recordId){
        var databaseInfo = GetDatabaseInfo(tableName);
        string databaseName = databaseInfo.databaseName;
        List<string> desiredColumns = databaseInfo.columns;
        
        string selectAllString = $"SELECT * FROM {tableName} WHERE ID = {recordId};";
        
        return GetDataFromDB(databaseName, selectAllString, desiredColumns);
    }

    public string GetMostRecentValueFromTable(string tableName){
        var databaseInfo = GetDatabaseInfo(tableName);
        string databaseName = databaseInfo.databaseName;
        List<string> desiredColumns = databaseInfo.columns;
        string selectMostRecentString = $"SELECT MAX(ID) FROM {tableName};";
        
        return GetDataFromDB(databaseName, selectMostRecentString, desiredColumns).ToString()!;
    }
    
    public Dictionary<string, Dictionary<string, string>> GetFilteredDBResults(string recordType, Dictionary<string, string> filters, string comparative="=", bool idOnly=false){
        var databaseInfo = GetDatabaseInfo(recordType);
        string tableName = databaseInfo.tableName;
        string databaseName = databaseInfo.databaseName;
        List<string> desiredColumns = databaseInfo.columns;

        List<string> filterList = GetFilterListForFilteredSearch(filters, comparative);
        string returns;
        if(idOnly){
            returns = "ID";
        }
        else{
            returns = "*";
        }

        string selectFilteredResults = $"SELECT {returns} FROM {tableName} WHERE {string.Join(" AND ", filterList)};";
        
        return GetDataFromDB(databaseName, selectFilteredResults, desiredColumns);
    }

    // helpers
    protected internal void CreateSqlDatabases(string databaseName){
        List<string> commandsList = new List<string>();
        if(databaseName == "learnings.db"){
            commandsList = new List<string>{
                createTablesCommandsDicts["Skill"], createTablesCommandsDicts["Goal"], createTablesCommandsDicts["Milestone"]};
        }
        else if(databaseName == "notes.db"){
            commandsList = new List<string>{createTablesCommandsDicts["Note"]};
        }

        try{
            using var connection = new SqliteConnection($@"Data Source={databaseName}");
            connection.Open();

            foreach(string commandString in commandsList){
                using var command = new SqliteCommand(commandString, connection);
                command.ExecuteNonQuery();
            }
        }
        catch (SqliteException ex){
            Console.WriteLine(ex.Message);
        }
    }

    protected internal (string databaseName, string tableName, List<string> columns) GetDatabaseInfo(string databaseReference){
        if(databaseReference.ToLower().Contains("note")){
            return ("notes.db", "notes", new List<string>{"ID", "Name", "Description", "Body", "ConnectedLearningCode"});
        }
        else{
            string dbName = "learnings.db";
            List<string> columns = new List<string>{"ID", "Name", "Description", "Status"};
            if(databaseReference.ToLower().Contains("skill")){
                return (dbName, "skills", columns);
            }
            else{
                columns.Add("ParentID");
                if(databaseReference.ToLower().Contains("goal")){
                    return (dbName, "goals", columns);
                }
                else{
                    return (dbName, "milestones", columns);
                }
            }
        }
    }

    protected internal (string valuesCommand, List<List<string>> values) GetAddCommandValues(string tableName, Dictionary<string, string> data){
        if(tableName == "notes"){
            return ($"@Name, @Description, @Body, @ConnectedLearningCode", 
                new List<List<string>>{
                    new List<string>{"@Name", data["Name"]},
                    new List<string>{"@Description", data["Description"]},
                    new List<string>{"@Body", data["Body"]},
                    new List<string>{"@ConnectedLearningCode", data["ConnectedLearningCode"]}
                });
        }
        else{
            string valuesCommand=$"@Name, @Description, @Status";
            List<List<string>> values = new List<List<string>>{
                new List<string>{"@Name", data["Name"]},
                new List<string>{"@Description", data["Description"]},
                new List<string>{"@Status", data["Status"]}
            };
            if(tableName != "skills"){
                valuesCommand+=", @ParentID";
                values.Add(new List<string>{"@ParentID", data["ParentID"]});
            }
            return (valuesCommand, values);
        }
    }

    protected internal Dictionary<string, Dictionary<string, string>> GetResultsFromReader(SqliteDataReader dataReader, Dictionary<string, Dictionary<string, string>> dataResultsDict, List<string> valuesToExtract){
        if (dataReader.HasRows){
            while (dataReader.Read()) {
                string id;
                if(dataReader.GetName(0) == "ID"){ // this should always be the case?
                    id = dataReader[0].ToString()!;
                    dataResultsDict[id] = new Dictionary<string, string>();
                
                    for (int i=1;i<dataReader.FieldCount;i++){
                        if(valuesToExtract.Contains(dataReader.GetName(i))){
                            dataResultsDict[id][dataReader.GetName(i)] = dataReader[i].ToString()!; // null tolerant operator
                        }
                    }
                }
            }
        }
        return dataResultsDict;
    }

    protected internal void ExecuteSQLCommand(string databaseName, string commandString){
        try{
            using var connection = new SqliteConnection($@"Data Source={databaseName}");
            connection.Open();
            
            using var command = new SqliteCommand(commandString, connection);
            var executionResult = command.ExecuteNonQuery();
        }
        catch (SqliteException ex){
            Console.WriteLine(ex.Message);
        }
    }

    protected internal List<string> GetFilterListForFilteredSearch(Dictionary<string, string> filters, string comparative="="){
        List<string> filterList = new List<string>();
        foreach(KeyValuePair<string, string> entry in filters){
            filterList.Add($"{entry.Key} {comparative} {entry.Value}");
        }
        return filterList;
    }
}