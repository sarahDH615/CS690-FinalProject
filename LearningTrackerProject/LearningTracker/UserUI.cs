namespace LearningTracker;
using Spectre.Console;

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
        string action;

        do{
            List<string> actionTypes = new List<string>{"Progress Summary", "Update Progress", "BACK", "EXIT"};
            action = ChooseFromSelection("Choose action, or BACK to exit Progress Menu, or EXIT to exit LearningTracker:", actionTypes);
            
            // pass result of child menu back to allow for EXIT if passed
            if(action == "Progress Summary"){
                action = EnterProgressSummaryMenu();
            }
            else if(action == "Update Progress"){
                action = UpdateProgress();
            }
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(action != "EXIT" && action != "BACK"); 

        return action;
    }

    public string EnterNotesMenu(){
        string action;

        do{
            List<string> actionTypes = new List<string>{"View", "Add", "Edit", "Delete", "BACK", "EXIT"};
            action = ChooseFromSelection("Choose action, or BACK to exit Manage Notes Menu, or EXIT to exit LearningTracker:", actionTypes);
            
            // pass result of child menu back to allow for EXIT if passed
            if(action == "View"){
                action = ViewNotes();
            }
            else if(action == "Add"){
                action = AddNotes();
            }
            else if(action == "Edit"){
                action = EditNotes();
            }
            else if(action == "Delete"){
                action = DeleteNotes();
            }
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(action != "EXIT" && action != "BACK"); 

        return action;
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
            else if(action == "Edit"){
                action = EditLearning();
            }
            else if(action == "Delete"){
                action = DeleteLearning();
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

    public string ViewNotes(){
        throw new NotImplementedException();
    }

    public string AddNotes(){
        string keepAdding;

        do{
            // enter note values
            Dictionary<string, string> metadataDict = notesManager.GetCommonMetadataFieldsTextEntry();
            foreach(var field in metadataDict.Keys){
                string value = GetTextFromUser(field);
                metadataDict[field] = value;
            }
            // choose learning type to connect the note to
            string connectedLearningType = ChooseFromSelection(
                "Select type of learning to connect the note to:", new List<string>{"Skill", "Goal", "Milestone"}
            );
            // get components for note id
            List<string> noteIdComponents = new List<string>();
            if(connectedLearningType != "Skill"){
                Console.WriteLine($"Required! Name of skill related to {connectedLearningType}.");
            }
            string skillId = LookupLearningByName("Skill");
            noteIdComponents.Add(skillId);
            if(connectedLearningType == "Goal" || connectedLearningType == "Milestone"){
                if(connectedLearningType == "Milestone"){
                    Console.WriteLine($"Required! Name of goal related to {connectedLearningType}.");
                }
                string goalId = LookupLearningByName("Goal", skillId);
                noteIdComponents.Add(goalId);
                if(connectedLearningType == "Milestone" && goalId != ""){
                    string milestoneId = LookupLearningByName("Milestone", goalId);
                    noteIdComponents.Add(milestoneId);
                }
            }
            // Console.WriteLine(string.Join(", ", metadataDict.Keys));
            // Console.WriteLine(string.Join(", ", metadataDict.Values));
            // save
            notesManager.SaveNote(metadataDict, noteIdComponents);
            
            keepAdding = ChooseFromSelection("Select Add More to continue adding notes, or BACK to exit Add Notes Menu, or EXIT to exit LearningTracker:", new List<string>{"Add More", "BACK", "EXIT"});
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(keepAdding != "EXIT" && keepAdding != "BACK"); 

        return keepAdding;
    }

    public string EditNotes(){
        throw new NotImplementedException();
    }

    public string DeleteNotes(){
        throw new NotImplementedException();
    }

    public string EnterProgressSummaryMenu(){
        string viewMoreSummaries;

        do{
            List<string> actionTypes = new List<string>{"View all", "View Completed", "View To-Do", "BACK", "EXIT"};
            viewMoreSummaries = ChooseFromSelection("Choose what progress summary you'd like to view, or BACK to exit Progress Summary Menu, or EXIT to exit LearningTracker:", actionTypes);
            
            if(viewMoreSummaries == "View all"){
                ViewSummaries();
            }
            else if(viewMoreSummaries == "View Completed"){
                ViewSummaries("Completed");
            }
            else if(viewMoreSummaries == "View To-Do"){
                ViewSummaries("To-Do");
            }
            string viewMore = ChooseFromSelection("View other summaries?", new List<string>{"Yes", "No"});
            if(viewMore == "No"){
                viewMoreSummaries = "BACK";
            }
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(viewMoreSummaries != "EXIT" && viewMoreSummaries != "BACK"); 

        return viewMoreSummaries;
    }

    public string UpdateProgress(){
        // string action;

        // do{
        //     List<string> actionTypes = new List<string>{"View all", "View Completed", "View To-Do", "BACK", "EXIT"};
        //     action = ChooseFromSelection("Choose what progress summary you'd like to view, or BACK to exit Progress Summary Menu, or EXIT to exit LearningTracker:", actionTypes);
            
        //     // pass result of child menu back to allow for EXIT if passed
        //     if(action == "View all"){
        //         action = EnterProgressSummaryMenu();
        //     }
        //     else if(action == "View Completed"){
        //         action = EnterProgressSummaryMenu();
        //     }
        //     else if(action == "View To-Do"){
        //         action = EnterProgressSummaryMenu();
        //     }
        // }
        // // BACK will go back to the previous while loop; EXIT will exit the script entirely
        // while(action != "EXIT" && action != "BACK"); 

        // return action;
        throw new NotImplementedException();
    }

    // public string ViewSummaries(string filter=""){
    public void ViewSummaries(string filter=""){
        // string viewMoreSummaries;
        List<string> summaries = progressManager.GetProgressSummary(filter);
        if(summaries.Count == 0 && filter != ""){
            Console.WriteLine($"No summaries with status {filter}");
        }
        foreach(string summary in summaries){
            Console.WriteLine(summary);
        }
    }
}