using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
