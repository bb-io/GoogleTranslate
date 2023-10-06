using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleTranslate
{
    public class GoogleTranslateApplication : IApplication
    {
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
}
