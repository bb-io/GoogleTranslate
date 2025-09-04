using Apps.GoogleTranslate.Models.Requests;
using Tests.GoogleTranslate.Base;
using Apps.GoogleTranslate.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Apps.GoogleTranslate.Actions;

namespace Tests.GoogleTranslate;

[TestClass]
public class DataSourceHandlersTests : TestBase
{
    private static void PrintDataSources(IEnumerable<DataSourceItem> options)
    {
        if (options.Any())
            options.ToList().ForEach(o => Console.WriteLine($"{o.Value}: {o.DisplayName}"));
        else
            Console.WriteLine("No data items");
    }

    [TestMethod]
    public async Task LanguagesDataSourceHandler_ShouldReturnLanguages()
    {
        var handler = new LanguageDataHandler(InvocationContext);
        var languages = await handler.GetDataAsync(new DataSourceContext(), new CancellationToken());

        PrintDataSources(languages);
        Assert.IsTrue(languages.Any());
    }

    [TestMethod]
    public async Task GlossaryDataSourceHandler_ShouldReturnGlossaries()
    {
        var handler = new GlossaryDataHandler(InvocationContext);
        var glossaries = await handler.GetDataAsync(new DataSourceContext(), new CancellationToken());

        PrintDataSources(glossaries);
        Assert.IsTrue(glossaries.Any());
    }

    [TestMethod]
    public async Task CustomModelDataHandler()
    {
        var handler = new CustomModelDataHandler(InvocationContext);
        var datasets = await handler.GetDataAsync(new DataSourceContext(), new CancellationToken());

        PrintDataSources(datasets);
        Assert.IsTrue(datasets.Any());
    }

    [TestMethod]
    public async Task AdaptiveDatasetDataHandler()
    {
        var handler = new AdaptiveDatasetDataHandler(InvocationContext);
        var datasets = await handler.GetDataAsync(new DataSourceContext(), new CancellationToken());

        PrintDataSources(datasets);
        Assert.IsTrue(datasets.Any());
    }

    [TestMethod]
    [Ignore("Only for manual run to create an adaptive dataset")]
    public async Task Helper_Created_AdaptiveDataset()
    {
        var actions = new AdaptiveDatasetActions(InvocationContext, FileManager);
        await actions.CreateAdaptiveMtAsync(new CreateAdaptiveDatasetRequest
        {
            Name = "sample-datatest",
            SourceLanguageCode = "en",
            TargetLanguageCode = "fr",
            File = await FileManager.UploadTestFileAsync("sample-adaptive-dataset-en-fr.tsv", "text/tab-separated-values")
        });
    }
}
