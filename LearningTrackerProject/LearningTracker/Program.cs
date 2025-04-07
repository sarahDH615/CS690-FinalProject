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
        else if(learningType == "Goal"){
            Goal newGoal = new Goal();
            newGoal.addLearning();
            newGoal.saveToDataStore();
        }
        // if milestone, go to milestone addLearning()
        else if(learningType == "Milestone"){
            Milestone newMilestone = new Milestone();
            newMilestone.addLearning();
            newMilestone.saveToDataStore();
        }

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

    public Dictionary<string, string> getLearningIdsAndNames(List<string> learnings){
        Dictionary<string, string> idsAndNamesDict = new Dictionary<string, string>();
        foreach(string learning in learnings){
            string[] learningSplit = learning.Split("|||");
            idsAndNamesDict[learningSplit[1]] = learningSplit[0]; // name: id
        }
        return idsAndNamesDict;
    }

    public string getParentLearning(string parentLearningType){
        List<string> existingParentLearnings = new List<string>();
        if(parentLearningType == "goal"){
            GoalsManager parentLearningManager = new GoalsManager();
            existingParentLearnings = parentLearningManager.getGoals();
        }
        else if(parentLearningType == "skill"){
            SkillsManager parentLearningManager = new SkillsManager();
            existingParentLearnings = parentLearningManager.getSkills();
        }
        
        Dictionary<string, string> learningsInfo = getLearningIdsAndNames(existingParentLearnings);
        string learningName = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Choose parent {parentLearningType}: ")
                .PageSize(10)
                .MoreChoicesText("")
                .AddChoices(learningsInfo.Keys));
        return learningsInfo[learningName];
    }
}

public class SkillsManager : LearningsManager {
    
    public string skillsFileName = "Skills.txt";
    // List<string> existingSkills = getSkills();

    public List<string> getSkills(){
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
    public string goalsFileName = "Goals.txt";
    // List<string> existingGoals = getGoals();
    public GoalsManager(){
        headers.Add("parentSkillID");
    }

    public List<string> getGoals(){
        return getLearnings(this.goalsFileName, this.headers);
    }

    public string getParentSkill(){
        SkillsManager skillsManager = new SkillsManager();
        List<string> existingSkills = skillsManager.getSkills();
        Dictionary<string, string> skillsInformation = getLearningIdsAndNames(existingSkills);
        string skillName = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose completion status:")
                .PageSize(10)
                .MoreChoicesText("")
                .AddChoices(skillsInformation.Keys));
                // .AddChoices(new[] {

                // }));
        return skillsInformation[skillName];
    }

    public void saveGoal(string name, string description, string status){
        if(name != "" && description != "" && status != ""){
            List<string> existingGoals = getGoals();
            string goalId = existingGoals.Count.ToString();
            string parentSkillId = getParentSkill();
            string goalRecord = goalId +"|||"+ name +"|||"+ description +"|||"+ status +"|||"+ parentSkillId;
            saveLearning(this.goalsFileName, goalRecord);
        }
        else{
            throw new InvalidOperationException("Required values are not present.");
        }
    }
}

public class MilestonesManager : LearningsManager{
    // update headers variable
    public string milestonesFileName = "Milestones.txt";
    // List<string> existingGoals = getGoals();
    public MilestonesManager(){
        headers.Add("parentGoalID");
    }
    List<string> getMilestones(){
        return getLearnings(this.milestonesFileName, this.headers);
    }

    public string getParentGoal(){
        return getParentLearning("goal");
    }

    public void saveMilestone(string name, string description, string status){
        if(name != "" && description != "" && status != ""){
            List<string> existingMilestones = getMilestones();
            string milestoneId = existingMilestones.Count.ToString();
            string parentGoalID = getParentGoal();
            string milestoneRecord = milestoneId +"|||"+ name +"|||"+ description +"|||"+ status +"|||"+ parentGoalID;
            saveLearning(this.milestonesFileName, milestoneRecord);
        }
        else{
            throw new InvalidOperationException("Required values are not present.");
        }
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

public class Goal : Learning{
    
    public void saveToDataStore(){
        GoalsManager goalManager = new GoalsManager();
        goalManager.saveGoal(this.name, this.description, this.status);
    }
}

public class Milestone : Learning{
    
    public void saveToDataStore(){
        MilestonesManager milestoneManager = new MilestonesManager();
        milestoneManager.saveMilestone(this.name, this.description, this.status);
    }
}

// public class Milestone
// {}