﻿using Apps.GoogleTranslate.Models.Requests;
using Tests.GoogleTranslate.Base;
using Blackbird.Applications.Sdk.Common.Files;
using Apps.GoogleTranslate.Actions;

namespace Tests.GoogleTranslate;

[TestClass]
public class TranslationActionsGenericBackendTests : TestBase
{
    private TranslationActions _actions => new(InvocationContext, FileManager);

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
            TargetLanguage = "fr",
        };
        var config = new BaseTranslationConfig();

        var result = await _actions.TranslateContent(config, translateRequest);

        Assert.StartsWith(fileName, result.File.Name);
    }

    [TestMethod]
    [DataRow("Sample-v1.2-inline-formatting.xlf", "application/x-xliff")]
    [DataRow("Sample-v2.2-inline-formatting.xliff", "application/xliff+xml")]
    public async Task Translate_InteroperableFileReplaceSource_IsSuccess(string fileName, string contentType)
    {
        var sampleFile = new FileReference
        {
            Name = fileName,
            ContentType = contentType,
        };
        var translateRequest = new ContentTranslationRequest
        {
            File = sampleFile,
            TargetLanguage = "fr",
            PreserveXliffFormatting = true,
        };
        var config = new BaseTranslationConfig();

        var result = await _actions.TranslateContent(config, translateRequest);

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
            TargetLanguage = "fr",
            OutputFileHandling = "original"
        };
        var config = new BaseTranslationConfig();

        var result = await _actions.TranslateContent(config, translateRequest);

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
            TargetLanguage = "de",
            FileTranslationStrategy = "native"
        };
        var config = new BaseTranslationConfig();

        var result = await _actions.TranslateContent(config, translateRequest);

        Assert.IsTrue(result.DetectedSourceLanguage.StartsWith("en", StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public async Task TranslateText_IsSuccess()
    {
        var translateRequest = new TextTranslationRequest
        {
            Text = "One",
            TargetLanguage = "de"
        };
        var config = new BaseTranslationConfig();

        var result = await _actions.TranslateText(config, translateRequest);

        Assert.AreEqual<string>("Eins", result.TranslatedText);
    }
}
