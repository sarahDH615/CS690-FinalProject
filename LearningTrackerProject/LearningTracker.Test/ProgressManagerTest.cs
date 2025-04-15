namespace LearningTracker.Test;

public class ProgressManagerTest
{
    ProgressManager testProgressManager = new ProgressManager();
    
    [Fact]
    public void TestGetProgressSummary()
    {
        // string filter
        List<string> testProgressSummaryReturn = testProgressManager.GetProgressSummary("");
        Assert.Empty(testProgressSummaryReturn);
    }
}