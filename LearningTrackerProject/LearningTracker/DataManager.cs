namespace LearningTracker;

public class DataManager{
    public Dictionary<string, string> learningsFilenameDict = new Dictionary<string, string>{
        {"Skill", "Skills.txt"},
        {"Goal", "Goals.txt"},
        {"Milestone", "Milestones.txt"}
    };
    public Dictionary<string, List<string>> learningsHeadersDict = new Dictionary<string, List<string>>{
        {"Skill", new List<string>{"ID", "Name", "Description", "Status"}},
        {"Goal", new List<string>{"ID", "Name", "Description", "Status", "parentID"}},
        {"Milestone", new List<string>{"ID", "Name", "Description", "Status", "parentID"}}
    };
    DataIO dataIO = new DataIO();
    public List<string> Skills = new List<string>();
    public List<string> Goals = new List<string>();
    public List<string> Milestones = new List<string>();

    public DataManager(){
        Skills = dataIO.GetOrCreateFile(learningsFilenameDict["Skill"], learningsHeadersDict["Skill"]);
        Goals = dataIO.GetOrCreateFile(learningsFilenameDict["Goal"], learningsHeadersDict["Goal"]);
        Milestones = dataIO.GetOrCreateFile(learningsFilenameDict["Milestone"], learningsHeadersDict["Milestone"]);
    }

    public Dictionary<string, Dictionary<string, string>> GetLearning(string learningType){
        List<string> learningsList = dataIO.GetOrCreateFile(learningsFilenameDict[learningType], learningsHeadersDict[learningType]);
        return FormatLearningsForScript(learningsList, learningsHeadersDict[learningType]);
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

    public Dictionary<string, Dictionary<string, string>> FormatLearningsForScript(List<string> learnings, List<string> orderedHeaders){
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
        List<string> learningHeaders = learningsHeadersDict[learningType];
        string learningFileName = learningsFilenameDict[learningType];
        
        string learningString = FormatLearningForDataStore(learningID, learningDict, learningHeaders);
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

    public string FormatLearningForDataStore(string id, Dictionary<string, string> recordDict, List<string> headers){
        
        List<string> recordList = new List<string>();
        foreach(string header in headers){ 
            if(header == "ID"){
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