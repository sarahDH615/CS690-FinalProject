namespace LearningTracker;

class Program
{
    static void Main(string[] args)
    {
        UserUI userUi = new UserUI();
        userUi.EnterStartMenu();
        
    }

    static void PrintDatabaseResults(Dictionary<string, Dictionary<string, string>> databaseResults){
        foreach(KeyValuePair<string, Dictionary<string, string>> entry in databaseResults){
            Console.WriteLine($"Index: {entry.Key}");
            foreach(KeyValuePair<string, string> subentry in entry.Value){
                Console.WriteLine($"{subentry.Key}: {subentry.Value}");
            }
        }
    }
}