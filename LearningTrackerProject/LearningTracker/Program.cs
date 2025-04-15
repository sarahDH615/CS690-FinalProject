namespace LearningTracker;

class Program
{
    static void Main(string[] args)
    {
        UserUI userUi = new UserUI();
        userUi.EnterStartMenu();
        // DataIOSQL dataIOSQL = new DataIOSQL();

        // Dictionary<string, string> entry = new Dictionary<string, string>{
        //     {"Name", "Test new note"},
        //     {"Description", "Test description"},
        //     {"Body", "Test body"},
        //     {"ConnectedLearningCode", "xx-xx-xx"}
        // };

        // dataIOSQL.AddToDB("Note", entry);

        // Dictionary<string, Dictionary<string, string>> notesDBReturns = dataIOSQL.GetDataFromDB(
        //     "Note", "SELECT * FROM notes;", new List<string>{"ID", "Name", "Description", "Body", "ConnectedLearningCode"});
        
        // PrintDatabaseResults(notesDBReturns);

        // dataIOSQL.DeleteRecord("Note", "notes", "5");

        // Dictionary<string, Dictionary<string, string>> notesDBReturns1 = dataIOSQL.GetDataFromDB(
        //     "Note", "SELECT * FROM notes;", new List<string>{"ID", "Name", "Description", "Body", "ConnectedLearningCode"});
        
        // PrintDatabaseResults(notesDBReturns1);

        // dataIOSQL.UpdateRecord(
        //     "Learning", 
        //     "skills", 
        //     "1", 
        //     new Dictionary<string, string>{{"Name", "'Making coffee well'"}});
        
        // Dictionary<string, Dictionary<string, string>> testReturns = dataIOSQL.GetDataFromDB(
        //     "Learning", "SELECT * FROM skills where ID = 1;", new List<string>{"ID", "Name", "Description", "Status"});
        
        // PrintDatabaseResults(testReturns);

        // dataIOSQL.CreateOrOpenSQLDB();

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

        // List<Dictionary<string, string>> skillsUploads = new List<Dictionary<string, string>>{
        //     new Dictionary<string, string>{
        //         {"Name", "Making coffee"},
        //         {"Description", "Learning to make a good cup of coffee without burning it"},
        //         {"Status", "To-Do"}
        //     },
        //     new Dictionary<string, string>{
        //         {"Name", "Sewing"},
        //         {"Description", "Learning how to do several simple stitches well"},
        //         {"Status", "To-Do"}
        //     },
        //     new Dictionary<string, string>{
        //         {"Name", "Knitting"},
        //         {"Description", "Learning how to knit socks and gloves!"},
        //         {"Status", "To-Do"}
        //     },
        // };

        
        // foreach(var entry in skillsUploads){
        //     dataIOSQL.AddToDB("Skill", entry);
        // }

        // List<Dictionary<string, string>> goalsUploads = new List<Dictionary<string, string>>{
        //     new Dictionary<string, string>{
        //         {"Name", "Backstitch"},
        //         {"Description", "Learning how to do a backstitch"},
        //         {"Status", "To-Do"},
        //         {"ParentID", "1"}
        //     },
        //     new Dictionary<string, string>{
        //         {"Name", "Buying a French press"},
        //         {"Description", "Buying a French press coffee machine for making good coffee :)"},
        //         {"Status", "To-Do"},
        //         {"ParentID", "0"}
        //     },
        // };

        // foreach(var entry in goalsUploads){
        //     dataIOSQL.AddToDB("Goal", entry);
        // }

        // List<Dictionary<string, string>> milestonesUploads = new List<Dictionary<string, string>>{
        //     new Dictionary<string, string>{
        //         {"Name", "Watch all sewing videos on backstitch"},
        //         {"Description", "Watching all videos labelled backstitch at learntosew.com"},
        //         {"Status", "To-Do"},
        //         {"ParentID", "0"}
        //     },
        //     new Dictionary<string, string>{
        //         {"Name", "Look up french press price"},
        //         {"Description", "Research french press prices on google|"},
        //         {"Status", "To-Do"},
        //         {"ParentID", "1"}
        //     },
        // };

        // foreach(var entry in milestonesUploads){
        //     dataIOSQL.AddToDB("Milestone", entry);
        // }



        // Dictionary<string, Dictionary<string, string>> notesDBReturns = dataIOSQL.GetDataFromDB(
        //     "Note", "SELECT * FROM notes;", new List<string>{"ID", "Name", "Description", "Body", "ConnectedLearningCode"});
        
        // PrintDatabaseResults(notesDBReturns);

        // Dictionary<string, Dictionary<string, string>> skillsDBReturns = dataIOSQL.GetDataFromDB(
        //     "Learning", "SELECT * FROM skills;", new List<string>{"ID", "Name", "Description", "Status"});
        
        // PrintDatabaseResults(skillsDBReturns);

        // Dictionary<string, Dictionary<string, string>> goalsDBReturns = dataIOSQL.GetDataFromDB(
        //     "Learning", "SELECT * FROM goals;", new List<string>{"ID", "Name", "Description", "Status", "ParentID"});
        
        // PrintDatabaseResults(goalsDBReturns);

        // Dictionary<string, Dictionary<string, string>> milestonesDBReturns = dataIOSQL.GetDataFromDB(
        //     "Learning", "SELECT * FROM milestones;", new List<string>{"ID", "Name", "Description", "Status", "ParentID"});
        
        // PrintDatabaseResults(milestonesDBReturns);
        
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