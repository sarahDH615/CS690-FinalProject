namespace LearningTracker;

using Microsoft.VisualBasic;
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
                if(learningId != ""){
                    // get learning record
                    Dictionary<string, string> learningRecord = learningManager.GetLearningByID(action, learningId);
                    // give option to change
                    string newStatus = GetStatusForDB();
                    // no need to actually change in the database if the status is already equal to the supposed update
                    Console.WriteLine(newStatus);
                    if(learningRecord["Status"] != newStatus){
                        learningManager.UpdateLearning(action, learningId, new Dictionary<string, string>{{"Status", newStatus}});
                    }
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
            string viewType = ChooseFromSelection("View flat list of notes or go through learning menu:", 
                new List<string>{"View flat", "Go through learning menu"});
            if(viewType == "View flat"){
                PickFromFlatListOfNotesAndView();
            }
            else{
                string learningType = ChooseFromSelection("Choose learning type:", new List<string>{"Skill", "Goal", "Milestone"});
                string learningId = GetLearningInDialogue(learningType);
                if(learningId != ""){
                    ShowNotes(learningType, learningId);
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
            Dictionary<string, string> noteContent = MakeNoteContent();
            string continueWithAddingNote = "No";
            string existingNoteId = notesManager.CheckForExistingNameLearningCombo(noteContent);
            while(existingNoteId != ""){
                // var remediationResults = RemediateDuplicateNote(noteContent, existingNoteId)[0];
                var remediationResults = RemediateDuplicateContent(noteContent, existingNoteId, "Note")[0];
                existingNoteId = remediationResults.ID;
                continueWithAddingNote = remediationResults.continueAdding;
                // noteContent = remediationResults.note;
                noteContent = remediationResults.content;
            }
            // save
            if(noteContent["ConnectedLearningCode"] != ""){
                if(continueWithAddingNote == "Yes"){
                    notesManager.SaveNote(noteContent);
                }
            }
            else{
                AnsiConsole.Write(new Markup($"[yellow]Note could not be made: not enough linkages to related learning.{Environment.NewLine}[/]"));
            }
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
            List<List<string>> noteContentLists = GetNotesForEditing();
            List<string> noteNames = noteContentLists[0];
            List<string> noteIds = noteContentLists[1];
            string chosenNoteName = ChooseFromSelection(
                "Choose what note field you'd like to edit, or BACK to exit Edit Notes Menu, or EXIT to exit LearningTracker:", 
                noteNames);
            string chosenNoteId = noteIds[noteNames.IndexOf(chosenNoteName)];
            Dictionary<string, string> note = notesManager.GetNoteByID(chosenNoteId);
            // choose which field to edit
            // note = EditNoteField(note);
            note = EditField(note, "Note");
            string existingNoteId = notesManager.CheckForExistingNameLearningCombo(note);
            if(existingNoteId == ""){
                // save back to data store
                notesManager.UpdateNote(chosenNoteId, note);
            }
            else{
                AnsiConsole.Write(
                    new Markup(
                        $"[yellow]Could not edit note as updated name/related learning combination conflicts with an existing note{Environment.NewLine}[/]"));
            }
            
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
        if(learningId != ""){
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

                if(learningType == "Skill" || learningType == "Goal"){
                    Console.WriteLine("Descendant learnings:");
                    List<string> columns = new List<string>{"Milestones"};
                    if(learningType == "Skill"){
                        columns.Insert(0, "Goals");
                    }
                    Dictionary<string, List<string>> descendants = learningManager.GetDescendantLearningNames(learningType, learningId);
                    // DisplayDescendantsTree(descendants);
                    DisplayDescendantsTable(descendants, columns);
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
                metadataDict["ParentID"] = GetParentId(learningType);
            }
            // if(learningType == "Goal" || learningType == "Milestone"){
            //     string ancestorSkillId = LookupLearningByName("Skill");
            //     if(learningType == "Goal"){
            //         metadataDict["ParentID"] = ancestorSkillId;
            //     }
            //     else{
            //         string ancestorGoalId = LookupLearningByName("Goal", ancestorSkillId);
            //         metadataDict["ParentID"] = ancestorGoalId;
            //     }
            // }
            string existingLearningId = learningManager.CheckForExistingNameAndParentCombo(learningType, metadataDict);
            while(existingLearningId != ""){
                // var remediationResults = RemediateDuplicateLearning(metadataDict, existingLearningId)[0];
                var remediationResults = RemediateDuplicateContent(metadataDict, existingLearningId, learningType)[0];
                existingLearningId = remediationResults.ID;
                metadataDict = remediationResults.content;
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
                AnsiConsole.Write(new Markup($"[dim]Existing field content: {learning[editField]}{Environment.NewLine}[/]")); // display existing to help them know whether they want to add/overwrite
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
            // if(learningType == "Skill"){
            //     expectedDescendantTypes.Add("Goal");
            //     expectedDescendantTypes.Add("Milestone");
            // }
            // else{
            //     expectedDescendantTypes.Add("Milestone");
            // }
            expectedDescendantTypes = learningManager.GetExpectedDescendantTypes(learningType);
            // AnsiConsole.Write(
            //     new Markup($"[red bold]Warning![/][red] Deleting learning would delete the following {expectedDescendantTypes[0]}s:{Environment.NewLine}[/]"));
            // foreach(KeyValuePair<string, Dictionary<string, string>> entry in relatedLearnings[0]){
            //     Console.WriteLine($"\t{entry.Value["Name"]}");
            // }
            // if(relatedLearnings.Count > 1){
            //     AnsiConsole.Write(
            //         new Markup($"[red bold]Warning![/][red] Deleting learning would delete the following Milestones:{Environment.NewLine}[/]"));
            //     foreach(KeyValuePair<string, Dictionary<string, string>> entry in relatedLearnings[1]){
            //         Console.WriteLine($"\t{entry.Value["Name"]}");
            //     }
            // }
            DisplayLearningsAffectedByPotentialDelete(relatedLearnings, expectedDescendantTypes);
        }
        // notes
        if(relatedNotes.Keys.Count > 0){
            DisplayNotesAffectedByPotentialDelete(relatedNotes);
            // AnsiConsole.Write(
            //     new Markup($"[red bold]Warning![/][red] Deleting learning would delete the following Notes:{Environment.NewLine}[/]"));
            // foreach(KeyValuePair<string, Dictionary<string, string>> entry in relatedNotes){
            //     Console.WriteLine($"\t{entry.Value["Name"]}");
            // }
        }
        // confirm/deny deletes
        string nextAction = ChooseFromSelection($"Do you wish to proceed with deletion(s)?:", new List<string>{"Yes", "No"});
        if(nextAction == "Yes"){
            // // delete the learning itself
            // learningManager.DeleteLearning(learningId, learningType);
            // // delete the related learnings
            // if(relatedLearnings.Count > 0){
            //     foreach(KeyValuePair<string, Dictionary<string, string>> learning in relatedLearnings[0]){
            //         learningManager.DeleteLearning(learning.Key, expectedDescendantTypes[0]);
            //     }
            //     if(relatedLearnings.Count > 1){
            //         foreach(KeyValuePair<string, Dictionary<string, string>> learning in relatedLearnings[1]){
            //             learningManager.DeleteLearning(learning.Key, expectedDescendantTypes[1]);
            //         }
            //     }
            // }
            learningManager.DeleteLearningAndDescendants(learningId, learningType, relatedLearnings, expectedDescendantTypes);
            // delete related notes
            if(relatedNotes.Keys.Count > 0){
                notesManager.DeleteMultipleNotes(relatedNotes.Keys.ToList());
                // foreach(KeyValuePair<string, Dictionary<string, string>> note in relatedNotes){
                //     notesManager.DeleteNote(note.Key);
                // }
            }
        }
        menuHistory.RemoveAt(menuHistory.Count-1);

        return "";
    }

    // ---- ADDITIONAL LEVEL MENUS ---- //
    // ---- PROGRESS ---- //
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
            Tree summaryTree = MakeProgressTree(summary);
            AnsiConsole.Write(summaryTree);
        }
        menuHistory.RemoveAt(menuHistory.Count-1);
    }

    // ---- HELPER FUNCTIONS BELOW ---- //
    public string GetParentId(string learningType){
        string ancestorSkillId = LookupLearningByName("Skill");
        if(learningType == "Goal"){
            return ancestorSkillId;
        }
        else{
            string ancestorGoalId = LookupLearningByName("Goal", ancestorSkillId);
            return ancestorGoalId;
        }
    }

    public void DisplayLearningsAffectedByPotentialDelete(List<Dictionary<string, Dictionary<string, string>>> affectedLearnings, List<string> affectedLearningTypes){
        AnsiConsole.Write(
            new Markup($"[red bold]Warning![/][red] Deleting learning would delete the following {affectedLearningTypes[0]}s:{Environment.NewLine}[/]"));
        foreach(KeyValuePair<string, Dictionary<string, string>> entry in affectedLearnings[0]){
            Console.WriteLine($"\t{entry.Value["Name"]}");
        }
        if(affectedLearnings.Count > 1){
            AnsiConsole.Write(
                new Markup($"[red bold]Warning![/][red] Deleting learning would delete the following Milestones:{Environment.NewLine}[/]"));
            foreach(KeyValuePair<string, Dictionary<string, string>> entry in affectedLearnings[1]){
                Console.WriteLine($"\t{entry.Value["Name"]}");
            }
        }
    }

    public void DisplayNotesAffectedByPotentialDelete(Dictionary<string, Dictionary<string, string>> affectedNotes){
        AnsiConsole.Write(
            new Markup($"[red bold]Warning![/][red] Deleting learning would delete the following Notes:{Environment.NewLine}[/]"));
        foreach(KeyValuePair<string, Dictionary<string, string>> entry in affectedNotes){
            Console.WriteLine($"\t{entry.Value["Name"]}");
        }
    }

    public Dictionary<string, string> MakeNoteContent(){
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
        string connLearningId = notesManager.MakeConnectedLearningId(noteIdComponents);
        metadataDict["ConnectedLearningCode"] = connLearningId;
        return metadataDict;
    }

    public void ShowNotes(string learningType, string learningId){
        // show notes related to that learning
        Dictionary<string, Dictionary<string, string>> notesInfo = notesManager.GetNotes(learningType, learningId);
        if(notesInfo.Keys.Count == 0){
            AnsiConsole.Write(new Markup($"[yellow]No notes associated with this {learningType}.{Environment.NewLine}[/]"));
        }
        else{
            foreach(KeyValuePair<string, Dictionary<string, string>> entry in notesInfo){
                WriteNoteView(entry.Key, entry.Value);
            }
        }
    }

    public void WriteNoteView(string noteId, Dictionary<string, string> noteInfo){
        Console.WriteLine();
        var rule = new Rule($"[dim]Note ID: {noteId}[/]");
        rule.Justification = Justify.Center;
        AnsiConsole.Write(rule);
        AnsiConsole.Write(new Markup($"[bold underline]Note ID[/]: {noteId}{Environment.NewLine}"));
        foreach(KeyValuePair<string, string> subentry in noteInfo){
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

    void DisplayDescendantsTree(Dictionary<string, List<string>> descendants){
        // var root = new Tree("");
        foreach(KeyValuePair<string, List<string>> descendant in descendants){
            var root = new Tree(descendant.Key);
            // var descNode = root.AddNode(descendant.Key);
            if(descendant.Value.Count > 0){
                foreach(string descendantChild in descendant.Value){
                    root.AddNode(descendantChild);
                    // descNode.AddNode(descendantChild);
                }
            }
            // Render the tree
            AnsiConsole.Write(root);
        }
    }

    void DisplayDescendantsTable(Dictionary<string, List<string>> descendants, List<string> columns){
        var table = new Table();
        foreach(string column in columns){
            table.AddColumn(column);
        }
        foreach(KeyValuePair<string, List<string>> descendant in descendants){
            if(columns.Count == 2){
                if(descendant.Value.Count > 0){
                    table.AddRow(descendant.Key, descendant.Value[0]);
                    for(int i=1; i < descendant.Value.Count; i++ ){
                        table.AddRow("   ", descendant.Value[i]);
                    }
                }
                else{
                    table.AddRow(descendant.Key, "");
                }
            }
            else{
                table.AddRow(descendant.Key);
            }
        }
        // Render the tree
        AnsiConsole.Write(table);
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
        int expectedIdCount = notesManager.GetNumberOfExpectedRelatedLearningIds(learningType);
        List<string> components = new List<string>();
        int successfulSearchCount = 0;
        if(learningType != "Skill"){
            AnsiConsole.Write(new Markup($"[bold red]Required![/] [bold]Name of skill related to {learningType}.[/]{Environment.NewLine}"));
        }
        string skillId = LookupLearningByName("Skill");
        if(skillId != ""){
            components.Add(skillId);
            successfulSearchCount+=1;
            if(learningType == "Goal" || learningType == "Milestone"){
                if(learningType == "Milestone"){
                    AnsiConsole.Write(new Markup($"[bold red]Required![/] [bold]Name of goal related to {learningType}.[/]{Environment.NewLine}"));
                }
                string goalId = LookupLearningByName("Goal", skillId);
                if(goalId != ""){
                    components.Add(goalId);
                    successfulSearchCount+=1;
                    if(learningType == "Milestone"){
                        string milestoneId = LookupLearningByName("Milestone", goalId);
                        if(milestoneId != ""){
                            components.Add(milestoneId);
                            successfulSearchCount+=1;
                        }
                    }
                }
            }
        }
        if(expectedIdCount == successfulSearchCount){
            return components;
        }
        return new List<string>(); // if one or more searches failed
    }

    public string GetStatusForDB(){
        string status = ChooseFromSelection(
            "Choose new status:", 
            new List<string>{"[green]Complete![/]", "[yellow]Return to To-Do[/]"});
        if(status.Contains("Complete!")){
            return "Completed";
        }
        else{
            return "To-Do";
        }
    }

    public Dictionary<string, string> EditNoteField(Dictionary<string, string> noteContent){
        string editField = ChooseFromSelection("Choose what field to edit:", noteContent.Keys.ToList());
        if(editField == "ConnectedLearningCode"){
            string connectedLearningType = GetLearningType();
            // get components for note id
            List<string> noteIdComponents = GetNoteIdComponents(connectedLearningType);
            
            if(noteIdComponents.Count > 0){
                string connLearningId = notesManager.MakeConnectedLearningId(noteIdComponents);
                noteContent["ConnectedLearningCode"] = connLearningId;
            }
        }
        else{
            // overwrite or add on
            AnsiConsole.Write(new Markup($"[dim]Existing field content: {noteContent[editField]}{Environment.NewLine}[/]"));
            string editKind = ChooseFromSelection("Do you want to add on to or overwrite the field?", new List<string>{"Add", "Overwrite"});
            string newContent = GetTextFromUser(editField);
            if(editKind == "Overwrite"){
                noteContent[editField] = newContent;
            }
            else {
                noteContent[editField]+=newContent;
            }
        }

        AnsiConsole.Write(new Markup($"[dim]Updated {editField}: {noteContent[editField]}{Environment.NewLine}[/]"));
        return noteContent;
    }

    // public List<(string ID, string continueAdding, Dictionary<string, string> note)> RemediateDuplicateNote(Dictionary<string, string> noteContent, string existingNoteId){
    //     AnsiConsole.Write(new Markup(($"[red]Cannot create note with Name '{noteContent["Name"]}' as it already exists{Environment.NewLine}[/]")));
    //     string decision = ChooseFromSelection(
    //         "How would you like to fix the issue?", 
    //         new List<string>{"Overwrite existing note", "Edit existing note", "Change name of note being created"});
    //     // string continueWithAddingNote = "No"; // default
    //     // overwrite: apply the fields for the new note into the old note
    //     if(decision.StartsWith("Overwrite")){
    //         return RemediateDuplicateNoteOverwrite(existingNoteId, noteContent);
    //     }
    //     // edit: go to edit menu with the existing note's ID
    //     else if(decision.StartsWith("Edit")){
    //         return RemediateDuplicateNoteEdit(existingNoteId, noteContent);
    //     }
    //     // if(decision.StartsWith("Overwrite") || decision.StartsWith("Edit")){
    //     //     Dictionary<string, string> existingNote = notesManager.GetNoteByID(existingNoteId);
    //     //     string pasttense = "";
    //     //     if(decision.StartsWith("Overwrite")){
    //     //         foreach(KeyValuePair<string, string> pair in existingNote){
    //     //             existingNote[pair.Key] = noteContent[pair.Key];
    //     //         }
    //     //         pasttense = "overwritten";
    //     //         return new List<(string ID, string continueAdding, Dictionary<string, string> note)>{
    //     //             ("", continueWithAddingNote, noteContent)};
    //     //     }
    //     //     // edit: go to edit menu with the existing note's ID
    //     //     else if(decision.StartsWith("Edit")){
    //     //         existingNote = EditNoteField(existingNote);
    //     //         pasttense = "edited";
    //     //     }
    //     //     notesManager.UpdateNote(existingNoteId, existingNote);
    //     //     Console.Write($"Older note with same name {pasttense}.");
    //     //     if(decision.StartsWith("Edit")){
    //     //         continueWithAddingNote = ChooseFromSelection(
    //     //             "Would you like to continue with the note you started entering?", 
    //     //             new List<string>{"Yes", "No"});
    //     //         if(continueWithAddingNote == "Yes"){
    //     //             Console.WriteLine($"Rechecking ability to save new note");
    //     //             existingNoteId = notesManager.CheckForExistingNameLearningCombo(noteContent);
    //     //             return new List<(string ID, string continueAdding, Dictionary<string, string> note)>{
    //     //                 (existingNoteId, continueWithAddingNote, noteContent)};
    //     //         }
    //     //         else{
    //     //             return new List<(string ID, string continueAdding, Dictionary<string, string> note)>{
    //     //                 ("", continueWithAddingNote, noteContent)};
    //     //         }
    //     //     }
    //     // }
    //     // change: update the name for this note and save
    //     else{
    //         // string newNoteName = GetTextFromUser("Name");
    //         // noteContent["Name"] = newNoteName;
    //         // Console.WriteLine($"Rechecking ability to save new note");
    //         // existingNoteId = notesManager.CheckForExistingNameLearningCombo(noteContent);
    //         // return new List<(string ID, string continueAdding, Dictionary<string, string> note)>{
    //         //     (existingNoteId, "Yes", noteContent)};
    //         return RemediateDuplicateNoteChangeName(noteContent);
    //     }
    //     // return new List<(string ID, string continueAdding, Dictionary<string, string> note)>{
    //     //     (existingNoteId, continueWithAddingNote, noteContent)};
    // }

    public void OutputDuplicateWarning(string type, string name, bool hasParent=false){
        string warning;
        string typeForWarning;
        string insertIntoWarning = "";
        if(type != "Note"){
            if(hasParent){
                insertIntoWarning = " under its parent learning";
            }
            typeForWarning = "learning";
        }
        else{
            typeForWarning = "note";
        }
        warning = $"Cannot create {typeForWarning} with Name '{name}' as it already exists{insertIntoWarning}.";
        AnsiConsole.Write(new Markup(($"[red]{warning}{Environment.NewLine}[/]")));
    }

    public List<(string ID, string continueAdding, Dictionary<string, string> content)> RemediateDuplicateContent(Dictionary<string, string> contentMetadata, string existingId, string type){
        OutputDuplicateWarning(type, contentMetadata["Name"], contentMetadata.Keys.Contains("ParentID"));
        string decision = ChooseFromSelection(
            "How would you like to fix the issue?", 
            new List<string>{$"Overwrite existing {type}", $"Edit existing {type}", $"Change name of {type} being created"});
        // overwrite: apply the fields for the new content into the old content
        if(decision.StartsWith("Overwrite")){
            return RemediateDuplicateContentOverwrite(existingId, contentMetadata, type);
        }
        // edit: go to edit menu with the existing content's ID
        else if(decision.StartsWith("Edit")){
            return RemediateDuplicateContentEdit(existingId, contentMetadata, type);
        }
        // change: update the name for this content and save
        else{
            return RemediateDuplicateContentChangeName(contentMetadata, type);
        }
    }

    public List<(string ID, string continueAdding, Dictionary<string, string> note)>RemediateDuplicateContentOverwrite(string existingId, Dictionary<string, string> content, string type){
        Dictionary<string, string> existingContent = new Dictionary<string, string>{};
        if(type == "Note"){
            existingContent = notesManager.GetNoteByID(existingId);
        }
        else{
            existingContent = learningManager.GetLearningByID(type, existingId);
        }
        
        foreach(KeyValuePair<string, string> pair in existingContent){
            existingContent[pair.Key] = content[pair.Key];
        }
        Console.Write($"Older {type} with same name overwritten.");
        return new List<(string ID, string continueAdding, Dictionary<string, string> content)>{
            ("", "No", content)};
    }

    public List<(string ID, string continueAdding, Dictionary<string, string> note)>RemediateDuplicateContentEdit(string existingId, Dictionary<string, string> content, string type){
        Dictionary<string, string> existingContent = new Dictionary<string, string>{};
        if(type == "Note"){
            existingContent = notesManager.GetNoteByID(existingId);
            existingContent = EditField(existingContent, type);
            notesManager.UpdateNote(existingId, existingContent);
        }
        else{
            existingContent = learningManager.GetLearningByID(type, existingId);
            existingContent = EditField(existingContent, type);
            learningManager.UpdateLearning(type, existingId, existingContent);
        }
        Console.Write($"Older {type} with same name edited.");
        string continueWithAdding = ChooseFromSelection(
            $"Would you like to continue with the {type} you started entering?", 
            new List<string>{"Yes", "No"});
        if(continueWithAdding == "Yes"){
            Console.WriteLine($"Rechecking ability to save new {type}");
            if(type == "Note"){
                existingId = notesManager.CheckForExistingNameLearningCombo(existingContent);
            }
            else{
                existingId = learningManager.CheckForExistingNameAndParentCombo(type, existingContent);
            }
            return new List<(string ID, string continueAdding, Dictionary<string, string> content)>{
                (existingId, continueWithAdding, existingContent)};
        }
        else{
            return new List<(string ID, string continueAdding, Dictionary<string, string> content)>{
                ("", continueWithAdding, existingContent)};
        }
    }

    public Dictionary<string, string> EditField(Dictionary<string, string> content, string type){
        string editField = ChooseFromSelection("Choose what field to edit:", content.Keys.ToList());
        if(editField == "ParentID"){
            content["ParentID"] = GetParentId(type);
        }
        else if(editField == "ConnectedLearningCode"){
            string connectedLearningType = GetLearningType();
            // get components for note id
            List<string> noteIdComponents = GetNoteIdComponents(connectedLearningType);
            
            if(noteIdComponents.Count > 0){
                string connLearningId = notesManager.MakeConnectedLearningId(noteIdComponents);
                content["ConnectedLearningCode"] = connLearningId;
            }
        }
        else{
            // overwrite or add on
            AnsiConsole.Write(new Markup($"[dim]Existing field content: {content[editField]}{Environment.NewLine}[/]"));
            string editKind = ChooseFromSelection("Do you want to add on to or overwrite the field?", new List<string>{"Add", "Overwrite"});
            string newContent = GetTextFromUser(editField);
            if(editKind == "Overwrite"){
                content[editField] = newContent;
            }
            else {
                content[editField]+=newContent;
            }
        }

        AnsiConsole.Write(new Markup($"[dim]Updated {editField}: {content[editField]}{Environment.NewLine}[/]"));
        return content;
    }

    public List<(string ID, string continueAdding, Dictionary<string, string> content)>RemediateDuplicateContentChangeName(Dictionary<string, string> content, string type){
        string newContentName = GetTextFromUser("Name");
        content["Name"] = newContentName;
        Console.WriteLine($"Rechecking ability to save new {type}");
        string existingId;
        if(type == "Note"){
            existingId = notesManager.CheckForExistingNameLearningCombo(content);
        }
        else{
            existingId = learningManager.CheckForExistingNameAndParentCombo(type, content);
        }
        return new List<(string ID, string continueAdding, Dictionary<string, string> content)>{
            (existingId, "Yes", content)};
    }

    // public List<(string ID, string continueAdding, Dictionary<string, string> note)>RemediateDuplicateNoteChangeName(Dictionary<string, string> noteContent){
    //     string newNoteName = GetTextFromUser("Name");
    //     noteContent["Name"] = newNoteName;
    //     Console.WriteLine($"Rechecking ability to save new note");
    //     string existingNoteId = notesManager.CheckForExistingNameLearningCombo(noteContent);
    //     return new List<(string ID, string continueAdding, Dictionary<string, string> note)>{
    //         (existingNoteId, "Yes", noteContent)};
    // }

    public List<List<string>> GetNotesForEditing(){
        List<List<string>> notesStrings = new List<List<string>>();
        Dictionary<string, Dictionary<string, string>> notesInfo = notesManager.GetNotes();
        foreach(KeyValuePair<string, Dictionary<string, string>> entry in notesInfo){
            notesStrings.Add(new List<string>{entry.Key, CreateNoteStringForEditing(entry.Value["ConnectedLearningCode"], entry.Value["Name"])});
        }
        List<string> names = notesStrings.Select(noteString => noteString[1]).ToList();
        List<string> ids = notesStrings.Select(noteString => noteString[0]).ToList();
        return new List<List<string>>{names, ids};
    }

    public string CreateNoteStringForEditing(string relatedLearningId, string noteName){
        // get associated skills using id
        string noteString = $"{noteName}";
        List<string> idSplit = relatedLearningId.Split("-").ToList();
        List<string> noteStringComponents = new List<string>();
        // skill
        if(idSplit[0] != "xx"){
            noteStringComponents.Add(
                $"related skill: {learningManager.GetLearningByID("Skill", idSplit[0])["Name"]}");
        }
        if(idSplit[1] != "xx"){
            noteStringComponents.Add(
                $"related goal: {learningManager.GetLearningByID("Goal", idSplit[1])["Name"]}");
        }
        if(idSplit[2] != "xx"){
            noteStringComponents.Add(
                $"related milestone: {learningManager.GetLearningByID("Milestone", idSplit[2])["Name"]}");
        }
        noteString+=$" ({string.Join(", ", noteStringComponents)})";
        return noteString;
    }

    public void PickFromFlatListOfNotesAndView(){
        List<List<string>> noteContentLists = GetNotesForEditing();
        List<string> noteNames = noteContentLists[0];
        List<string> noteIds = noteContentLists[1];
        string chosenNoteName = ChooseFromSelection(
            "Choose what note field you'd like to edit, or BACK to exit Edit Notes Menu, or EXIT to exit LearningTracker:", 
            noteNames);
        string chosenNoteId = noteIds[noteNames.IndexOf(chosenNoteName)];
        Dictionary<string, string> note = notesManager.GetNoteByID(chosenNoteId);
        WriteNoteView(chosenNoteId, note);
    }
}