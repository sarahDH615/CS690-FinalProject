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

    public string MakeConnectedLearningId(List<string> idComponents, int componentLength=3){
        List<string> components = new List<string>();
        for(int segment = 0; segment < componentLength; segment++){
            if(idComponents.Count > segment && idComponents[segment] != ""){
                components.Add(idComponents[segment]);
            }
            else{
                components.Add("xx");
            }
        }
        return string.Join("-", components);
    }

    public int GetNumberOfExpectedRelatedLearningIds(string relatedLearningType){
        if(relatedLearningType == "Skill"){
            return 1;
        }
        else if(relatedLearningType == "Goal"){
            return 2;
        }
        return 3;
    }

    public void SaveNote(Dictionary<string, string> noteMetadata, List<string> idComponents){
        string connLearningId = MakeConnectedLearningId(idComponents);
        noteMetadata["ConnectedLearningCode"] = connLearningId;
        dataManager.SaveNote(noteMetadata);
    }

    public string CheckForExistingNameLearningCombo(Dictionary<string, string> noteInfo){
        return dataManager.UniquenessCheck(
            "Note", 
            new Dictionary<string, string>{
                {"Name", noteInfo["Name"]}, 
                {"ConnectedLearningCode", noteInfo["ConnectedLearningCode"]}});
    }

    public void SaveNote(Dictionary<string, string> noteMetadata){
        // polymorphic - save note with connected learning code already added
        dataManager.SaveNote(noteMetadata);
    }

    public Dictionary<string, Dictionary<string, string>> GetNotes(string relatedLearningType="", string relatedLearningID=""){
        if(relatedLearningID !=""){
            List<string> parts = new List<string>{"xx", "xx", "xx"};
            string matchIdPattern = relatedLearningID; // default for milestone
            if(relatedLearningType == "Goal"){
                matchIdPattern += "-xx";
            }
            else if(relatedLearningType == "Skill"){
                matchIdPattern += "-xx-xx";
            }

            List<Dictionary<string, Dictionary<string, string>>> relatedNotes = new List<Dictionary<string, Dictionary<string, string>>>{};
            var filteredNotes = dataManager.notesDict.Where(kvp => kvp.Value["ConnectedLearningCode"].EndsWith(matchIdPattern));
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