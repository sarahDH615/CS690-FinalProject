namespace LearningTracker;
using System.IO;
using Spectre.Console;

class Program
{
    static void Main(string[] args)
    {
        // while loop to keep asking for activity

        string activity;  // warning CS8600: Converting null literal or possible null value to non-nullable type.

        do{
            activity = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose activity, or END to exit LearningTracker:")
                    .PageSize(10)
                    .MoreChoicesText("")
                    .AddChoices(new[] {
                        "Progress", "Notes", "Manage Learning", "END",
                    }));
            // Console.WriteLine("You chose: "+activity);

            // pass result of child menu back to allow for END if passed
            if(activity == "Manage Learning"){
                activity = ManageLearningMenu();
            }
        }
        while(activity != "END");

    }

    public static string ManageLearningMenu()
    {
        string action;

        do{
            action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose action, or BACK to exit Manage Learning Menu, or END to exit LearningTracker:")
                    .PageSize(10)
                    .MoreChoicesText("")
                    .AddChoices(new[] {
                        "View", "Add", "Edit", "Delete", "BACK", "END",
                    }));
            // Console.WriteLine("You chose: "+action);

            // pass result of child menu back to allow for END if passed
            if(action == "Add"){
                action = AddLearning();
            }
        }
        // BACK will go back to the previous while loop; END will exit the script entirely
        while(action != "END" && action != "BACK"); 

        return action;

    }

    public static string AddLearning()
    {
        string exitFunc = "";

        Dictionary<string, string> fileDictionary = new Dictionary<string, string>(){
            ["Skill"] = "Skills.txt",
            ["Goal"] = "Goals.txt",
            ["Milestone"] = "Milestones.txt",
        };

        string[] requiredPromptValues = new string[2];

        string[] requiredTextEntryFields = {"Name", "Description"};
        string[] requiredTextEntryValues = new string[requiredTextEntryFields.Length];

        for(int i=0; i < requiredTextEntryFields.Length; i++){
            string requiredField = requiredTextEntryFields[i];
            string requiredValue = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter "+requiredField+": ")); // empty input is not allowed, so something must be entered
            
            if( requiredValue == "END" || requiredValue == "BACK" ){
                exitFunc = requiredValue;
                break;
            }
            requiredTextEntryValues[i] = requiredValue;
            
        }

        // add status
        string status = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose completion status:")
                .PageSize(10)
                .MoreChoicesText("")
                .AddChoices(new[] {
                    "To-Do", "Completed",
                }));
        requiredPromptValues[0] = status;

        // learning type - should END and BACK be allowed here?
        string learningType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose learning type:")
                .PageSize(10)
                .MoreChoicesText("")
                .AddChoices(new[] {
                    "Skill", "Goal", "Milestone",
                }));
        requiredPromptValues[1] = learningType;
        // TO DO: if learning type is not skill, send to skill attach function

        // combine all values 
        string[] recordValues = requiredTextEntryValues.Concat(requiredPromptValues).ToArray();
        // save to file
        string record = string.Join(", ", recordValues);
        File.AppendAllText(fileDictionary[learningType], record);

        return exitFunc;

    }
}
