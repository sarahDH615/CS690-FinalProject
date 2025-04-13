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
            // Dictionary<string, string> metadataDict = learningManager.GetCommonMetadataFieldsTextEntry();
            // foreach(var field in metadataDict.Keys){
            //     string value = GetTextFromUser(field);
            //     metadataDict[field] = value;
            // }
            Dictionary<string, string> metadataDict = GetMetadataFieldFromTextEntry(learningManager.GetCommonMetadataFieldsTextEntry());
            // Dictionary<string, List<string>> metadataDictSelection = learningManager.GetCommonMetadataFieldsSelection();
            // foreach(var field in metadataDictSelection.Keys){
            //     string value = ChooseFromSelection($"Select {field}:", metadataDictSelection[field]);
            //     metadataDict[field] = value;
            // }
            metadataDict = GetMetadataFieldFromSelection(metadataDict, learningManager.GetCommonMetadataFieldsSelection());
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
            
            keepAdding = ChooseFromSelection(
                "Select Add More to continue adding learnings, or BACK to exit Add Learning Menu, or EXIT to exit LearningTracker:", 
                new List<string>{"Add More", "BACK", "EXIT"});
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(keepAdding != "EXIT" && keepAdding != "BACK"); 

        return keepAdding;
    }

    Dictionary<string, string> GetMetadataFieldFromSelection(Dictionary<string, string> metadataDictionary, Dictionary<string, List<string>> sourceDictionary){
        foreach(var field in sourceDictionary.Keys){
            string value = ChooseFromSelection($"Select {field}:", sourceDictionary[field]);
            metadataDictionary[field] = value;
        }
        return metadataDictionary;
    }

    Dictionary<string, string> GetMetadataFieldFromTextEntry(Dictionary<string, string> metadataDictionary){
        foreach(var field in metadataDictionary.Keys){
            string value = GetTextFromUser(field);
            metadataDictionary[field] = value;
        }
        return metadataDictionary;
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
        // get the type of the learning
        string learningType = GetLearningType();
        // get the name of the learning
        string learningId = GetLearningInDialogue(learningType);
        List<string> expectedDescendantTypes = new List<string>();
        // get learnings related to that learning
        List<Dictionary<string, Dictionary<string, string>>> relatedLearnings = new List<Dictionary<string, Dictionary<string, string>>>();
        if(learningType != "Milestone"){
            relatedLearnings = learningManager.GetDescendantLearnings(learningId, learningType);
        }
        // get notes related to that learning
        Dictionary<string, Dictionary<string, string>> relatedNotes = notesManager.GetNotes(learningType, learningId);
        // if there are any, check before deleting
        // learnings
        if(relatedLearnings.Count > 0){
            if(learningType == "Skill"){
                expectedDescendantTypes.Add("Goal");
                expectedDescendantTypes.Add("Milestone");
            }
            else{
                expectedDescendantTypes.Add("Milestone");
            }
            Console.WriteLine($"Warning! Deleting learning would delete the following {expectedDescendantTypes[0]}s:");
            foreach(KeyValuePair<string, Dictionary<string, string>> entry in relatedLearnings[0]){
                Console.WriteLine($"\t{entry.Value["Name"]}");
            }
            if(relatedLearnings.Count > 1){
                Console.WriteLine($"Warning! Deleting learning would delete the following Milestones:");
                foreach(KeyValuePair<string, Dictionary<string, string>> entry in relatedLearnings[1]){
                    Console.WriteLine($"\t{entry.Value["Name"]}");
                }
            }
        }
        // notes
        if(relatedNotes.Keys.Count > 0){
            Console.WriteLine($"Warning! Deleting learning would delete the following Notes:");
            foreach(KeyValuePair<string, Dictionary<string, string>> entry in relatedNotes){
                Console.WriteLine($"\t{entry.Value["Name"]}");
            }
        }
        // confirm/deny deletes
        string nextAction = ChooseFromSelection($"Do you wish to proceed with deletion(s)?:", new List<string>{"Yes", "No"});
        if(nextAction == "Yes"){
            // delete the learning itself
            learningManager.DeleteLearning(learningId, learningType);
            // delete the related learnings
            if(relatedLearnings.Count > 0){
                foreach(KeyValuePair<string, Dictionary<string, string>> learning in relatedLearnings[0]){
                    learningManager.DeleteLearning(learning.Key, expectedDescendantTypes[0]);
                }
                if(relatedLearnings.Count > 1){
                    foreach(KeyValuePair<string, Dictionary<string, string>> learning in relatedLearnings[1]){
                        learningManager.DeleteLearning(learning.Key, expectedDescendantTypes[1]);
                    }
                }
            }
            // delete related notes
            if(relatedNotes.Keys.Count > 0){
                foreach(KeyValuePair<string, Dictionary<string, string>> note in relatedNotes){
                    notesManager.DeleteNote(note.Key);
                }
            }
        }
        

        return "";
    }

    public string GetLearningInDialogue(string learningType){
        string learningId = "";
        if(learningType != "Skill"){
            Console.WriteLine($"Required! Name of skill related to {learningType}.");
        }
        string skillId = LookupLearningByName("Skill");
        if(learningType == "Skill"){
            learningId = skillId;
        }
        else{
            if(learningType == "Milestone"){
                Console.WriteLine($"Required! Name of goal related to {learningType}.");
            }
            string goalId = LookupLearningByName("Goal", skillId);
            if(learningType == "Goal"){
                learningId = goalId;
            }
            else if(learningType == "Milestone" && goalId != ""){
                learningId = LookupLearningByName("Milestone", goalId);
            }
        }
        return learningId;
    }

    public string ViewNotes(){
        throw new NotImplementedException();
    }

    public string GetLearningType(){
        string learningType = ChooseFromSelection(
            "Select type of learning:", new List<string>{"Skill", "Goal", "Milestone"}
        );
        return learningType;
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
            Console.WriteLine("Note will be connected to a learning:");
            string connectedLearningType = GetLearningType();
            // get components for note id
            List<string> noteIdComponents = GetNoteIdComponents(connectedLearningType);
            // save
            notesManager.SaveNote(metadataDict, noteIdComponents);
            
            keepAdding = ChooseFromSelection("Select Add More to continue adding notes, or BACK to exit Add Notes Menu, or EXIT to exit LearningTracker:", new List<string>{"Add More", "BACK", "EXIT"});
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(keepAdding != "EXIT" && keepAdding != "BACK"); 

        return keepAdding;
    }

    public List<string> GetNoteIdComponents(string learningType){
        List<string> components = new List<string>();
        if(learningType != "Skill"){
            Console.WriteLine($"Required! Name of skill related to {learningType}.");
        }
        string skillId = LookupLearningByName("Skill");
        components.Add(skillId);
        if(learningType == "Goal" || learningType == "Milestone"){
            if(learningType == "Milestone"){
                Console.WriteLine($"Required! Name of goal related to {learningType}.");
            }
            string goalId = LookupLearningByName("Goal", skillId);
            components.Add(goalId);
            if(learningType == "Milestone" && goalId != ""){
                string milestoneId = LookupLearningByName("Milestone", goalId);
                components.Add(milestoneId);
            }
        }
        return components;
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
        string action;
        do{
            // get type of learning it's connected to
            action = ChooseFromSelection(
                "Choose learning type to update progress on, or BACK to exit Progress Summary Menu, or EXIT to exit LearningTracker:", 
                new List<string>{"Skill", "Goal", "Milestone", "BACK", "EXIT"});
            if(action != "EXIT" && action != "BACK"){
                // get list of those learnings
                string learningId = LookupLearningByName(action);
                // get learning record
                Dictionary<string, string> learningRecord = learningManager.GetLearningByID(action, learningId);
                // give option to change
                string newStatus = GetStatusForDB();
                // no need to actually change in the database if the status is already equal to the supposed update
                if(learningRecord["Status"] != newStatus){
                    learningRecord["Status"] = newStatus;
                    learningManager.UpdateLearning(action, learningId, learningRecord);
                }
            }
        }
        while(action != "EXIT" && action != "BACK"); 

        return action;
    }

    public string GetStatusForDB(){
        string status = ChooseFromSelection(
            "Choose new status:", 
            new List<string>{"Complete!", "Return to To-Do"});
        if(status == "Complete!"){
            return "Completed";
        }
        else{
            return "To-Do";
        }
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