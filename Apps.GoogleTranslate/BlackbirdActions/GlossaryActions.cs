using Apps.GoogleTranslate.Models.Requests;
using Apps.GoogleTranslate.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Google.Cloud.Translate.V3;

namespace Apps.GoogleTranslate.BlackbirdActions;

/* ActionList attribute was removed due to we want to keep this function but not expose it to the user */
public class GlossaryActions(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [Action("Get all glossaries", Description = "List all glossaries")]
    public Task<GetAllGlossariesResponse> GetAllGlossaries()
    {
        var parent = Client.ProjectName + "/locations/us-central1";
        var glossaries = Client.TranslateClient.ListGlossaries(new ListGlossariesRequest
        {
            Parent = parent
        });

        return Task.FromResult(new GetAllGlossariesResponse
        {
            Glossaries = glossaries.Select(x => new GlossaryResponse
            {
                GlossaryName = x.DisplayName,
                FullName = x.Name,
                SubmitTime = x.SubmitTime.ToDateTime(),
                EndTime = x.EndTime.ToDateTime(),
                SourceLanguage = x.LanguagePair.SourceLanguageCode,
                TargetLanguage = x.LanguagePair.TargetLanguageCode
            }).ToList()
        });
    }
    
    [Action("Get glossary", Description = "Get glossary based on name")]
    public async Task<GlossaryResponse> GetGlossary([ActionParameter] GlossaryRequest request)
    {
        var glossary = await Client.TranslateClient.GetGlossaryAsync(new GetGlossaryRequest
        {
            Name = request.GlossaryName
        });

        return new GlossaryResponse
        {
            GlossaryName = glossary.DisplayName,
            FullName = glossary.Name,
            SubmitTime = glossary.SubmitTime.ToDateTime(),
            EndTime = glossary.EndTime.ToDateTime(),
            SourceLanguage = glossary.LanguagePair.SourceLanguageCode,
            TargetLanguage = glossary.LanguagePair.TargetLanguageCode
        };
    }
    
    [Action("Import glossary", Description = "Import glossary from Google Cloud Storage. Supported formats: CSV, TMX, TSV")]
    public async Task<GlossaryResponse> ImportGlossary([ActionParameter] ImportGlossaryRequest request)
    {        
        var parent = Client.ProjectName + "/locations/us-central1";
        var createGlossaryResponse = await Client.TranslateClient.CreateGlossaryAsync(new CreateGlossaryRequest
        {
            Parent = parent,
            Glossary = new Glossary
            {
                Name = request.Name,
                LanguageCodesSet = new Glossary.Types.LanguageCodesSet
                {
                    LanguageCodes = { request.SourceLanguageCode, request.TargetLanguageCode }
                },
                InputConfig = new GlossaryInputConfig
                {
                    GcsSource = new GcsSource
                    {
                        InputUri = request.GcsInputSource
                    }
                }
            }
        });

        var operation = await createGlossaryResponse.PollUntilCompletedAsync();
        return new GlossaryResponse
        {
            GlossaryName = operation.Result.DisplayName,
            FullName = operation.Result.Name,
            SubmitTime = operation.Result.SubmitTime.ToDateTime(),
            EndTime = operation.Result.EndTime.ToDateTime(),
            SourceLanguage = operation.Result.LanguagePair.SourceLanguageCode,
            TargetLanguage = operation.Result.LanguagePair.TargetLanguageCode,
        };
    }
    
    [Action("Delete glossary", Description = "Delete glossary based on name")]
    public async Task DeleteGlossary([ActionParameter] GlossaryRequest request)
    {
        await Client.TranslateClient.DeleteGlossaryAsync(new DeleteGlossaryRequest
        {
            Name = request.GlossaryName
        });
    }
}