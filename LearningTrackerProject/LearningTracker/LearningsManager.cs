namespace LearningTracker;

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

    public Dictionary<string, List<string>> GetLearningIdsAndNames(string learningType, string filterParentId = ""){
        Dictionary<string, List<string>> learningIdAndNamesDict = new Dictionary<string, List<string>>{};
        // Dictionary<string, Dictionary<string, string>> learningDict = new Dictionary<string, Dictionary<string, string>>{};
        List<string> learningIDs = new List<string>();
        List<string> learningNames = new List<string>();
        // if(learningType == "Skill"){
        //     learningDict = dataManager.FormatForScript(dataManager.Skills, dataManager.headersDict[learningType]);
        // }
        // else if(learningType == "Goal"){
        //     learningDict = dataManager.FormatForScript(dataManager.Goals, dataManager.headersDict[learningType]);
        // }
        // else if(learningType == "Milestone"){
        //     learningDict = dataManager.FormatForScript(dataManager.Milestones, dataManager.headersDict[learningType]);
        // }
        Dictionary<string, Dictionary<string, string>> learningDict = dataManager.GetFilteredLearnings(learningType, "parentID", filterParentId);
        

        foreach( KeyValuePair<string, Dictionary<string, string>> kvp in learningDict ){
            string key = kvp.Key;
            Dictionary<string, string> value = kvp.Value;
            // if(filterParentId != "" && learningType != "Skill"){

            //     if(value["parentID"] == filterParentId) {
            //         learningIDs.Add(key);
            //         learningNames.Add(value["Name"]);
            //     }
            // }
            // else{
            //     learningIDs.Add(key);
            //     learningNames.Add(value["Name"]);
            // }
            learningIDs.Add(key);
            learningNames.Add(value["Name"]);

        }
        learningIdAndNamesDict["IDs"] = learningIDs;
        learningIdAndNamesDict["Names"] = learningNames;

        return learningIdAndNamesDict;        
    }

    public void SaveLearning(string learningType, Dictionary<string, string> learningMetadata){
        dataManager.SaveLearning(learningType, learningMetadata);
    }

}