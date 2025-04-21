namespace LearningTracker;
using System.Linq;

public class LearningsManager{
    
    protected List<string> headers = new List<string>{"ID", "Name", "Description", "Status"};
    
    protected internal DataManager dataManager = new DataManager();

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

    public List<Dictionary<string, Dictionary<string, string>>> GetDescendantLearnings(string learningId, string learningType){
        List<Dictionary<string, Dictionary<string, string>>> relatedLearnings = new List<Dictionary<string, Dictionary<string, string>>>{};
        Dictionary<string, Dictionary<string, string>> filteredGoals = dataManager.FilterByDictValue(
            dataManager.learningsDict["Goal"], "ParentID", learningId);
        relatedLearnings.Add(filteredGoals);
        
        if(learningType != "Goal"){
            List<string> goalIds = filteredGoals.Keys.ToList();
            if(goalIds.Count > 0){
                foreach(string goalId in goalIds){
                    Dictionary<string, Dictionary<string, string>> filteredMilestones = dataManager.FilterByDictValue(
                        dataManager.learningsDict["Milestone"], "ParentID", goalId);
                    relatedLearnings.Add(filteredMilestones);
                }
            }
        } 
        return relatedLearnings;   
    }

    public Dictionary<string, List<string>> GetLearningIdsAndNames(string learningType, string filterParentId = ""){
        Dictionary<string, List<string>> learningIdAndNamesDict = new Dictionary<string, List<string>>{};
        // Dictionary<string, Dictionary<string, string>> learningDict = new Dictionary<string, Dictionary<string, string>>{};
        List<string> learningIDs = new List<string>();
        List<string> learningNames = new List<string>();
        Dictionary<string, string> filterDict = dataManager.MakeFilterDict("ParentID", filterParentId);
        
        Dictionary<string, Dictionary<string, string>> learningDict = dataManager.GetFilteredLearnings(learningType, filterDict);
        
        foreach( KeyValuePair<string, Dictionary<string, string>> kvp in learningDict ){
            string key = kvp.Key;
            Dictionary<string, string> value = kvp.Value;
            learningIDs.Add(key);
            learningNames.Add(value["Name"]);

        }
        learningIdAndNamesDict["IDs"] = learningIDs;
        learningIdAndNamesDict["Names"] = learningNames;

        return learningIdAndNamesDict;        
    }

    public Dictionary<string, string> GetLearningByID(string learningType, string learningID){
        try{
            return dataManager.learningsDict[learningType][learningID];
        }
        catch{
            Console.WriteLine($"No {learningType} associated with ID {learningID}");
            return new Dictionary<string, string>{};
        }
    }

    public Dictionary<string, string> GetLearningNamesByParentID(string learningType, string learningID){
        // get relevant dict
        Dictionary<string, Dictionary<string, string>> lookupDict = dataManager.learningsDict[learningType];
        Dictionary<string, string> relatedLearnings = new Dictionary<string, string>{};
        
        foreach( KeyValuePair<string, Dictionary<string, string>> learning in lookupDict ){
            if(learning.Value["ParentID"] == learningID){
                relatedLearnings[learning.Key] = learning.Value["Name"];
            }
        }
        return relatedLearnings;
    }

    public Dictionary<string, List<string>> GetDescendantLearningNames(string learningType, string learningID){
        Dictionary<string, List<string>> descendants = new Dictionary<string, List<string>>{};
        if(learningType == "Skill"){
            Dictionary<string, string> intermediateLevel = GetLearningNamesByParentID("Goal", learningID);
            foreach(KeyValuePair<string, string> entry in intermediateLevel){
                Dictionary<string, string> final = GetLearningNamesByParentID("Milestone", entry.Key);
                descendants[entry.Value] = final.Values.ToList();
            }
        }
        else{
            Dictionary<string, string> final = GetLearningNamesByParentID("Milestone", learningID);
            foreach(KeyValuePair<string, string> entry in final){
                descendants[entry.Value] = new List<string>();
            }
        }
        return descendants;
    }

    public void SaveLearning(string learningType, Dictionary<string, string> learningMetadata){
        dataManager.SaveLearning(learningType, learningMetadata);
    }

    public void UpdateLearning(string learningType, string learningID, Dictionary<string, string> updatedContent){
        // Console.WriteLine($"{string.Join(", ", updatedContent.Keys)}: {string.Join(", ", updatedContent.Values)}");
        dataManager.UpdateLearning(learningType, learningID, updatedContent);
    }

    public void DeleteLearning(string learningId, string learningType){
        dataManager.DeleteLearning(learningId, learningType);
    }

}