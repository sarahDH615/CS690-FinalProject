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

    public DataManager(){
        Skills = dataIO.GetOrCreateFile(filenameDict["Skill"], headersDict["Skill"]);
        Goals = dataIO.GetOrCreateFile(filenameDict["Goal"], headersDict["Goal"]);
        Milestones = dataIO.GetOrCreateFile(filenameDict["Milestone"], headersDict["Milestone"]);
        Notes = dataIO.GetOrCreateFile(filenameDict["Note"], headersDict["Note"]);
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

    public void SaveLearning(string learningType, Dictionary<string, string> learningDict){
        // create id for dict based on type
        string learningID = GenerateLearningID(learningType);
        List<string> learningHeaders = headersDict[learningType];
        string learningFileName = filenameDict[learningType];
        
        string learningString = FormatForDataStore(learningID, learningDict, learningHeaders);
        // update the learnings lists
        if(learningType == "Skill"){
            Skills.Add(learningString);
        }
        else if(learningType == "Goal"){
            Goals.Add(learningString);
        }
        else if(learningType == "Milestone"){
            Milestones.Add(learningString);
        }
        // save back to data store
        dataIO.SaveRecord(learningFileName, learningString);
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
    
}