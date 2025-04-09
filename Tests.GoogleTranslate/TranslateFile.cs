using Apps.GoogleTranslate;
using Apps.GoogleTranslate.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.GoogleTranslate.Base;

namespace Tests.GoogleTranslate
{
    [TestClass]
    public class TranslateFile :TestBase
    {
        [TestMethod]
        public async Task TranslateFile_IsSuccess()
        {
            var aciton = new Actions(InvocationContext,FileManager);

            var result = await aciton.TranslateDocumentLanguage(
                new TranslateDocumentRequest { File= new Blackbird.Applications.Sdk.Common.Files.FileReference { Name="YOUR_FILE_NAME" }, TargetLanguageCode="de" });

            Assert.IsNotNull(result);
        }
    }
}
