namespace LearningTracker;

public class ProgressManager{

    protected internal DataManager dataManager = new DataManager();

    public List<string> GetProgressSummary(string filter){
        List<string> learningList = new List<string>();
        
        // eventual output: name : status
        // need ids to look up goals and milestones
        Dictionary<string, string> filterDict;
        if(filter == ""){
            filterDict = new Dictionary<string, string>{};
        }
        else{
            filterDict = new Dictionary<string, string>{{"Status", $"'{filter}'"}};
        }
        Dictionary<string, Dictionary<string, string>> skills = dataManager.GetFilteredLearnings("Skill", filterDict);
        foreach( KeyValuePair<string, Dictionary<string, string>> kvp in skills ){
            string learningString = ""; 
            string skillId = kvp.Key;
            learningString+= $"{kvp.Value["Name"]}: {kvp.Value["Status"]}{Environment.NewLine}";
            filterDict["ParentID"] = skillId; //  add to dict
            Dictionary<string, Dictionary<string, string>> goals = dataManager.GetFilteredLearnings("Goal", filterDict);
            foreach( KeyValuePair<string, Dictionary<string, string>> pair in goals ){
                string goalId = pair.Key;
                learningString+= $"\t{pair.Value["Name"]}: {pair.Value["Status"]}{Environment.NewLine}";
                filterDict["ParentID"] = goalId; //  replace val in dict
                Dictionary<string, Dictionary<string, string>> milestones = dataManager.GetFilteredLearnings("Milestone", filterDict);
                foreach( Dictionary<string, string> mpair in milestones.Values ){
                    learningString+= $"\t\t{mpair["Name"]}: {mpair["Status"]}{Environment.NewLine}";
                }
            }
            learningList.Add(learningString);
        }

        return learningList;
    }

    public List<List<string>> GetLearningsLevel(Dictionary<string, Dictionary<string, string>> learnings ){
        List<List<string>> levelList = new List<List<string>>();
        string learningsString = "";
        foreach( KeyValuePair<string, Dictionary<string, string>> kvp in learnings ){
            string id = kvp.Key;
            learningsString+= $"{kvp.Value["Name"]}: {kvp.Value["Status"]}{Environment.NewLine}";
            levelList.Add(new List<string>{id, learningsString});
        }
        return levelList;
    }
}