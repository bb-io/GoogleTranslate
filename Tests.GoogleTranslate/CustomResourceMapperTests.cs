using Apps.GoogleTranslate.Utils;
using Google.Cloud.Translate.V3;
using Google.Protobuf.WellKnownTypes;

namespace Tests.GoogleTranslate;

[TestClass]
public class CustomResourceMapperTests
{
    [TestMethod]
    public void Dataset_IsMappedToResponse()
    {
        var createdAt = DateTime.SpecifyKind(new DateTime(2026, 8, 25, 10, 0, 0), DateTimeKind.Utc);
        var updatedAt = createdAt.AddMinutes(5);
        var dataset = new Dataset
        {
            Name = "projects/project/locations/us-central1/datasets/dataset-id",
            DisplayName = "French_dataset",
            SourceLanguageCode = "en",
            TargetLanguageCode = "fr",
            ExampleCount = 1_000,
            TrainExampleCount = 800,
            ValidateExampleCount = 100,
            TestExampleCount = 100,
            CreateTime = Timestamp.FromDateTime(createdAt),
            UpdateTime = Timestamp.FromDateTime(updatedAt)
        };

        var result = CustomResourceMapper.ToResponse(dataset);

        Assert.AreEqual(dataset.Name, result.Name);
        Assert.AreEqual("French_dataset", result.DisplayName);
        Assert.AreEqual("en", result.SourceLanguageCode);
        Assert.AreEqual("fr", result.TargetLanguageCode);
        Assert.AreEqual(1_000, result.ExampleCount);
        Assert.AreEqual(800, result.TrainingExampleCount);
        Assert.AreEqual(100, result.ValidationExampleCount);
        Assert.AreEqual(100, result.TestExampleCount);
        Assert.AreEqual(createdAt, result.CreatedAt);
        Assert.AreEqual(updatedAt, result.UpdatedAt);
        Assert.IsFalse(result.DataImported);
        Assert.IsNull(result.ImportedDataGcsUri);
    }

    [TestMethod]
    public void Model_IsMappedToResponse()
    {
        var createdAt = DateTime.SpecifyKind(new DateTime(2026, 8, 25, 11, 0, 0), DateTimeKind.Utc);
        var model = new Model
        {
            Name = "projects/project/locations/us-central1/models/model-id",
            DisplayName = "French_model",
            Dataset = "projects/project/locations/us-central1/datasets/dataset-id",
            SourceLanguageCode = "en",
            TargetLanguageCode = "fr",
            TrainExampleCount = 800,
            ValidateExampleCount = 100,
            TestExampleCount = 100,
            CreateTime = Timestamp.FromDateTime(createdAt)
        };

        var result = CustomResourceMapper.ToResponse(model);

        Assert.AreEqual(model.Name, result.Name);
        Assert.AreEqual("French_model", result.DisplayName);
        Assert.AreEqual(model.Dataset, result.DatasetName);
        Assert.AreEqual("en", result.SourceLanguageCode);
        Assert.AreEqual("fr", result.TargetLanguageCode);
        Assert.AreEqual(800, result.TrainingExampleCount);
        Assert.AreEqual(100, result.ValidationExampleCount);
        Assert.AreEqual(100, result.TestExampleCount);
        Assert.AreEqual(createdAt, result.CreatedAt);
        Assert.IsNull(result.UpdatedAt);
    }
}
