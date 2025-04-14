namespace LearningTracker;

class Program
{
    static void Main(string[] args)
    {
        // UserUI userUi = new UserUI();
        // userUi.EnterStartMenu();
        DataIOSQL dataIOSQL = new DataIOSQL();

        // // entry 1
        // Dictionary<string, string> entry1 = new Dictionary<string, string>{
        //     {"Name", "Stitch types"},
        //     {"Description", "Types of stitches"},
        //     {"Body", "Backstitch, halfstitch, running"},
        //     {"ConnectedLearningCode", "1-0-0"}
        // };

        // // entry 2
        // Dictionary<string, string> entry2 = new Dictionary<string, string>{
        //     {"Name", "Coffee types"},
        //     {"Description", "types of coffee I've tried"},
        //     {"Body", "Arabica, Kenyan"},
        //     {"ConnectedLearningCode", "0-xx-xx"}
        // };
        // // entry 3
        // Dictionary<string, string> entry3 = new Dictionary<string, string>{
        //     {"Name", "Thoughts on needles"},
        //     {"Description", "Observations about sewing needles"},
        //     {"Body", "I think I prefer longer needles as I can pick up more stitches"},
        //     {"ConnectedLearningCode", "1-xx-xx"}
        // };
        // // entry 4
        // Dictionary<string, string> entry4 = new Dictionary<string, string>{
        //     {"Name", "Thoughts on backstitches"},
        //     {"Description", "Observations whilst sewing with a backstitch"},
        //     {"Body", "I don't think it is as slow as people often say"},
        //     {"ConnectedLearningCode", "1-0-0"}
        // };

        // dataIOSQL.AddToDB("Note", entry1);
        // dataIOSQL.AddToDB("Note", entry2);
        // dataIOSQL.AddToDB("Note", entry3);
        // dataIOSQL.AddToDB("Note", entry4);

        Dictionary<string, Dictionary<string, string>> notesDBReturns = dataIOSQL.GetNotesDataFromDB(
            "SELECT * FROM notes;", new List<string>{"Name", "Description", "Body", "ConnectedLearningCode"});
        
        PrintDatabaseResults(notesDBReturns);
        
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