namespace LearningTracker;

public class DataManager{
    public Dictionary<string, string> typesToTables = new Dictionary<string, string>{
        {"Skill", "skills"},
        {"Goal", "goals"},
        {"Milestone", "milestones"},
        {"Note", "notes"}
    };
    public Dictionary<string, List<string>> headersDict = new Dictionary<string, List<string>>{
        {"Skill", new List<string>{"ID", "Name", "Description", "Status"}},
        {"Goal", new List<string>{"ID", "Name", "Description", "Status", "ParentID"}},
        {"Milestone", new List<string>{"ID", "Name", "Description", "Status", "ParentID"}},
        {"Note", new List<string>{"ID", "Name", "Description", "Body"}}
    };
    public DataIO dataIO = new DataIO();
    public List<string> Skills = new List<string>();
    public List<string> Goals = new List<string>();
    public List<string> Milestones = new List<string>();
    public List<string> Notes = new List<string>();

    public Dictionary<string, Dictionary<string, Dictionary<string, string>>> learningsDict = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>{}; 
    public Dictionary<string, Dictionary<string, string>> notesDict = new Dictionary<string, Dictionary<string, string>>{}; 

    public DataManager(){
        learningsDict["Skill"] = dataIO.GetAllResultsFromTable("skills");
        learningsDict["Goal"] = dataIO.GetAllResultsFromTable("goals");
        learningsDict["Milestone"] = dataIO.GetAllResultsFromTable("milestones");
        notesDict = dataIO.GetAllResultsFromTable("notes");

        Skills = FormatAllAsList(learningsDict["Skill"], headersDict["Skill"]);
        Goals = FormatAllAsList(learningsDict["Goal"], headersDict["Goal"]);
        Milestones = FormatAllAsList(learningsDict["Milestone"], headersDict["Milestone"]);
        Notes = FormatAllAsList(notesDict, headersDict["Note"]);
    }

    // learning and note methods
    public string UniquenessCheck(string recordType, Dictionary<string, string> filters){
        Dictionary<string, string> formattedFilters = new Dictionary<string, string>{};
        foreach (KeyValuePair<string, string> pair in filters){
            formattedFilters[pair.Key] = FormatStringForDBFilter(pair.Value);
        }
        Dictionary<string, Dictionary<string, string>> results = dataIO.GetFilteredDBResults(
            recordType, formattedFilters, "=", true);
        if(results.Keys.Count > 0){
            return results.Keys.ToList()[0];
        }
        return "";
        // return _UniquenessCheck(recordType, filters, dataIO);
    }

    // public string _UniquenessCheck(string recordType, Dictionary<string, string> filters, IDataIO<DataIO> IO){
    //     Dictionary<string, string> formattedFilters = new Dictionary<string, string>{};
    //     foreach (KeyValuePair<string, string> pair in filters){
    //         formattedFilters[pair.Key] = FormatStringForDBFilter(pair.Value);
    //     }
    //     Dictionary<string, Dictionary<string, string>> results = IO.GetFilteredDBResults(
    //         recordType, formattedFilters, "=", true);
    //     if(results.Keys.Count > 0){
    //         return results.Keys.ToList()[0];
    //     }
    //     return "";
    // }

    public Dictionary<string, Dictionary<string, string>> FilterByDictValue(Dictionary<string, Dictionary<string, string>> dictToFilter, string filterValueName, string filterValue){
        var filterList = dictToFilter.Where(kvp => kvp.Value[filterValueName] == filterValue);
        // back to a dictionary
        var newDictionary = filterList.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);  
        return newDictionary;   
    }

    public Dictionary<string, Dictionary<string, string>> FilterByDictKey(Dictionary<string, Dictionary<string, string>> dictToFilter, string filterValue){
        var filterList = dictToFilter.Where(kvp => kvp.Key == filterValue);
        // back to a dictionary
        var newDictionary = filterList.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);  
        return newDictionary;   
    }

    public Dictionary<string, string> MakeFilterDict(string filterName, string filter){
        Dictionary<string, string> filterDict = new Dictionary<string, string>{};
        if(filter != ""){
            filterDict[filterName] = FormatStringForDBFilter(filter);
        }
        return filterDict;
    }

    // learning methods
    public void SaveLearning(string learningType, Dictionary<string, string> learningDict){
        dataIO.AddToDB(learningType, learningDict);
        string learningID = dataIO.GetMostRecentValueFromTable(typesToTables[learningType]);
        // update the list
        SaveOrUpdateLearningsList(learningType, FormatAsList(learningID, learningDict, headersDict[learningType]));
        // update dict
        learningsDict[learningType][learningID] = learningDict;
    }

    public Dictionary<string, Dictionary<string, string>> GetFilteredLearnings(string learningType, Dictionary<string, string> filters, string comparer="="){
        if(filters.Keys.Count == 0){
            return learningsDict[learningType];
        }
        else{
            return dataIO.GetFilteredDBResults(learningType, filters, comparer);
        }
    }

    public void UpdateLearning(string learningType, string learningID, Dictionary<string, string> learningMetadata){
        // update dict with any new values
        foreach(KeyValuePair<string, string> pair in learningMetadata){
            learningsDict[learningType][learningID][pair.Key] = pair.Value;
        }
        // reformat dict as string and save to list
        List<string> learningHeaders = headersDict[learningType];
        string learningString = FormatAsList(learningID, learningsDict[learningType][learningID], learningHeaders);
        SaveOrUpdateLearningsList(learningType, learningString, learningID);
        // save back to db
        Dictionary<string, string> formattedDict = new Dictionary<string, string>{};
        foreach(KeyValuePair<string, string> pair in learningMetadata){
            formattedDict[pair.Key] = FormatStringForDBFilter(pair.Value);
        }
        dataIO.UpdateRecord(learningID, formattedDict, learningType);
    }

    public void DeleteLearning(string learningId, string learningType){
        // get related list of learnings
        List<string> learningsList = GetLearningTypeList(learningType);
        int learningIndex = GetIntegerIndexFromIdentifier(learningsList, learningId);
        if(learningIndex > -1){
            learningsList.RemoveAt(learningIndex);
        }
        // update the learning list
        UpdateLearningList(learningType, learningsList);
        // update the learning dictionary
        learningsDict[learningType].Remove(learningId);
        // save to data store
        dataIO.DeleteRecord(learningType, learningId);
    }
    
    
    // note methods
    public void SaveOrUpdateNotesList(string noteString, string noteId=""){
        if(noteId != ""){
            int indexInt = GetIntegerIndexFromIdentifier(Notes, noteId);
            if(indexInt > -1){
                Notes[indexInt] = noteString;
            }
        }
        else{
            Notes.Add(noteString);
        }
        
    }

    public void SaveNote(Dictionary<string, string> noteMetadata){
        // create id for dict based on type
        List<string> noteHeaders = headersDict["Note"];

        dataIO.AddToDB("Note", noteMetadata);
        string noteId = dataIO.GetMostRecentValueFromTable("notes");
        
        string noteString = FormatAsList(noteId, noteMetadata, noteHeaders);
        // update the notes list
        SaveOrUpdateNotesList(noteString);
        // update the notes dict
        notesDict[noteId] = noteMetadata;
    }

    public void UpdateNote(string noteID, Dictionary<string, string> noteMetadata){
        // update dict with any new values
        foreach(KeyValuePair<string, string> pair in noteMetadata){
            notesDict[noteID][pair.Key] = pair.Value;
        }
        // reformat dict as string and save to list
        List<string> headers = headersDict["Note"];
        string noteString = FormatAsList(noteID, notesDict[noteID], headers);
        SaveOrUpdateNotesList(noteString, noteID);
        // save back to db
        Dictionary<string, string> formattedDict = new Dictionary<string, string>{};
        foreach(KeyValuePair<string, string> pair in noteMetadata){
            formattedDict[pair.Key] = FormatStringForDBFilter(pair.Value);
        }
        dataIO.UpdateRecord(noteID, formattedDict, "Note");
    }

    public void DeleteNote(string noteId){
        // get related list of notes
        int noteIndex = GetIntegerIndexFromIdentifier(Notes, noteId);
        if(noteIndex > -1){
            Notes.RemoveAt(noteIndex); // this should update in place
        }
        // update the learning dictionary
        notesDict.Remove(noteId);
        // save to data store
        dataIO.DeleteRecord("Note", noteId);
    }

    // helpers
    protected internal  List<string> GetLearningTypeList(string learningType){
        if(learningType == "Skill"){
            return Skills;
        }
        else if(learningType == "Goal"){
            return Goals;
        }
        else{
            return Milestones;
        }
    }

    protected internal string FormatAsList(string id, Dictionary<string, string> recordDict, List<string> headers){
        
        List<string> recordList = new List<string>();
        foreach(string header in headers){ 
            if(header == "ID" && recordDict.ContainsKey("ID") is false){
                recordList.Add(id);
            }
            else{
                recordList.Add(recordDict[header]);
            }
        }

        string recordString = string.Join("|||", recordList);
        return recordString;
    }

    protected internal void SaveOrUpdateLearningsList(string learningType, string learningString, string index=""){
        List<string> learningList = GetLearningTypeList(learningType);
        // add to the learnings lists
        if(index != ""){
            int indexInt = GetIntegerIndexFromIdentifier(learningList, index);
            if(indexInt > -1){
                learningList[indexInt] = learningString;
            }
        }
        else{
            learningList.Add(learningString);
        }
    }

    protected internal int GetIntegerIndexFromIdentifier(List<string> lookupList, string stringIndex, string separator="|||"){
        List<string> previousStrings = lookupList.Where(
            learning => learning.StartsWith($"{stringIndex}{separator}")).ToList();
        if(previousStrings.Count == 0){
            return -1;
        }
        else{
            return lookupList.IndexOf(previousStrings[0]);
        }
    }

    protected internal void UpdateLearningList(string learningType, List<string> learningInfo){
        if(learningType == "Skill"){
            Skills = learningInfo;
        }
        else if(learningType == "Goal"){
            Goals = learningInfo;
        }
        else{
            Milestones = learningInfo;
        }
    }

    protected internal List<string> FormatAllAsList(Dictionary<string, Dictionary<string, string>> dictToFormat, List<string> headers){
        List<string> formattedList = new List<string>();
        foreach(KeyValuePair<string, Dictionary<string, string>> entry in dictToFormat){
            string indivValue = FormatAsList(entry.Key, entry.Value, headers);
            formattedList.Add(indivValue);
        }
        return formattedList;
    }

    protected internal string FormatStringForDBFilter(string filterValue){
        int i = 0;
        bool result = int.TryParse(filterValue, out i);
        if(result){
            return filterValue;
        }
        if(filterValue.Contains("'")){
            filterValue = filterValue.Replace("'", "''");
        }
        return $"'{filterValue}'";
    }
}