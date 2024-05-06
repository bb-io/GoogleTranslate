using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.GoogleTranslate;

public class GoogleTranslateApplication : IApplication, ICategoryProvider
{
    public IEnumerable<ApplicationCategory> Categories
    {
        get => [ApplicationCategory.GoogleApps, ApplicationCategory.MachineTranslationAndMtqe];
        set { }
    }
        
    public string Name
    {
        get => "Google Translate";
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}