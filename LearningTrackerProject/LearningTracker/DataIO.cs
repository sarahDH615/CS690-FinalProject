namespace LearningTracker;
using System.IO;
using Microsoft.Data.Sqlite;

public class DataIO{

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

    protected internal void CreateSqlDatabases(string databaseName){
        
        List<string> commandsList = new List<string>();
        if(databaseName == "learnings.db"){
            commandsList = new List<string>{
                createTablesCommandsDicts["Skill"], createTablesCommandsDicts["Goal"], createTablesCommandsDicts["Milestone"]};
        }
        else if(databaseName == "notes.db"){
            commandsList = new List<string>{createTablesCommandsDicts["Note"]};
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
            valuesCommand+=$"@Name, @Description, @Status";
            values = new List<List<string>>{
                new List<string>{"@Name", data["Name"]},
                new List<string>{"@Description", data["Description"]},
                new List<string>{"@Status", data["Status"]}
            };
            if(addType == "Skill"){
                tableName = "skills";
            }
            else if(addType == "Goal"){
                tableName = "goals";
                valuesCommand+=", @ParentID";
                values.Add(new List<string>{"@ParentID", data["ParentID"]});
            }
            else{
                tableName = "milestones";
                valuesCommand+=", @ParentID";
                values.Add(new List<string>{"@ParentID", data["ParentID"]});
            }
        }

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

    public void UpdateRecord(string recordId, Dictionary<string, string> updatedFileComponents, string recordType, string databaseReference=""){
        
        string tableName = GetTableName(recordType);
        string databaseName;
        if(databaseReference == ""){
            var returns = GetSetupForDBSearch(tableName);
            databaseName = returns.dbName;
        }
        else{
            databaseName = GetDataBaseNameFromReference(databaseReference);
        }
        
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

    public string GetDataBaseNameFromReference(string reference){
        if(reference.EndsWith(".db")){
            return reference;
        }
        else{
            if(databaseNameDict.Keys.Contains(reference)){
                return databaseNameDict[reference];
            }
            throw new ArgumentException($"No database name for reference {reference}."); 
        }
    }

    public void DeleteRecord(string recordType, string recordId, string databaseReference = ""){
        string databaseName;
        if(databaseReference == ""){
            if(recordType == "Note"){
                databaseName = "notes.db";
            }
            else{
                databaseName = "learnings.db";   
            }
        }
        else{
            databaseName = GetDataBaseNameFromReference(databaseReference);
        }
        string tableName = GetTableName(recordType);
        string deleteCommandString = $@"DELETE FROM {tableName} WHERE ID = {recordId};";
        ExecuteSQLCommand(databaseName, deleteCommandString);
    }

    public void ExecuteSQLCommand(string databaseName, string commandString){
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

    public Dictionary<string, Dictionary<string, string>> GetAllResultsFromTable(string tableName){
        string databaseName;
        List<string> desiredColumns = new List<string>();
        if(tableName == "notes"){
            databaseName = "notes.db";
            desiredColumns = new List<string>{"ID", "Name", "Description", "Body", "ConnectedLearningCode"};
        }
        else{
            databaseName = "learnings.db";
            if(tableName == "skills"){
                desiredColumns = new List<string>{"ID", "Name", "Description", "Status"};
            }
            else{
                desiredColumns = new List<string>{"ID", "Name", "Description", "Status", "ParentID"};
            }
        }
        string selectAllString = $"SELECT * FROM {tableName};";
        
        return GetDataFromDB(databaseName, selectAllString, desiredColumns);
    }

    public Dictionary<string, Dictionary<string, string>> GetOneResultFromTableByID(string tableName, string recordId){
        string databaseName;
        List<string> desiredColumns = new List<string>();
        var setupVars = GetSetupForDBSearch(tableName);
        databaseName = setupVars.dbName;
        desiredColumns = setupVars.columns;
        string selectAllString = $"SELECT * FROM {tableName} WHERE ID = {recordId};";
        
        return GetDataFromDB(databaseName, selectAllString, desiredColumns);
    }

    protected internal (string dbName, List<string> columns) GetSetupForDBSearch(string tableName){
        string dbName;
        List<string> columns = new List<string>();
        if(tableName == "notes"){
            dbName = "notes.db";
            columns = new List<string>{"ID", "Name", "Description", "Body", "ConnectedLearningCode"};
        }
        else{
            dbName = "learnings.db";
            if(tableName == "skills"){
                columns = new List<string>{"ID", "Name", "Description", "Status"};
            }
            else{
                columns = new List<string>{"ID", "Name", "Description", "Status", "ParentID"};
            }
        }
        return (dbName, columns);
    }

    public string GetMostRecentValueFromTable(string tableName){
        var setupVars = GetSetupForDBSearch(tableName);
        string databaseName = setupVars.dbName;
        List<string> desiredColumns = setupVars.columns;
        string selectMostRecentString = $"SELECT MAX(ID) FROM {tableName};";
        
        return GetDataFromDB(databaseName, selectMostRecentString, desiredColumns).ToString()!;
    }

    protected internal string GetTableName(string recordType){
        if(new List<string>{"skills", "goals", "milestones", "notes"}.Contains(recordType)){
            return recordType;
        }
        else if(recordType == "Skill"){
            return "skills";
        }
        else if(recordType == "Goal"){
            return "goals";
        }
        else if(recordType == "Milestone"){
            return "milestones";
        }
        else{
            return "notes";
        }
    }

    protected internal List<string> GetFilterListForFilteredSearch(Dictionary<string, string> filters, string comparative="="){
        List<string> filterList = new List<string>();
        foreach(KeyValuePair<string, string> entry in filters){
            filterList.Add($"{entry.Key} {comparative} {entry.Value}");
        }
        return filterList;
    }

    public Dictionary<string, Dictionary<string, string>> GetFilteredDBResults(string recordType, Dictionary<string, string> filters, string comparative="=", bool idOnly=false){
        string tableName = GetTableName(recordType);
        var setupVars = GetSetupForDBSearch(tableName);
        string databaseName = setupVars.dbName;
        List<string> desiredColumns = setupVars.columns;

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
}