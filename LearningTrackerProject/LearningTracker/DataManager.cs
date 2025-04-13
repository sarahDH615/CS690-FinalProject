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
    DataIO dataIO = new DataIO();
    public List<string> Skills = new List<string>();
    public List<string> Goals = new List<string>();
    public List<string> Milestones = new List<string>();
    public List<string> Notes = new List<string>();

    public Dictionary<string, Dictionary<string, Dictionary<string, string>>> learningsDict = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>{}; 
    public Dictionary<string, Dictionary<string, string>> notesDict = new Dictionary<string, Dictionary<string, string>>{}; 

    public DataManager(){
        Skills = dataIO.GetOrCreateFile(filenameDict["Skill"], headersDict["Skill"]);
        Goals = dataIO.GetOrCreateFile(filenameDict["Goal"], headersDict["Goal"]);
        Milestones = dataIO.GetOrCreateFile(filenameDict["Milestone"], headersDict["Milestone"]);
        Notes = dataIO.GetOrCreateFile(filenameDict["Note"], headersDict["Note"]);

        learningsDict["Skill"] = FormatForScript(Skills, headersDict["Skill"]);
        learningsDict["Goal"] = FormatForScript(Goals, headersDict["Goal"]);
        learningsDict["Milestone"] = FormatForScript(Milestones, headersDict["Milestone"]);
        notesDict = FormatForScript(Notes, headersDict["Note"]);
    }

    public Dictionary<string, Dictionary<string, string>> GetLearning(string learningType){
        List<string> learningsList = dataIO.GetOrCreateFile(filenameDict[learningType], headersDict[learningType]);
        return FormatForScript(learningsList, headersDict[learningType]);
    }

    public string GenerateLearningID(string learningType){
        int count = 0;
        if(learningType == "Skill"){
            count = Skills.Count + 1;
        }
        else if(learningType == "Goal"){
            count = Goals.Count + 1;
        }
        else if(learningType == "Milestone"){
            count = Milestones.Count + 1;
        }
        return count.ToString();
    }

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

    public Dictionary<string, Dictionary<string, string>> FormatForScript(List<string> learnings, List<string> orderedHeaders){
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

    public void SaveLearning(string learningType, Dictionary<string, string> learningDict, string learningID=""){
        // create id for dict based on type
        if(learningID == ""){
            learningID = GenerateLearningID(learningType);
        }
        List<string> learningHeaders = headersDict[learningType];
        string learningFileName = filenameDict[learningType];
        
        string learningString = FormatForDataStore(learningID, learningDict, learningHeaders);
        // add to the learning list
        SaveOrUpdateLearningsList(learningType, learningString);
        // update dict
        learningsDict[learningType][learningID] = learningDict;

        // save back to data store
        dataIO.SaveRecord(learningFileName, learningString);
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
        // add to the learnings lists
        List<string> learningList = GetLearningTypeList(learningType);
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

    public int convertStringIdToInt(string stringId){
        int intOfString;
        bool successfulConversion = Int32.TryParse(stringId, out intOfString);
        if(successfulConversion){
            return intOfString;
        }
        else{
            return -1;
        }
    }

    public void SaveNote(Dictionary<string, string> noteMetadata, List<string> idComponents){
        // create id for dict based on type
        string noteId = CreateNoteID(idComponents);
        List<string> noteHeaders = headersDict["Note"];
        string noteFileName = filenameDict["Note"];
        
        string noteString = FormatForDataStore(noteId, noteMetadata, noteHeaders);
        // Console.WriteLine(noteString);
        // update the notes list
        Notes.Add(noteString);
        // save back to data store
        dataIO.SaveRecord(noteFileName, noteString);
    }

    public string FormatForDataStore(string id, Dictionary<string, string> recordDict, List<string> headers){
        
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

    public Dictionary<string, Dictionary<string, string>> GetFilteredLearnings(string learningType, string filterField="", string filter="", string filterByParent=""){
        Dictionary<string, Dictionary<string, string>> learningDict = new Dictionary<string, Dictionary<string, string>>{};
        // if(learningType == "Skill"){
        //     learningDict = FormatForScript(Skills, headersDict[learningType]);
        // }
        // else if(learningType == "Goal"){
        //     learningDict = FormatForScript(Goals, headersDict[learningType]);
        // }
        // else if(learningType == "Milestone"){
        //     learningDict = FormatForScript(Milestones, headersDict[learningType]);
        // }
        learningDict = learningsDict[learningType];
        if(filter == "" && filterByParent == ""){
            return learningDict;
        }
        else if(filterField == "" && filter == "" && filterByParent != ""){
            filterField = "parentID";
            filter = filterByParent;
            filterByParent = "";
        }
        Dictionary<string, Dictionary<string, string>> filteredDict = new Dictionary<string, Dictionary<string, string>>{};
        List<Dictionary<string, string>> learningDictValues = learningDict.Values.ToList();
        if(learningDictValues[0].Keys.Contains(filterField)){
            foreach( KeyValuePair<string, Dictionary<string, string>> kvp in learningDict ){
                string key = kvp.Key;
                Dictionary<string, string> value = kvp.Value;
                if(value[filterField] == filter){
                    if(filterByParent == ""){
                        filteredDict[key] = value;
                    }
                    else if(learningType != "Skill" && value["parentID"] == filterByParent){
                        filteredDict[key] = value;
                    }
                    
                }
            }
        }
        else{
            throw new ArgumentException($"{filterField} is not a valid filter for {learningType}");  
        }
        return filteredDict;
    }
    
    public void UpdateLearning(string learningType, string learningID, Dictionary<string, string> learningMetadata){
        // the ids should correspond to the place in the list
        // the list and dictionary can then be updated using that id
        // update list
        List<string> learningHeaders = headersDict[learningType];
        string learningString = FormatForDataStore(learningID, learningMetadata, learningHeaders);
        // update the learnings lists
        SaveOrUpdateLearningsList(learningType, learningString, learningID);
        // update dict
        learningsDict[learningType][learningID] = learningMetadata;

        // save back to data store
        dataIO.UpdateRecord(filenameDict[learningType], GetLearningTypeList(learningType));
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
        learningsDict[learningType] = FormatForScript(learningsList, headersDict[learningType]);
        // save to data store
        learningsList.Insert(0, $"{string.Join("|||", headersDict[learningType])}");
        dataIO.UpdateRecord(filenameDict[learningType], learningsList);
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
        List<string> notesWithHeader = Enumerable.Concat(
            new List<string>{$"{string.Join("|||", headersDict["Note"])}"}, 
            Notes).ToList();
        dataIO.UpdateRecord(filenameDict["Note"], notesWithHeader);
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
}