namespace LearningTracker;

public class ProgressManager{

    protected internal DataManager dataManager = new DataManager();

    public List<string> GetProgressSummary(string filter){
        List<string> learningList = new List<string>();
        
        // eventual output: name : status
        // need ids to look up goals and milestones
        string filterName;
        if(filter == ""){
            filterName = "";
        }
        else{
            filterName = "Status";
        }
        Dictionary<string, Dictionary<string, string>> skills = dataManager.GetFilteredLearnings("Skill", new Dictionary<string, string>{{filterName, filter}});
        foreach( KeyValuePair<string, Dictionary<string, string>> kvp in skills ){
            string learningString = ""; 
            string skillId = kvp.Key;
            learningString+= $"{kvp.Value["Name"]}: {kvp.Value["Status"]}{Environment.NewLine}";
            Dictionary<string, Dictionary<string, string>> goals = dataManager.GetFilteredLearnings("Goal", new Dictionary<string, string>{{filterName, filter}, {"ParentID", skillId}});
            foreach( KeyValuePair<string, Dictionary<string, string>> pair in goals ){
                string goalId = pair.Key;
                learningString+= $"\t{pair.Value["Name"]}: {pair.Value["Status"]}{Environment.NewLine}";
                Dictionary<string, Dictionary<string, string>> milestones = dataManager.GetFilteredLearnings("Milestone", new Dictionary<string, string>{{filterName, filter}, {"ParentID", goalId}});
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