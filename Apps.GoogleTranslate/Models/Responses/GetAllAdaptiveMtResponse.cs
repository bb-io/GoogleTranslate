using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleTranslate.Models.Responses;

public class GetAllAdaptiveMtResponse
{
    [Display("Adaptive MTs")]
    public List<AdaptiveMtResponse> AdaptiveMts { get; set; }   
}