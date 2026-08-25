using Apps.GoogleTranslate.Models.Responses;
using Google.Cloud.Translate.V3;

namespace Apps.GoogleTranslate.Utils;

public static class CustomResourceMapper
{
    public static CustomDatasetResponse ToResponse(Dataset dataset) => new()
    {
        Name = dataset.Name,
        DisplayName = dataset.DisplayName,
        SourceLanguageCode = dataset.SourceLanguageCode,
        TargetLanguageCode = dataset.TargetLanguageCode,
        ExampleCount = dataset.ExampleCount,
        TrainingExampleCount = dataset.TrainExampleCount,
        ValidationExampleCount = dataset.ValidateExampleCount,
        TestExampleCount = dataset.TestExampleCount,
        CreatedAt = dataset.CreateTime?.ToDateTime(),
        UpdatedAt = dataset.UpdateTime?.ToDateTime()
    };

    public static CustomModelResponse ToResponse(Model model) => new()
    {
        Name = model.Name,
        DisplayName = model.DisplayName,
        DatasetName = model.Dataset,
        SourceLanguageCode = model.SourceLanguageCode,
        TargetLanguageCode = model.TargetLanguageCode,
        TrainingExampleCount = model.TrainExampleCount,
        ValidationExampleCount = model.ValidateExampleCount,
        TestExampleCount = model.TestExampleCount,
        CreatedAt = model.CreateTime?.ToDateTime(),
        UpdatedAt = model.UpdateTime?.ToDateTime()
    };
}
