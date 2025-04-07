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
                    .Title("Choose activity, or EXIT to exit LearningTracker:")
                    .PageSize(10)
                    .MoreChoicesText("")
                    .AddChoices(new[] {
                        "Progress", "Notes", "Manage Learning", "EXIT",
                    }));
            // Console.WriteLine("You chose: "+activity);

            // pass result of child menu back to allow for EXIT if passed
            if(activity == "Manage Learning"){
                activity = ManageLearningMenu();
            }
        }
        while(activity != "EXIT");

    }

    public static string ManageLearningMenu()
    {
        string action;

        do{
            action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose action, or BACK to exit Manage Learning Menu, or EXIT to exit LearningTracker:")
                    .PageSize(10)
                    .MoreChoicesText("")
                    .AddChoices(new[] {
                        "View", "Add", "Edit", "Delete", "BACK", "EXIT",
                    }));
            // Console.WriteLine("You chose: "+action);

            // pass result of child menu back to allow for EXIT if passed
            if(action == "Add"){
                action = AddLearning();
            }
            else if(action == "View"){
                action = ViewLearning();
            }
        }
        // BACK will go back to the previous while loop; EXIT will exit the script entirely
        while(action != "EXIT" && action != "BACK"); 

        return action;

    }

    public static string AddLearning()
    {
        string exitFunc = "";

        // determine type of learrning
        string learningType = chooseLearningType();
        // if skill, go to skill addLearning()
        if(learningType == "Skill"){
            Skill newSkill = new Skill();
            newSkill.addLearning();
            newSkill.saveToDataStore();
        }
        // if goal, go to goal addLearning()

        // if milestone, go to milestone addLearning()

        // Dictionary<string, string> fileDictionary = new Dictionary<string, string>(){
        //     ["Skill"] = "Skills.txt",
        //     ["Goal"] = "Goals.txt",
        //     ["Milestone"] = "Milestones.txt",
        // };

        // string[] requiredPromptValues = new string[2];

        // string[] requiredTextEntryFields = {"Name", "Description"};
        // string[] requiredTextEntryValues = new string[requiredTextEntryFields.Length];

        // for(int i=0; i < requiredTextEntryFields.Length; i++){
        //     string requiredField = requiredTextEntryFields[i];
        //     string requiredValue = AnsiConsole.Prompt(
        //         new TextPrompt<string>("Enter "+requiredField+": ")); // empty input is not allowed, so something must be entered
            
        //     if( requiredValue == "EXIT" || requiredValue == "BACK" ){
        //         exitFunc = requiredValue;
        //         break;
        //     }
        //     requiredTextEntryValues[i] = requiredValue;
            
        // }

        // // add status
        // string status = AnsiConsole.Prompt(
        //     new SelectionPrompt<string>()
        //         .Title("Choose completion status:")
        //         .PageSize(10)
        //         .MoreChoicesText("")
        //         .AddChoices(new[] {
        //             "To-Do", "Completed",
        //         }));
        // requiredPromptValues[0] = status;

        // learning type - should EXIT and BACK be allowed here?
        // string learningType = AnsiConsole.Prompt(
        //     new SelectionPrompt<string>()
        //         .Title("Choose learning type:")
        //         .PageSize(10)
        //         .MoreChoicesText("")
        //         .AddChoices(new[] {
        //             "Skill", "Goal", "Milestone",
        //         }));
        // requiredPromptValues[1] = learningType;
        // TO DO: if learning type is not skill, send to skill attach function

        // combine all values 
        // string[] recordValues = requiredTextEntryValues.Concat(requiredPromptValues).ToArray();
        // // save to file
        // string record = string.Join(", ", recordValues);
        // File.AppendAllText(fileDictionary[learningType], record);

        return exitFunc;

    }

    public static string ViewLearning()
    {
        throw new NotImplementedException();

    }

    public static string chooseLearningType(){
        string learningType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose learning type:")
                .PageSize(10)
                .MoreChoicesText("")
                .AddChoices(new[] {
                    "Skill", "Goal", "Milestone",
                }));
        return learningType;
    }
    
}

public class DatabaseManager
{
    /// <summary>
    /// interface with the data stores; can get or save data to those stores
    /// </summary>
    /// <returns></returns>
    // public int generateId(){}

    public List<string> getOrCreateFile(string fileName, List<string> headers, string separator = "|||"){
    // private void getOrCreateFile(string fileName, List<string> headers, string separator = "|||"){
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

    public void saveRecord(string fileName, string newEntry){
        File.AppendAllText(fileName, newEntry+"\n");
    }
}

public class LearningsManager{
    protected List<string> headers = new List<string>{"ID", "Name", "Description", "Status"};
    protected DatabaseManager databaseConnection = new DatabaseManager();

    protected List<string> getLearnings(string learningFileName, List<string>learningHeaders){
        List<string> learningList = databaseConnection.getOrCreateFile(learningFileName, learningHeaders);
        return learningList;
    }

    public void saveLearning(string learningFileName, string record){
        databaseConnection.saveRecord(learningFileName, record);
    }

    

    // List<string> learningNames = new List<string>();

    // List<string> getLearningNames(List<Learning> learnings){
    //     List<string> learningNames = new List<string>();

    //     foreach (Learning learning in learnings)
    //     {
    //         learningNames.Add(Learning.name);
    //     }

    //     return learningNames;
    // }
}

public class SkillsManager : LearningsManager {
    
    public string skillsFileName = "Skills.txt";
    // headers are the same as in base class
    // public SkillsManager(){
    //     this.headers = headers;
    //     this.skillsFileName = skillsFileName;
    // }

    List<string> getSkills(){
        return getLearnings(this.skillsFileName, this.headers);
    }

    public void saveSkill(string name, string description, string status){
        if(name != "" && description != "" && status != ""){
            List<string> existingSkills = getSkills();
            string skillId = existingSkills.Count.ToString();
            string skillRecord = skillId +"|||"+ name +"|||"+ description +"|||"+ status;
            saveLearning(this.skillsFileName, skillRecord);
        }
        else{
            throw new InvalidOperationException("Required values are not present.");
        }
    }
}

public class GoalsManager : LearningsManager {
    // update headers variable
    public GoalsManager(){
        // List<string> headers = base.headers.Add("parentSkillID");
        headers.Add("parentSkillID");
    }

    List<string> getGoals(){
        string goalsFileName = "Goals.txt";
        return getLearnings(goalsFileName, headers);
    }
}

public class MilestonesManager : LearningsManager{
    public MilestonesManager(){
        headers.Add("parentGoalID");
    }
    List<string> getMilestones(){
        string milestonesFileName = "Milestones.txt";
        return getLearnings(milestonesFileName, headers);
    }
}

public class Learning{
    public string name;
    public string description;
    public string status;

    public Learning(string name="", string description="", string status = "To-Do")
    {
        this.name = name;
        this.description = description;
        this.status = status;
    }

    public void viewLearning(){
        Console.WriteLine(this.name);
        Console.WriteLine(this.description);
        Console.WriteLine(this.status);
    }

    public void addLearning(){
        // update name
        this.name = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter name: ")); // empty input is not allowed, so something must be entered
        this.description = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter description: ")); // empty input is not allowed, so something must be entered
        this.status = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose completion status:")
                .PageSize(10)
                .MoreChoicesText("")
                .AddChoices(new[] {
                    "To-Do", "Completed",
                }));
    }

    // public void saveToDataStore(){
    //     // pass info to database manager
    // }

    // generate id
}

public class Skill : Learning{
    
    public void saveToDataStore(){
        SkillsManager skillManager = new SkillsManager();
        skillManager.saveSkill(this.name, this.description, this.status);
    }
    // public string getName(){
        
    // }
}

// public class Goal
// {}

// public class Milestone
// {}