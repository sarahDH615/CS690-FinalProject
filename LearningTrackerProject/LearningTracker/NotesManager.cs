namespace LearningTracker;

public class NotesManager{

    protected internal DataManager dataManager = new DataManager();

    public Dictionary<string, string> GetCommonMetadataFieldsTextEntry(){
        Dictionary<string, string> metadataDictionary = new Dictionary<string, string>{
            {"Name", ""},
            {"Description", ""},
            {"Body", ""}
        };
        return metadataDictionary;
    }

    public void SaveNote(Dictionary<string, string> noteMetadata, List<string> idComponents){
        dataManager.SaveNote(noteMetadata, idComponents);
    }

    public Dictionary<string, Dictionary<string, string>> GetNotes(string relatedLearningType="", string relatedLearningID=""){
        if(relatedLearningID !=""){
            int index = 0; // default, skills
            if(relatedLearningType == "Goal"){
                index += 1;
            }
            else if(relatedLearningType == "Milestone"){
                index += 2;
            }

            List<Dictionary<string, Dictionary<string, string>>> relatedNotes = new List<Dictionary<string, Dictionary<string, string>>>{};
            var filteredNotes = dataManager.notesDict.Where(kvp => kvp.Key.Split("-").ToList()[index] == relatedLearningID);
            // back to a dictionary
            var filteredNotesDict = filteredNotes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value); 
            return filteredNotesDict;
        }
        else{
            return dataManager.notesDict;
        }
    }

    public Dictionary<string, string> GetNoteByID(string noteID){
        return dataManager.notesDict[noteID];
    }

    public void DeleteNote(string noteId){
        dataManager.DeleteNote(noteId);
    }

    public void UpdateNote(string noteId, Dictionary<string, string> noteContent){
        dataManager.UpdateNote(noteId, noteContent);
    }

}