using Apps.GoogleTranslate.Models.Requests;
using Tests.GoogleTranslate.Base;
using Blackbird.Applications.Sdk.Common.Files;
using Apps.GoogleTranslate.Actions;

namespace Tests.GoogleTranslate;

[TestClass]
public class TranslationActionsCustomModelTests : TestBase
{
    private TranslationActions _actions => new(InvocationContext, FileManager);
    private const string CustomModelName = "projects/alex-test-wif-to-azure/locations/us-central1/models/TRL123456789";

    [TestMethod]
    [DataRow("Sample.html", "text/html")]
    [DataRow("Sample-v1.2.xliff", "application/x-xliff")]
    [DataRow("Sample-v2.2.xliff", "application/xliff+xml")]
    public async Task Translate_InteroperableFileTranslationStrategy_IsSuccess(string fileName, string contentType)
    {
        var sampleFile = new FileReference
        {
            Name = fileName,
            ContentType = contentType,
        };
        var translateRequest = new ContentTranslationRequest
        {
            File = sampleFile,
            SourceLanguage = "en",
            TargetLanguage = "fr",
            CustomModelName = CustomModelName,
        };

        var result = await _actions.TranslateContent(translateRequest);

        Assert.StartsWith(fileName, result.File.Name);
    }

    [TestMethod]
    public async Task Translate_Interoperable_OutputFileHandlingOriginal_HtmlNameUnchanged()
    {
        var fileName = "Sample.html";
        var sampleFile = new FileReference
        {
            Name = fileName,
            ContentType = "text/html",
        };

        var translateRequest = new ContentTranslationRequest
        {
            File = sampleFile,
            SourceLanguage = "en",
            TargetLanguage = "fr",
            OutputFileHandling = "original",
            CustomModelName = CustomModelName,
        };

        var result = await _actions.TranslateContent(translateRequest);

        Assert.AreEqual(fileName, result.File.Name);
    }

    [TestMethod]
    public async Task Translate_NativeFileTranslationStrategy_IsSuccess()
    {
        var sampleFile = new FileReference {
            Name = "Sample.docx",
            ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        };
        var translateRequest = new ContentTranslationRequest
        {
            File = sampleFile,
            SourceLanguage = "en",
            TargetLanguage = "fr",
            FileTranslationStrategy = "native",
            CustomModelName = CustomModelName,
        };

        var result = await _actions.TranslateContent(translateRequest);

        Assert.IsTrue(result.DetectedSourceLanguage.StartsWith("en", StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public async Task TranslateText_IsSuccess()
    {
        var translateRequest = new TextTranslationRequest
        {
            Text = "One",
            SourceLanguage = "en",
            TargetLanguage = "fr",
            CustomModelName = CustomModelName,
        };

        var result = await _actions.TranslateText(translateRequest);

        Assert.AreEqual<string>("Un", result.TranslatedText);
    }
}
