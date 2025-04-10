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

}