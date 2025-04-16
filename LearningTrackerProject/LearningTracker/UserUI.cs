namespace LearningTracker;
using Spectre.Console;

public class UserUI{

    LearningsManager learningManager = new LearningsManager();
    NotesManager notesManager = new NotesManager();
    ProgressManager progressManager = new ProgressManager();

    List<string> menuHistory = new List<string>();

    public UserUI(){
        menuHistory.Add("Start Menu");
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
                ResetToStartMenuAndDisplay();
            }
            catch (NotImplementedException){
                AnsiConsole.Write(new Markup($"[red]Accessing un-implemented method occurred. Exiting script.{Environment.NewLine}[/]"));
                activity = "EXIT";
            }
        }
        while(activity != "EXIT");
    }
    // ---- FIRST LEVEL MENUS ---- //
    public string EnterProgressMenu(){
        string action;
        AddToAndDisplayMenuHistory("Progress Menu");
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
            OnlyDisplayMenuHistory();
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(action != "EXIT" && action != "BACK"); 
        menuHistory.RemoveAt(menuHistory.Count-1);

        return action;
    }

    public string EnterNotesMenu(){
        string action;
        AddToAndDisplayMenuHistory("Notes Menu");
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
            OnlyDisplayMenuHistory();
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(action != "EXIT" && action != "BACK"); 
        menuHistory.RemoveAt(menuHistory.Count-1);

        return action;
    }

    public string EnterLearningsMenu(){
        string action;
        AddToAndDisplayMenuHistory("Learnings Menu");
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
            OnlyDisplayMenuHistory();
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(action != "EXIT" && action != "BACK"); 
        menuHistory.RemoveAt(menuHistory.Count-1);

        return action;
    }
    // ---- SECOND LEVEL MENUS ---- //
    // ---- PROGRESS ---- //
    public string EnterProgressSummaryMenu(){
        string viewMoreSummaries;
        AddToAndDisplayMenuHistory("Progress Summary Menu");
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
            OnlyDisplayMenuHistory();
            string viewMore = ChooseFromSelection("View other summaries?", new List<string>{"Yes", "No"});
            if(viewMore == "No"){
                viewMoreSummaries = "BACK";
            }
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(viewMoreSummaries != "EXIT" && viewMoreSummaries != "BACK"); 
        menuHistory.RemoveAt(menuHistory.Count-1);

        return viewMoreSummaries;
    }

    public string UpdateProgress(){
        string action;
        AddToAndDisplayMenuHistory("Update Progress Menu");
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
        menuHistory.RemoveAt(menuHistory.Count-1);

        return action;
    }
    // ---- NOTES ---- //
    public string ViewNotes(){
        string keepViewing;
        AddToAndDisplayMenuHistory("View Notes Menu");
        do{
            string learningType = ChooseFromSelection("Choose learning type:", new List<string>{"Skill", "Goal", "Milestone"});
            string learningId = GetLearningInDialogue(learningType);
            // show notes related to that learning
            Dictionary<string, Dictionary<string, string>> notesInfo = notesManager.GetNotes(learningType, learningId);
            if(notesInfo.Keys.Count == 0){
                AnsiConsole.Write(new Markup($"[yellow]No notes associated with this {learningType}.{Environment.NewLine}[/]"));
            }
            else{
                foreach(KeyValuePair<string, Dictionary<string, string>> entry in notesInfo){
                    Console.WriteLine();
                    var rule = new Rule($"[dim]Note ID: {entry.Key}[/]");
                    rule.Justification = Justify.Center;
                    AnsiConsole.Write(rule);
                    AnsiConsole.Write(new Markup($"[bold underline]Note ID[/]: {entry.Key}{Environment.NewLine}"));
                    foreach(KeyValuePair<string, string> subentry in entry.Value){
                        if(subentry.Key == "Body"){
                            AnsiConsole.Write(new Markup($"[bold]Body[/]:{Environment.NewLine}"));
                            var panel = new Panel(subentry.Value);
                            panel.Border = BoxBorder.Rounded;
                            panel.Padding = new Padding(2, 2, 2, 2);
                            AnsiConsole.Write(panel);
                        }
                        else{
                            AnsiConsole.Write(new Markup($"[bold]{subentry.Key}[/]: {subentry.Value}{Environment.NewLine}"));
                        }
                    }
                    Console.WriteLine();
                }
            }
            
            keepViewing = ChooseFromSelection(
                "Select View More to continue viewing notes, or BACK to exit View Notes Menu, or EXIT to exit LearningTracker:", 
                new List<string>{"View More", "BACK", "EXIT"});
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(keepViewing != "EXIT" && keepViewing != "BACK"); 
        menuHistory.RemoveAt(menuHistory.Count-1);

        return keepViewing;
    }

    public string AddNotes(){
        string keepAdding;
        AddToAndDisplayMenuHistory("Add Notes Menu");
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
        menuHistory.RemoveAt(menuHistory.Count-1);

        return keepAdding;
    }

    public string EditNotes(){
        string editMoreNotes = "";
        AddToAndDisplayMenuHistory("Edit Notes Menu");
        do{
            // get note
            // get all notes 
            List<List<string>> notesStrings = new List<List<string>>();
            Dictionary<string, Dictionary<string, string>> notesInfo = notesManager.GetNotes();
            foreach(KeyValuePair<string, Dictionary<string, string>> entry in notesInfo){
                notesStrings.Add(new List<string>{entry.Key, CreateNoteString(entry.Key, entry.Value["Name"])});
            }
            List<string> noteNames = notesStrings.Select(noteString => noteString[1]).ToList();
            List<string> noteIds = notesStrings.Select(noteString => noteString[0]).ToList();
            string chosenNoteName = ChooseFromSelection(
                "Choose what note field you'd like to edit, or BACK to exit Edit Notes Menu, or EXIT to exit LearningTracker:", 
                noteNames);
            string chosenNoteId = noteIds[noteNames.IndexOf(chosenNoteName)];
            Dictionary<string, string> note = notesManager.GetNoteByID(chosenNoteId);
            // choose which field to edit
            string editField = ChooseFromSelection("Choose what field to edit:", note.Keys.ToList());
            // overwrite or add on
            string editKind = ChooseFromSelection("Do you want to add on to or overwrite the field?", new List<string>{"Add", "Overwrite"});
            AnsiConsole.Write(new Markup($"[dim]Existing field content: {note[editField]}{Environment.NewLine}[/]"));
            string newContent = GetTextFromUser(editField);
            if(editKind == "Overwrite"){
                note[editField] = newContent;
            }
            else {
                note[editField]+=newContent;
            }
            AnsiConsole.Write(new Markup($"[dim]Updated {editField}: {note[editField]}{Environment.NewLine}[/]"));
            // save back to data store
            notesManager.UpdateNote(chosenNoteId, note);
            
            string editMore = ChooseFromSelection("Edit other notes?", new List<string>{"Yes", "No"});
            if(editMore == "No"){
                editMoreNotes = "BACK";
            }
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(editMoreNotes != "EXIT" && editMoreNotes != "BACK"); 
        menuHistory.RemoveAt(menuHistory.Count-1);

        return editMoreNotes;
    }

    public string DeleteNotes(){
        // find relevant note
        string learningType = ChooseFromSelection("Choose learning type:", new List<string>{"Skill", "Goal", "Milestone"});
        string learningId = GetLearningInDialogue(learningType);
        // show notes related to that learning
        Dictionary<string, Dictionary<string, string>> notesInfo = notesManager.GetNotes(learningType, learningId);
        List<(string ID, string Name)> notesInfoSubset = notesInfo.Select(noteInfo => (noteInfo.Key, noteInfo.Value["Name"])).ToList();
        List<string> noteIds = notesInfoSubset.Select(infoSubset => infoSubset.ID).ToList();
        List<string> noteNames = notesInfoSubset.Select(infoSubset => infoSubset.Name).ToList();
        if(noteNames.Count == 0){
            AnsiConsole.Write(new Markup($"[yellow]No notes relate to that {learningType}{Environment.NewLine}[/]"));
        }
        else{
            string selectedNoteName = ChooseFromSelection(
                "Choose existing note to delete:", noteNames);
            string selectedNoteId = noteIds[noteNames.IndexOf(selectedNoteName)];
            string nextAction = ChooseFromSelection($"[red bold]Do you wish to proceed with deletion(s)?:[/]", new List<string>{"Yes", "No"});
            if(nextAction == "Yes"){
                // send to delete function
                notesManager.DeleteNote(selectedNoteId);
            }
        }
        
        return "";
    }
    // ---- LEARNINGS ---- //
    public string ViewLearning(){
        string keepViewing;
        AddToAndDisplayMenuHistory("View Learnings Menu");
        do{
            string learningType = ChooseFromSelection("Choose learning type:", new List<string>{"Skill", "Goal", "Milestone"});
            string learningId = GetLearningInDialogue(learningType);
            if(learningId != ""){
                Console.WriteLine($"{learningType} ID: {learningId}");
                Dictionary<string, string> learningInfo = learningManager.GetLearningByID(learningType, learningId);
                foreach(KeyValuePair<string, string> entry in learningInfo){
                    Console.WriteLine($"{entry.Key}: {entry.Value}");
                }
            }
            else{
                AnsiConsole.Write(new Markup($"[yellow]Unable to view {learningType}[/]"));
            }
            keepViewing = ChooseFromSelection(
                "Select View More to continue viewing learnings, or BACK to exit View Learning Menu, or EXIT to exit LearningTracker:", 
                new List<string>{"View More", "BACK", "EXIT"});
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(keepViewing != "EXIT" && keepViewing != "BACK");
        menuHistory.RemoveAt(menuHistory.Count-1); 

        return keepViewing;
    }

    public string AddLearning(){

        string keepAdding;
        AddToAndDisplayMenuHistory("Add Learning Menu");
        do{
            string learningType = ChooseFromSelection("Choose learning type:", new List<string>{"Skill", "Goal", "Milestone"});
            Dictionary<string, string> metadataDict = GetMetadataFieldFromTextEntry(learningManager.GetCommonMetadataFieldsTextEntry());
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
        menuHistory.RemoveAt(menuHistory.Count-1);

        return keepAdding;
    }

    public string EditLearning(){
        string editMoreLearnings = "";
        AddToAndDisplayMenuHistory("Edit Learning Menu");
        do{
            // get learnings
            string learningType = GetLearningType();
            // get the name of the learning
            string learningId = GetLearningInDialogue(learningType);
            if(learningId != ""){
                Dictionary<string, string> learning = learningManager.GetLearningByID(learningType, learningId);
                // choose which field to edit
                string editField = ChooseFromSelection("Choose what field to edit:", learning.Keys.ToList());
                Console.WriteLine($"Existing field content: {learning[editField]}"); // display existing to help them know whether they want to add/overwrite
                // overwrite or add on
                string editKind = ChooseFromSelection("Do you want to add on to or overwrite the field?", new List<string>{"Add", "Overwrite"});
                string newContent = GetTextFromUser(editField);
                if(editKind == "Overwrite"){
                    learning[editField] = newContent;
                }
                else {
                    learning[editField]+=newContent;
                }
                Console.WriteLine($"Updated {editField}: {learning[editField]}");
                // save back to data store
                learningManager.UpdateLearning(learningType, learningId, learning);
            }
            else{
                Console.WriteLine($"Unable to edit {learningType} as it could not be found.");
            }
            string editMore = ChooseFromSelection("Edit other learnings?", new List<string>{"Yes", "No"});
            if(editMore == "No"){
                menuHistory.RemoveAt(menuHistory.Count-1);
                editMoreLearnings = "BACK";
            }
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(editMoreLearnings != "EXIT" && editMoreLearnings != "BACK"); 
        menuHistory.RemoveAt(menuHistory.Count-1);

        return editMoreLearnings;
    }

    public string DeleteLearning(){
        AddToAndDisplayMenuHistory("Delete Learning Menu");
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
            AnsiConsole.Write(
                new Markup($"[red bold]Warning![/][red] Deleting learning would delete the following {expectedDescendantTypes[0]}s:{Environment.NewLine}[/]"));
            foreach(KeyValuePair<string, Dictionary<string, string>> entry in relatedLearnings[0]){
                Console.WriteLine($"\t{entry.Value["Name"]}");
            }
            if(relatedLearnings.Count > 1){
                AnsiConsole.Write(
                    new Markup($"[red bold]Warning![/][red] Deleting learning would delete the following Milestones:{Environment.NewLine}[/]"));
                foreach(KeyValuePair<string, Dictionary<string, string>> entry in relatedLearnings[1]){
                    Console.WriteLine($"\t{entry.Value["Name"]}");
                }
            }
        }
        // notes
        if(relatedNotes.Keys.Count > 0){
            AnsiConsole.Write(
                new Markup($"[red bold]Warning![/][red] Deleting learning would delete the following Notes:{Environment.NewLine}[/]"));
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
        menuHistory.RemoveAt(menuHistory.Count-1);

        return "";
    }

    // ---- ADDITIONAL LEVEL MENUS ---- //
    public void ViewSummaries(string filter=""){
        if(filter == ""){
            AddToAndDisplayMenuHistory("Update Progress Menu: View All");
        }
        else{
            AddToAndDisplayMenuHistory("Update Progress Menu: View {filter}");
        }
        List<string> summaries = progressManager.GetProgressSummary(filter);
        if(summaries.Count == 0 && filter != ""){
            Console.WriteLine($"No summaries with status {filter}");
        }
        foreach(string summary in summaries){
            // Console.WriteLine(summary);
            Tree summaryTree = MakeProgressTree(summary);
            AnsiConsole.Write(summaryTree);
        }
        menuHistory.RemoveAt(menuHistory.Count-1);
    }

    // ---- HELPER FUNCTIONS BELOW ---- //
    public Tree MakeProgressTree(string progressSummary){
        List<string> summarySplit = progressSummary.Split(Environment.NewLine).ToList();
        var root = new Tree(FormatProgressStatusText(summarySplit[0]));
        List<TreeNode> topLevelNodesList = new List<TreeNode>();
        // TreeNode nearestNode;
        for(int i=1; i < summarySplit.Count; i++){
            string progressLine = summarySplit[i];
            string nodeText = FormatProgressStatusText(progressLine);
            if(progressLine.StartsWith("\t") && !progressLine.StartsWith("\t\t")){
                TreeNode newNode = root.AddNode(nodeText);
                if(topLevelNodesList.Count == 0){
                    topLevelNodesList.Add(newNode);
                }
                else{
                    topLevelNodesList[0] = newNode;
                }
            }
            else{
                if(topLevelNodesList.Count > 0){
                    topLevelNodesList[0].AddNode(nodeText);
                }
            }
        }
        return root;
    }

    string FormatProgressStatusText(string progress){
        progress = progress.Replace("\t", "");
        progress = progress.Replace("To-Do", "[orange3]To-Do[/]");
        progress = progress.Replace("Completed", "[green]Completed[/]");
        return progress;
    }

    public string ChooseFromSelection(string title, List<string> selectionList){
        string response = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .PageSize(10)
                .MoreChoicesText("")
                .AddChoices(selectionList));
        AnsiConsole.Write(new Markup($"[bold dim]{title}[/][dim] {response}[/]"));
        Console.WriteLine();
        return response;
    }

    public string GetTextFromUser(string fieldName){
        string promptString = $"Enter {fieldName}: ";
        string response = AnsiConsole.Prompt(
                new TextPrompt<string>(promptString));
        AnsiConsole.Write(new Markup($"[bold dim]{promptString}[/][dim]: {response}[/]"));
        Console.WriteLine();
        return response;
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

    public void AddToAndDisplayMenuHistory(string currentMenuName){
        menuHistory.Add(currentMenuName);
        OnlyDisplayMenuHistory();
    }

    public void OnlyDisplayMenuHistory(){
        AnsiConsole.Write(new Markup($"[dim]You are here: {string.Join(">", menuHistory)}{Environment.NewLine}[/]"));
    }

    public void ResetToStartMenuAndDisplay(){
        menuHistory.Clear();
        menuHistory.Add("Start Menu");
        OnlyDisplayMenuHistory();
    }

    public string LookupLearningByName(string learningType, string filterId = ""){
        Dictionary<string, List<string>> learningInfo = learningManager.GetLearningIdsAndNames(learningType, filterId);
        if(learningInfo["Names"].Count == 0){
            AnsiConsole.Write(new Markup($"[yellow]No related {learningType}s{Environment.NewLine}[/]"));
            // TO DO: allow re-entry
            return "";
        }
        string ancestorName = ChooseFromSelection($"Choose {learningType} name:", learningInfo["Names"]);
        int index = learningInfo["Names"].IndexOf(ancestorName);
        string learningId = learningInfo["IDs"][index];
        return learningId; 
    }

    public string GetLearningInDialogue(string learningType){
        string learningId = "";
        if(learningType != "Skill"){
            AnsiConsole.Write(new Markup($"[bold red]Required! [/][bold]Name of skill related to {learningType}.[/]{Environment.NewLine}"));
        }
        string skillId = LookupLearningByName("Skill");
        if(learningType == "Skill"){
            learningId = skillId;
        }
        else{
            if(learningType == "Milestone"){
                AnsiConsole.Write(new Markup($"[bold red]Required! [/][bold]Name of goal related to {learningType}.[/]{Environment.NewLine}"));
            }
            string goalId = LookupLearningByName("Goal", skillId);
            if(learningType == "Goal" && skillId != ""){
                learningId = goalId;
            }
            else if(learningType == "Milestone" && goalId != ""){
                learningId = LookupLearningByName("Milestone", goalId);
            }
        }
        return learningId;
    }

    public string GetLearningType(){
        string learningType = ChooseFromSelection(
            "Select type of learning:", new List<string>{"Skill", "Goal", "Milestone"}
        );
        return learningType;
    }

    public List<string> GetNoteIdComponents(string learningType){
        List<string> components = new List<string>();
        if(learningType != "Skill"){
            AnsiConsole.Write(new Markup($"[bold red]Required![/] [bold]Name of skill related to {learningType}.[/]{Environment.NewLine}"));
        }
        string skillId = LookupLearningByName("Skill");
        components.Add(skillId);
        if(learningType == "Goal" || learningType == "Milestone"){
            if(learningType == "Milestone"){
                AnsiConsole.Write(new Markup($"[bold red]Required![/] [bold]Name of goal related to {learningType}.[/]{Environment.NewLine}"));
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

    public string CreateNoteString(string noteId, string noteName){
        // get associated skills using id
        string noteString = $"{noteName}";
        List<string> noteIdSplit = noteId.Split("-").ToList();
        List<string> noteStringComponents = new List<string>();
        // skill
        if(noteIdSplit[0] != "xx"){
            noteStringComponents.Add(
                $"related skill: {learningManager.GetLearningByID("Skill", noteIdSplit[0])["Name"]}");
        }
        if(noteIdSplit[1] != "xx"){
            noteStringComponents.Add(
                $"related goal: {learningManager.GetLearningByID("Goal", noteIdSplit[1])["Name"]}");
        }
        if(noteIdSplit[2] != "xx"){
            noteStringComponents.Add(
                $"related milestone: {learningManager.GetLearningByID("Milestone", noteIdSplit[2])["Name"]}");
        }
        noteString+=$" ({string.Join(", ", noteStringComponents)})";
        return noteString;
    }

    public string GetStatusForDB(){
        string status = ChooseFromSelection(
            "Choose new status:", 
            new List<string>{"[green]Complete![/]", "[yellow]Return to To-Do[/]"});
        if(status == "Complete!"){
            return "Completed";
        }
        else{
            return "To-Do";
        }
    }
}