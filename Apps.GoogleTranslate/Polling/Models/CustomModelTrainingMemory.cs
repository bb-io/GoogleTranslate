namespace Apps.GoogleTranslate.Polling.Models;

public class CustomModelTrainingMemory
{
    public DateTime LastPollingTime { get; set; }

    public bool Triggered { get; set; }
}
