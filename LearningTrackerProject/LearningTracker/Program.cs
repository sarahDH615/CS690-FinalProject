namespace LearningTracker;
using System.IO;
using Spectre.Console;

class Program
{
    static void Main(string[] args)
    {
        UserUI userUi = new UserUI();
        userUi.EnterStartMenu();
    }
}

public class LearningsManager{
    
    protected List<string> headers = new List<string>{"ID", "Name", "Description", "Status"};
    
    DataManager dataManager = new DataManager();

    public Dictionary<string, string> GetCommonMetadataFieldsTextEntry(){
        Dictionary<string, string> metadataDictionary = new Dictionary<string, string>{
            {"Name", ""},
            {"Description", ""}
        };
        return metadataDictionary;
    }

    public Dictionary<string, List<string>> GetCommonMetadataFieldsSelection(){
        Dictionary<string, List<string>> metadataDictionary = new Dictionary<string, List<string>>{
            {"Status", new List<string>{"To-Do", "Completed"}}
        };
        return metadataDictionary;
    }

    public Dictionary<string, List<string>> GetLearningIdsAndNames(string learningType, string filterParentId = ""){
        Dictionary<string, List<string>> learningIdAndNamesDict = new Dictionary<string, List<string>>{};
        Dictionary<string, Dictionary<string, string>> learningDict = new Dictionary<string, Dictionary<string, string>>{};
        List<string> learningIDs = new List<string>();
        List<string> learningNames = new List<string>();
        if(learningType == "Skill"){
            learningDict = dataManager.FormatLearningsForScript(dataManager.Skills, dataManager.learningsHeadersDict[learningType]);
        }
        else if(learningType == "Goal"){
            learningDict = dataManager.FormatLearningsForScript(dataManager.Goals, dataManager.learningsHeadersDict[learningType]);
        }
        else if(learningType == "Milestone"){
            learningDict = dataManager.FormatLearningsForScript(dataManager.Milestones, dataManager.learningsHeadersDict[learningType]);
        }

        foreach( KeyValuePair<string, Dictionary<string, string>> kvp in learningDict ){
            string key = kvp.Key;
            Dictionary<string, string> value = kvp.Value;
            if(filterParentId != "" && learningType != "Skill"){

                if(value["parentID"] == filterParentId) {
                    learningIDs.Add(key);
                    learningNames.Add(value["Name"]);
                }
            }
            else{
                learningIDs.Add(key);
                learningNames.Add(value["Name"]);
            }

        }
        learningIdAndNamesDict["IDs"] = learningIDs;
        learningIdAndNamesDict["Names"] = learningNames;

        return learningIdAndNamesDict;        
    }

    public void SaveLearning(string learningType, Dictionary<string, string> learningMetadata){
        dataManager.SaveLearning(learningType, learningMetadata);
    }

}

public class UserUI{

    LearningsManager learningManager = new LearningsManager();
    NotesManager notesManager = new NotesManager();
    ProgressManager progressManager = new ProgressManager();

    public UserUI(){

    }


    public void EnterStartMenu(){
        string activity;  // warning CS8600: Converting null literal or possible null value to non-nullable type.

        do{
            List<string> activityTypes = new List<string>{"Progress", "Notes", "Manage Learning", "EXIT"};
            activity = ChooseFromSelection("Choose activity, or EXIT to exit LearningTracker:", activityTypes);
            
            try{
                // pass result of child menu back to allow for EXIT if passed
                if(activity == "Progress"){
                    activity = EnterProgressMenu();
                }
                else if(activity == "Notes"){
                    activity = EnterNotesMenu();
                }
                else if(activity == "Manage Learning"){
                    activity = EnterLearningsMenu();
                }
            }
            catch (NotImplementedException){
                Console.WriteLine("Accessing un-implemented method occurred. Exiting script.");
                activity = "EXIT";
            }
        }
        while(activity != "EXIT");
    }

    public string ChooseFromSelection(string title, List<string> selectionList){
        string response = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .PageSize(10)
                .MoreChoicesText("")
                .AddChoices(selectionList));
        return response;
    }

    public string GetTextFromUser(string fieldName){
        string response = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter {fieldName}: "));
        return response;
    }
    
    public string EnterProgressMenu(){
        throw new NotImplementedException();
    }

    public string EnterNotesMenu(){
        throw new NotImplementedException();
    }

    public string EnterLearningsMenu(){
        string action;

        do{
            List<string> actionTypes = new List<string>{"View", "Add", "Edit", "Delete", "BACK", "EXIT"};
            action = ChooseFromSelection("Choose action, or BACK to exit Manage Learning Menu, or EXIT to exit LearningTracker:", actionTypes);
            
            // pass result of child menu back to allow for EXIT if passed
            if(action == "View"){
                action = ViewLearning();
            }
            else if(action == "Add"){
                action = AddLearning();
            }
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(action != "EXIT" && action != "BACK"); 

        return action;
    }

    public string ViewLearning(){
        throw new NotImplementedException();
    }

    public string AddLearning(){

        string keepAdding;

        do{
            string learningType = ChooseFromSelection("Choose learning type:", new List<string>{"Skill", "Goal", "Milestone"});
            Dictionary<string, string> metadataDict = learningManager.GetCommonMetadataFieldsTextEntry();
            foreach(var field in metadataDict.Keys){
                string value = GetTextFromUser(field);
                metadataDict[field] = value;
            }
            Dictionary<string, List<string>> metadataDictSelection = learningManager.GetCommonMetadataFieldsSelection();
            foreach(var field in metadataDictSelection.Keys){
                string value = ChooseFromSelection($"Select {field}:", metadataDictSelection[field]);
                metadataDict[field] = value;
            }
            if(learningType == "Goal" || learningType == "Milestone"){
                string ancestorSkillId = LookupLearningByName("Skill");
                if(learningType == "Goal"){
                    metadataDict["parentID"] = ancestorSkillId;
                }
                else{
                    string ancestorGoalId = LookupLearningByName("Goal", ancestorSkillId);
                    metadataDict["parentID"] = ancestorGoalId;
                }
            }
            learningManager.SaveLearning(learningType, metadataDict);
            
            keepAdding = ChooseFromSelection("Select Add More to continue adding learnings, or BACK to exit Add Learning Menu, or EXIT to exit LearningTracker:", new List<string>{"Add More", "BACK", "EXIT"});
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(keepAdding != "EXIT" && keepAdding != "BACK"); 

        return keepAdding;
    }

    public string LookupLearningByName(string learningType, string filterId = ""){
        Dictionary<string, List<string>> learningInfo = learningManager.GetLearningIdsAndNames(learningType, filterId);
        if(learningInfo["Names"].Count == 0){
            Console.WriteLine($"No related {learningType}s");
            // TO DO: allow re-entry
            return "";
        }
        string ancestorName = ChooseFromSelection($"Choose {learningType} name:", learningInfo["Names"]);
        int index = learningInfo["Names"].IndexOf(ancestorName);
        string learningId = learningInfo["IDs"][index];
        return learningId; 
    }

    public string EditLearning(){
        throw new NotImplementedException();
    }

    public string DeleteLearning(){
        throw new NotImplementedException();
    }
}

public class NotesManager{}

public class ProgressManager{}

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

public class DataIO{

    public List<string> GetOrCreateFile(string fileName, List<string> headers, string separator = "|||"){
        if (!File.Exists(fileName))
        {
            // Create a file to write to
            using (StreamWriter sw = File.CreateText(fileName))
            {
                // add headers
                sw.WriteLine(string.Join(separator, headers));
            }
        }
        // open the file and return any data existing in it
        // open
        List<string> fileContents = new List<string>();
        using (StreamReader sr = File.OpenText(fileName))
        {
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                fileContents.Add(s);
            }
        }
        // remove header
        if(fileContents.Count > 0){
            fileContents.RemoveAt(0);
        }
        // // return data
        return fileContents;
    }

    public void SaveRecord(string fileName, string newEntry){
        File.AppendAllText(fileName, newEntry+"\n");
    }
}