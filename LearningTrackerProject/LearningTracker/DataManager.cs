namespace LearningTracker;

public class DataManager{
    public Dictionary<string, string> filenameDict = new Dictionary<string, string>{
        {"Skill", "Skills.txt"},
        {"Goal", "Goals.txt"},
        {"Milestone", "Milestones.txt"},
        {"Note", "Notes.txt"}
    };
    public Dictionary<string, List<string>> headersDict = new Dictionary<string, List<string>>{
        {"Skill", new List<string>{"ID", "Name", "Description", "Status"}},
        {"Goal", new List<string>{"ID", "Name", "Description", "Status", "parentID"}},
        {"Milestone", new List<string>{"ID", "Name", "Description", "Status", "parentID"}},
        {"Note", new List<string>{"ID", "Name", "Description", "Body"}}
    };
    DataIOSQL dataIO = new DataIOSQL();
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
        Notes = FormatAllAsList(learningsDict["Note"], headersDict["Note"]);

        
    }

    // public Dictionary<string, Dictionary<string, string>> GetLearning(string learningType){
    //     List<string> learningsList = dataIO.GetOrCreateFile(filenameDict[learningType], headersDict[learningType]);
    //     return FormatAsDict(learningsList, headersDict[learningType]);
    // }

    // public string GenerateLearningID(string learningType){
    //     int count = 0;
    //     if(learningType == "Skill"){
    //         count = Skills.Count + 1;
    //     }
    //     else if(learningType == "Goal"){
    //         count = Goals.Count + 1;
    //     }
    //     else if(learningType == "Milestone"){
    //         count = Milestones.Count + 1;
    //     }
    //     return count.ToString();
    // }

    public string CreateNoteID(List<string>idComponents, int componentLength=3){
        List<string> components = new List<string>();
        for(int segment = 0; segment < componentLength; segment++){
            if(idComponents.Count > segment){
                components.Add(idComponents[segment]);
            }
            else{
                components.Add("xx");
            }
        }
        return string.Join("-", components);
    }

    public Dictionary<string, Dictionary<string, string>> FormatAsDict(List<string> learnings, List<string> orderedHeaders){
        Dictionary<string, Dictionary<string, string>> learningsDict = new Dictionary<string, Dictionary<string, string>>{};
        foreach(string learning in learnings){
            List<string> learningSplit = learning.Split("|||").ToList();
            string learningID = learningSplit[0];
            Dictionary<string, string> innerDict = new Dictionary<string, string>{};
            if(learningSplit.Count == orderedHeaders.Count){
                for (int index = 1; index < learningSplit.Count; index++){ // skipping the first index
                    innerDict[orderedHeaders[index]] = learningSplit[index];
                }
            }
            learningsDict[learningID] = innerDict;
        }
        return learningsDict;
    }

    public void SaveLearning(string learningType, Dictionary<string, string> learningDict){
        dataIO.AddToDB(learningType, learningDict);
        string learningID = dataIO.GetMostRecentValueFromTable(learningType);
        // update the list
        SaveOrUpdateLearningsList(learningType, FormatAsList(learningID, learningDict, headersDict[learningType]));
        // update dict
        learningsDict[learningType][learningID] = learningDict;
    }

    public List<string> GetLearningTypeList(string learningType){
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
    
    public void SaveOrUpdateLearningsList(string learningType, string learningString, string index=""){
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

    public void SaveOrUpdateNotesList(string noteString, string index=""){
        if(index != ""){
            int indexInt = GetIntegerIndexFromIdentifier(Notes, index);
            if(indexInt > -1){
                Notes[indexInt] = noteString;
            }
        }
        else{
            Notes.Add(noteString);
        }
        
    }

    public int GetIntegerIndexFromIdentifier(List<string> lookupList, string stringIndex, string separator="|||"){
        List<string> previousStrings = lookupList.Where(
            learning => learning.StartsWith($"{stringIndex}{separator}")).ToList();
        if(previousStrings.Count == 0){
            return -1;
        }
        else{
            return lookupList.IndexOf(previousStrings[0]);
        }
    }

    public void SaveNote(Dictionary<string, string> noteMetadata, List<string> idComponents){
        // create id for dict based on type
        string code = CreateNoteID(idComponents);
        noteMetadata["ConnectedLearningCode"] = code;
        List<string> noteHeaders = headersDict["Note"];

        dataIO.AddToDB("Note", noteMetadata);
        string noteId = dataIO.GetMostRecentValueFromTable("notes");
        
        string noteString = FormatAsList(noteId, noteMetadata, noteHeaders);
        // update the notes list
        SaveOrUpdateNotesList(noteString);
        // update the notes dict
        notesDict[noteId] = noteMetadata;
    }

    public string FormatAsList(string id, Dictionary<string, string> recordDict, List<string> headers){
        
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

    public Dictionary<string, Dictionary<string, string>> GetFilteredLearnings(string learningType, Dictionary<string, string> filters){
        Dictionary<string, Dictionary<string, string>> learningDict = new Dictionary<string, Dictionary<string, string>>{};
        learningDict = learningsDict[learningType];
        if(filters.Keys.Count == 0){
            return learningDict;
        }
        else{
            return dataIO.GetFilteredDBResults(learningType, filters);
        }
    }
    
    public void UpdateLearning(string learningType, string learningID, Dictionary<string, string> learningMetadata){
        // the list and dictionary can then be updated using that id
        // update list
        List<string> learningHeaders = headersDict[learningType];
        string learningString = FormatAsList(learningID, learningMetadata, learningHeaders);
        // update the learnings lists
        SaveOrUpdateLearningsList(learningType, learningID);
        // update dict
        learningsDict[learningType][learningID] = learningMetadata;
        // save back to data store
        dataIO.UpdateRecord(learningID, learningMetadata, learningType);
    }

    public void UpdateNote(string noteID, Dictionary<string, string> noteMetadata){
        // update list
        List<string> headers = headersDict["Note"];
        string noteString = FormatAsList(noteID, noteMetadata, headers);
        // update the learnings lists
        SaveOrUpdateNotesList(noteString, noteID);
        // update dict
        notesDict[noteID] = noteMetadata;
        // save back to data store
        dataIO.UpdateRecord(noteID, noteMetadata, "Note");
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

    public void UpdateLearningList(string learningType, List<string> learningInfo){
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

    public List<string> FormatAllAsList(Dictionary<string, Dictionary<string, string>> dictToFormat, List<string> headers){
        List<string> formattedList = new List<string>();
        foreach(KeyValuePair<string, Dictionary<string, string>> entry in dictToFormat){
            string indivValue = FormatAsList(entry.Key, entry.Value, headers);
            formattedList.Add(indivValue);
        }
        return formattedList;
    }
}